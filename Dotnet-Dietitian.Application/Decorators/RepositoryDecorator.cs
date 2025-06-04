using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Dotnet_Dietitian.Application.Decorators
{
    public abstract class RepositoryDecorator<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly IRepository<T> _decoratedRepository;

        protected RepositoryDecorator(IRepository<T> repository)
        {
            _decoratedRepository = repository;
        }

        public virtual async Task<T> GetByIdAsync(Guid id)
        {
            return await _decoratedRepository.GetByIdAsync(id);
        }

        public virtual async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _decoratedRepository.GetAllAsync();
        }

        public virtual async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await _decoratedRepository.GetAsync(predicate);
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            return await _decoratedRepository.AddAsync(entity);
        }        public virtual async Task UpdateAsync(T entity)
        {
            await _decoratedRepository.UpdateAsync(entity);
        }

        public virtual async Task DeleteAsync(T entity)
        {
            await _decoratedRepository.DeleteAsync(entity);
        }

        public virtual async Task<T?> GetByFilterAsync(Expression<Func<T, bool>> predicate)
        {
            return await _decoratedRepository.GetByFilterAsync(predicate);
        }
    }
}