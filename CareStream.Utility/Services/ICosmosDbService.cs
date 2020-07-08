using CareStream.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CareStream.Utility.Services
{
    //public interface ICosmosDbService<T>
    public interface ICosmosDbService<TEntity> where TEntity : class

    {
        //Task<IEnumerable<ProductFamilyModel>> GetItemsAsync(string query);
        //Task<ProductFamilyModel> GetItemAsync(string id);
        //Task AddItemAsync(ProductFamilyModel item, string id);
        //Task UpdateItemAsync(string id, ProductFamilyModel item);
        //Task DeleteItemAsync(string id);

      
            Task<List<TEntity>> GetAsync();
            Task<TEntity> GetAsync(TEntity Id);
            Task<TEntity> CreateAsync(TEntity entity);
            Task<bool> Delete(TEntity entity);
       
       



        //Task<List<T>> GetItemsAsync(string queryString);       
        //Task<T> FindByCondition(Expression<Func<T, bool>> expression);      
        //void Create(T entity, string id);       
        //Task UpdateItemAsync(string id, T item);      
        //Task DeleteItemAsync(string id);
    }
}
