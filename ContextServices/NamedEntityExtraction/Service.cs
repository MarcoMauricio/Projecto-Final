using System;
using EggOn.Context.Annotations;
using EggOn.Context.NLP.Services;

namespace NamedEntityExtraction
{
    public class Service : IContextService
    {
        public MinedObject GetContext([CanBeNull] string title, [NotNull] string text)
        {
            if (text == null) throw new ArgumentNullException("text");
            var obj = new MinedObject
            {
                Entities = NamedEntitiesExtraction.GetEntities(text),
                Summary = NamedEntitySummary.GetSummary(text),
                Category = "lala"
            };


            return obj;
        }
    }
}
