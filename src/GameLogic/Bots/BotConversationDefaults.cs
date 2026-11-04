// <copyright file="BotConversationDefaults.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using System.Globalization;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// The conversations a server starts with (see <see cref="BotChatConfiguration.Conversations"/>):
/// English, each with a Polish translation. Kept apart from the rest of the default configuration,
/// since they are many.
/// </summary>
/// <remarks>
/// Like the other default lines, they are configuration data a server owner is expected to rewrite
/// and extend. Map, item and monster names stay English in the Polish lines - the game data names
/// them that way.
/// </remarks>
internal static class BotConversationDefaults
{
    /// <summary>
    /// Adds the default conversations; most fit any time of day, a few only the morning, the evening
    /// or the night.
    /// </summary>
    /// <param name="conversations">The list of the configuration.</param>
    internal static void AddTo(ICollection<BotConversation> conversations)
    {
        Add(
            conversations,
            BotConversationPlace.Town,
            ("great weather for exp today", "indeed, not a cloud over {map}", "świetna pogoda na expa dzisiaj", "fakt, ani chmurki nad {map}"),
            ("did you hear? a red dragon was seen near devias", "then i'm staying in town today", "słyszałeś? podobno widzieli czerwonego smoka koło devias", "to ja dziś zostaję w mieście"),
            ("my potions are running low", "the shop here sells them cheap", "kończą mi się potki", "w sklepie tutaj są tanie"),
            ("anyone know a good spot for exp?", "lost tower, if you can take the hits", "zna ktoś dobre miejsce na expa?", "lost tower, jak wytrzymasz bicie"),
            ("i heard the blacksmith raised his prices", "everyone raises prices, except the monsters", "kowal podobno podniósł ceny", "wszyscy podnoszą ceny, tylko potwory nie"),
            ("what a crowd today", "everyone wants that jewel of bless", "ale tłok dzisiaj", "każdy chce ten jewel of bless"),
            ("how much is a jewel of soul now?", "too much, as always", "ile teraz chodzi jewel of soul?", "za dużo, jak zawsze"),
            ("i failed +7 again...", "the chaos machine hates us all", "znowu spaliłem +7...", "chaos machine nas wszystkich nienawidzi"),
            ("ever tried blood castle solo?", "once. never again", "próbowałeś kiedyś bc solo?", "raz. nigdy więcej"),
            ("the guards here look bored", "better bored than busy", "strażnicy tu wyglądają na znudzonych", "lepiej znudzeni niż zajęci"),
            ("have you seen my wings? oh wait, i don't have any", "one day, friend, one day", "widziałeś moje skrzydła? a nie, nie mam", "kiedyś, stary, kiedyś"),
            ("need a break from killing spiders", "spiders never take a break", "muszę odpocząć od pająków", "pająki nie robią przerw"),
            ("i dropped a jewel of chaos yesterday", "lucky you, i got only apples", "wczoraj wypadł mi jewel of chaos", "szczęściarz, mi tylko jabłka"),
            ("who sells the best armor in town?", "nobody, you have to find it out there", "kto sprzedaje najlepszą zbroję w mieście?", "nikt, trzeba ją znaleźć za murami"),
            ("is devil square open soon?", "check the clock, every few hours", "niedługo devil square?", "patrz na zegar, co kilka godzin"),
            ("i miss the old days in lorencia", "the budge dragons miss you too", "tęsknię za starymi czasami w lorencii", "budge dragony też za tobą tęsknią"),
            ("my sword got dull again", "the repair guy will love you", "znowu stępił mi się miecz", "naprawiacz cię pokocha"),
            ("anyone seen the golden goblins?", "they run faster than my wallet empties", "ktoś widział złote gobliny?", "uciekają szybciej niż pustoszeje mi sakiewka"),
            ("time for a quick rest", "rest is for the weak. and for me", "czas na krótki odpoczynek", "odpoczynek jest dla słabych. i dla mnie"),
            ("i heard kundun is waking up", "let him sleep, please", "słyszałem, że kundun się budzi", "niech śpi, błagam"),
            ("the elves are singing again", "noria always sounds like a song", "elfy znów śpiewają", "noria zawsze brzmi jak piosenka"),
            ("lag?", "nope, just you", "lag?", "nie, tylko u ciebie"),
            ("i should finally reset", "and lose all those levels? brave", "w końcu powinienem zrobić reset", "i stracić te wszystkie levele? odważnie"),
            ("where do you sell your junk?", "the merchant takes anything", "gdzie sprzedajesz śmieci?", "kupiec bierze wszystko"),
            ("nice armor, where did you get it?", "found it on a skeleton. he didn't need it anymore", "fajna zbroja, skąd masz?", "znalazłem przy szkielecie. już mu się nie przyda"),
            ("did the gm say anything about an event?", "not yet, keep an eye on the chat", "gm mówił coś o evencie?", "jeszcze nie, patrz na czat"),
            ("ugh, i died twice in atlans", "the bahamuts don't forgive", "eh, dwa razy padłem w atlans", "bahamuty nie wybaczają"),
            ("how long have you been playing today?", "too long to admit", "długo dziś grasz?", "za długo, żeby się przyznać"),
            ("{map} is lively today", "the market always is", "{map} dziś tętni życiem", "rynek zawsze tętni"),
            ("i bought too many potions again", "never too many, trust me", "znowu kupiłem za dużo potek", "nigdy za dużo, uwierz mi"),
            ("the sky looks strange today", "maybe a dragon is coming", "niebo dziś dziwnie wygląda", "może leci smok"),
            ("{partner}, still selling that old shield?", "it's a classic, not old", "{partner}, dalej sprzedajesz tę starą tarczę?", "to klasyk, a nie stara"));
        Add(
            conversations,
            BotConversationPlace.Hunting,
            ("these monsters never end", "keep swinging, we're almost done", "te potwory się nie kończą", "bij dalej, zaraz koniec"),
            ("watch out, behind you!", "got it, thanks", "uważaj, za tobą!", "mam go, dzięki"),
            ("my mana is almost gone", "drink something, quick", "kończy mi się mana", "wypij coś, szybko"),
            ("nice hit!", "lucky one, don't expect it again", "niezły cios!", "fart, nie licz na powtórkę"),
            ("did something drop?", "just zen, as usual", "coś wypadło?", "tylko zen, jak zwykle"),
            ("this spot is good for exp", "until someone takes it", "dobre miejsce na expa", "dopóki ktoś go nie zajmie"),
            ("i need a break soon", "one more round, then we rest", "zaraz muszę odpocząć", "jeszcze jedna runda i przerwa"),
            ("my armor won't last long", "then don't get hit", "zbroja długo nie pociągnie", "to nie daj się trafić"),
            ("they hit harder than i thought", "that's why we go together", "biją mocniej, niż myślałem", "dlatego chodzimy razem"),
            ("look, a jewel!", "wait, it's just an apple", "patrz, jewel!", "czekaj, to tylko jabłko"),
            ("almost leveled up", "nice, keep going", "prawie level", "nieźle, dawaj dalej"),
            ("is it me or are there more of them?", "it's not you, they keep coming", "to ja czy jest ich coraz więcej?", "to nie ty, ciągle przychodzą"),
            ("pull them to me", "careful what you wish for", "ściągaj je do mnie", "uważaj, czego sobie życzysz"),
            ("how much exp do you need?", "too much, as always", "ile ci brakuje expa?", "za dużo, jak zawsze"),
            ("i hate the ones with poison", "everyone does", "nienawidzę tych z trucizną", "wszyscy nienawidzą"),
            ("stay close, {partner}", "right behind you", "trzymaj się blisko, {partner}", "jestem tuż za tobą"),
            ("this map is full of surprises", "mostly the bad kind", "ta mapa jest pełna niespodzianek", "głównie tych złych"),
            ("my hands are tired from clicking", "the monsters don't care", "ręce mnie bolą od klikania", "potwory to nie obchodzi"),
            ("we should try a harder map", "after the next level", "powinniśmy spróbować trudniejszej mapy", "po następnym levelu"),
            ("did you see that crit?", "i did, now do it again", "widziałeś tego kryta?", "widziałem, teraz zrób to jeszcze raz"),
            ("one more and i go repair", "you said that ten monsters ago", "jeszcze jeden i idę naprawiać", "mówiłeś to dziesięć potworów temu"),
            ("not much drop here", "but good exp", "mały drop tutaj", "ale exp dobry"),
            ("the boss should spawn soon", "then we wait", "boss powinien się zaraz pojawić", "to czekamy"),
            ("my potions are gone", "take a few of mine... joking", "skończyły mi się potki", "weź kilka moich... żartuję"),
            ("{map} at this hour, nice", "less people, more monsters", "{map} o tej porze, fajnie", "mniej ludzi, więcej potworów"));
        Add(
            conversations,
            BotConversationPlace.Anywhere,
            ("how is your day going?", "better now, thanks", "jak ci mija dzień?", "teraz lepiej, dzięki"),
            ("have you been to kalima?", "once, i still have nightmares", "byłeś w kalimie?", "raz, do dziś mam koszmary"),
            ("do you know a good guild?", "a few, ask around in town", "znasz jakąś dobrą gildię?", "kilka, popytaj w mieście"),
            ("i wonder what's behind the next map", "more monsters, probably", "ciekawe, co jest za następną mapą", "pewnie więcej potworów"),
            ("anyone up for blood castle later?", "sure, if we find a third", "ktoś na bc później?", "jasne, jak znajdziemy trzeciego"),
            ("looking for a party, you?", "always", "szukam grupy, a ty?", "zawsze"),
            ("remember when we were level 1?", "the spiders were scary back then", "pamiętasz, jak mieliśmy level 1?", "pająki były wtedy straszne"),
            ("the server runs smooth today", "no lag, no complaints", "serwer dziś dobrze chodzi", "zero lagów, zero narzekań"),
            ("what's your favorite map?", "the one where i don't die", "jaka twoja ulubiona mapa?", "ta, na której nie ginę"),
            ("do you think the dragons are real?", "ask the ones who met them", "myślisz, że smoki istnieją?", "zapytaj tych, którzy je spotkali"),
            ("gm online?", "dunno, didn't see any", "jest gm?", "nie wiem, nie widziałem"),
            ("so many people today", "the more the merrier", "dużo ludzi dzisiaj", "im więcej, tym weselej"),
            ("i should sleep", "one more level first", "powinienem iść spać", "najpierw jeszcze jeden level"),
            ("have you seen a dark lord around?", "he was looking for his horse", "widziałeś gdzieś dark lorda?", "szukał swojego konia"),
            ("what's the best class?", "the one you enjoy", "jaka klasa jest najlepsza?", "ta, którą lubisz"),
            ("need a party for exp", "same here, let's wait for someone", "potrzebna grupa na expa", "też, poczekajmy na kogoś"));
        AddAt(
            conversations,
            BotConversationPlace.Anywhere,
            BotChatTimeOfDay.Morning,
            ("morning! coffee first", "potions first, coffee later", "dzień dobry! najpierw kawa", "najpierw potki, kawa później"),
            ("up early today?", "the early elf catches the drop", "wcześnie dziś wstałeś?", "kto rano wstaje, temu drop wypada"));
        AddAt(
            conversations,
            BotConversationPlace.Anywhere,
            BotChatTimeOfDay.Evening,
            ("long day at work", "this is the best part of it", "długi dzień w pracy", "to jego najlepsza część"),
            ("dinner time soon", "one more spot first", "zaraz kolacja", "najpierw jeszcze jeden spot"));
        AddAt(
            conversations,
            BotConversationPlace.Anywhere,
            BotChatTimeOfDay.Night,
            ("still up?", "the monsters don't sleep, neither do i", "jeszcze nie śpisz?", "potwory nie śpią, ja też nie"),
            ("late night grind?", "can't sleep anyway", "nocny exp?", "i tak nie mogę spać"));
    }

    private static void Add(ICollection<BotConversation> conversations, BotConversationPlace place, params (string Opener, string Reply, string PolishOpener, string PolishReply)[] texts)
        => AddAt(conversations, place, BotChatTimeOfDay.Anytime, texts);

    private static void AddAt(ICollection<BotConversation> conversations, BotConversationPlace place, BotChatTimeOfDay when, params (string Opener, string Reply, string PolishOpener, string PolishReply)[] texts)
    {
        var polish = CultureInfo.GetCultureInfo("pl");
        foreach (var text in texts)
        {
            conversations.Add(new BotConversation
            {
                Opener = new LocalizedString(text.Opener).WithTranslation(polish, text.PolishOpener),
                Reply = new LocalizedString(text.Reply).WithTranslation(polish, text.PolishReply),
                Where = place,
                When = when,
            });
        }
    }
}
