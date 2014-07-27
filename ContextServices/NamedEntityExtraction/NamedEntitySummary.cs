using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NamedEntityExtraction
{
    internal class NamedEntitySummary
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
        protected internal static string GetSummary(string text)
        {
            char[] delimiterChars = { '.', '\t' };
            var entitiesList = NamedEntitiesExtraction.GetEntities(text);
            var sentences = new Dictionary<string, int>();
            foreach (var s in from s in text.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries) from keyValuePair in entitiesList where s.Contains(keyValuePair) select s)
            {
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
