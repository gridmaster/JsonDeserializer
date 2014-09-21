using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using JsonDeserialize.BulkOperations;
using JsonDeserialize.Core;
using JsonDeserialize.Models;
using Newtonsoft.Json;

namespace JsonDeserialize.Services
{
    public class WebService
    {
// ReSharper disable SuggestUseVarKeywordEvident
        private static SymbolContext db = new SymbolContext();

        public static string urlSectorIndustry = "http://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20yahoo.finance.sectors&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys";
        public static string urlIndustryCompany = "http://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20yahoo.finance.industry%20where%20id%20in%20(select%20industry.id%20from%20yahoo.finance.sectors)&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys";

        public static void BulkLoadTickers(Dictionary<string, SymbolDetail> dick)
        {
            string config = ConfigurationManager.ConnectionStrings["SymbolContext"].ConnectionString;
            Log.WriteLog(new LogEvent("BulkLoadTickers", string.Format("Load Bulk at: " + DateTime.Now)));

            SymbolDetails bulkSymbols = new SymbolDetails();

            BulkLoadSymbols bls = new BulkLoadSymbols(config);

            var dt = bls.ConfigureDataTable();

            bulkSymbols.AddRange(dick.Select(entry => entry.Value));

            dt = bls.LoadDataTableWithIndustries(bulkSymbols, dt);

            bls.BulkCopy<SymbolDetails>(dt);

            Log.WriteLog(new LogEvent("BulkLoadTickers", string.Format("Ending at: " + DateTime.Now)));
        }

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

                        Log.WriteLog(new LogEvent("WebService - GetResponse", string.Format("Error for Uri {0}.Error: {1}", sUri, error)));
                    }
                } while (!string.IsNullOrEmpty(error));
            }

            return "";
        }

        public static BaseSectors GetSectorsIndustires()
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

        public static BaseIndustries GetIndustiresCompanies()
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

        public static string FixEtfJson(string json)
        {
            json = json.Replace("\\", "");
            json = json.Substring(1);
            return json.Substring(0, json.Length - 1);
        }

        public static SymbolDetails LoadSymbols(BaseSectors bs, BaseIndustries bi)
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

        public static Tickers GetExistingSymbols()
        {
            Tickers tix = new Tickers();
            string connString = ConfigurationManager.ConnectionStrings["SymbolContext"].ConnectionString;

            DateTime maxDate = DateTime.Now;

            Log.WriteLog(new LogEvent(string.Format("SymbolService - ReturnSectores() for date {0}", maxDate), " - do bulk insert"));

            string json = string.Empty;

            try
            {
                var tixx = db.Tickers.GroupBy(t => t.Symbol, (key, group) => group.FirstOrDefault()).OrderBy(t => t.Symbol).ToArray();

                tix.AddRange(tixx.Select(ticker => new Ticker()
                {
                    Symbol = ticker.Symbol,
                    Name = ticker.Name,
                    ExchangeName = ticker.ExchangeName,
                    Industry = ticker.Industry,
                    Sector = ticker.Sector,
                    Id = ticker.Id
                }));

            }
            catch (Exception ex)
            {
                json = string.Format("GetExistingSymbols threw error: {0}", ex.Message);
                Log.WriteLog(new LogEvent(string.Format("WebService - GetExistingSymbols for date {0}", maxDate), json));
            }

            return tix;
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
// ReSharper restore SuggestUseVarKeywordEvident
    }
}
