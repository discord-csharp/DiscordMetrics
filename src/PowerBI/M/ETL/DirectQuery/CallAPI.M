(current) =>
let 
    token = "token", //bot token
    base_url = "https://discord.com/api/v6/channels/", //api url - csharp general channel
    api_headers = [#"Content-Type" = "application/x-www-form-urlencoded", Authorization = "Bot " & token, #"Keep-Alive" = "1"], //headers
    
    result = Json.Document(
                Web.Contents(
                    Text.Combine(
                        {base_url, Number.ToText(current), "/messages", "?limit=100"}
                    ), 
                [
                    Headers = api_headers
                ]
        )
    )
in
    result