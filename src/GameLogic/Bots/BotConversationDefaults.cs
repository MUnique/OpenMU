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
        Add(
            conversations,
            BotConversationPlace.Town,
            ("did you see the guy with the shiny wings?", "it's the excellent options that count", "widziałeś gościa w tych błyszczących skrzydłach?", "liczą się opcje exe, nie blask"),
            ("i need a jewel of life for my sword", "good luck, they're rarer than dragons", "potrzebuję jewela life do miecza", "powodzenia, rzadsze niż smoki"),
            ("the merchant here never has what i need", "that's merchants for you", "kupiec tu nigdy nie ma tego, czego chcę", "tacy już są kupcy"),
            ("{partner}, you look stronger today", "new gloves, thanks for noticing", "{partner}, wyglądasz dziś groźniej", "nowe rękawice, dzięki że zauważyłeś"),
            ("anyone selling a pet?", "a dark horse would cost you a fortune", "ktoś sprzedaje peta?", "dark horse to majątek"),
            ("heard there's a castle siege soon", "my guild isn't ready, as always", "podobno niedługo castle siege", "moja gildia jak zwykle niegotowa"),
            ("the chaos machine keeps failing me", "try an item with luck", "chaos machine ciągle mi pali", "spróbuj z luckiem na itemie"),
            ("it's quiet in {map} today", "the calm before the invasion", "cicho dziś w {map}", "cisza przed inwazją"),
            ("my inventory is full of junk again", "welcome to the club", "znowu plecak pełen śmieci", "witaj w klubie"),
            ("should i buy a ticket for blood castle?", "only if you have a party", "kupić bilet na bc?", "tylko jak masz grupę"),
            ("how do you even get to icarus?", "with wings and patience", "jak się w ogóle dostać na icarus?", "ze skrzydłami i cierpliwością"),
            ("i'm saving zen for a jewel of soul", "i'm saving zen for repairs", "zbieram zen na jewel of soul", "a ja na naprawy"),
            ("the town music never gets old", "after ten years? it does", "ta muzyczka w mieście się nie nudzi", "po dziesięciu latach? nudzi"),
            ("{partner}, did you finish your quest?", "almost, one more item", "{partner}, skończyłeś questa?", "prawie, jeszcze jeden item"),
            ("i saw a golden budge dragon earlier", "and you didn't call me?", "widziałem wcześniej złotego budge dragona", "i nie zawołałeś?"),
            ("time to repair everything", "my wallet is crying already", "czas wszystko naprawić", "mój portfel już płacze"),
            ("what's the best set for a {class}?", "the one you can afford", "jaki set najlepszy dla {class}?", "ten, na który cię stać"),
            ("i got lost in the market again", "just follow the noise", "znowu się zgubiłem na rynku", "idź za hałasem"),
            ("someone sold me a +0 sword as +9", "always check before you trade", "ktoś mi wcisnął miecz +0 jako +9", "zawsze sprawdzaj przed handlem"),
            ("is the arena open today?", "for the brave, always", "arena dziś otwarta?", "dla odważnych zawsze"),
            ("nice horse, {partner}", "it's a rental", "fajny koń, {partner}", "wypożyczony"),
            ("do you ever take a day off?", "the monsters don't, so i don't", "robisz sobie czasem wolne?", "potwory nie robią, to ja też nie"),
            ("{map} looks different today", "maybe they painted the walls", "{map} dziś jakieś inne", "może pomalowali mury"),
            ("my pet ran away again", "pets have feelings too", "znowu mi pet uciekł", "pety też mają uczucia"),
            ("i should finally join a guild", "pick one with a nice emblem", "w końcu muszę dołączyć do gildii", "wybierz taką z ładnym herbem"),
            ("the shop ran out of large potions", "everyone's stocking up for the event", "w sklepie skończyły się duże potki", "wszyscy się zbroją na event"),
            ("how many resets do you have?", "enough to stop counting", "ile masz resetów?", "tyle, że przestałem liczyć"),
            ("who keeps dropping apples here?", "some generous soul", "kto tu ciągle rzuca jabłka?", "jakaś dobra dusza"),
            ("can you lend me some zen?", "nice try", "pożyczysz trochę zenu?", "niezła próba"),
            ("the safe zone feels so safe", "until someone sends a duel request", "w safe zonie tak bezpiecznie", "dopóki ktoś nie wyśle duela"));
        Add(
            conversations,
            BotConversationPlace.Hunting,
            ("careful, that one hits hard", "i noticed. with my face", "uważaj, ten mocno bije", "zauważyłem. twarzą"),
            ("pull fewer next time", "but it's faster this way", "następnym razem ściągaj mniej", "ale tak jest szybciej"),
            ("i'm almost out of arrows", "then punch them", "kończą mi się strzały", "to bij pięściami"),
            ("{partner}, heal please", "on it", "{partner}, ulecz pls", "już się robi"),
            ("this one dropped nothing again", "poor monsters, poor drops", "znowu nic nie wypadło", "biedne potwory, biedny drop"),
            ("that was a nice combo", "practice, lots of practice", "niezłe combo", "praktyka, dużo praktyki"),
            ("i think a jewel dropped over there", "go, i'll cover you", "chyba tam wypadł jewel", "leć, osłaniam cię"),
            ("the respawn here is crazy", "perfect for exp", "respawn tu jest szalony", "idealnie na expa"),
            ("my weapon is breaking", "one more pack, then repair", "broń mi się sypie", "jeszcze jedna paczka i naprawa"),
            ("i hate when they run away", "they know you're strong", "nie znoszę, jak uciekają", "wiedzą, że jesteś mocny"),
            ("another level soon", "you'll be stronger than me", "zaraz kolejny level", "zaraz będziesz mocniejszy ode mnie"),
            ("these skeletons are tougher than they look", "strong bones", "te szkielety są twardsze, niż wyglądają", "mają mocne kości"),
            ("did you hear that roar?", "let's not find out", "słyszałeś ten ryk?", "lepiej nie sprawdzajmy"),
            ("i need mana, badly", "blue potion, now", "potrzebuję many, pilnie", "niebieska potka, już"),
            ("this spot is ours now", "until the next party shows up", "ten spot jest teraz nasz", "dopóki nie przyjdzie następna grupa"),
            ("nice drop!", "just junk, as usual", "fajny drop!", "śmieć, jak zwykle"),
            ("keep them away from me", "i'm trying!", "trzymaj je ode mnie z daleka", "próbuję!"),
            ("one more and we move", "you always say that", "jeszcze jeden i ruszamy", "zawsze tak mówisz"),
            ("where did that one come from?", "they always spawn behind you", "skąd ten się wziął?", "zawsze spawnują się za plecami"),
            ("i'm getting good exp here", "same, let's stay a bit", "dobry tu exp", "u mnie też, zostańmy chwilę"),
            ("watch the poison!", "too late", "uważaj na truciznę!", "za późno"),
            ("{partner}, go left, i take right", "deal", "{partner}, idź w lewo, ja w prawo", "stoi"),
            ("do they ever stop coming?", "only when the server restarts", "one kiedyś przestają przychodzić?", "tylko jak serwer się restartuje"),
            ("my shield saved me there", "best zen you ever spent", "tarcza mnie uratowała", "najlepiej wydany zen"),
            ("that one is a boss, right?", "big, ugly, yes, a boss", "to boss, nie?", "duży, brzydki, tak, boss"),
            ("low hp, back off a bit", "go, i'll hold them", "mało hp, cofnij się trochę", "leć, trzymam je"),
            ("i love this map", "the monsters don't love you back", "uwielbiam tę mapę", "potwory nie odwzajemniają uczucia"),
            ("is it worth staying here?", "the exp says yes", "warto tu zostać?", "exp mówi, że tak"),
            ("i think we're too strong for this spot", "let's try the next map then", "chyba jesteśmy za mocni na ten spot", "to spróbujmy następnej mapy"),
            ("hey, that was my kill!", "sharing is caring", "ej, to był mój kill!", "trzeba się dzielić"));
        Add(
            conversations,
            BotConversationPlace.Anywhere,
            ("do you remember your first wings?", "like it was yesterday", "pamiętasz swoje pierwsze skrzydła?", "jakby to było wczoraj"),
            ("what are you saving for?", "a better weapon, as always", "na co zbierasz?", "na lepszą broń, jak zawsze"),
            ("{partner}, how long have you been playing?", "since the very beginning", "{partner}, od kiedy grasz?", "od samego początku"),
            ("do you play another class too?", "a little elf, for the buffs", "grasz jeszcze inną klasą?", "małą elfką, dla buffów"),
            ("i dream of a full excellent set", "we all do", "marzy mi się full exe set", "wszystkim się marzy"),
            ("they say the server got an update", "more monsters, i hope", "podobno serwer dostał update", "oby więcej potworów"),
            ("what's your favorite event?", "blood castle, no doubt", "jaki twój ulubiony event?", "blood castle, bez dwóch zdań"),
            ("i should write down my coords", "i always forget where i hunt", "muszę zapisywać koordy", "ja zawsze zapominam, gdzie expię"),
            ("would you trade your weapon for mine?", "not in a million years", "zamieniłbyś swoją broń na moją?", "za nic w świecie"),
            ("i miss my old guild", "they probably miss you too", "tęsknię za starą gildią", "oni za tobą pewnie też"),
            ("level {level} and still learning", "that never stops", "level {level} i dalej się uczę", "to się nigdy nie kończy"),
            ("do you play with the sound on?", "the music keeps me awake", "grasz z dźwiękiem?", "muzyka nie daje mi zasnąć"),
            ("{class} is the best class", "everyone says that about their class", "{class} to najlepsza klasa", "każdy tak mówi o swojej klasie"),
            ("let's do a quest together sometime", "sure, just ask", "zróbmy kiedyś razem questa", "jasne, daj znać"),
            ("have you seen the new players?", "everyone was new once", "widziałeś nowych graczy?", "każdy kiedyś był nowy"),
            ("i heard someone got a jewel of creation", "lucky one", "podobno ktoś wydropił jewel of creation", "szczęściarz"));
        AddWithRiposte(
            conversations,
            BotConversationPlace.Town,
            ("i finally got my wings", "nice! what level?", "level 2, the expensive ones", "w końcu mam skrzydła", "nieźle! jaki level?", "dwójki, te drogie"),
            ("guess who failed +10 again", "you?", "me. again.", "zgadnij, kto znowu spalił +10", "ty?", "ja. znowu."),
            ("lend me 10k zen?", "what for?", "potions. and maybe a pet", "pożyczysz 10k zenu?", "na co?", "na potki. i może peta"),
            ("{partner}, nice armor", "thanks, crafted it myself", "with a little help from the chaos goblin", "{partner}, fajna zbroja", "dzięki, sam zrobiłem", "z małą pomocą chaos goblina"),
            ("the guards here never move", "maybe they're bots", "haha, no way", "strażnicy tu w ogóle się nie ruszają", "może to boty", "haha, niemożliwe"),
            ("i'm going to devias", "the snow is lovely this time of year", "and the yetis are hungry", "idę do devias", "śnieg o tej porze jest piękny", "a yeti głodne"),
            ("ever been to the land of trials?", "once, by accident", "how do you get there by accident?", "byłeś kiedyś w land of trials?", "raz, przez przypadek", "jak tam się trafia przez przypadek?"),
            ("i need a party for bc", "i'm in", "great, one more and we go", "potrzebuję grupy na bc", "wchodzę", "super, jeszcze jeden i idziemy"));
        AddWithRiposte(
            conversations,
            BotConversationPlace.Hunting,
            ("i'll pull, you hit", "deal", "here they come!", "ja ściągam, ty bijesz", "stoi", "o, już idą!"),
            ("how's your hp?", "half", "drink, i'll hold them", "jak tam hp?", "połowa", "pij, ja je trzymam"),
            ("that was a jewel, right?", "no, a rock", "a very shiny rock", "to był jewel, nie?", "nie, kamień", "bardzo błyszczący kamień"),
            ("we're fast today", "it's the new weapon", "or the coffee", "szybko dziś idzie", "to ta nowa broń", "albo kawa"),
            ("careful, the big one", "i see him", "then why are you running?", "uważaj, ten duży", "widzę go", "to czemu uciekasz?"),
            ("one more level and i'm done", "you said that an hour ago", "and i meant it", "jeszcze jeden level i kończę", "mówiłeś to godzinę temu", "i wtedy też tak myślałem"));
        AddWithRiposte(
            conversations,
            BotConversationPlace.Anywhere,
            ("what are you doing tonight?", "the same as every night", "exp?", "co robisz wieczorem?", "to co co wieczór", "expisz?"),
            ("have you seen my old guild master?", "he quit last week", "again?", "widziałeś mojego starego mistrza gildii?", "odszedł w zeszłym tygodniu", "znowu?"),
            ("i think i'll try a new class", "which one?", "the one with the biggest sword", "chyba spróbuję nowej klasy", "której?", "tej z największym mieczem"),
            ("do you believe in lucky days?", "only when something drops", "then today isn't one", "wierzysz w szczęśliwe dni?", "tylko jak coś wypadnie", "to dziś nie jest taki dzień"),
            ("guess what dropped yesterday", "a jewel?", "an apple. a really nice one", "zgadnij, co mi wczoraj wypadło", "jewel?", "jabłko. bardzo ładne"),
            ("the server is busy today", "event soon?", "maybe, let's ask a gm", "dużo dziś ludzi na serwerze", "niedługo event?", "może, zapytajmy gma"));

        // A small easter egg: the bots know the open source projects this server and the client come from.
        AddWithRiposte(
            conversations,
            BotConversationPlace.Anywhere,
            ("openmu and mumain are doing a great job", "and how!", "open source, baby", "openmu i mumain robią robotę", "i to jaką!", "open source, ziomek"),
            ("you know the server and client of this game are open source?", "openmu on the server, mumain in the client", "and we're the bots in between", "wiesz, że serwer i klient tej gry to open source?", "openmu na serwerze, mumain w kliencie", "a my, boty, pomiędzy"),
            ("who made this server?", "openmu, written in c#", "so we're made of c# too?", "kto zrobił ten serwer?", "openmu, napisany w c#", "czyli my też jesteśmy z c#?"),
            ("my client runs on mumain now", "smooth?", "smoother than my combos", "mój klient chodzi teraz na mumain", "płynnie?", "płynniej niż moje combosy"));
        Add(
            conversations,
            BotConversationPlace.Anywhere,
            ("found a bug today", "report it to openmu, they fix them fast", "znalazłem dziś buga", "zgłoś do openmu, szybko łatają"),
            ("who do we thank for this world?", "the openmu and mumain folks", "komu dziękujemy za ten świat?", "ekipie openmu i mumain"),
            ("i'd like to help with openmu and mumain", "pull requests are welcome", "chciałbym pomóc przy openmu i mumain", "pull requesty mile widziane"));
        AddAt(
            conversations,
            BotConversationPlace.Anywhere,
            BotChatTimeOfDay.Day,
            ("lunch break exp?", "just one more level", "exp w przerwie na lunch?", "tylko jeden level"),
            ("busy day in {map}", "everyone's online after school", "dużo dziś ludzi w {map}", "wszyscy już po szkole"),
            ("the sun is high over {map}", "and the monsters are wide awake", "słońce wysoko nad {map}", "a potwory nie śpią"));
        AddAt(
            conversations,
            BotConversationPlace.Anywhere,
            BotChatTimeOfDay.Morning,
            ("morning! coffee first", "potions first, coffee later", "dzień dobry! najpierw kawa", "najpierw potki, kawa później"),
            ("up early today?", "the early elf catches the drop", "wcześnie dziś wstałeś?", "kto rano wstaje, temu drop wypada"),
            ("first coffee, then the dragons", "best time, the spots are empty", "najpierw kawa, potem smoki", "najlepsza pora, spoty puste"),
            ("good morning, {partner}", "morning, slept well?", "dzień dobry, {partner}", "dzień dobry, wyspany?"),
            ("morning exp is the best exp", "and the quietest", "poranny exp najlepszy", "i najcichszy"));
        AddAt(
            conversations,
            BotConversationPlace.Anywhere,
            BotChatTimeOfDay.Evening,
            ("long day at work", "this is the best part of it", "długi dzień w pracy", "to jego najlepsza część"),
            ("dinner time soon", "one more spot first", "zaraz kolacja", "najpierw jeszcze jeden spot"),
            ("finally home", "and finally online", "w końcu w domu", "i w końcu online"),
            ("good evening, {partner}", "good evening, ready for the events?", "dobry wieczór, {partner}", "dobry wieczór, gotowy na eventy?"),
            ("prime time, the server is full", "more people, more fun", "prime time, serwer pełny", "więcej ludzi, więcej zabawy"));
        AddAt(
            conversations,
            BotConversationPlace.Anywhere,
            BotChatTimeOfDay.Night,
            ("still up?", "the monsters don't sleep, neither do i", "jeszcze nie śpisz?", "potwory nie śpią, ja też nie"),
            ("late night grind?", "can't sleep anyway", "nocny exp?", "i tak nie mogę spać"),
            ("it's late, {partner}", "just one more level", "późno już, {partner}", "jeszcze tylko jeden level"),
            ("quiet night", "perfect for grinding", "cicha noc", "w sam raz na grind"),
            ("my eyes are closing", "drink a potion", "oczy mi się kleją", "wypij potkę"));
    }

    private static void Add(ICollection<BotConversation> conversations, BotConversationPlace place, params (string Opener, string Reply, string PolishOpener, string PolishReply)[] texts)
        => AddAt(conversations, place, BotChatTimeOfDay.Anytime, texts);

    private static void AddWithRiposte(ICollection<BotConversation> conversations, BotConversationPlace place, params (string Opener, string Reply, string Riposte, string PolishOpener, string PolishReply, string PolishRiposte)[] texts)
    {
        var polish = CultureInfo.GetCultureInfo("pl");
        foreach (var text in texts)
        {
            conversations.Add(new BotConversation
            {
                Opener = new LocalizedString(text.Opener).WithTranslation(polish, text.PolishOpener),
                Reply = new LocalizedString(text.Reply).WithTranslation(polish, text.PolishReply),
                Riposte = new LocalizedString(text.Riposte).WithTranslation(polish, text.PolishRiposte),
                Where = place,
            });
        }
    }

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
