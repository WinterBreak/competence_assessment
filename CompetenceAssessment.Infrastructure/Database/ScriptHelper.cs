namespace CompetenceAssessment.Infrastructure.Database;

public class ScriptHelper
{
    public static string GetScriptPath(string relativePath)
    {
        var assembly = typeof(InfrastructureServiceRegistration).Assembly;
        var assemblyLocation = assembly.Location;
        var assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
        
        return Path.Combine(assemblyDirectory, relativePath);
    }
}