using System.Collections.Generic;
using System.Linq;

namespace Context.NLP.Algorithms
{
    class NamedEntitySummary
    {
        /// <summary>
        /// Algoritmo que retorna as frases com o maior número de entidades
        /// </summary>
        /// 
        /// <param name="text">
        /// Texto a ser analisado
        /// </param>
        /// 
        /// <returns>
        /// Lista de frases ordenada de forma descendente pelo número de entidades presentes
        /// </returns>
        public static List<string> GetSummary(string text)
        {
            var entitiesList = NamedEntityExtraction.GetEntities(text);
            var sentences = new Dictionary<string, int>();
            foreach (
                var s in from s in text.Split('.')
                         from keyValuePair
                         in entitiesList
                         where s.Contains(keyValuePair)
                         select s)
            {
                int currentCount;
                sentences.TryGetValue(s, out currentCount);
                sentences[s] = currentCount + 1;
            }
            List<string> returnList = new List<string>();
            foreach (var keyValue in sentences.OrderByDescending(key => key.Value))
            {
                returnList.Add(keyValue.Key);
            }
            return returnList;
        }
    }
}
