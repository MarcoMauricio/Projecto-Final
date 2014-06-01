using Context.NLP.Services;
using Dummy;
using System;
namespace Context
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

        public MinedObject GetContext()
        {
            string Text = PDFUtils.GetPDFText(FilePath);
            return Service.GetContext("", Text.Trim().Normalize());
        }
    }
}
