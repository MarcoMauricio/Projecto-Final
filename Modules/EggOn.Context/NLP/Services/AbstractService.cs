using System.Collections.Generic;
using Context.NLP.Services;

namespace EggOn.Context.NLP.Services
{
    public abstract class AbstractService : IContextService
    {
        protected string Title { get; set; }
        protected string Text { get; set; }

        public MinedObject GetContext(string title, string text)
        {
            Title = title;
            Text = text.Replace("\n", " ");

            return new MinedObject
            {
                Summary = GetSummary(),
                Entities = GetEntities(),
                Category = GetCategory(),
                Sentiment = GetSentiment(),
                Language = GetLanguage()
            };
        }

        protected abstract string GetLanguage();

        protected abstract string GetSentiment();

        protected abstract string GetCategory();

        protected abstract List<string> GetEntities();

        protected abstract string GetSummary();
    }
}