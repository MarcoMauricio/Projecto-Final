using System;
using System.IO;
using Context.NLP.Services;
using EggOn.Context.Annotations;
using EggOn.Context.NLP.Services;
using EggOn.Context.Utils;

namespace EggOn.Context.NLP
{

    public static class ContextCore
    {

        /// <summary>
        /// Classe intermédia entre os serviços de contextualização e a web API.
        /// </summary>
        /// 
        private static readonly RemoteService AylienService = new RemoteService();
        private static readonly LocalService LocalService = new LocalService();
        /// <summary>
        ///  Método a ser chamado para haver a obtenção do contexto do ficheiro a ser considerado
        /// </summary>
        /// 
        /// <returns>
        /// Classe representativa da contextualização do documento. 
        /// É caracterizada pelos campos referentes a cada resultado de contextualização, entidades, sumário, etc.
        /// </returns>
        public static MinedObject GetContext([NotNull] string filePath)
        {
            var text = filePath.EndsWith(".pdf") ? PdfUtils.GetPdfText(filePath) : File.ReadAllText(filePath);
            text = text.Replace("\n", "");
            text = text.Replace("\r", " . ");
            text = text.Replace("\t", " . ");
            try
            {
                return AylienService.GetContext("", text.Trim());
            }
            catch (Exception)
            {
                return LocalService.GetContext("", text.Trim());
            }
        }
    }
}
