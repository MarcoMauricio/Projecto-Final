using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dummy
{
    public class PDFUtils
    {
        public static string GetPDFText(String pdfPath)
        {
            ITextExtractionStrategy strategy = new LocationTextExtractionStrategy();
            using (PdfReader reader = new PdfReader(pdfPath))
            {
                StringBuilder text = new StringBuilder();

                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    text.Append(PdfTextExtractor.GetTextFromPage(reader, i,strategy));
                }

                return text.ToString();
            }
        }
    }
}
