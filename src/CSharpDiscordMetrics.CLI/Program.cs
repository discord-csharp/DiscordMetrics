using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace CSharpDiscordMetrics.CLI
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine(@"
        CCCCCCCCCCCCC                          
     CCC::::::::::::C     ######    ######     
   CC:::::::::::::::C     #::::#    #::::#     
  C:::::CCCCCCCC::::C     #::::#    #::::#     
 C:::::C       CCCCCC######::::######::::######
C:::::C              #::::::::::::::::::::::::#
C:::::C              ######::::######::::######
C:::::C                   #::::#    #::::#     
C:::::C                   #::::#    #::::#     
C:::::C              ######::::######::::######
C:::::C              #::::::::::::::::::::::::#
 C:::::C       CCCCCC######::::######::::######
  C:::::CCCCCCCC::::C     #::::#    #::::#     
   CC:::::::::::::::C     #::::#    #::::#     
     CCC::::::::::::C     ######    ######     
        CCCCCCCCCCCCC                          
");
            
            Console.WriteLine("CSharp Discord Metrics CLI");

            var logLevelSwitch = new LoggingLevelSwitch { MinimumLevel = LogEventLevel.Information };

            var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(logLevelSwitch)
                .WriteTo.Console();

            var rootCommand = new RootCommand
            {
                new Option<string?>(new[] {"--token", "-t"}, () => null, "Set output to be verbose"),
                new Option<bool>("--no-split", () => false, "Generates split JSON files for each channel in guild"),
                new Option<bool>("--combine", () => true, "Generates one JSON file at the end of the process, with all messages from all channels"),
                new Option<string?>("--output", () => "output", "Generates one JSON file at the end of the process, with all messages from all channels"),
                new Option<bool>("--verbose", () => false, "Set output to be verbose"),
                new Option<string?>("--log-file", () => null, "Path to log file"),
            };
            
            rootCommand.Description = "CSharp Discord Metrics CLI";
            
            rootCommand.Handler = CommandHandler.Create<string?, bool, bool, string?, bool, string?>(async (token, noSplit, combine, output, verbose, logFile) =>
            {
                if (verbose)
                {
                    logLevelSwitch.MinimumLevel = LogEventLevel.Debug;
                }

                if (!string.IsNullOrWhiteSpace(logFile))
                {
                    loggerConfiguration
                        .WriteTo.File(logFile!);
                }
                
                Log.Logger = loggerConfiguration.CreateLogger();

                while (string.IsNullOrWhiteSpace(token))
                {
                    Log.Warning("Discord token not provided, please provide a token now to use for authentication, find a token at: https://discord.com/developers/applications");

                    token = Console.ReadLine();
                }

                while (string.IsNullOrWhiteSpace(output) || !Directory.Exists(output))
                {
                    Log.Warning("Output path for JSON files not provided or doesn't exist, please provide a path now");

                    output = Console.ReadLine();
                }

                Log.Debug("Starting Discord metrics CLI with provided options");
                
                await Run(token!, output!, noSplit, combine);
            });
            
            await rootCommand.InvokeAsync(args);
        }

        private static async Task Run(string token, string output, bool doNotSplitChannelsIntoFiles, bool combineMessagesIntoOneFile)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bot", token);
            
            var service = new DiscordMessageService(client, output, doNotSplitChannelsIntoFiles, combineMessagesIntoOneFile);

            await service.GetMessages();
        }
    }
}