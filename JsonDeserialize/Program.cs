using System;
using System.Collections.Generic;
using System.Linq;
using JsonDeserialize.BulkOperations;
using JsonDeserialize.Core;
using JsonDeserialize.Models;
using JsonDeserialize.Services;
using Newtonsoft.Json;

namespace JsonDeserialize
{
    class Program
    {
// ReSharper disable SuggestUseVarKeywordEvident
        static void Main(string[] args)
        {
            Log.WriteLog(new LogEvent("Main", string.Format("{0}: Start", DateTime.Now)));

            Tickers sds = WebService.GetExistingSymbols();

            BaseSectors bs = JsonDeserialize.Services.WebService.GetSectorsIndustires();
            BaseIndustries bi = WebService.GetIndustiresCompanies();

            Log.WriteLog(new LogEvent("Main", string.Format("{0}: Web data pulled", DateTime.Now)));

            SymbolDetails sdList = WebService.LoadSymbols(bs, bi);

            Log.WriteLog(new LogEvent("Main", string.Format("{0}: Symbol Detail loaded", DateTime.Now)));

            string json = JsonConvert.SerializeObject(bs);

            /* To test from a file or text string use these...
            string myText = "{\"ETFReturns\":[{\"IntradayReturn\":null,\"ThreeMoReturn\":null,\"YTDReturn\":null,\"OneYrReturn\":null,\"ThreeYrReturn\":null,\"FiveYrReturn\":null,\"Id\":0,\"Date\":\"0001-01-01\",\"EtfName\":\"UBS E-TRACS CMCI Silver TR ETN\",\"Ticker\":\"USV\",\"Category\":\"Commodities Precious Metals\",\"FundFamily\":\"UBS AG\"},{\"IntradayReturn\":null,\"ThreeMoReturn\":null,\"YTDReturn\":null,\"OneYrReturn\":null,\"ThreeYrReturn\":null,\"FiveYrReturn\":null,\"Id\":0,\"Date\":\"0001-01-01\",\"EtfName\":\"Huntington US Equity Rotation Strat ETF\",\"Ticker\":\"HUSE\",\"Category\":\"Large Growth\",\"FundFamily\":\"Huntington Strategy Shares\"},{\"IntradayReturn\":null,\"ThreeMoReturn\":null,\"YTDReturn\":null,\"OneYrReturn\":null,\"ThreeYrReturn\":null,\"FiveYrReturn\":null,\"Id\":0,\"Date\":\"0001-01-01\",\"EtfName\":\"iShares MSCI Netherlands\",\"Ticker\":\"EWN\",\"Category\":\"Miscellaneous Region\",\"FundFamily\":\"iShares\"}]}";
            using (StreamReader sr = new StreamReader(@"C:\Projects\JsonDeserialize\JsonDeserialize\crapToo.txt"))
            {
                myText = sr.ReadToEnd();
            }
            */

            json = WebService.GetResponse("http://tickersymbol.info/GetETFList");

            json = WebService.FixEtfJson(json);

            EtfReturns etfs = JsonConvert.DeserializeObject<EtfReturns>(json);

            foreach (EtfReturn etf in etfs )
            {
                SymbolDetail sd = new SymbolDetail()
                    {
                        Symbol = etf.Ticker,
                        Date = etf.Date,
                        Name = etf.EtfName
                    };
                sdList.Add(sd);
            }

            foreach (Ticker tkr in sds)
            {
                SymbolDetail sd = new SymbolDetail()
                {
                    Symbol = tkr.Symbol,
                    Name = tkr.Name,
                    Sector = tkr.Sector,
                    Industry = tkr.Industry,
                    ExchangeName = tkr.ExchangeName,
                    Date = DateTime.Now
                };
                sdList.Add(sd);
            }

            int i = 1;
            Dictionary<string, SymbolDetail> dic = new Dictionary<string, SymbolDetail>();
            foreach (SymbolDetail sd in sdList)
            {
                try
                {
                    if (!dic.ContainsKey(sd.Symbol))
                    {
                        SymbolDetail sdet = WebService.GetActiveLinks(sd);
                        dic.Add(sd.Symbol, sdet);
                    }
                    else
                    {
                        Console.WriteLine("{0}:Dup symbol = {1}", i++, sd.Symbol);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0}:dic error = {1} = {2}", i++, sd.Symbol , ex.Message);
                }
            }

            SymbolDetails bulkSymbols = new SymbolDetails();
            bulkSymbols.AddRange(dic.Select(entry => entry.Value));
            //now we have a dic full of 101173 symbols...
            WebService.BulkLoadTickers(bulkSymbols);
            //WebService.BulkLoadTickers(dic);

            Console.ReadKey();
        }
// ReSharper restore SuggestUseVarKeywordEvident
    }
}
