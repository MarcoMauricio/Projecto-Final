using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Context.NLP.Algorithms
{
    public class NamedEntityExtraction
    {
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
            return Filters.All(filter => !match.ToLower().StartsWith(filter) && !match.ToLower().Equals(filter) && !match.ToLower().EndsWith(filter));
        }
    }
}
