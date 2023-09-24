using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Worker
    {
        public class Root
        {
            public ObjectId _id { get; set; }
            public bool IsOK { get; set; }
            public object Error { get; set; }

            public int Comitente { get; set; }
            public Resultado Resultado { get; set; }


        }

        public class Detalle
        {
            public string Moneda { get; set; }
            public string Importe { get; set; }
            public string DetalleDesc { get; set; }
            public object Cantidad { get; set; }
            public string TipoTotal { get; set; }
            public string TipoCambio { get; set; }
        }

        public class TotalPosicion
        {
            public string TotalPosicionValue { get; set; }
            public List<Detalle> Detalle { get; set; }
        }

        public class APERTURA
        {
            public object EnGarantia { get; set; }
            public string Saldo { get; set; }
            public string Total { get; set; }
            public string Vencimiento { get; set; }
        }

        public class Subtotal
        {
            public string Moneda { get; set; }
            public List<APERTURA> APERTURA { get; set; }
            public string Importe { get; set; }
            public string Costo { get; set; }
            public string DetalleDesc { get; set; }
            public string Ticker { get; set; }
            public string TipoEspecie { get; set; }
            public string Participacion { get; set; }
            public string Variacion { get; set; }
            public string Resultado { get; set; }
            public string CodigoEspecie { get; set; }
            public object Cantidad { get; set; }
            public string TipoTotal { get; set; }
            public string Especie { get; set; }
            public string Divisor { get; set; }
            public string NombreEspecieReducido { get; set; }
            public string Precio { get; set; }
        }

        public class Activo
        {
            public string TipoActivo { get; set; }
            public string Moneda { get; set; }
            public string Resultado { get; set; }
            public string Importe { get; set; }
            public object Cantidad { get; set; }
            public string TipoTotal { get; set; }
            public List<Subtotal> Subtotal { get; set; }
            public string Participacion { get; set; }
            public string TipoCambio { get; set; }
        }

        public class Resultado
        {
            public TotalPosicion Totales { get; set; }
            public List<Activo> Activos { get; set; }
        }
    }

}
