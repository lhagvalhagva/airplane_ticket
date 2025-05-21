using Microsoft.EntityFrameworkCore;
using DataAccess.Models;

namespace DataAccess.Extensions
{
    public static class DbSetExtensions
    {
        public static IQueryable<T> IncludeAll<T>(this DbSet<T> dbSet) where T : class
        {
            if (typeof(T) == typeof(Seat))
            {
                return dbSet
                    .Include(s => (s as Seat).Flight)
                    .Include(s => (s as Seat).Passenger)
                    .AsSplitQuery()
                    as IQueryable<T>;
            }
            else if (typeof(T) == typeof(Flight))
            {
                return dbSet
                    .Include(f => (f as Flight).Seats)
                    .Include(f => (f as Flight).FlightPassengers)
                    .ThenInclude(fp => fp.Passenger)
                    .AsSplitQuery()
                    as IQueryable<T>;
            }
            
            return dbSet;
        }
    }
} 