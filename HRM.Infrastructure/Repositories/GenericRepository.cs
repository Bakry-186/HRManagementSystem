using HRM.Application.Common.Models;
using HRM.Application.Interfaces;
using HRM.Domain.Entities;
using HRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HRM.Infrastructure.Repositories;

public class GenericRepository<T>(AppDbContext context) : IGenericRepository<T> where T : BaseEntity
{
    protected readonly DbSet<T> DbSet = context.Set<T>();

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await DbSet.FirstOrDefaultAsync(e => e.Id == id && e.IsActive);
    }

    public async Task<PagedResult<T>> GetAllAsync(int pageNumber, int pageSize)
    {
        var query = DbSet.Where(e => e.IsActive);
        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(e => e.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<T>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<T> CreateAsync(T entity)
    {
        entity.Id = Guid.NewGuid();
        DbSet.Add(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    public async Task<T> UpdateAsync(T entity)
    {
        DbSet.Update(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeactivateAsync(Guid id)
    {
        var entity = await DbSet.FindAsync(id);
        if (entity is null) return false;

        entity.IsActive = false;
        await context.SaveChangesAsync();
        return true;
    }
}

