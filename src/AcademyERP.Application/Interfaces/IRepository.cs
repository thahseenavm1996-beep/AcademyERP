using System.Linq.Expressions;
using AcademyERP.Domain.Entities.Common;

namespace AcademyERP.Application.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id);

    Task<List<T>> GetAllAsync();

    Task AddAsync(T entity);

    Task UpdateAsync(T entity);

    Task DeleteAsync(Guid id);

    IQueryable<T> Query();
}