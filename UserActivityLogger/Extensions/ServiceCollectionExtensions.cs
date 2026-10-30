using Microsoft.Extensions.DependencyInjection.Extensions;

namespace UserActivityLogger.Extensions;

public static class ServiceCollectionExtensions
{
    /// 
    /// Adds UserActivityLogger services to the service collection.
    /// 
    /// The DbContext type that contains UserLog entity.
    /// The service collection.
    /// Optional configuration action.
    /// The service collection for chaining.
    public static IServiceCollection AddUserActivityLogger<TContext>(
        this IServiceCollection services,
        Action<UserActivityLoggerOptions>? configure = null)
        where TContext : DbContext
    {
        // Configure options
        var options = new UserActivityLoggerOptions();
        configure?.Invoke(options);
        services.AddSingleton(options);

        // Register core services
        services.AddHttpContextAccessor();

        // Register filter
        services.TryAddScoped<IUserActivityLogger, UserActivityLoggerService<TContext>>();
        services.TryAddScoped<UserActivityLoggingFilter>();

        return services;
    }

    /// 
    /// Adds the UserActivityLoggingFilter to MVC options.
    /// Call this method inside AddControllersWithViews() or AddControllers() configuration.
    /// 
    /// The MVC options.
    /// The MVC options for chaining.
    public static MvcOptions AddUserActivityLogging(this MvcOptions options)
    {
        bool filtreVar = options.Filters
            .OfType<ServiceFilterAttribute>()
            .Any(filtre =>
                filtre.ServiceType == typeof(UserActivityLoggingFilter));

        if (!filtreVar)
        {
            options.Filters.AddService<UserActivityLoggingFilter>();
        }

        return options;
        }    
}
