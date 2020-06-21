using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CSharpDiscordMetrics.CLI.Models
{
    public class DiscordMessage
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("type")]
        public long Type { get; set; }
	
        public bool IsUserMessage => Type == 0;

        [JsonProperty("content")]
        public string Content { get; set; } = null!;

        [JsonProperty("channel_id")]
        public long ChannelId { get; set; }

        [JsonProperty("author")]
        public Author Author { get; set; } = null!;

        public string AuthorName => $"{Author.Username}#{Author.Discriminator}";

        [JsonProperty("mentions")]
        public List<Author>? Mentions { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    public class Author
    {
        [JsonProperty("id")]
        public string Id { get; set; } = null!;

        [JsonProperty("username")]
        public string Username { get; set; } = null!;

        [JsonProperty("discriminator")]
        public long Discriminator { get; set; }
    }
}