using HRM.Application.Common.Models;
using HRM.Domain.Entities;

namespace HRM.Application.Interfaces;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id);
    Task<PagedResult<T>> GetAllAsync(int pageNumber, int pageSize);
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<bool> DeactivateAsync(Guid id);
}

