using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repositories
{
    public interface IWorkerRepository
    {
        List<int> GetComitentes(string database);
        bool InsertEvento(Worker.Root worker, int nroComitente, string cliente);
        bool UpdateEvento(Worker.Root worker, string cliente);
    }
}
