using System.Collections.Generic;
using Context.NLP.Algorithms;

namespace EggOn.Context.NLP.Services
{

    /// <summary>
    /// Serviço que utiliza implementações locais para a criação de contexto do texto a ser considerado.
    /// Os algoritmos utilizados estão presentes através de classes estáticas, promovendo dessa maneira a 
    /// rápida alteração do algoritmo a ser utilizado.
    /// </summary>
    public class LocalService : AbstractService
    {

        protected override string GetLanguage()
        {
            return "Not Implemented";
        }

        protected override string GetSentiment()
        {
            return "Not Implemented";
        }

        protected override string GetCategory()
        {
            return TextClassifier.Classify(Text);
        }

        protected override List<string> GetEntities()
        {
            return NamedEntityExtraction.GetEntities(Text);
        }

        protected override string GetSummary()
        {
            return NamedEntitySummary.GetSummary(Text);
        }

    }
}
