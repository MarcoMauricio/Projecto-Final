using System.Collections.Generic;
namespace Context.NLP.Services
{
    public abstract class AbstractService : IContextService
    {
        protected string Title { get; set; }
        protected string Text { get; set; }

        public MinedObject GetContext(string title, string text)
        {
            Title = title;
            if (text.Length > 5000) text = text.Remove(5000);
            Text = text.Replace("\n", " ");

            return new MinedObject
            {
                Summary = GetSummary(),
                Entities = GetEntities(),
                Classification = GetClassification(),
                Sentiment = GetSentiment(),
                Language = GetLanguage()
            };
        }

        protected abstract string GetLanguage();

        protected abstract string GetSentiment();

        protected abstract string GetClassification();

        protected abstract List<string> GetEntities();

        protected abstract List<string> GetSummary();
    }
}