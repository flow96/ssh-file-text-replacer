using Microsoft.Extensions.Configuration;

namespace SshFileTextReplacer;

public class SshSettings
{
    public SshServerSettings ServerSettings { get; set; } = new SshServerSettings();
    public string LineToFind { get; set; } = "";
    public string LineToReplace { get; set; } = "";
    public string FilePath { get; set; } = "";
    
    public string[] CommandsToRunAfterwards { get; set; }

    
    public SshSettings(string hostIp = "", string username = "", string password = "", string lineToFind = "", string lineToReplace = "", string filePath = "", params string[] commands)
    {
        ServerSettings.Host = hostIp;
        ServerSettings.Username = username;
        ServerSettings.Password = password;
        LineToFind = lineToFind;
        LineToReplace = lineToReplace;
        FilePath = filePath;
        CommandsToRunAfterwards = commands;
    }

    public static SshSettings? ParseFromConfig(IConfiguration configuration)
    {
        var settings = new SshSettings();
        configuration.Bind(settings);
        
        // Validate the parsed data
        if (!settings.Validate())
            return null;

        return settings;
    }

    /// <summary>
    /// Validates that all parameters are given and none are null/empty or whitespace
    /// </summary>
    public bool Validate()
    {
        List<string> errorMsgs = new List<string>();
        
        // Basic validation
        if (string.IsNullOrWhiteSpace(ServerSettings.Host))
            errorMsgs.Add("Property 'host' must not be null or empty!");
        if (string.IsNullOrWhiteSpace(ServerSettings.Username))
            errorMsgs.Add("Property 'username' must not be null or empty!");
        if (string.IsNullOrWhiteSpace(LineToFind))
            errorMsgs.Add("Property 'lineToFind' must not be null or empty!");
        if (string.IsNullOrWhiteSpace(LineToReplace))
            errorMsgs.Add("Property 'lineToReplace' must not be null or empty!");
        if (string.IsNullOrWhiteSpace(FilePath))
            errorMsgs.Add("Property 'filePath' must not be null or empty!");

        if (errorMsgs.Any())
        {
            Console.WriteLine("********** ERROR **********");
            foreach (var msg in errorMsgs)
                Console.WriteLine(" >> " + msg);
            Console.WriteLine("***************************");
            Console.WriteLine("Please check the appsettings.json file!");
        }

        return errorMsgs.Count == 0;
    }   
}

public class SshServerSettings
{
    public string Host { get; set; } = "";
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
}