(input) =>
let
    values = {
        {364492811489509376, "void-chat"},
		{143867839282020352, "general-csharp"},
		{169726586931773440, "beginner-0"},
		{610909097897754665, "beginner-1"},
		{663803973119115264, "advanced"},
		{401165307475132416, "architecture-and-tooling"},
		{156079822454390784, "web"},
		{169726357331378176, "gui"},
		{191922757452169216, "game-dev"},
		{169726485865824256, "database"},
		{175153581664501761, "mobile"},
		{598678594750775301, "roslyn"},
		{312132327348240384, "lowlevel"},
		{266990476366839808, "career-talk"},
		{172705163981750272, "code-review"},
		{486662788677238805, "up-for-grabs"},
		{578057213084434433, "creative-waywo"},
		{368103255307190272, "hangout-notes"},
		{359110900197621760, "vbdotnet"},
		{360513441628160001, "fsharp"},
		{269089786818592769, "code-horror"},
		{536023005164470303, "modix-development"},
		{712329296416473119, "build-feed"},
		{712329379681927189, "build-chat"},
		{402290219216404480, "data-etl-reporting"},
		{658824448236847124, "new-to-csharp"},
        {input, input}
    },
    Result = List.First(List.Select(values, each _{0}=input)){1}
in
    Result