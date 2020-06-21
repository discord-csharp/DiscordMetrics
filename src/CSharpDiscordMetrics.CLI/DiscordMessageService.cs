using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CSharpDiscordMetrics.CLI.Models;
using Newtonsoft.Json;
using Serilog;

namespace CSharpDiscordMetrics.CLI
{
    public class DiscordMessageService
    {
        private readonly HttpClient _httpClient;
        private readonly string _outputPath;
        private readonly bool _doNotSplitChannelsIntoFiles;
        private readonly bool _combineMessagesIntoOneFile;
        
        private const string URL = "https://discord.com/api/v6/channels/{0}/messages?limit=100";
        private static readonly DateTime DateToStart = new DateTime(2020, 06, 01);

        public DiscordMessageService(HttpClient httpClient, string outputPath, 
            bool doNotSplitChannelsIntoFiles, bool combineMessagesIntoOneFile)
        {
            _httpClient = httpClient;
            _outputPath = outputPath;
            _doNotSplitChannelsIntoFiles = doNotSplitChannelsIntoFiles;
            _combineMessagesIntoOneFile = combineMessagesIntoOneFile;
        }
        
        private readonly Dictionary<string, long> _channelsToExamine = new Dictionary<string, long>
        {
            ["void-chat"] = 364492811489509376,
            ["general-csharp"] = 143867839282020352,
            ["beginner-0"] = 169726586931773440,
            ["beginner-1"] = 610909097897754665,
            ["advanced"] = 663803973119115264,
            ["architecture-and-tooling"] = 401165307475132416,
            ["web"] = 156079822454390784,
            ["gui"] = 169726357331378176,
            ["game-dev"] = 191922757452169216,
            ["database"] = 169726485865824256,
            ["mobile"] = 175153581664501761,
            ["roslyn"] = 598678594750775301,
            ["lowlevel"] = 312132327348240384,
            ["career-talk"] = 266990476366839808,
            ["code-review"] = 172705163981750272,
            ["up-for-grabs"] = 486662788677238805,
            ["creative-waywo"] = 578057213084434433,
            ["hangout-notes"] = 368103255307190272,
            ["vbdotnet"] = 359110900197621760,
            ["fsharp"] = 360513441628160001,
            ["code-horror"] = 269089786818592769,
            ["modix-development"] = 536023005164470303,
            ["build-feed"] = 712329296416473119,
            ["build-chat"] = 712329379681927189,
            ["data-etl-reporting"] = 402290219216404480,
            ["new-to-csharp"] = 658824448236847124,
        };
        
        public async Task GetMessages()
        {
            var fullPayload = new List<DiscordMessage>();
            
            foreach(var (name, id) in _channelsToExamine)
            {
                try
                {
                    Log.Debug("Processing channel {Name} ({Id})", name, id);
                    
                    var messagesInChannel = await ProcessChannel(id, name);

                    if (_combineMessagesIntoOneFile)
                    {
                        fullPayload.AddRange(messagesInChannel);
                    }
                    
                    Log.Debug("Done processing channel {Name} ({Id})", name, id);
                    
                    Log.Debug("Delaying before next channel...");
			
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
                catch(Exception ex)
                {
                    Log.Error(ex, "Failed collecting for channel {ChannelName}", name);
                }
            }
            
            if (_combineMessagesIntoOneFile)
            {
                Log.Information("Combining all messages into one file...");
                
                var serializedPayload = JsonConvert.SerializeObject(fullPayload);
                var filePath = Path.Combine(_outputPath, "csharpguild.full.json");
                await File.WriteAllTextAsync(filePath, serializedPayload);
                
                Log.Information("Combined CSharp guild messages into one file at {FilePath}", filePath);
            }
        }
        
        private async Task<List<DiscordMessage>> ProcessChannel(long id, string name)
        {
            long? messageIdToFetchFrom = null;

            var payloadForChannel = new List<DiscordMessage>();

            var numberOfRequests = 1;

            while (true)
            {
                Log.Debug("Performing request {NumberOfRequests} for channel {ChannelName} ({Id})", numberOfRequests, name, id);
                
                var requestUrl = string.Format(URL, id);

                if (messageIdToFetchFrom != null)
                {
                    requestUrl += $"&before={messageIdToFetchFrom.Value}";
                }

                var response = await _httpClient.GetAsync(requestUrl);
                var content = await response.Content.ReadAsStringAsync();

                if (content.Contains("Missing Access"))
                {
                    // we don't have access, at the moment, we won't care and just skip
                    Log.Warning("Do not have access to channel {ChannelName} ({Id}), skipping...", name, id);
                    break;
                }

                var deserialized = JsonConvert.DeserializeObject<List<DiscordMessage>>(content);

                var messagesToRecord = deserialized
                    .Where(x => x.Timestamp >= DateToStart)
                    .Where(x => x.IsUserMessage);

                if (!messagesToRecord.Any())
                {
                    Log.Debug("No more messages found for channel {ChannelName} ({Id})", name, id);
                    break;
                }

                payloadForChannel.AddRange(deserialized);

                var lastMessage = deserialized.Last();

                messageIdToFetchFrom = lastMessage.Id;
                
                Log.Debug("Presumably there will be more messages, set message head Id to {MessageId}", messageIdToFetchFrom);

                numberOfRequests++;
            }

            if (!_doNotSplitChannelsIntoFiles)
            {
                Log.Information("Adding channel messages into file...");
                
                var serializedPayload = JsonConvert.SerializeObject(payloadForChannel);
                var filePath = Path.Combine(_outputPath, $"{name}.json");
                await File.WriteAllTextAsync(filePath, serializedPayload);   
                
                Log.Information("Channel messages inserted into file at {FilePath}", filePath);
            }

            return payloadForChannel;
        }
    }
}