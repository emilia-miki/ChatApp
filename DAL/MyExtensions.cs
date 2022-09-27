using System.Linq.Expressions;

namespace ChatApp.DAL;

public static class MyExtensions
{
    public static IOrderedQueryable<TSource> OrderBy<TSource>(
        this IQueryable<TSource> query, string? propertyName, bool sortDesc)
    {
        if (propertyName == null)
        {
            return (IOrderedQueryable<TSource>) query;
        }
        
        var entityType = typeof(TSource);

        //Create x => x.PropName
        var propertyInfo = entityType.GetProperty(propertyName);
        var arg = Expression.Parameter(entityType, "x");
        var property = Expression.Property(arg, propertyName);
        var selector = Expression.Lambda(property, arg);
        
        var enumerableType = typeof(Queryable);
        var methodName = sortDesc ? "OrderByDescending" : "OrderBy";
        var method = enumerableType.GetMethods()
            .Where(m => m.Name == methodName 
                        && m.IsGenericMethodDefinition)
            .Single(m =>
            {
                var parameters = m.GetParameters().ToList();
                //Put more restriction here to ensure selecting the right overload                
                return parameters.Count == 2;//overload that has 2 parameters
            });
        //The linq's OrderBy<TSource, TKey> has two generic types, which provided here
        var genericMethod = method
            .MakeGenericMethod(entityType, propertyInfo!.PropertyType);

        /*Call query.OrderBy(selector), with query and selector: x=> x.PropName
          Note that we pass the selector as Expression to the method and we don't compile it.
          By doing so EF can extract "order by" columns and generate SQL for it.*/
        var newQuery = (IOrderedQueryable<TSource>)genericMethod
            .Invoke(genericMethod, new object[] { query, selector })!;
        return newQuery;
    }
}