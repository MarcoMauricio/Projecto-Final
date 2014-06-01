using Context.NLP.Algorithms;
using System.Collections.Generic;
namespace Context.NLP.Services
{
    public class LocalService : AbstractService
    {

        protected override string GetLanguage()
        {
            return "Not Implemented";
        }

        protected override string GetSentiment()
        {
            return "Not Implemented";
        }

        protected override string GetClassification()
        {
            return TextClassifier.Classify(Text);
        }

        protected override List<string> GetEntities()
        {
            return NamedEntityExtraction.GetEntities(Text);
        }

        protected override List<string> GetSummary()
        {
            return NamedEntitySummary.GetSummary(Text);
        }

    }
}
