using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace EggOn.Context.NLP.Algorithms
{
    class NamedEntityExtraction
    {
        private static readonly List<string> FiltersEnglish = new List<string>
        {
            "--",
            "-",
            "a",
            "about",
            "again",
            "all",
            "along",
            "almost",
            "also",
            "always",
            "am",
            "among",
            "an",
            "and",
            "another",
            "any",
            "anybody",
            "anything",
            "anywhere",
            "apart",
            "are",
            "around",
            "as",
            "at",
            "be",
            "because",
            "been",
            "before",
            "being",
            "between",
            "both",
            "but",
            "by",
            "can",
            "cannot",
            "comes",
            "could",
            "couldn",
            "did",
            "didn",
            "different",
            "do",
            "does",
            "doesn",
            "done",
            "don",
            "down",
            "during",
            "each",
            "either",
            "enough",
            "etc",
            "even",
            "every",
            "everybody",
            "everything",
            "everywhere",
            "except",
            "few",
            "final",
            "first",
            "for",
            "from",
            "get",
            "go",
            "goes",
            "gone",
            "good",
            "got",
            "had",
            "has",
            "have",
            "having",
            "he",
            "hence",
            "her",
            "him",
            "his",
            "how",
            "however",
            "I",
            "i.e",
            "if",
            "in",
            "initial",
            "into",
            "is",
            "isn",
            "it",
            "its",
            "it",
            "itself",
            "just",
            "last",
            "least",
            "less",
            "let",
            "lets",
            "let's",
            "like",
            "lot",
            "made",
            "make",
            "many",
            "may",
            "maybe",
            "me",
            "might",
            "mine",
            "more",
            "most",
            "Mr",
            "much",
            "must",
            "my",
            "near",
            "need",
            "next",
            "niether",
            "no",
            "nobody",
            "nor",
            "not",
            "nothing",
            "now",
            "nowhere",
            "of",
            "off",
            "often",
            "oh",
            "ok",
            "okay",
            "on",
            "once",
            "one",
            "only",
            "onto",
            "or",
            "other",
            "our",
            "ours",
            "out",
            "over",
            "own",
            "perhaps",
            "previous",
            "quite",
            "rather",
            "re",
            "really",
            "s",
            "said",
            "same",
            "say",
            "see",
            "seems",
            "several",
            "shall",
            "she",
            "should",
            "shouldn't",
            "since",
            "so",
            "some",
            "somebody",
            "something",
            "somewhere",
            "still",
            "stuff",
            "such",
            "than",
            "t",
            "that",
            "the",
            "their",
            "theirs",
            "them",
            "then",
            "there",
            "these",
            "they",
            "thing",
            "things",
            "this",
            "those",
            "through",
            "thus",
            "to",
            "too",
            "top",
            "two",
            "under",
            "unless",
            "until",
            "up",
            "upon",
            "us",
            "use",
            "v",
            "ve",
            "very",
            "want",
            "was",
            "we",
            "well",
            "went",
            "were",
            "what",
            "when",
            "where",
            "which",
            "while",
            "who",
            "whom",
            "why",
            "will",
            "with",
            "without",
            "won",
            "would",
            "x",
            "yes",
            "yet",
            "you",
            "you",
            "your",
            "yours"
        };
        /// Lista de palavras que não devem ser consideradas
        private static readonly List<string> Filters = new List<string>
        {
            "apesar","a","abaixo","acaso","acerca","acima","acolá","adiante","agora","ah","ah-ah","ai","aí","ao",
            "ainda","além","algo","alguém","algum","alguns","algures","alhures","ali","aliás","alô",
            "ambos","amanhã","anterior","anteriormente","antes","antigamente","aonde","apart",
            "apenas","aquela","aquelas","aquele","aqueles","aquilo","aquém","aqui","aquilo","as",
            "assim","até","atrás","através","atual","atualmente","bastante","bem","bom","cá","cada",
            "caminho","causa","cedo","certamente","chamada","chamado","co","coisas","colocado",
            "colocar","com","comigo","como","contudo","connosco","couldn","cujo","d","de","da","das",
            "dantes","do","dos","define","deixar","dela","dele","deles","demais","demasiado","demasiadamente",
            "decerto","depois","depressa","desde","desligado","devagar","deve","deveria","didn","diferente",
            "directamente","disse","disso","dito","diz","deste","destes","desta","destas","dois","don","e","é","eis","efectivamente","ela",
            "elas","ele","eles","Eles","eles","em","enfim","enquanto","Enquanto","então","entre","era","eram",
            "éramos","eras","éreis","és","esta","estas","está","estais","estamos","estão","estar","estás","estava",
            "estavam","estávamos","estavas","estáveis","este","estes","esteve","estive","estivemos","estiveram",
            "esse","esses","essa","essas","estiveste","estou","etc","eu","excepcionalmente","excepto","exceto","exclusivamente","faz","fazer",
            "feito","fez","final","finalizado","foi","fomos","fora","foram","foste","fostes","frequente","fui","há","hoje",
            "i.e","ides","inclusivamente","inicial","ir","isn","isso","isto","it's","itself","já","jamais","ligado","ll","lá","logo",
            "m","mais","mal","mas","máximo","melhor","menor","menos","mesmo","meu","minha","muito","muitos","must",
            "nada","não","nele","nesta","nem","nisso","nither","no","na","nos","nas","Nos","nós","nossa","nosso","novamente",
            "nt","o","obter","obtido","Oh","ok","okay","onde","ora","os","ou","outra","outrem","outro","outrora","outrossim",
            "par","para","parada","parado","parece","pela","pelo","pelos","pelas","pensa","pensar","perto","pode","podem","podia","por","porém",
            "porque","porquê","pot","pouco","poucos","precisa","prefer","preferia","preferir","primeiro","principalmente",
            "primeiramente","provavelmente","própria","própria","próprio","próximo","qualquer","Quando","quando",
            "quanto","quão","quase","quatro","que","quem","Quem","quer","re","realmente","repetir","s","sabe","salvo",
            "são","se","seguinte","sem","sempre","sendo","ser","seu","sim","sob","sobre","sois","somente","somos","sou",
            "Sr","sua","suficiente","t","tal","Talvês","talvez","também","tanto","tão","tem","têm","temos","tendes","tenho",
            "tens","ter","teu","teve","tinha","tinham","tínhamos","tinhas","tínheis","tive","tivemos","tiveram","tiveste","tivestes",
            "tipo","todo","todos","topo","três","tu","tua","tudo","último","um","uma","unicamente","up","ultimamente","use","vai","vais",
            "vamos","vão","várias","vários","ve","vê","vem","vistas","você","vocês","vós","vosso","vou"
        };


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
            var entities = new Dictionary<string, double>();
            _filtertextwithregex(text, entities, "[\"A-Z|À|È|Ì|Ò|Ù|Á|É|Í|Ó|Ú|Â|Ê|Î|Ô|Û|Ã|Õ|Ç|ð][a-z|à|è|ì|ò|ù|á|é|í|ó|ú|â|ê|î|ô|û|ã|õ|ç|ð]+(\\s[de|da|e|ou|of the|of|and|or|e de]*)[A-Z|À|È|Ì|Ò|Ù|Á|É|Í|Ó|Ú|Â|Ê|Î|Ô|Û|Ã|Õ|Ç|ð][a-z|à|è|ì|ò|ù|á|é|í|ó|ú|â|ê|î|ô|û|ã|õ|ç|ð\"]+", 5);
            _filtertextwithregex(text, entities, "(\"[A-Z|À|È|Ì|Ò|Ù|Á|É|Í|Ó|Ú|Â|Ê|Î|Ô|Û|Ã|Õ|Ç|ð][a-z|à|è|ì|ò|ù|á|é|í|ó|ú|â|ê|î|ô|û|ã|õ|ç|ð]+)(\\s)([A-Z|À|È|Ì|Ò|Ù|Á|É|Í|Ó|Ú|Â|Ê|Î|Ô|Û|Ã|Õ|Ç|ð][a-z|à|è|ì|ò|ù|á|é|í|ó|ú|â|ê|î|ô|û|ã|õ|ç|ð]+)\"", 3);
            _filtertextwithregex(text, entities, "[\\w]+", 0.5);


            return (from kv in entities orderby kv.Value select kv)
                .OrderByDescending(key => key.Value)
                .Take(15)
                .Select(keyValue => keyValue.Key).
                ToList();
        }

        /// <summary>
        /// Utiliza uma expressão regular para extrair entidades e verificar número de vezes que essa entidade ocorre no texto.
        /// Utiliza um método auxiliar para verificar se a entidade encontrada não se encontra na lista de filtros.
        /// </summary>
        /// <param name="text">
        ///     Texto a ser analisado
        /// </param>
        /// <param name="entitiesDictionary">
        ///     entitiesDictionary Dicionário que representa o par NomeEntidade e número de vezes que foi encontrada no texto
        /// </param>
        /// <param name="regularExpression">
        ///     regularExpression expressão regular a encontrar
        /// </param>
        /// <param name="weight">
        /// 
        /// </param>
        /// <returns>
        /// Texto sem as entidades encontradas
        /// </returns>
        private static void _filtertextwithregex(string text, Dictionary<string, double> entitiesDictionary, string regularExpression, double weight)
        {
            if (entitiesDictionary == null) throw new ArgumentNullException("entitiesDictionary");

            var regex = new Regex(regularExpression, RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
            foreach (
                      var match in regex.Matches(text)
                          .Cast<Match>()
                          .Select(itemmatch => itemmatch.ToString())
                          .Where(IsValidEntity))
            {
                double currentcount;
                entitiesDictionary.TryGetValue(match, out currentcount);
                entitiesDictionary[match] = currentcount + weight;
                text = text.Replace(match, "");
            }
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
            return Filters.All(filter => !match.ToLower().Equals(filter)) && FiltersEnglish.All(filter => !match.ToLower().Equals(filter));

        }
    }
}

