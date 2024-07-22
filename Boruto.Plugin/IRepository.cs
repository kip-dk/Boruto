using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boruto
{
    public interface IRepository<T> where T: Microsoft.Xrm.Sdk.Entity, new()
    {
        T Get(Guid id);
        Guid Create(T entity);
        void Update(T entity);
        void Delete(T entity);
        void Delete(Guid id);
        IQueryable<T> GetQuery();
    }
}
