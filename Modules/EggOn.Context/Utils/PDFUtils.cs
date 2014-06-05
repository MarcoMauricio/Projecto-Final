using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Text;

namespace Dummy
{
    public class PDFUtils
    {


        /// <summary>
        /// Utilização do pacote iText para a leitura de texto a partir de documentos PDF.
        ///  Foi escolhida a estratégia "LocationTextExtractionStrategy" porque tem em consideração a posição do texto 
        ///  no documento. 
        /// </summary>
        /// 
        /// <param name="pdfPath">String com o caminho absoluto do ficheiro para fazer a leitura</param>
        /// 
        /// <returns>Texto extraido do ficheiro PDF</returns>

        public static string GetPDFText(String pdfPath)
        {
            ITextExtractionStrategy strategy = new LocationTextExtractionStrategy();
            using (PdfReader reader = new PdfReader(pdfPath))
            {
                StringBuilder text = new StringBuilder();

                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    text.Append(PdfTextExtractor.GetTextFromPage(reader, i, strategy));
                }

                return text.ToString();
            }
        }
    }
}
