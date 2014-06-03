using System;
using System.Collections.Generic;
namespace Context.NLP.Services
{

    public interface IContextService
    {
        MinedObject GetContext(String Title, String Text);
    }

    public class MinedObject
    {
        public List<string> Summary { get; set; }
        public string Classification { get; set; }
        public List<string> Entities { get; set; }
        public string Sentiment { get; set; }
        public string Language { get; set; }
    }
}
