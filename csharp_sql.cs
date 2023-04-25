using System.Data.SqlClient;

namespace csharp_sql
{
    class Program
    {
        static void Main(string[] args)
        {
            // Option 1: a raw connection string
            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=iowa;Integrated Security=True;MultipleActiveResultSets=true";

            // Option 2: a connection string builder
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(); 
            builder.DataSource = "(localdb)\\MSSQLLocalDB";
            builder.InitialCatalog = "iowa";
            builder.IntegratedSecurity = true;
            builder.MultipleActiveResultSets = true;

            // A new sql connection
            var cnn = new SqlConnection(builder.ConnectionString);
            cnn.Open();

            string query = """
                SELECT i.Date, i.City, SUM(i.SaleDollars) as SaleDollars
                FROM dbo.IowaSales i
                WHERE CountyId = @0
                GROUP BY i.Date, i.City
                HAVING SUM(i.SaleDollars) > @1
                ORDER BY i.Date desc
                """;

            // A new SQL command
            SqlCommand sqlCommand = new SqlCommand(query, cnn);

            // Assign parameters to a SQL query
            sqlCommand.Parameters.Add(new SqlParameter("0", 4));
            sqlCommand.Parameters.Add(new SqlParameter("1", 3500));

            // Run and read the sql query
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            while (sqlDataReader.Read())
            {
                System.Console.WriteLine($"""
                Date: {sqlDataReader.GetInt32(0)}, 
                City: {sqlDataReader.GetString(1)}, 
                SalesDollars: {sqlDataReader.GetInt32(2)}
                """);
            }

            // DML with an error handling
            string insertError = "INSERT INTO dbo.IowaSales VALUES ('150', '150', '150', '150')";
            SqlCommand insertCommand = new SqlCommand(insertError, cnn);
            try
            {
                insertCommand.ExecuteNonQuery();
            } catch (Exception ex) {
                System.Console.WriteLine($"INSERT INTO - has na error.{ex.Message}");
            }

            // Close connection
            cnn.Close();
        }
    }
}
