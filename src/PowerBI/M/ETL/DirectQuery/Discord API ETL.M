let 
    channel_ids = {
        364492811489509376, // void-chat
        143867839282020352, // general
        169726586931773440, // beginner 0
        610909097897754665, // beginner 1
        663803973119115264, // advanced
        401165307475132416, // architecture
        156079822454390784, // web
        169726357331378176, // gui
        191922757452169216, // game dev
        169726485865824256, // database
        175153581664501761, // mobile
        598678594750775301, // roslyn
        312132327348240384, // lowlevel
        266990476366839808, // career-talk
        172705163981750272, // code-review
        486662788677238805, // up-for-grabs
        578057213084434433, // creative-waywo
        368103255307190272, // hangout-notes
        359110900197621760, // vbdotnet
        360513441628160001, // fsharp
        269089786818592769, // code-horror
        536023005164470303, // modix-development
        712329296416473119, // build-feed
        712329379681927189, // build-chat
        402290219216404480, // data-etl-reporting
        658824448236847124 // new-to-csharp
    }, // list of our channel ids we want to loop through

    startDate = DateTime.FromText("06-01-2020", "en-US"), // our start date
    token = "token", //bot token
    base_url = "https://discord.com/api/v6/channels/", //api url - csharp general channel
    api_headers = [#"Content-Type" = "application/x-www-form-urlencoded", Authorization = "Bot " & token, #"Keep-Alive" = "1"], //headers
    
    Loop = List.Accumulate(
                channel_ids, 
                0,
                (state, current) => 
                            Json.Document(
                                Web.Contents(
                                    Text.Combine(
                                        {base_url, Number.ToText(current), "/messages", "?limit=100"}
                                    ), 
                                [
                                    Headers = api_headers
                                ]
                        )
                    )
                ),
    #"Converted to Table" = Table.FromList(Loop, Splitter.SplitByNothing(), null, null, ExtraValues.Error),
    #"Expanded Column1" = Table.ExpandRecordColumn(#"Converted to Table", "Column1", {"id", "type", "content", "channel_id", "author", "attachments", "embeds", "mentions", "mention_roles", "pinned", "mention_everyone", "tts", "timestamp", "edited_timestamp", "flags"}, {"Column1.id", "Column1.type", "Column1.content", "Column1.channel_id", "Column1.author", "Column1.attachments", "Column1.embeds", "Column1.mentions", "Column1.mention_roles", "Column1.pinned", "Column1.mention_everyone", "Column1.tts", "Column1.timestamp", "Column1.edited_timestamp", "Column1.flags"}),
    #"Expanded Column1.author" = Table.ExpandRecordColumn(#"Expanded Column1", "Column1.author", {"username", "discriminator"}, {"Column1.author.username", "Column1.author.discriminator"})
in
    #"Expanded Column1.author"