using System.Collections.Generic;
using System.Linq;


namespace NLP
{
    class NamedEntitySummary
    {
        /*
         * Method that allows the retrieval of summary with well-formed entities of a given text
         * The return value is a list of pair <Sentence , Number Of Entity Hits>
         * 
        */
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
