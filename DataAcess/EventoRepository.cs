using Entities;
using Entities.Repositories;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class EventoRepository : IEventoRepository
    {
        private readonly IMongoClient _mongoClient;
        public EventoRepository(IMongoClient mongoClient) 
        {
            _mongoClient=  mongoClient; 
        }
        public bool UpdateEvent(int comitente, string cliente)
        {
            var database = _mongoClient.GetDatabase(cliente);
            var mongoCollection = database.GetCollection<Evento>("Eventos");

            var filter = Builders<Evento>.Filter.Eq("Comitente", comitente);

            var update = Builders<Evento>.Update
                .Set(w => w.Estado, "Actualizado")
                .Set(w => w.Fecha, DateTime.Now.ToString());

            var result = mongoCollection.UpdateOne(filter, update);

            if (result.ModifiedCount > 0)
            {
                return true;
            }
            return false;
        }
    }
}
