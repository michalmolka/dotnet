using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;


// dotnet add package Microsoft.EntityFrameworkCore
// dotnet add package Microsoft.EntityFrameworkCore.Design
// dotnet add package Microsoft.EntityFrameworkCore.SqlServer
// dotnet tool install --global dotnet-ef
// dotnet ef migrations add "migration01"
// dotnet ef database update

//Toquerystring
namespace csharp_ef;
class Program
{
    static void Main(string[] args)
    {

        var context = new IowaContext();

        // Clasic SQL
        var countyIdParam = new SqlParameter("countyId", 4);
        var countyNameParam = new SqlParameter("countyName", "Adair");
        var ipRawSql = context.IowaPopulations.FromSqlRaw(
            $"SELECT * FROM dbo.IowaPopulations p where p.CountyId = @countyId or p.CountyName = @countyName",
            countyIdParam,
            countyNameParam)
            .ToList();

        foreach (var item in ipRawSql)
        {
            System.Console.WriteLine($"{item.CountyName}: {item.Quantity}");
        }


        // Simple LINQ Query syntax
        var efQuerySimple = (
            from i in context.IowaSales
            where i.City.Contains("a") || i.City == "Welby"
            join p in context.IowaPopulations on i.CountyId equals p.CountyId
            orderby i.IowaPopulation.CountyName descending, i.City descending
            select new
            {
                County = i.IowaPopulation.CountyName,
                City = i.City,
                Sale = i.SaleDollars
            }
        )
        .ToList();
        // .ToQueryString();

        foreach (var item in efQuerySimple)
        {
            System.Console.WriteLine($"{item.County}, {item.City}, {item.Sale} ");
        }


        // LINQ method syntax
        var efQueryMethod = context.IowaSales
            .Where(p => p.City.Contains("a") || p.City == "Welby")
            .Include(i => i.IowaPopulation)
            .Where(i => i.IowaPopulation.Quantity > 2000)
            .GroupBy(i => new { i.City, i.IowaPopulation.CountyName })
            .Select(g => new
            {
                City = g.Key.City,
                County = g.Key.CountyName,
                SalesSum = g.Sum(i => i.SaleDollars),
                SalesAvg = g.Average(i => i.SaleDollars),
                Population = g.First().IowaPopulation.Quantity
            })
            .Where(g => g.SalesSum > 10 && g.SalesSum < 10000000)
            .OrderByDescending(g => g.SalesSum)
            .ThenBy(g => g.SalesAvg)
            .ToList();

        foreach (var item in efQueryMethod)
        {
            System.Console.WriteLine($"{item.City}, {item.County}, {item.SalesSum}, {item.SalesAvg}, {item.Population}");
        }


        // LINQ query syntax
        var efQuery = (
            from i in context.IowaSales
            where i.City.Contains("a") || i.City == "Welby"
            join p in context.IowaPopulations on i.CountyId equals p.CountyId
            where p.Quantity > 2000
            group i by new { i.City, i.IowaPopulation.CountyName } into g
            select new
            {
                City = g.Key.City,
                County = g.Key.CountyName,
                SalesSum = g.Sum(s => s.SaleDollars),
                SalesAvg = g.Average(s => s.SaleDollars),
                Population = g.First().IowaPopulation.Quantity
            }
        )
        .Where(o => o.SalesSum > 10 && o.SalesSum < 10000000)
        .OrderByDescending(o => o.SalesSum)
        .ThenBy(o => o.SalesAvg)
        .ToList();

        foreach (var item in efQuery)
        {
            System.Console.WriteLine($"{item.City}, {item.County}, {item.SalesSum}, {item.SalesAvg}, {item.Population}");
        }


        // Insert one record
        IowaSale iowaSale01 = new IowaSale { Date = 20230412, CountyId = 8, City = "Florence", SaleDollars = 2500 };
        context.Add<IowaSale>(iowaSale01);
        context.SaveChanges();

        // // Insert more records
        List<IowaSale> iowaSalesList = new List<IowaSale>{
            new IowaSale {Date = 20230416, CountyId = 7, City = "Waterloo", SaleDollars = 3570},
            new IowaSale {Date = 20230414, CountyId = 7, City = "Cedar Falls", SaleDollars = 1500},
            new IowaSale {Date = 202304121, CountyId = 7, City = "Hudson", SaleDollars = 2150}
        };
        context.AddRange(iowaSalesList);
        context.SaveChanges();


        // // Update one record
        var iowaSaleToUpdate = context.IowaSales.First(i => i.Id == 26);
        iowaSaleToUpdate.SaleDollars = 2200;
        context.SaveChanges();

        // Update more records
        context.IowaSales
            .Where(i => i.CountyId == 8)
            .ExecuteUpdate(i => i.SetProperty(u => u.City, u => "Changed value"));


        // // Remove one record
        context.Remove(context.IowaSales.Single(i => i.Id == 1002));
        context.SaveChanges();

        // Remove more records
        context.IowaSales
            .Where(i => i.CountyId == 8)
            .ExecuteDelete();

    }
}
