public static class MultiLanguage
{
    public static Language lang = Language.En;
    public static char space = '\u202F';

    private static string[] textRu = new string[]
    {
        $"\"e\"{space}-{space}открыть", //0
        "заперто", //1
        $"\"e\"{space}-{space}взять", //2
        $"\"e\"{space}-{space}закрыть", //3
        $"Вы нашли записку. \"F\"{space}-{space}посмотреть", //4
        "недостаточно энергии. вставьте батарейки", //5
        "вставьте батарейки, когда будет менее 10%", //6
        "у вас нет батареек", //7
        "выкл", //8
        $"\"e\"{space}-{space}замок", //9
        "неверный код", //10
        "ошибка", //11
        "закрыть (\"F\")", //12
        "Вы пока не нашли<br>ни единой записки", //13
        "громкость звука", //14
        "чувствительность мыши", //15
        "назад", //16
        "сбросить", //17
        "главное меню", //18
        "настройки", //19
        "продолжить", //20
        "выйти (\"E\")", //21
        "открыть", //22
        "играть", //23
        "загрузка...", //24
        "конец", //25
        "вы смогли сбежать", //26
        "взять", //27
        "закрыть", //28
        "Вы нашли записку", //29
        "замок", //30
        "выйти", //31
        "*игра приостановлена*", //32
        "\"Tab\"<br>пауза", //33
    };

    private static string[] textEn = new string[]
    {
        $"\"e\"{space}-{space}open", //0
        "locked", //1
        $"\"e\"{space}-{space}take", //2
        $"\"e\"{space}-{space}close", //3
        $"You found the note. \"F\"{space}-{space}view", //4
        "not enough energy. insert the batteries", //5
        "Insert the batteries when it is less than 10%", //6
        "You don't have batteries", //7
        "off", //8
        $"\"e\"{space}-{space}lock", //9
        "invalid code", //10
        "error", //11
        "close (\"F\")", //12
        "You haven't found<br>a single note yet", //13
        "sound volume", //14
        "mouse sensitivity", //15
        "back", //16
        "reset", //17
        "main menu", //18
        "settings", //19
        "continue", //20
        "exit (\"E\")", //21
        "open", //22
        "play", //23
        "loading...", //24
        "The End", //25
        "You were able to escape", //26
        "take", //27
        "close", //28
        "You found the note", //29
        "lock", //30
        "exit", //31
        "*the game is paused*", //32
        "\"Tab\"<br>pause", //33
    };

    public static string GetText(int id)
    {
        if (lang == Language.Ru)
            return textRu[id];
        else
            return textEn[id];
    }

    public enum Language
    {
        Ru,
        En
    }
}
