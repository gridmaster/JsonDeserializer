using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using Newtonsoft.Json;

namespace JsonDeserialize
{
    class Program
    {
        //private static string JsonInput =
        //"{"sector":[{"name":"Basic Materials","industry":[{"id":"112","name":"Agricultural Chemicals"},{"id":"132","name":"Aluminum"}]},{"name":"Conglomerates","industry":{"id":"210","name":"Conglomerates"}},{"name":"Consumer Goods","industry":[{"id":"310","name":"Appliances"},{"id":"344","name":"Dairy Products"},{"id":"314","name":"Electronic Equipment"},{"id":"331","name":"Trucks &amp; Other Vehicles"}]},{"name":"Financial","industry":[{"id":"431","name":"Accident &amp; Health Insurance"},{"id":"422","name":"Asset Management"},{"id":"433","name":"Surety &amp; Title Insurance"}]},{"name":"Utilities","industry":[{"id":"913","name":"Diversified Utilities"},{"id":"911","name":"Electric Utilities"},{"id":"910","name":"Foreign Utilities"},{"id":"912","name":"Gas Utilities"},{"id":"914","name":"Water Utilities"}]}]}";"
        public static string url = "http://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20yahoo.finance.sectors&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys";
       
        public static string GetResponse(string sUri)
        {
            var httpWRequest = (HttpWebRequest)WebRequest.Create(sUri);

            try
            {
                httpWRequest.KeepAlive = true;
                httpWRequest.ProtocolVersion = HttpVersion.Version10;

                var httpWebResponse = (HttpWebResponse)httpWRequest.GetResponse();
                var responseStream = httpWebResponse.GetResponseStream();

                if (responseStream != null)
                {
                    using (StreamReader reader = new StreamReader(responseStream, Encoding.UTF8))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                return "WebWorks.GetResponse ERROR: " + ex.Message;
            }

            return "";
        }

        public class BaseSectors
        {
            [JsonProperty(PropertyName = "sector")]
            public List<Sectors> Companies { get; set; }
        }

        public class Sectors
        {
            [JsonProperty(PropertyName = "name")]
            public string Name { get; set; }

            [JsonProperty(PropertyName = "industry")]
            public List<Industry> Industries { get; set; } 
        }

        public class Industry
        {
            [JsonProperty(PropertyName = "id")]
            public string Id { get; set; }
 
            [JsonProperty(PropertyName = "name")]
            public string Name { get; set; }
        }

        static void Main(string[] args)
        {
            int i = 0;
            string json = string.Empty;
            do
            {
                json = GetResponse(url);
                Thread.Sleep(2500);
                i++;
            } while (json == "WebWorks.GetResponse ERROR: Unable to connect to the remote server");

            Console.WriteLine("i = " + i);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(json);
            json = Newtonsoft.Json.JsonConvert.SerializeXmlNode(doc);


            json = json.Replace("\r", "").Replace("\t", "").Replace("\n", "");
            json = json.Replace("@", "");
            while (json.IndexOf("\"industry\":{\"id\"", System.StringComparison.Ordinal) > -1)
            {
                var regex = new Regex(Regex.Escape("\"industry\":{\"id\""));
                json = regex.Replace(json, "\"industry\":[{\"id\"", 1);

                regex = new Regex(Regex.Escape("}},{"));
                json = regex.Replace(json, "}]},{", 1);
            }

            var result = JsonConvert.DeserializeObject<BaseSectors>(json);

            Console.ReadKey();

            using (StreamReader sr = new StreamReader(@"C:\Projects\JsonDeserialize\JsonDeserialize\Sectors.txt"))
            {
                json = sr.ReadToEnd();

                Console.WriteLine(json);
                json = json.Replace("\r", "").Replace("\t", "").Replace("\n", "");
                json = json.Replace("@", "");
                while (json.IndexOf("\"industry\":{\"id\"", System.StringComparison.Ordinal) > -1)
                {
                    var regex = new Regex(Regex.Escape("\"industry\":{\"id\""));
                    json = regex.Replace(json, "\"industry\":[{\"id\"", 1);

                    regex = new Regex(Regex.Escape("}},{"));
                    json = regex.Replace(json, "}]},{", 1);
                }

                result = JsonConvert.DeserializeObject<BaseSectors>(json);

                Console.ReadKey();
            }
        }
    }
}
