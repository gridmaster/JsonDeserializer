using System.Data.Entity;
using JsonDeserialize.Core;

namespace JsonDeserialize.Models
{
    class SymbolContext : DbContext
    {
        public SymbolContext()
            : base("SymbolContext")
        {
        }
        public DbSet<SymbolDetail> SymbolDetail { get; set; }
        public DbSet<Industry> Industries { get; set; }
        public DbSet<Sector> Sectors { get; set; }
        public DbSet<LogEvent> Logs { get; set; }
    }
}
