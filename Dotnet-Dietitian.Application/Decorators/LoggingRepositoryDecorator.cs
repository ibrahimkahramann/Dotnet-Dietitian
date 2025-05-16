using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Dotnet_Dietitian.Application.Decorators
{
    public class LoggingRepositoryDecorator<T> : RepositoryDecorator<T> where T : BaseEntity
    {
        private readonly ILogger<LoggingRepositoryDecorator<T>> _logger;

        public LoggingRepositoryDecorator(IRepository<T> repository, ILogger<LoggingRepositoryDecorator<T>> logger)
            : base(repository)
        {
            _logger = logger;
        }

        public override async Task<T> GetByIdAsync(Guid id)
        {
            _logger.LogInformation($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Fetching {typeof(T).Name} with ID: {id}");
            return await base.GetByIdAsync(id);
        }

        public override async Task<IReadOnlyList<T>> GetAllAsync()
        {
            _logger.LogInformation($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Fetching all {typeof(T).Name} entities");
            return await base.GetAllAsync();
        }

        public override async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate)
        {
            _logger.LogInformation($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Fetching {typeof(T).Name} entities with filter");
            return await base.GetAsync(predicate);
        }

        public override async Task<T> AddAsync(T entity)
        {
            _logger.LogInformation($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Adding new {typeof(T).Name} entity");
            return await base.AddAsync(entity);
        }

        public override async Task UpdateAsync(T entity)
        {
            _logger.LogInformation($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Updating {typeof(T).Name} with ID: {entity.Id}");
            await base.UpdateAsync(entity);
        }

        public override async Task DeleteAsync(T entity)
        {
            _logger.LogInformation($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Deleting {typeof(T).Name} with ID: {entity.Id}");
            await base.DeleteAsync(entity);
        }
    }
}