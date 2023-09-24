using Entities;
using Entities.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DataAccess
{
    public class WorkerRepository : IWorkerRepository
    {
        private readonly IMongoClient _mongoclient;
        //private readonly string _cliente = Environment.GetEnvironmentVariable("Database_Cliente");

        public WorkerRepository(IMongoClient mongoClient) 
        { 
            _mongoclient= mongoClient;
        }

        public List<int> GetComitentes(string databaseCliente)
        {
           var database = _mongoclient.GetDatabase(databaseCliente);
            var collection = database.GetCollection<Worker.Root>("PortafolioReducido");
            

            var nroComitente = collection.AsQueryable()
                .Select(x => x.Comitente)
                .ToList();

            return nroComitente;
        }

        public bool InsertEvento(Worker.Root worker, int nroComitente, string cliente)
        {
            
            var database = _mongoclient.GetDatabase(cliente);
            worker.Comitente = nroComitente;
            var mongoCollection = database.GetCollection<Worker.Root>("PortafolioReducido");
            var task = mongoCollection.InsertOneAsync(worker);
            if (task.Exception == null) { return true; }

            return false;

        }

        public bool UpdateEvento(Worker.Root worker, string cliente)
        {
            var database = _mongoclient.GetDatabase(cliente);
            var mongoCollection = database.GetCollection<Worker.Root>("PortafolioReducido");

            var filter = Builders<Worker.Root>.Filter.Eq("Comitente", worker.Comitente);

            var replaceResult = mongoCollection.ReplaceOne(filter, worker);

            if (replaceResult.ModifiedCount > 0)
            {
                return true;
            }
            return false;
        }
    }
}
