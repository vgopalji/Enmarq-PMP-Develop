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
    public class CosmosDBService<TEntity> : ICosmosDbService<TEntity> where TEntity : class
    {
        private readonly CosmosDbContext ctx;

        public CosmosDBService(CosmosDbContext ctx)
        {
            this.ctx = ctx;
            ctx.Database.EnsureCreated();
        }

        public async Task<TEntity> CreateAsync(TEntity entity)
        {
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

    }
}
