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
        public static string urlSectorIndustry = "http://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20yahoo.finance.sectors&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys";
        public static string urlIndustryCompany = "http://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20yahoo.finance.industry%20where%20id%20in%20(select%20industry.id%20from%20yahoo.finance.sectors)&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys";
       
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
            public List<Sectors> Sectors { get; set; }
        }

        public class BaseIndustries
        {
            [JsonProperty(PropertyName = "industry")]
            public List<Industry> Industries { get; set; }
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

            [JsonProperty(PropertyName = "company")]
            public List<Company> Companies { get; set; } 
        }

        public class Company
        {
            [JsonProperty(PropertyName = "name")]
            public string Name { get; set; }

            [JsonProperty(PropertyName = "symbol")]
            public string Symbol { get; set; }
        }

        static BaseSectors GetSectorsIndustires()
        {
            string json = GetJson(urlSectorIndustry);

            json = CleanJson(json);

            while (json.IndexOf("\"industry\":{\"id\"", System.StringComparison.Ordinal) > -1)
            {
                var regex = new Regex(Regex.Escape("\"industry\":{\"id\""));
                json = regex.Replace(json, "\"industry\":[{\"id\"", 1);

                regex = new Regex(Regex.Escape("}},{"));
                json = regex.Replace(json, "}]},{", 1);
            }

            return JsonConvert.DeserializeObject<BaseSectors>(json);
        }

        static BaseIndustries GetIndustiresCompanies()
        {
            string json = GetJson(urlIndustryCompany);

            json = CleanJson(json);

            string problem = "{\"id\":\"133\",\"name\":\"\"},";
            int ndx = json.IndexOf(problem, System.StringComparison.Ordinal);

            json = json.Replace(problem, "");

            problem = "\"company\":{";

            while (json.IndexOf(problem, System.StringComparison.Ordinal) > -1)
            {
                ndx = json.IndexOf(problem, System.StringComparison.Ordinal);

                var regex = new Regex(Regex.Escape(problem));
                json = regex.Replace(json, "\"company\":[{", 1);

                ndx += json.Substring(ndx).IndexOf("}", System.StringComparison.Ordinal);

                json = json.Substring(0, ndx + 1) + "]" + json.Substring(ndx + 1); 
            }
            var crap = json.Substring(1098068);

            return JsonConvert.DeserializeObject<BaseIndustries>(json);
        }

        private static string GetJson(string url)
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
            return Newtonsoft.Json.JsonConvert.SerializeXmlNode(doc);
        }

        private static string CleanJson(string json)
        {
            string newjson = json.Substring(json.IndexOf("\"results\":", System.StringComparison.Ordinal) + "\"results\":".Length);
            newjson = newjson.Substring(0, newjson.IndexOf("}]}]}", System.StringComparison.Ordinal) + "}]}]}".Length);
            newjson = newjson.Replace("\r", "").Replace("\t", "").Replace("\n", "");
            return newjson.Replace("@", "");
        }

        static void Main(string[] args)
        {
            BaseSectors bs = GetSectorsIndustires();

            BaseIndustries bi = GetIndustiresCompanies();

            foreach (Sectors s in bs.Sectors)
            {
                foreach (Industry i in s.Industries)
                {
                    var wtf = bi.Industries.Find(ind => ind.Id == i.Id);
                    if (wtf == null) continue;
                    
                    i.Companies = wtf.Companies;
                }
            }

            string json = JsonConvert.SerializeObject(bs);

            Console.ReadKey();
        }
    }
}
