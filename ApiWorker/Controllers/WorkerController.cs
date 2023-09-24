using Microsoft.AspNetCore.Mvc;
using System;
using Entities;
using Logic;
using System.Text.Json;
using Newtonsoft.Json;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using Amazon.Runtime.Internal;
using System.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Hangfire;

namespace ApiWorker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkerController: ControllerBase
    {
        #region
        private readonly IWorkerLogic _workerLogic;
        private readonly IEventoLogic _eventoLogic;
        private readonly ILogger<WorkerController> _logger;
        private readonly Dictionary<string, string> _urls;

        #endregion
        public WorkerController(IWorkerLogic workerLogic, ILogger<WorkerController> logger, IOptions<Dictionary<string,string>> urls, IEventoLogic eventoLogic, CronJob cronJob)
        {
            _workerLogic = workerLogic;
            _logger = logger;
            _urls = urls.Value;
            _eventoLogic = eventoLogic;
        }
        [HttpPost]

        public async Task<IActionResult> PostAsync(string ID, int nroComitente, string FechaDesde, string FechaHasta, string cliente)
        {

            string token = await GetAuthTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogInformation("Fallo al obtener el Token");
                return StatusCode(500, "Failed to get authentication token.");
            }

            var requestData = new
            {
                user = "egws",
                comi = nroComitente,
                fecd = FechaDesde,
                fech = FechaHasta
            };

            Worker.Root jsonPortafolioReducido = await GetPortafolioReducidoAsync(token, requestData);
            if (jsonPortafolioReducido == null)
            {
                _logger.LogInformation("Fallo al buscar data en EGWS");
                return StatusCode(500, "Failed to get PortafolioReducido data.");
            } 
            if (_workerLogic.InsertarMongo(jsonPortafolioReducido, nroComitente,cliente))
            {
                if (_eventoLogic.UpdateEvent(jsonPortafolioReducido.Comitente,cliente))
                {
                    return Ok();
                }
            }
            _logger.LogInformation("Failed to insert data into MongoDB.");
            return StatusCode(500);
        }
        [HttpGet]
        public ActionResult Get() 
        { 
            return Ok("Gonzalo"); 
        }

        private async Task<string> GetAuthTokenAsync()
        {
            string _urlToken = Environment.GetEnvironmentVariable("LoginToken");
            using (HttpClient client = new HttpClient())
            {
                var formData = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("user", "egws"),
                    new KeyValuePair<string, string>("passwd", "Estudio123")
                });
                HttpResponseMessage response = await client.PostAsync(_urlToken, formData);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    dynamic jsonResponse = JObject.Parse(responseBody);
                    return jsonResponse.Token;
                }

                return null;
            }
        }

        private async Task<Worker.Root> GetPortafolioReducidoAsync(string token, object requestData)
        {
            string _urlPortafolioReducido = Environment.GetEnvironmentVariable("PortafolioReducido");
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("token", token);


                var formData = new FormUrlEncodedContent(
           requestData.GetType()
               .GetProperties()
               .Select(prop => new KeyValuePair<string, string>(prop.Name, prop.GetValue(requestData)?.ToString() ?? "")));

                HttpResponseMessage response = await client.PostAsync(_urlPortafolioReducido, formData);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Worker.Root>(responseBody);
                }

                return null;
            }
        }

    }
}
