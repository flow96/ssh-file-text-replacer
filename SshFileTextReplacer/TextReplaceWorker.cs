using Microsoft.Extensions.Configuration;
using Renci.SshNet;

namespace SshFileTextReplacer;

public class TextReplaceWorker
{
    private readonly SshSettings? _settings;

    public TextReplaceWorker(IConfiguration configuration)
    {
        // Parse the settings from the appsettings.json
        _settings = SshSettings.ParseFromConfig(configuration);
    }

    public void ConnectAndStartWork()
    {
        if (_settings == null)
        {
            Console.WriteLine("Run cancelled due to invalid settings.");
            return;
        }

        SshClient client = new SshClient(_settings.ServerSettings.Host, _settings.ServerSettings.Username, _settings.ServerSettings.Password);
        try
        {
            client.Connect();
            if (client.IsConnected)
            {
                ReplaceLines(client);
                RunCommandsAfterExecution(client);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
        finally
        {
            if (client.IsConnected)
                client.Disconnect();
        }
    }

    
    private void ReplaceLines(SshClient client)
    {
        if (_settings == null)
            return;
        SshCommand cmd = client.CreateCommand($"cat {_settings.FilePath}");
        string result = cmd.Execute();

        // Find the first index 
        var replaceStartIndex = result.IndexOf(_settings.LineToFind, StringComparison.CurrentCultureIgnoreCase);
        if (replaceStartIndex >= 0)
        {
            // Search the linebreak and replace everything until there, to fully replace the line
            // Otherwise we will add up the same data when ran twice
            var startIndex = replaceStartIndex;
            while (startIndex > 0)
            {
                var part = result.Substring(startIndex - 1, 1);
                if (part.Equals("\n"))
                    break;
                startIndex--;
            }

            var lengthToReplace = _settings.LineToFind.Length + (replaceStartIndex - startIndex);
            var lineToBeReplaced = result.Substring(startIndex, lengthToReplace);

            result = result.Replace(lineToBeReplaced, _settings.LineToReplace);
            cmd = client.CreateCommand($"echo -n '{result}' > {_settings.FilePath}");
            cmd.Execute();

            Console.WriteLine("*****************************************************");
            Console.WriteLine($"Replaced: \t'{lineToBeReplaced}'");
            Console.WriteLine($"By: \t\t'{_settings.LineToReplace}'");
            Console.WriteLine("*****************************************************");
        }
        else
        {
            Console.WriteLine("************* ERROR *************");
            Console.WriteLine($" >> Given filter: '{_settings.LineToFind}' was not found in the given file.");
        }
    }

    private void RunCommandsAfterExecution(SshClient client)
    {
        if (_settings == null)
            return;
        if (_settings.CommandsToRunAfterwards.Length <= 0)
            return;
        Console.WriteLine();
        Console.WriteLine($" >> Running {_settings.CommandsToRunAfterwards.Length} command(s)");
        for (int i = 0; i < _settings.CommandsToRunAfterwards.Length; i++)
        {
            Console.WriteLine($"***** COMMAND {(i + 1)}/{_settings.CommandsToRunAfterwards.Length} *****");
            var cmdString = _settings.CommandsToRunAfterwards[i];
            SshCommand cmd = client.CreateCommand(cmdString);
            var result = cmd.Execute();
            Console.WriteLine($"$ {cmdString}");
            Console.WriteLine($"{result}");
        }
    }
}