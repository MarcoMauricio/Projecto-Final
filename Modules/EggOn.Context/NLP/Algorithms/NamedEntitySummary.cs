using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EggOn.Context.NLP.Algorithms
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
        public static string GetSummary(string text)
        {
            char[] delimiterChars = { '.', '\t' };
            var entitiesList = NamedEntityExtraction.GetEntities(text);
            var sentences = new Dictionary<string, int>();
            foreach (var s in text.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries))
                foreach (var keyValuePair in entitiesList)
                {
                    if (!s.Contains(keyValuePair)) continue;
                    int currentCount;
                    sentences.TryGetValue(s, out currentCount);
                    sentences[s] = currentCount + 1;
                }

            var summary = new StringBuilder();
            foreach (var keyValue in sentences.OrderByDescending(key => key.Value).Take(3))
            {
                summary.AppendLine(keyValue.Key + ".\n");
            }
            return summary.ToString();
        }
    }
}
