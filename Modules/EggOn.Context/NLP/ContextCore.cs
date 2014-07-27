using System;
using System.IO;
using EggOn.Context.Annotations;
using EggOn.Context.NLP.Services;
using EggOn.Context.Utils;
using System.Collections.Generic;

namespace EggOn.Context.NLP
{
    /// <summary>
    /// Classe intermédia entre os serviços de contextualização e a web API.
    /// </summary>
    /// 
    public class ContextCore
    {
        private List<IContextService> services;

        public ContextCore(List<IContextService> services)
        {
            if (services == null) throw new NullReferenceException();
            this.services = services;
        }
        /// <summary>
        ///  Método a ser chamado para haver a obtenção do contexto do ficheiro a ser considerado
        /// </summary>
        /// 
        /// <returns>
        /// Classe representativa da contextualização do documento. 
        /// É caracterizada pelos campos referentes a cada resultado de contextualização, entidades, sumário, etc.
        /// </returns>
        public MinedObject GetContext(String filePath)
        {
            var title = filePath.Split('/')[filePath.Split('/').Length - 1];
            var text = filePath.EndsWith(".pdf") ? PdfUtils.GetPdfText(filePath) : File.ReadAllText(filePath);
            foreach (IContextService service in services)
            {
                try
                {
                    return service.GetContext(filePath, text);
                }
                catch (Exception)
                {

                }
            }
            return null;
        }
    }
}
