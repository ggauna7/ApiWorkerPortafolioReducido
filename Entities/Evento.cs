using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Evento
    {
        public ObjectId _id { get; set; }
        public string ID { get; set; }

        public int Comitente { get; set; }

        public string Estado { get; set; }

        public string Fecha { get; set; }
    }
}
