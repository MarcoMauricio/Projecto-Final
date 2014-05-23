using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EggOn.Files.Utils
{
    public class RemoteService : IContextService
    {
        static String servicelink = "https://aylien-text.p.mashape.com/";
        public string Text { get; set; }
        public string Title { get; set; }
        public Context GetContext(String title, String text)
        {
            Title = title;
            Text = text;
            return new Context
            {
                Summary = GetSummary(),
                Entities = GetEntities(),
                Classification = GetClassification(),
                Sentiment = GetSentiment(),
                Language = GetLanguage()
            };

        }

        private string GetSentiment()
        {
            WebRequest request = WebRequest.Create(servicelink + "sentiment?text=" + Text);
            // If required by the server, set the credentials.
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Headers.Add("X-Mashape-Authorization", "MNJVFUtDMGjo6bQFZ7wHeMu5DIdTtDnA");
            // Get the response.
            Stream dataStream;
            using (WebResponse response = request.GetResponse())
            {
                Console.WriteLine(((HttpWebResponse)response).StatusDescription);
                // Get the stream containing content returned by the server.
                dataStream = response.GetResponseStream();


                // Open the stream using a StreamReader for easy access.
                if (dataStream != null)
                {
                    var reader = new StreamReader(dataStream);
                    // Read the content.
                    var responseFromServer = reader.ReadToEnd();
                    dynamic sentiment = JsonConvert.DeserializeObject(responseFromServer);

                    return sentiment.polarity;
                }

            }
            return null;
        }

        private string GetLanguage()
        {
            WebRequest request = WebRequest.Create(servicelink + "language?text=" + Text);
            // If required by the server, set the credentials.
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Headers.Add("X-Mashape-Authorization", "MNJVFUtDMGjo6bQFZ7wHeMu5DIdTtDnA");
            // Get the response.
            Stream dataStream;
            using (WebResponse response = request.GetResponse())
            {
                Console.WriteLine(((HttpWebResponse)response).StatusDescription);
                // Get the stream containing content returned by the server.
                dataStream = response.GetResponseStream();


                // Open the stream using a StreamReader for easy access.
                if (dataStream != null)
                {
                    var reader = new StreamReader(dataStream);
                    // Read the content.
                    var responseFromServer = reader.ReadToEnd();
                    dynamic Language = JsonConvert.DeserializeObject(responseFromServer);

                    return Language.lang;
                }

            }
            return null;
        }

        private string GetClassification()
        {
            WebRequest request = WebRequest.Create(servicelink + "classify?text=" + Text);
            // If required by the server, set the credentials.
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Headers.Add("X-Mashape-Authorization", "MNJVFUtDMGjo6bQFZ7wHeMu5DIdTtDnA");
            // Get the response.
            Stream dataStream;
            using (WebResponse response = request.GetResponse())
            {
                Console.WriteLine(((HttpWebResponse)response).StatusDescription);
                // Get the stream containing content returned by the server.
                dataStream = response.GetResponseStream();


                // Open the stream using a StreamReader for easy access.
                if (dataStream != null)
                {
                    var reader = new StreamReader(dataStream);
                    // Read the content.
                    var responseFromServer = reader.ReadToEnd();
                    dynamic Categories = JsonConvert.DeserializeObject(responseFromServer);
                    dynamic cat = new {label = "", confidence =0.0};

                    // Display the content.
                    foreach (var category in Categories.categories)
                    {
                        if (category.confidence > cat.confidence) cat.label = category.label;
                    }
                    return cat.label;
                }

            }
            return null;
        }



        private string GetSummary()
        {
            // Create a request for the URL. 
            WebRequest request = WebRequest.Create(servicelink + "summarize?text=" + Text + "&title=" + Title);
            // If required by the server, set the credentials.
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Headers.Add("X-Mashape-Authorization", "MNJVFUtDMGjo6bQFZ7wHeMu5DIdTtDnA");
            // Get the response.
            Stream dataStream;
            using (WebResponse response = request.GetResponse())
            {
                Console.WriteLine(((HttpWebResponse)response).StatusDescription);
                // Get the stream containing content returned by the server.
                dataStream = response.GetResponseStream();


                // Open the stream using a StreamReader for easy access.
                if (dataStream != null)
                {
                    var reader = new StreamReader(dataStream);
                    // Read the content.
                    var responseFromServer = reader.ReadToEnd();
                    dynamic sum = JsonConvert.DeserializeObject(responseFromServer);
                    // Display the content.
                    var returnedBuilder = new StringBuilder();
                    foreach (String str in sum.sentences)
                    {
                        returnedBuilder.AppendLine(str);
                    }
                    return returnedBuilder.ToString();
                }

            }
            return null;
        }

        private List<string> GetEntities()
        {
            // Create a request for the URL. 
            WebRequest request = WebRequest.Create(servicelink + "summarize?text=" + Text + "&title=" + Title);
            // If required by the server, set the credentials.
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Headers.Add("X-Mashape-Authorization", "MNJVFUtDMGjo6bQFZ7wHeMu5DIdTtDnA");
            // Get the response.
            Stream dataStream;
            using (WebResponse response = request.GetResponse())
            {
                Console.WriteLine(((HttpWebResponse)response).StatusDescription);
                // Get the stream containing content returned by the server.
                dataStream = response.GetResponseStream();


                // Open the stream using a StreamReader for easy access.
                if (dataStream != null)
                {
                    var reader = new StreamReader(dataStream);
                    // Read the content.
                    var responseFromServer = reader.ReadToEnd();
                    dynamic Entities = JsonConvert.DeserializeObject(responseFromServer);
                    // Display the content.
                    List<string> toReturn = new List<string>();
                    foreach (var entity in Entities.entities)
                    {
                        toReturn.AddRange(entity);
                    }
                    return toReturn;
                }

            }
            return null;
        }
    }
}
