using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EggOn.Files.Context
{
    public interface IContextualize
    {
        Context GetContext(String Title, String Text);
    }
    public class Context
    {
        public string Summary { get; set; }
        public string Classification { get; set; }
        public List<string> Entities { get; set; }
        public string Sentiment { get; set; }
        public string Language { get; set; }
    }
}
