using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EggOn.Files.Utils
{
    public interface IContextService
    {
        Context GetContext(String Title, String Text);
    }
    public class Context
    {
        public List<string> Summary { get; set; }
        public string Classification { get; set; }
        public List<string> Entities { get; set; }
        public string Sentiment { get; set; }
        public string Language { get; set; }
    }
}
