using System;
using Context.NLP.Services;
using EggOn.Context.Utils;

namespace EggOn.Context.NLP
{

    public class ContextCore
    {
        private readonly IContextService _service;
        private readonly String _filePath;

        /// <summary>
        /// Classe intermédia entre os serviços de contextualização e a web API.
        /// </summary>
        /// 
        /// <param name="service">
        /// Caracterização do serviço que contêm a implementação da contextualização de documentos. 
        /// Esse serviço é obrigado a implementar a interface \c IContextSservice que tem um 
        /// único método com a assinatura GetContext(Title, Text)
        /// </param>
        /// 
        /// <param name="filePath">
        /// Caminho absoluto do ficheiro a ser considerado. 
        /// Este caminho deve incluir a hierarquia de pastas, assim como o nome do ficheiro e a 
        /// sua respectiva extensão.
        /// </param>
        public ContextCore(IContextService service, String filePath)
        {
            _service = service;
            _filePath = filePath;
        }
        /// <summary>
        ///  Método a ser chamado para haver a obtenção do contexto do ficheiro a ser considerado
        /// </summary>
        /// 
        /// <returns>
        /// Classe representativa da contextualização do documento. 
        /// É caracterizada pelos campos referentes a cada resultado de contextualização, entidades, sumário, etc.
        /// </returns>
        public MinedObject GetContext()
        {
            var text = PdfUtils.GetPdfText(_filePath);
            return _service.GetContext("", text.Trim().Normalize());
        }
    }
}
