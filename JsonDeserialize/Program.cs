using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using JsonDeserialize.Core;
using JsonDeserialize.Models;
using Newtonsoft.Json;

namespace JsonDeserialize
{
    class Program
    {
        public static string urlSectorIndustry = "http://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20yahoo.finance.sectors&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys";
        public static string urlIndustryCompany = "http://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20yahoo.finance.industry%20where%20id%20in%20(select%20industry.id%20from%20yahoo.finance.sectors)&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys";
       
        public static string GetResponse(string sUri)
        {
            using (WebClient wc = new WebClient())
            {
                string webData = string.Empty;

                string error = string.Empty;
                do
                {
                    error = string.Empty;
                    try
                    {
                        webData = wc.DownloadString(sUri);
                        return webData;
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.IndexOf("The remote server returned an error: (502) Bad Gateway", System.StringComparison.Ordinal) > -1)
                            error = ex.Message;
                        else
                            error = ex.Message;

                        Log.WriteLog(string.Format("Error for Uri {0}.Error: {1}", sUri, error));
                    }
                } while (!string.IsNullOrEmpty(error));
            }

            return "";
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
            string json = string.Empty;
            do
            {
                json = GetResponse(url);
            } while (json == "WebWorks.GetResponse ERROR: Unable to connect to the remote server");

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

        private static SymbolDetails LoadSymbols(BaseSectors bs, BaseIndustries bi)
        {
            SymbolDetails sdList = new SymbolDetails();
            int sectorId = 0;

            foreach (Sector s in bs.Sectors)
            {
                sectorId++;
                foreach (Industry i in s.Industries)
                {
                    var wtf = bi.Industries.Find(ind => ind.Id == i.Id);
                    if (wtf == null) continue;

                    if (wtf.Companies == null) continue;

                    i.Companies = wtf.Companies;

                    foreach (Company c in i.Companies)
                    {
                        SymbolDetail sd = new SymbolDetail();
                        sd.SectorId = sectorId;
                        sd.Sector = s.Name;
                        sd.Industry = i.Name;
                        sd.IndustryId = System.Convert.ToInt32(i.Id);
                        sd.Symbol = c.Symbol;
                        sd.Name = c.Name;
                        sd.Date = DateTime.Now;
                        sdList.Add(sd);
                    }
                }
            }
            return sdList;
        }

        static void Main(string[] args)
        {
            Log.WriteLog(string.Format("{0}: Start", DateTime.Now));
            /*
            BaseSectors bs = GetSectorsIndustires();
            BaseIndustries bi = GetIndustiresCompanies();

            Log.WriteLog(string.Format("{0}: Web data pulled", DateTime.Now));

            SymbolDetails sdList = LoadSymbols(bs, bi);

            Log.WriteLog(string.Format("{0}: Symbol Detail loaded", DateTime.Now));

            string json = JsonConvert.SerializeObject(bs);
            */
            string myText = "{\"ETFReturns\":[{\"IntradayReturn\":null,\"ThreeMoReturn\":null,\"YTDReturn\":null,\"OneYrReturn\":null,\"ThreeYrReturn\":null,\"FiveYrReturn\":null,\"Id\":0,\"Date\":\"0001-01-01\",\"EtfName\":\"UBS E-TRACS CMCI Silver TR ETN\",\"Ticker\":\"USV\",\"Category\":\"Commodities Precious Metals\",\"FundFamily\":\"UBS AG\"},{\"IntradayReturn\":null,\"ThreeMoReturn\":null,\"YTDReturn\":null,\"OneYrReturn\":null,\"ThreeYrReturn\":null,\"FiveYrReturn\":null,\"Id\":0,\"Date\":\"0001-01-01\",\"EtfName\":\"Huntington US Equity Rotation Strat ETF\",\"Ticker\":\"HUSE\",\"Category\":\"Large Growth\",\"FundFamily\":\"Huntington Strategy Shares\"},{\"IntradayReturn\":null,\"ThreeMoReturn\":null,\"YTDReturn\":null,\"OneYrReturn\":null,\"ThreeYrReturn\":null,\"FiveYrReturn\":null,\"Id\":0,\"Date\":\"0001-01-01\",\"EtfName\":\"iShares MSCI Netherlands\",\"Ticker\":\"EWN\",\"Category\":\"Miscellaneous Region\",\"FundFamily\":\"iShares\"}]}";
            using (StreamReader sr = new StreamReader(@"C:\Projects\JsonDeserialize\JsonDeserialize\crapToo.txt"))
            {
                myText = sr.ReadToEnd();
            }
           // myText = myText.Replace("\r", "").Replace("\t", "").Replace("\n", "");

            myText = "[{\"EtfName\":\"UBS E-TRACS CMCI Silver TR ETN\",\"Ticker\":\"USV\",\"Category\":\"Commodities Precious Metals\",\"FundFamily\":\"UBS AG\"},"
                + "{\"EtfName\":\"Huntington US Equity Rotation Strat ETF\",\"Ticker\":\"HUSE\",\"Category\":\"Large Growth\",\"FundFamily\":\"Huntington Strategy Shares\"}]";

            var json = GetResponse("http://tickersymbol.info/GetETFList");
            myText = json;

            json = json.Replace("\\", "");
            json = json.Substring(1);
            json = json.Substring(0, json.Length - 1);
            //json = json.Replace("0001-01-01", "2014-09-20");
            //json = json.Replace(":null,", ":\"\",");

            //json = "{ etfreturn:" + json + "}";
            //List<EtfReturns> rootObject = JsonConvert.DeserializeObject<List<EtfReturns>>(json);
            EtfReturns etfs = JsonConvert.DeserializeObject<EtfReturns>(json);


            Console.ReadKey();
        }
    }
}
