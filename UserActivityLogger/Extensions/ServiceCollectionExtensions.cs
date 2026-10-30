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
        bool optionsRegistered = services.Any(
            service => service.ServiceType == typeof(UserActivityLoggerOptions)
        );

        if (optionsRegistered)
        {
            if (configure != null)
            {
                throw new InvalidOperationException(
                    "UserActivityLogger is already configured."
                );
            }
    
            return services;
        }
        
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
        bool filterExists = options.Filters
            .OfType<ServiceFilterAttribute>()
            .Any(filter =>
                filter.ServiceType == typeof(UserActivityLoggingFilter));

        if (!filterExists)
        {
            options.Filters.AddService<UserActivityLoggingFilter>();
        }

        return options;
    }    
}
