using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace csharp_ef
{
    public class IowaPopulation
    {
        [Key] // CountyId is an IDENTITY column
        public int CountyId { get; set; }
        public string? CountyName { get; set; }
        public int Quantity { get; set; }
        // A relationship to the IowaSale table, 1-side
        public ICollection<IowaSale>? IowaSale { get; set; }
    };
    public class IowaSale
    {
        [Key] // Id is an IDENTITY column
        public int Id { get; set; }
        public int Date { get; set; }
        public int CountyId { get; set; }
        public string? City { get; set; }
        public int SaleDollars { get; set; }
        // A relationship to the IowaPopulation table, many-side
        [ForeignKey("CountyId")] // Points which column is Foreign Key
        public IowaPopulation? IowaPopulation { get; set; }
    };
    // A database context and its configuration
    public class IowaContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=iowa;Trusted_Connection=True;");
        }
        // Entities in the context
        public DbSet<IowaSale>? IowaSales { get; set; }
        public DbSet<IowaPopulation>? IowaPopulations { get; set; }
    }
}
