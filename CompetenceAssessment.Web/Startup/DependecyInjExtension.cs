using System.Reflection;

namespace CompetenceAssessment.Web.Startup;

public static class DependencyInjExtension
{
    public static void RegisterBySuffix(this IServiceCollection services, 
        string suffix, ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.FullName.StartsWith("CompetenceAssessment"))
            .ToList();
    
        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith(suffix))
                .ToList();
        
            foreach (var implementationType in types)
            {
                var interfaceType = implementationType.GetInterface($"I{implementationType.Name}");
            
                if (interfaceType != null)
                {
                    services.Add(new ServiceDescriptor(interfaceType, implementationType, lifetime));
                }
                else
                {
                    services.Add(new ServiceDescriptor(implementationType, implementationType, lifetime));
                }
            }
        }
    }
}