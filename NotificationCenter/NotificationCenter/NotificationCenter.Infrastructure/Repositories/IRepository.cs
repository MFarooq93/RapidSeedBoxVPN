using NotificationCenter.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NotificationCenter.Infrastructure.Repositories
{
    public interface IRepository<T> : IDisposable
    {
        /// <summary>
        /// Gets All <see cref="T"/>
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> GetAll();

        /// <summary>
        /// Gets <see cref="T"/> by <paramref name="id"/>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T Get(string id);

        /// <summary>
        /// Gets <see cref="IEnumerable{T}"/> filtered with <paramref name="predicate"/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IEnumerable<T> Get(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Adds or update <see cref="T"/> <paramref name="entity"/>
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        T Upsert(T entity);


        /// <summary>
        /// Removes an <see cref="T"/> by <paramref name="id"/>
        /// </summary>
        /// <param name="id"></param>
        void Remove(string id);

        /// <summary>
        /// Deletes all records of Type <see cref="T"/> & inserts <paramref name="entities"/>
        /// </summary>
        /// <param name="entities"></param>
        void TruncateAndDump(IEnumerable<T> entities);

        void OnChange(Action<List<DataChangeDTO<T>>> callback);

        /// <summary>
        /// Gets the count for <see cref="T"/> with <paramref name="predicate"/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        int Count(Expression<Func<T, bool>> predicate);
    }
}
