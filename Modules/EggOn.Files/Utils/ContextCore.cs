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
        private IContextService Service;
        private String FilePath;

        public ContextCore(IContextService Service, String FilePath)
        {
            this.Service = Service;
            this.FilePath = FilePath;
        }

        public Context GetContext()
        {
            string Text = PDFUtils.GetPDFText(FilePath);
            return Service.GetContext("", Text);
        }
    }
}
