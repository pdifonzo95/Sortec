using Microsoft.EntityFrameworkCore;
using Sortec.Domain.Entities;
using Sortec.Domain.Interface;
using Sortec.Infrastructure.Context;
using Sortec.Infrastructure.ExeptionHandling;
using System.Linq.Expressions;

namespace Sortec.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly SortecDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(SortecDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<Response<T>> AddAsync(T entity)
        {
            Response<T> response = new Response<T>();

            try
            {
                await _dbSet.AddAsync(entity);
                await _context.SaveChangesAsync();
                response.Data = entity;
                response.Message = $"Registration has been created successfully";

                return response;
            }
            catch (Exception ex) 
            {
                throw;
            }
        }

        public async Task<Response<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            Response<T> response = new Response<T>();

            try
            {
                await _context.Set<T>().AddRangeAsync(entities);
                await _context.SaveChangesAsync();
                response.Message = $"Adding {entities.Count()} records has been successfully executed";
                response.Status = true;

                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Response<T>> DeleteAsync(int id)
        {
            Response<T> response = new Response<T>();

            try
            {
                var entity = await _dbSet.FindAsync(id);

                if (entity == null)
                {
                    throw new CustomHttpException(404, $"Record with id {id} was not found.");
                }
                else 
                {
                    _dbSet.Remove(entity);
                    await _context.SaveChangesAsync();
                    response.Message = $"The record with id {id} has been found successfully";
                    response.Status = true;

                    return response;
                }
            }
            catch (Exception ex) 
            {
                throw;
            }
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(predicate, cancellationToken);
        }

        public async Task<Response<IEnumerable<T>>> GetAllAsync()
        {
            Response<IEnumerable<T>> response = new Response<IEnumerable<T>>();

            try
            {
                var query = _dbSet.AsQueryable();

                var enabledProperty = typeof(T).GetProperty("Enabled");
                if (enabledProperty != null && enabledProperty.PropertyType == typeof(bool)) 
                {
                    query = query.Where(e => EF.Property<bool>(e, "Enabled") == true);
                }

                response.Data = await query.ToListAsync();
                response.Message = $"Successfully fetched {response.Data.Count()} records";
                response.Status = true;

                return response;
            }
            catch (Exception ex) 
            {
                throw;
            }
        }

        public async Task<Response<T>> GetByIdAsync(int id)
        {
            Response<T> response = new Response<T>();

            try
            {
                var entity = await _dbSet.FindAsync(id);
                if (entity != null)
                {
                    throw new CustomHttpException(404, $"Record with id {id} was not found");
                }
                else 
                {
                    response.Data = entity;
                    response.Message = $"The record with id {id} has been found successfully";
                    response.Status = true;

                    return response;
                }
            }
            catch (Exception ex) 
            {
                throw;
            }
        }

        public async Task<Response<T>> RemoveRangeAsync(IEnumerable<T> entities)
        {
            Response<T> response = new Response<T>();

            try
            {
                _context.Set<T>().RemoveRange(entities);
                await _context.SaveChangesAsync();
                response.Message = $"Deletion of {entities.Count()} records has been successfully executed";
                response.Status = true;

                return await Task.FromResult(response);
            }
            catch (Exception ex) 
            {
                throw;            
            }
        }

        public async Task<Response<T>> SoftDeleteAsync(int id)
        {
            Response<T> response = new Response<T>();

            try
            {
                var entity = await _dbSet.FindAsync(id);

                if (entity == null) 
                {
                    throw new CustomHttpException(404, $"Record with id {id} was not found");
                }

                var stateProperty = entity.GetType().GetProperty("Enabled");

                if (stateProperty != null && stateProperty.CanWrite) 
                {
                    stateProperty.SetValue(entity, false);
                }

                await _context.SaveChangesAsync();

                response.Data = entity;
                response.Message = $"record with id {id} has been logically deleted successfully";
                response.Status = true;

                return response;
            }
            catch (Exception ex) 
            {
                throw;
            }
        }

        public async Task<Response<T>> UpdateAsync(T entity)
        {
            Response<T> response = new Response<T>();

            try
            {
                _dbSet.Update(entity);
                await _context.SaveChangesAsync();
                response.Data = entity;
                response.Message = $"The record {entity} has been updated successfully";
                response.Status = true;

                return await Task.FromResult(response);
            }
            catch (Exception ex) 
            {
                throw;
            }
        }
    }
}