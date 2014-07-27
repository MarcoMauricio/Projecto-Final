using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using EggOn.Context.NLP.Services;
using Newtonsoft.Json;

namespace AylienService
{
    /// <summary>
    /// Caracteriza-se pela a utilização do serviço externo diponibilizado pela empresa Aylien.
    /// 
    /// Este serviço recebe na QueryString um parametro do tipo "&text=..." e retorna um objecto 
    /// JSON representativo do recurso identificado no pedido. Esta classe apenas faz a leitura das 
    /// respostas aos pedidos efectuados a esse serviço, e toda a documentação pode ser vista no url : 
    /// http://aylien.com/text-api-doc
    /// </summary>

    public class AylienService : IContextService
    {
        private readonly String _servicelink;
        public AylienService()
        {
            _servicelink = "https://aylien-text.p.mashape.com/";

        }


        internal string GetSentiment(string text)
        {
            var request = WebRequest.Create(_servicelink + "sentiment?text=" + text);
            // If required by the server, set the credentials.
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Headers.Add("X-Mashape-Authorization", "MNJVFUtDMGjo6bQFZ7wHeMu5DIdTtDnA");
            // Get the response.
            using (var response = request.GetResponse())
            {
                // Get the stream containing content returned by the server.
                var dataStream = response.GetResponseStream();


                // Open the stream using a StreamReader for easy access.
                if (dataStream == null) return null;
                var reader = new StreamReader(dataStream);
                // Read the content.
                var responseFromServer = reader.ReadToEnd();
                dynamic sentiment = JsonConvert.DeserializeObject(responseFromServer);

                return sentiment.polarity;
            }
        }

        internal string GetLanguage(string text)
        {
            var request = WebRequest.Create(_servicelink + "language?text=" + text);
            // If required by the server, set the credentials.
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Headers.Add("X-Mashape-Authorization", "MNJVFUtDMGjo6bQFZ7wHeMu5DIdTtDnA");
            // Get the response.
            using (var response = request.GetResponse())
            {
                // Get the stream containing content returned by the server.
                var dataStream = response.GetResponseStream();


                // Open the stream using a StreamReader for easy access.
                if (dataStream == null) return null;
                var reader = new StreamReader(dataStream);
                // Read the content.
                var responseFromServer = reader.ReadToEnd();
                dynamic language = JsonConvert.DeserializeObject(responseFromServer);

                return language.lang;
            }
        }

        internal string GetCategory(string text)
        {
            var request = WebRequest.Create(_servicelink + "classify?text=" + text);
            // If required by the server, set the credentials.
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Headers.Add("X-Mashape-Authorization", "MNJVFUtDMGjo6bQFZ7wHeMu5DIdTtDnA");
            // Get the response.
            using (var response = request.GetResponse())
            {
                // Get the stream containing content returned by the server.
                var dataStream = response.GetResponseStream();


                // Open the stream using a StreamReader for easy access.
                if (dataStream == null) return null;
                var reader = new StreamReader(dataStream);
                // Read the content.
                var responseFromServer = reader.ReadToEnd();
                dynamic categories = JsonConvert.DeserializeObject(responseFromServer);
                var cat = new Category();

                // Display the content.
                foreach (var category in categories.categories)
                {
                    if (category.confidence > cat.Confidence) cat.Label = category.label;
                }
                return cat.Label;
            }
        }

        /// <summary>
        /// Classe de uso interno devido ao serviço utilizado retornar uma lista desorganizada de 
        /// categorias que são identificadas com o seu nome/confiança  do texto se encontrar nessa categoria.
        /// </summary>
        private class Category
        {
            public string Label { get; set; }
            public float Confidence { get; set; }

        }

        internal string GetSummary(string text, string title)
        {
            // Create a request for the URL. 
            var request = WebRequest.Create(_servicelink + "summarize?text=\"" + text + "\"&title=Test" + title);
            // If required by the server, set the credentials.
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Headers.Add("X-Mashape-Authorization", "MNJVFUtDMGjo6bQFZ7wHeMu5DIdTtDnA");
            // Get the response.
            using (var response = request.GetResponse())
            {
                // Get the stream containing content returned by the server.
                var dataStream = response.GetResponseStream();


                // Open the stream using a StreamReader for easy access.
                if (dataStream == null) return null;
                var reader = new StreamReader(dataStream);
                // Read the content.
                var responseFromServer = reader.ReadToEnd();
                dynamic sum = JsonConvert.DeserializeObject(responseFromServer);
                // Display the content.

                var summary = new StringBuilder();
                foreach (var sentence in sum.sentences)
                {
                    summary.AppendLine(sentence.Value);
                }
                return summary.ToString();
            }
        }

        internal List<string> GetEntities(string text)
        {

            // Create a request for the URL. 
            var request = WebRequest.Create(_servicelink + "entities?text=" + text);
            // If required by the server, set the credentials.
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Headers.Add("X-Mashape-Authorization", "MNJVFUtDMGjo6bQFZ7wHeMu5DIdTtDnA");
            // Get the response.
            using (var response = request.GetResponse())
            {
                // Get the stream containing content returned by the server.
                var dataStream = response.GetResponseStream();


                // Open the stream using a StreamReader for easy access.
                if (dataStream == null) return null;
                var reader = new StreamReader(dataStream);
                // Read the content.
                var responseFromServer = reader.ReadToEnd();
                dynamic entities = JsonConvert.DeserializeObject(responseFromServer);
                // Display the content.
                var toReturn = new List<string>();
                foreach (var entity in entities.entities)
                {
                    foreach (var ent in entity.Value)
                        toReturn.Add(ent.Value);
                }
                return toReturn;
            }
        }

        public MinedObject GetContext(string title, string text)
        {
            if (text == null) throw new ArgumentNullException("text");
            var obj = new MinedObject
            {
                Entities = GetEntities(text),
                Summary = GetSummary(text, title),
                Category = GetCategory(text),
                Language = GetLanguage(text),
                Sentiment = GetSentiment(text)
            };
            return obj;
        }
    }
}
