using System;
using System.Collections.Generic;

namespace EggOn.Context.NLP.Services
{

    public interface IContextService
    {
        MinedObject GetContext(String title, String text);
    }

    public class MinedObject
    {
        public string Summary { get; set; }
        public string Category { get; set; }
        public List<string> Entities { get; set; }
        public string Sentiment { get; set; }
        public string Language { get; set; }
    }
}
