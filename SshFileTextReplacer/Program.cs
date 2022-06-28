

using Microsoft.Extensions.Configuration;
using SshFileTextReplacer;

public class Program
{
    public static void Main(string[] args)
    {
        IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        // Init the text replacer
        var textReplacer = new TextReplaceWorker(config);
        
        // Store the start and end time to log the execution time
        var startTime = DateTime.UtcNow;
        
        // Run the replacement
        textReplacer.ConnectAndStartWork();
        
        var endTime = DateTime.UtcNow;
        
        var diff = endTime - startTime;
        Console.WriteLine("Execution time: " + diff.TotalMilliseconds + "ms");
    }
}