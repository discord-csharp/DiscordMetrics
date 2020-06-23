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
    },
    #"Converted to Table" = Table.FromList(channel_ids, Splitter.SplitByNothing(), null, null, ExtraValues.Error),
    #"Renamed Columns" = Table.RenameColumns(#"Converted to Table",{{"Column1", "Channel Ids"}}),
    #"Changed Type" = Table.TransformColumnTypes(#"Renamed Columns",{{"Channel Ids", Int64.Type}}),
    #"Invoked Custom Function" = Table.AddColumn(#"Changed Type", "Query1", each CallApi([Channel Ids])),
    #"Expanded Query1" = Table.ExpandListColumn(#"Invoked Custom Function", "Query1"),
    #"Expanded Query2" = Table.ExpandRecordColumn(#"Expanded Query1", "Query1", {"id", "type", "content", "channel_id", "author", "attachments", "embeds", "mentions", "mention_roles", "pinned", "mention_everyone", "tts", "timestamp", "edited_timestamp", "flags"}, {"Query1.id", "Query1.type", "Query1.content", "Query1.channel_id", "Query1.author", "Query1.attachments", "Query1.embeds", "Query1.mentions", "Query1.mention_roles", "Query1.pinned", "Query1.mention_everyone", "Query1.tts", "Query1.timestamp", "Query1.edited_timestamp", "Query1.flags"}),
    #"Changed Type1" = Table.TransformColumnTypes(#"Expanded Query2",{{"Channel Ids", Int64.Type}, {"Query1.id", Int64.Type}, {"Query1.type", Int64.Type}, {"Query1.content", type text}, {"Query1.channel_id", Int64.Type}, {"Query1.author", type any}, {"Query1.attachments", type any}, {"Query1.embeds", type any}, {"Query1.mentions", type any}, {"Query1.mention_roles", type any}, {"Query1.pinned", type logical}, {"Query1.mention_everyone", type logical}, {"Query1.tts", type logical}, {"Query1.timestamp", type datetime}, {"Query1.edited_timestamp", type datetime}, {"Query1.flags", Int64.Type}}),
    #"Invoked Custom Function1" = Table.AddColumn(#"Changed Type1", "IdToChannelName", each IdToChannelName([Channel Ids])) // list of our channel ids we want to loop through

in
    #"Invoked Custom Function1"