using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using API.Attributes;


namespace API.Extensions;

public static class EntityExtensions
{
    public static IQueryable<T> IncludeNavigationPropertiesIfNeeded<T>(this IQueryable<T> query) where T : class
    {
        if (typeof(IEntityWithNavigation).IsAssignableFrom(typeof(T)))
        {
            var navigationProperties = typeof(T)
                .GetProperties()
                .Where(p => p.GetCustomAttribute<NavigationPropertyAttribute>() != null)
                .Select(p => p.Name);

            foreach (var navigationProperty in navigationProperties)
            {
                query = query.Include(navigationProperty);
            }
        }

        return query;
    }
}