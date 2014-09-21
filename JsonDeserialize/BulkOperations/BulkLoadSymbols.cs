using System.Collections.Generic;
using System.Data;
using JsonDeserialize.Models;

namespace JsonDeserialize.BulkOperations
{
    public class BulkLoadSymbols : BaseBulkLoad
    {
        // private static readonly string connString = ConfigurationManager.ConnectionStrings["ETFContext"].ConnectionString;
        // private static readonly string connString = ConfigurationManager.ConnectionStrings["SymbolContext"].ConnectionString;

        private static readonly string[] ColumnNames = new string[]
            {
                "Date", "ExchangeName", "HasAnalyst", "HasData",
                "HasEstimates", "HasHolders", "HasInsider", "HasKeyStats", "HasOptions", "HasSummary",
                "Industry", "IndustryId", "Name",
                "Sector", "SectorId", "Symbol"
            };

        public BulkLoadSymbols(string connString)
            : base(connString, ColumnNames)
        {

        }

        public DataTable LoadDataTableWithIndustries(IEnumerable<SymbolDetail> dStats, DataTable dt)
        {
            foreach (var value in dStats)
            {
                string sValue = value.Date + "^" + value.ExchangeName + "^" + value.HasAnalyst + "^"
                             + value.HasData + "^" + value.HasEstimates
                             + "^" + value.HasHolders + "^" + value.HasInsider + "^" + value.HasKeyStats
                             + "^" + value.HasOptions + "^" + value.HasSummary + "^" +
                             value.Industry + "^" + value.IndustryId + "^" + value.Name
                             + "^" + value.Sector + "^" + value.SectorId + "^" + value.Symbol;

                DataRow row = dt.NewRow();

// ReSharper disable CoVariantArrayConversion
                row.ItemArray = sValue.Split('^');
// ReSharper restore CoVariantArrayConversion

                dt.Rows.Add(row);
            }

            return dt;
        }
    }
}

