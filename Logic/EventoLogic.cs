using Entities.Repositories;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public interface IEventoLogic
    {
        bool UpdateEvent(int comitente, string cliente);
    }
    public class EventoLogic : IEventoLogic
    {
        private readonly IEventoRepository _eventoRepository;

        public EventoLogic(IEventoRepository eventoRepositori)
        {
            _eventoRepository = eventoRepositori;
        }

        public bool UpdateEvent(int comitente, string cliente)
        {
            return _eventoRepository.UpdateEvent(comitente,cliente);

        }
    }
}
