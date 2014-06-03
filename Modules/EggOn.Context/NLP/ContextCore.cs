using Context.NLP.Services;
using Dummy;
using System;
namespace Context
{

    public class ContextCore
    {
        private IContextService Service;
        private String FilePath;

        /// <summary>
        /// Classe intermédia entre os serviços de contextualização e a web API.
        /// </summary>
        /// 
        /// <param name="Service">
        /// Caracterização do serviço que contêm a implementação da contextualização de documentos. 
        /// Esse serviço é obrigado a implementar a interface \c IContextSservice que tem um 
        /// único método com a assinatura GetContext(Title, Text)
        /// </param>
        /// 
        /// <param name="FilePath">
        /// Caminho absoluto do ficheiro a ser considerado. 
        /// Este caminho deve incluir a hierarquia de pastas, assim como o nome do ficheiro e a 
        /// sua respectiva extensão.
        /// </param>
        public ContextCore(IContextService Service, String FilePath)
        {
            this.Service = Service;
            this.FilePath = FilePath;
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
            string Text = PDFUtils.GetPDFText(FilePath);
            return Service.GetContext("", Text.Trim().Normalize());
        }
    }
}
