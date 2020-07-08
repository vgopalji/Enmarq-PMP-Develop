using CareStream.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CareStream.Utility.Services
{
    //public class CosmosDBService<T>: ICosmosDbService<T> where T:class
    public class CosmosDBService<TEntity> : ICosmosDbService<TEntity> where TEntity : class
    {
        private readonly CosmosDbContext ctx;

        public CosmosDBService(CosmosDbContext ctx)
        {
            this.ctx = ctx;
            // this will make sure that the database is created
            ctx.Database.EnsureCreated();
        }

        public async Task<TEntity> CreateAsync(TEntity entity)
        {

            //entity.ProductFamilyId = Guid.NewGuid().ToString();
            var response = ctx.Set<TEntity>().Add(entity);
            await ctx.SaveChangesAsync();
            return response.Entity;

        }

        public async Task<List<TEntity>> GetAsync()
        {

            var profiles = ctx.Set<TEntity>().ToList(); //.AsNoTracking();
            return profiles;

        }

        public async Task<TEntity> GetAsync(TEntity id)
        {

            var profile = await ctx.Set<TEntity>().FindAsync(id);
            return profile;

        }

        public async Task<bool> Delete(TEntity entity)
        {
            ctx.Set<TEntity>().Remove(entity);
            return true;
        }

        //protected Container _container { get; set; }
        //public CosmosDBService(CosmosClient dbClient, string databaseName, string containerName)
        //{
        //    this._container = dbClient.GetContainer(databaseName, containerName);
        //}

        //public async Task AddItemAsync(ProductFamilyModel item, string Id)
        //{
        //    await this._container.CreateItemAsync<ProductFamilyModel>(item, new PartitionKey(Id));
        //}

        //public async Task DeleteItemAsync(string id)
        //{
        //    await this._container.DeleteItemAsync<ProductFamilyModel>(id, new PartitionKey(id));
        //}

        //public async Task<ProductFamilyModel> GetItemAsync(string id)
        //{
        //    try
        //    {
        //        ItemResponse<ProductFamilyModel> response = await this._container.ReadItemAsync<ProductFamilyModel>(id, new PartitionKey(id));
        //        return response.Resource;
        //    }
        //    catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        //    {
        //        return null;
        //    }

        //}

        //public async Task<IEnumerable<ProductFamilyModel>> GetItemsAsync(string queryString)
        //{
        //    var query = this._container.GetItemQueryIterator<ProductFamilyModel>(new QueryDefinition(queryString));
        //    List<ProductFamilyModel> results = new List<ProductFamilyModel>();
        //    while (query.HasMoreResults)
        //    {
        //        var response = await query.ReadNextAsync();

        //        results.AddRange(response.ToList());
        //    }

        //    return results;
        //}

        //public async Task UpdateItemAsync(string id, ProductFamilyModel item)
        //{
        //    await this._container.UpsertItemAsync<ProductFamilyModel>(item, new PartitionKey(id));
        //}


        //public async Task<List<T>> GetItemsAsync(string queryString)
        //{
        //    var query = this._container.GetItemQueryIterator<T>(new QueryDefinition(queryString));
        //    List<T> results = new List<T>();
        //    while (query.HasMoreResults)
        //    {
        //        var response = await query.ReadNextAsync();

        //        results.AddRange(response.ToList());
        //    }

        //    return results;
        //}

        //public async Task <T> FindByCondition(Expression<Func<T, bool>> expression)
        //    {
        //    try
        //    {
        //        ItemResponse<T> response = await this._container.ReadItemAsync<T>(expression.ToString(), new PartitionKey(expression.ToString()));
        //        return response.Resource;
        //    }
        //    catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        //    {
        //        return null;
        //    }
        //}

        //    public async void Create(T entity, string id)
        //    {
        //        await this._container.CreateItemAsync<T>(entity, new PartitionKey(id));
        //    }

        //public async Task UpdateItemAsync(string id, T item)
        //{
        //    await this._container.UpsertItemAsync<T>(item, new PartitionKey(id));
        //}

        //public async Task DeleteItemAsync(string id)
        //{
        //    await this._container.DeleteItemAsync<T>(id, new PartitionKey(id));
        //}
    }
}
