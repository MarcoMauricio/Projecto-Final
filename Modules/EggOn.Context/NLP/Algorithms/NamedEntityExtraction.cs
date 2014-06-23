using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Context.NLP.Algorithms
{
    public class NamedEntityExtraction
    {
        private static readonly List<string> FiltersEnglish = new List<string>{
            /*A*/   "and","all", "another", "any", "anybody", "anyone", "anything", "an","a","aboard","abnormally","about","abroad","absentmindedly","absolutely","abundantly","accidentally","accordingly","actively","actually","acutely","admiringly","affectionately","affirmatively","after","afterwards","agreeably","almost","already","always","amazingly","angrily","annoyingly","annually","anxiously","anyhow","anyplace","anyway","anywhere","appreciably","appropriately","around","arrogantly","aside","assuredly","astonishingly","away","awfully","awkwardly",
            /*B*/   "both","but","badly","barely","bashfully","beautifully","before","begrudgingly","believably","bewilderedly","bewilderingly","bitterly","bleakly","blindly","blissfully","boldly","boastfully","boldly","boyishly","bravely","briefly","brightly","brilliantly","briskly","brutally","busily",
            /*C*/   "calmly","candidly","carefully","carelessly","casually","cautiously","certainly","charmingly","cheerfully","chiefly","childishly","cleanly","clearly","cleverly","closely","cloudily","clumsily","coaxingly","coincidentally","coldly","colorfully","commonly","comfortably","compactly","compassionately","completely","confusedly","consequently","considerably","considerately","consistently","constantly","continually","continuously","coolly","correctly","courageously","covertly","cowardly","crazily","crossly","cruelly","cunningly","curiously","currently","customarily","cutely",
            /*D*/   "daily","daintily","dangerously","daringly","darkly","dastardly","dearly","decently","deeply","defiantly","deftly","deliberately","delicately","delightfully","densely","diagonally","differently","diligently","dimly","directly","disorderly","divisively","docilely","dopily","doubtfully","down","dramatically","dreamily","during",
            /*E*/   "each", "either" ,"everybody", "everyone", "everything","eagerly","early","earnestly","easily","efficiently","effortlessly","elaborately","eloquently","elegantly","elsewhere","emotionally","endlessly","energetically","enjoyably","enormously","enough","enthusiastically","entirely","equally","especially","essentially","eternally","ethically","even","evenly","eventually","evermore","every","everywhere","evidently","evocatively","exactly","exceedingly","exceptionally","excitedly","exclusively","explicitly","expressly","extensively","externally","extra","extraordinarily","extremely",
            /*F*/   "fairly","faithfully","famously","far","fashionably","fast","fatally","favorably","ferociously","fervently","fiercely","fiery","finally","financially","finitely","fluently","fondly","foolishly","forever","formally","formerly","fortunately","forward","frankly","frantically","freely","frequently","frenetically","fully","furiously","furthermore",
            /*G*/   "generally","generously","genuinely","gently","genuinely","girlishly","gladly","gleefully","gracefully","graciously","gradually","gratefully","greatly","greedily","grimly","grudgingly",
            /*H*/   "he", "her", "hers", "herself","his" ,"him", "himself", "his","habitually","half-heartedly","handily","handsomely","haphazardly","happily","hastily","harmoniously","harshly","hastily","hatefully","hauntingly","healthily","heartily","heavily","helpfully","hence","highly","hitherto","honestly","hopelessly","horizontally","hourly","how","however","hugely","humorously","hungrily","hurriedly","hysterically",
            /*I*/   "i", "it", "its" ,"itself","instead","if", "in","icily","identifiably","idiotically","imaginatively","immeasurably","immediately","immensely","impatiently","impressively","inappropriately","incessantly","incorrectly","indeed","independently","indoors","indubitably","inevitably","infinitely","informally","infrequently","innocently","inquisitively","instantly","intelligently","intensely","intently","interestingly","intermittently","internally","invariably","invisibly","inwardly","ironically","irrefutably","irritably",
            /*J*/   "jaggedly","jauntily","jealously","jovially","joyfully","joylessly","joyously","jubilantly","judgmentally","just","justly",
            /*K*/   "keenly","kiddingly","kindheartedly","kindly","knavishly","knottily","knowingly","knowledgeably","kookily",
            /*L*/   "lastly","late","lately","later","lazily","less","lightly","likely","limply","lithely","lively","loftily","longingly","loosely","loudly","lovingly","loyally","luckily","luxuriously",
            /*M*/   "my","many", "me", "mine", "more", "most" ,"much" ,"myself","madly","magically","mainly","majestically","markedly","materially","meaningfully","meanly","meantime","meanwhile","measurably","mechanically","medically","menacingly","merely","merrily","methodically","mightily","miserably","mockingly","monthly","morally","more","moreover","mortally","mostly","much","mysteriously",
            /*N*/   "neither" ,"no", "nobody", "none", "nothing","no","now","nastily","naturally","naughtily","nearby","nearly","neatly","needily","negatively","nervously","never","nevertheless","next","nicely","nightly","noisily","normally","nosily","not","now","nowadays","numbly",
            /*O*/   "one" ,"other" ,"others" ,"ours", "ourselves","obediently","obligingly","obnoxiously","obviously","occasionally","oddly","offensively","officially","often","ominously","once","only","openly","optimistically","orderly","ordinarily","outdoors","outrageously","outwardly","outwards","overconfidently","overseas",
            /*P*/   "painfully","painlessly","paradoxically","partially","particularly","passionately","patiently","perfectly","periodically","perpetually","persistently","personally","persuasively","physically","plainly","playfully","poetically","poignantly","politely","poorly","positively","possibly","potentially","powerfully","presently","presumably","prettily","previously","primly","principally","probably","promptly","properly","proudly","punctually","puzzlingly",
            /*Q*/   "quaintly","queasily","questionably","questioningly","quicker","quickly","quietly","quirkily","quite","quizzically",
            /*R*/   "randomly","rapidly","rarely","readily","really","reasonably","reassuringly","recently","recklessly","regularly","reliably","reluctantly","remarkably","repeatedly","reproachfully","reponsibly","resentfully","respectably","respectfully","restfully","richly","ridiculously","righteously","rightfully","rightly","rigidly","roughly","routinely","rudely","ruthlessly",
            /*S*/   "several", "she" ,"some" ,"somebody", "someone","sadly","safely","scarcely","scarily","scientifically","searchingly","secretively","securely","sedately","seemingly","seldom","selfishly","selflessly","separately","seriously","shakily","shamelessly","sharply","sheepishly","shoddily","shortly","shrilly","significantly","silently","simply","sincerely","singularly","shyly","skillfully","sleepily","slightly","slowly","slyly","smoothly","so","softly","solely","solemnly","solidly","silicitiously","somehow","sometimes","somewhat","somewhere","soon","specially","specifically","spectacularly","speedily","spiritually","splendidly","sporadically","spasmodically","startlingly","steadily","stealthily","sternly","still","strenuously","stressfully","strictly","structurally","studiously","stupidly","stylishly","subsequently","substantially","subtly","successfully","suddenly","sufficiently","suitably","superficially","supremely","surely","surprisingly","suspiciously","sweetly","swiftly","sympathetically","systematically",
            /*T*/   "that", "their", "theirs" ,"them", "themselves", "these", "they", "this", "those","the","temporarily","tenderly","tensely","tepidly","terribly","thankfully","then","there","thereby","thoroughly","thoughtfully","thus","tightly","today","together","tomorrow","too","totally","touchingly","tremendously","truly","truthfully","twice",
            /*U*/   "us","ultimately","unabashedly","unanimously","unbearably","unbelievably","unemotionally","unethically","unexpectedly","unfailingly","unfavorably","unfortunately","uniformly","unilaterally","unimpressively","universally","unnaturally","unnecessarily","unquestionably","unwillingly","up","upbeat","unkindly","upliftingly","upright","unselfishly","upside-down","unskillfully","upward","upwardly","urgently","usefully","uselessly","usually","utterly",
            /*V*/   "vacantly","vaguely","vainly","valiantly","vastly","verbally","vertically","very","viciously","victoriously","vigilantly","vigorously","violently","visibly","visually","vivaciously","voluntarily",
            /*W*/   "with","we", "what", "whatever", "which", "whichever" ,"who", "whoever", "whom" ,"whomever" ,"whose","warmly","weakly","wearily","weekly","well","wetly","when","where","while","whole-heartedly","wholly","why","wickedly","widely","wiggly","wildly","willfully","willingly","wisely","woefully","wonderfully","worriedly","worthily","wrongly",
            /*Y*/   "you", "your", "yours" ,"yourself" ,"yourselves","yearly","yearningly","yesterday","yet","youthfully",
            /*Z*/   "zanily","zealously","zestfully","zestily"};

        /// Lista de palavras que não devem ser consideradas
        private static readonly List<string> Filters = new List<string>{
            /*A*/   "a","as","antes","amanhã","ainda","antigamente","agora","aqui", "ali" ,"adiante", "abaixo" ,"aquém" ,"acolá", "atrás", "além", "aonde", "acima" , "algures","aí","aliás","assim","acaso","apenas","até","aquele","aquela","aqueles","aquelas","aquilo",
            /*B*/   "bem","bastante",
            /*C*/   "cedo","como","certamente","connosco","comigo",
            /*D*/   "dantes","depois","dentro","detrás","debaixo","depressa","devagar","demasiado","decerto",
            /*E*/   "então","enfim","efectivamente","exclusivamente","ele","eles","elas","eu", "este", "esta", "estes", "estas","esse", "essa", "esses", "essas",
            /*F*/   "fora",
            /*H*/   "hoje",
            /*I*/   "inclusivamente","isto","isso",
            /*J*/   "jamais","já",
            /*L*/   "logo","lá","longe",
            /*M*/   "melhor","mal","muito","mais","menos","meu","minha","meus","minhas",
            /*N*/   "não","nunca","nada","nem","nós","nos","nosso","nossa","nossos","nossas",
            /*O*/   "ontem","ora","outrora","onde","o","os",
            /*P*/   "primeiro","perto","posto","pior","principalmente","pouco","possivelmente","provavelmente","primeiramente",
            /*Q*/   "quase","quão","quanto","que",
            /*S*/   "sempre","sobretudo","sim","salvo","senão","somente","simplesmente","só","seu","sua","seus","suas",
            /*T*/   "tarde","tanto","tão","tudo","todo","talvez","também","teu","tua","teus","tuas",
            /*R*/   "realmente",
            /*U*/   "unicamente","ultimamente",
            /*V*/   "vocês","vós","vosso","vossa","vossos","vossas"   };


        /// <summary>
        /// Descrobre entidades presentes no texto.
        /// Utiliza um método auxiliar para a aplicação de um filtro através de uma expressão regular
        /// que representa a definição de "NamedEntity"
        /// </summary>
        /// 
        /// <param name="text">
        /// Texto a ser analisado para extração de entidades
        /// </param>
        /// 
        /// <returns>
        /// Lista de nomes das entidades ordenada de forma descendente pelo número de vezes que cada uma 
        /// das entidades foi encontrada quando o texto acaba de ser analisado
        /// </returns>
        public static List<string> GetEntities(string text)
        {
            Dictionary<string, int> entities;
            var temp = text;
            /// Filtra entidades compostas
            temp = _filtertextwithregex(temp, out entities, "([A-Z][a-z]*)[\\s-]([A-Z][a-z]*)");
            /// Filtra entitades duplas
            temp = _filtertextwithregex(temp, out entities, "[A-Z]([a-z]+|\\.)(?:\\s+[A-Z]([a-z]+|\\.))*(?:\\s+[a-z][a-z\\-]+){0,2}\\s+[A-Z]([a-z]+|\\.)");
            /// Filtra entidade simples ou separadas por hífens
            _filtertextwithregex(temp, out entities, "[A-Z][a-z|-]+");


            var sorted = (from kv in entities orderby kv.Value select kv).ToList().OrderByDescending(key => key.Value);
            List<string> returnList = new List<string>();
            foreach (var keyValue in sorted.ToList())
            {
                returnList.Add(keyValue.Key);
            }
            return returnList;
        }

        /// <summary>
        /// Utiliza uma expressão regular para extrair entidades e verificar número de vezes que essa entidade ocorre no texto.
        /// Utiliza um método auxiliar para verificar se a entidade encontrada não se encontra na lista de filtros.
        /// </summary>
        /// 
        /// <param name="text">
        /// Texto a ser analisado
        /// </param>
        /// 
        /// <param name="entitiesDictionary">
        /// entitiesDictionary Dicionário que representa o par NomeEntidade e número de vezes que foi encontrada no texto
        /// </param>
        /// 
        /// <param name="regularExpression">
        /// regularExpression expressão regular a encontrar
        /// </param>
        /// 
        /// <returns>
        /// Texto sem as entidades encontradas
        /// </returns>
        private static string _filtertextwithregex(string text, out Dictionary<string, int> entitiesDictionary, string regularExpression)
        {
            entitiesDictionary = new Dictionary<string, int>();
            var regex = new Regex(regularExpression);
            foreach (
                      var match in regex.Matches(text)
                          .Cast<Match>()
                          .Select(itemmatch => itemmatch.ToString())
                          .Where(IsValidEntity))
            {
                int currentcount;
                entitiesDictionary.TryGetValue(match, out currentcount);
                entitiesDictionary[match] = currentcount + 1;
                text = text.Replace(match, "");
            }
            return text;
        }
        /// <summary>
        /// Filtra palavras segundo a lista de filtros presente.
        /// </summary>
        /// <param name="match">
        /// String a ser filtrada
        /// </param>
        /// <returns>
        /// A não existencia da presença no inicio ou no fim de qualquer palavra presente na lista de filtros
        /// </returns>
        private static bool IsValidEntity(string match)
        {
            return FiltersEnglish.All(filter => !match.ToLower().StartsWith(filter) && !match.ToLower().Equals(filter) && !match.ToLower().EndsWith(filter));
        }
    }
}
