using System;
using System.Collections.Generic;
namespace Context.NLP.Services
{

    public class LocalWordsService : AbstractService
    {
        private static readonly List<string> FiltersPortuguese = new List<string>{
            /*A*/   "a","as","antes","amanhã","ainda","antigamente","agora","aqui", "ali" ,"adiante", "abaixo" ,"aquém" ,"acolá", "atrás", "além", "aonde", "acima" , "algures","aí","aliás","assim","acaso","apenas","até","aquele","aquela","aqueles","aquelas","aquilo",
            /*B*/   "bem","bastante",
            /*C*/   "cedo","como","certamente","connosco","comigo",
            /*D*/   "dantes","depois","dentro","detrás","debaixo","depressa","devagar","demasiado","decerto",
            /*E*/   "então","enfim","efectivamente","exclusivamente","ele","eles","elas","eu", "este", "esta", "estes", "estas","esse", "essa", "esses", "essas",
            /*F*/   "fora",
            /*H*/   "hoje",
            /*I*/   "inclusivamente","isto","isso",
            /*J*/   "jamais","já",
            /*L*/   "logo","lá","longe",
            /*M*/   "melhor","mal","muito","mais","menos","meu","minha","meus","minhas",
            /*N*/   "não","nunca","nada","nem","nós","nos","nosso","nossa","nossos","nossas",
            /*O*/   "ontem","ora","outrora","onde","o","os",
            /*P*/   "primeiro","perto","posto","pior","principalmente","pouco","possivelmente","provavelmente","primeiramente",
            /*Q*/   "quase","quão","quanto","que",
            /*S*/   "sempre","sobretudo","sim","salvo","senão","somente","simplesmente","só","seu","sua","seus","suas",
            /*T*/   "tarde","tanto","tão","tudo","todo","talvez","também","teu","tua","teus","tuas",
            /*R*/   "realmente",
            /*U*/   "unicamente","ultimamente",
            /*V*/   "vocês","vós","vosso","vossa","vossos","vossas"   };





        protected override string GetLanguage()
        {
            throw new NotImplementedException();
        }

        protected override string GetSentiment()
        {
            throw new NotImplementedException();
        }

        protected override string GetCategory()
        {
            throw new NotImplementedException();
        }

        protected override List<string> GetEntities()
        {
            throw new NotImplementedException();
        }

        protected override List<string> GetSummary()
        {
            throw new NotImplementedException();
        }
    }

}
