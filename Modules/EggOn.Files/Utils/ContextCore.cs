using EggOn.Files.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EggOn.Files.Utils
{
    public class ContextCore
    {
        private IContextService service = new RemoteService();
        public Context Contextualize(String FilePath)
        {
            string Text = PDFUtils.GetPDFText(FilePath);
            return service.GetContext("",Text);
        }
    }
}
