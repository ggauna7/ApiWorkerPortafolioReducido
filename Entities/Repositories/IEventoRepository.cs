using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repositories
{
    public interface IEventoRepository
    {
        bool UpdateEvent(int comitente, string cliente);
    }
}
