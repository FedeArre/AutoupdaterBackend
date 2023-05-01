using System;
using System.Collections.Generic;
using System.Text;

namespace Objects.Repositories.Interfaces
{
    public interface IRepository<T>
    {
        public bool Add(T entity);
        public bool Delete(T entity);
        public bool Update(T entity);
        public T FindById(string id);
        public IEnumerable<T> FindAll();
    }
}
