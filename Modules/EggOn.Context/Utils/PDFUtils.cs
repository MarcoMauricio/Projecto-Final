using System;
using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace EggOn.Context.Utils
{
    public class PdfUtils
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

        public static string GetPdfText(String pdfPath)
        {
            ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
            using (var reader = new PdfReader(pdfPath))
            {
                var text = new StringBuilder();

                for (var i = 1; i <= reader.NumberOfPages; i++)
                {
                    text.Append(PdfTextExtractor.GetTextFromPage(reader, i, strategy));
                }

                return text.ToString();
            }
        }
    }
}
