using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bor
{
    public interface ICmd
    {
        Task ExecuteAsync(string[] args);
    }
}
