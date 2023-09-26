using Hangfire;
using MongoDB.Driver;
using Logic;
using ApiWorker.Controllers;
using System.Xml.Linq;
using Entities;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace ApiWorker
{
    public class CronJob 
    {
        private readonly IMongoClient _mongClient;
        private readonly IWorkerLogic _workerLogic;
        private readonly ILogger<CronJob> _logger;
        private readonly IEventoLogic _eventoLogic;
        private static bool isExecuting = false;
        public CronJob(IMongoClient mongoClient, IWorkerLogic workerLogic, ILogger<CronJob> logger, IEventoLogic eventoLogic)
        {
            
            _mongClient = mongoClient;
            _workerLogic = workerLogic;
            _logger = logger;
            _eventoLogic = eventoLogic;
        }
        public void ExecuteAsync()
        {

                string cliente = "IEB";
                _logger.LogInformation("Se Inicia el Cron: " + DateTime.Now.ToString("hh:mm:ss"));
                var comitentesList = _workerLogic.GetComitentes(cliente);
                if (comitentesList != null)
                {
                    _logger.LogInformation(comitentesList.ToString());
                    string token = GetAuthTokenAsync();
                    if (string.IsNullOrEmpty(token))
                    {
                        _logger.LogInformation("Fallo al obtener el Token");
                    }
                    var requestData = new RequestData
                    {
                        User = "mangoodev",
                    };

                    foreach (var item in comitentesList)
                    {
                        requestData.Comi = item.ToString();
                        Worker.Root jsonPortafolioReducido = GetPortafolioReducidoAsync(token, requestData);
                        if (jsonPortafolioReducido == null)
                        {
                            _logger.LogInformation("Fallo al buscar data en EGWS");
                        }
                        if (_workerLogic.UpdatePortafolio(jsonPortafolioReducido, item, cliente))
                        {
                            if (_eventoLogic.UpdateEvent(item, cliente))
                            {
                                _logger.LogInformation("Se ha actualizaron Datos para el Comitente: " + item);
                            }

                        }

                    }
                    _logger.LogInformation("Finalizo el Cron: " + DateTime.Now.ToString("hh:mm:ss"));
                }
            
        }

        public Worker.Root GetPortafolioReducidoAsync(string token, object requestData)
        {
            string _urlPortafolioReducido = Environment.GetEnvironmentVariable("URL_PortafolioReducido");
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("token", token);


                var formData = new FormUrlEncodedContent(
           requestData.GetType()
               .GetProperties()
               .Select(prop => new KeyValuePair<string, string>(prop.Name, prop.GetValue(requestData)?.ToString() ?? "")));

                HttpResponseMessage response =  client.PostAsync(_urlPortafolioReducido, formData).Result;

                if (response.IsSuccessStatusCode)
                {
                    string responseBody =  response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<Worker.Root>(responseBody);
                }

                return null;
            }
        }

        public string GetAuthTokenAsync()
        {
            string _urlToken = Environment.GetEnvironmentVariable("URL_Token");
            using (HttpClient client = new HttpClient())
            {
                var formData = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("user", "mangoodev"),
                    new KeyValuePair<string, string>("passwd", "gooman246")
                });
                HttpResponseMessage response =  client.PostAsync(_urlToken, formData).Result;

                if (response.IsSuccessStatusCode)
                {
                    string responseBody =  response.Content.ReadAsStringAsync().Result;
                    dynamic jsonResponse = JObject.Parse(responseBody);
                    return jsonResponse.Token;
                }

                return null;
            }
        }
    }
}
