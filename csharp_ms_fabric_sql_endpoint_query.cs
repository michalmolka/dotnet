using Microsoft.Data.SqlClient;

namespace CsharpObjects;
class Program
{
    public class IowaCategory()
    {
        public long CategoryID;
        public string CategoryName;
    }

    static void Main(string[] args)
    {
        string clientId = "e*****4";
        string secret = "Z*****J";
        string serverAddress = "s*****i.datawarehouse.fabric.microsoft.com";

        string ConnectionStringUser = $"Server={serverAddress}; Authentication=Active Directory Default; Encrypt=True; Database=eightfive_lakehouse"; // eightfive_lakehouse - for a lakehouse, eightfive_warehouse - for DW
        string ConnectionStringSP = $"Server={serverAddress}; Authentication=Active Directory Service Principal; Encrypt=True; Database=eightfive_lakehouse;User Id={clientId}; Password={secret}"; // eightfive_lakehouse - for a lakehouse, eightfive_warehouse - for DW

        SqlConnection conn = new SqlConnection(ConnectionStringUser);
        conn.Open();

        string query = """
            SELECT Top 5 CategoryID, CategoryName
            FROM dbo.iowa_category
            """;

        SqlCommand sqlCommand = new SqlCommand(query, conn);
        SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

        List<IowaCategory> queryResult = new List<IowaCategory>();

        while (sqlDataReader.Read())
        {
            IowaCategory iowaCategory = new IowaCategory
            {
                CategoryID = sqlDataReader.GetInt64(0),
                CategoryName = sqlDataReader.GetString(1)
            };
            queryResult.Add(iowaCategory);
        }
        conn.Close();

        foreach (var item in queryResult)
        {
            System.Console.WriteLine($"1: {item.CategoryID}, 2: {item.CategoryName}");
        }
    }
}
