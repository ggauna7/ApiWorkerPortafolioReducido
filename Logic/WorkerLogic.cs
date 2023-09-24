using Amazon.Runtime.Internal.Util;
using Entities;
using Entities.Repositories;
using MongoDB.Driver;

namespace Logic
{
    public interface IWorkerLogic
    {
        List<int> GetComitentes(string database);
        bool InsertarMongo(Worker.Root worker, int nroComitente, string cliente);
        bool UpdatePortafolio(Worker.Root? jsonPortafolioReducido, int item, string cliente);
    }

    public class WorkerLogic : IWorkerLogic
    {

        private readonly IWorkerRepository _workerRepository;
        private readonly IMongoClient _mongoClient;

        public WorkerLogic(IWorkerRepository workerRepository, IMongoClient mongoClient)
        {
            _workerRepository = workerRepository;
            _mongoClient = mongoClient;
        }

        public List<int> GetComitentes(string database)
        {
            return _workerRepository.GetComitentes(database);
        }

        public bool InsertarMongo(Worker.Root worker, int nroComitente, string cliente)
        {
            var database = _mongoClient.GetDatabase(cliente);

            var mongoCollection = database.GetCollection<Worker.Root>("PortafolioReducido");

            var existingDocument = mongoCollection.Find(document => document.Comitente == nroComitente).FirstOrDefault();

            if (existingDocument == null) { return _workerRepository.InsertEvento(worker,nroComitente,cliente); }

            worker._id = existingDocument._id;
            worker.Comitente = nroComitente;
            return _workerRepository.UpdateEvento(worker, cliente);
        }

        public bool UpdatePortafolio(Worker.Root worker, int nroComitente, string cliente)
        {
            var database = _mongoClient.GetDatabase(cliente);

            var mongoCollection = database.GetCollection<Worker.Root>("PortafolioReducido");

            var existingDocument = mongoCollection.Find(document => document.Comitente == nroComitente).FirstOrDefault();

            worker._id = existingDocument._id;
            worker.Comitente = nroComitente;
            return _workerRepository.UpdateEvento(worker, cliente);
        }
    }
}
