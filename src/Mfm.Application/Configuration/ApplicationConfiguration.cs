using MediatR;
using Mfm.Application.UseCases.Base;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Mfm.Application.Configuration;
public static class ApplicationConfiguration
{
    public static void ConfigureApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        _ = services.AddMediatR(config => config.RegisterServicesFromAssembly(assembly));
        services.InjectUseCases(assembly);
    }

    private static void InjectUseCases(this IServiceCollection services, Assembly assembly)
    {
        var useCaseInterfaceType = typeof(IRequestHandler<,>);
        var useCaseBaseType = typeof(UseCaseBase);

        foreach (var type in assembly.GetTypes())
        {
            if (IsDerivedFromGenericType(type, useCaseBaseType))
            {
                var interfaceType = type
                    .GetInterfaces()
                    .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == useCaseInterfaceType);

                if (interfaceType != null)
                {
                    _ = services.AddScoped(interfaceType, type);
                }
            }
        }
    }

    private static bool IsDerivedFromGenericType(Type type, Type genericType)
    {
        while (type != null && type != typeof(object))
        {
            var typeWithGenericTypeDefinition = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
            if (genericType == typeWithGenericTypeDefinition)
            {
                return true;
            }

            type = type.BaseType!;
        }

        return false;
    }
}
