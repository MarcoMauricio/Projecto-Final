using NLP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EggOn.Files.Utils
{
    public class LocalService : IContextService
    {

        private string Title { get; set; }
        private string Text { get; set; }
  
        public Context GetContext(string title, string text)
        {
            Title = title;
            Text = text;
            return new Context
            {
                Summary = GetSummary(),
                Entities = GetEntities(),
                Classification = GetClassification(),
                Sentiment = GetSentiment(),
                Language = GetLanguage()
            };
        }

        private string GetLanguage()
        {
            return "Not Implemented";
        }

        private string GetSentiment()
        {
            return "Not Implemented";
        }

        private string GetClassification()
        {
            return TextClassifier.Classify(Text);
        }

        private List<string> GetEntities()
        {
            return NamedEntityExtraction.GetEntities(Text);
        }

        private string GetSummary()
        {
            return NamedEntitySummary.GetSummary(Text);
        }

    }
}
