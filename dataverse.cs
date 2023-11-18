using System;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Data.SqlClient;
using static CognitiveServices.AzureVariables;


namespace dataverse_sdk
{
  class Program
  {
    static async Task Main(string[] args)
    {
      // A service principal client
      string clientId = {Service Principal Client ID};
      string clientSecret = {Service Principal Secret};
      string dataverseEnvironment = {Power Platform Environment URL};
      string dataverseDatabase = {Dataverse Database Name}

      var connectionString = @$"Url=https://{dataverseEnvironment};AuthType=ClientSecret;ClientId={clientId};ClientSecret={clientSecret};RequireNewInstance=true";
      var serviceClient = new ServiceClient(connectionString);


      //A SDK query
      QueryExpression querySdk = new QueryExpression("crf8e_iowa_category");
      querySdk.ColumnSet.AddColumns("crf8e_categoryname", "crf8e_categorynumber");
      querySdk.TopCount = 10;

      querySdk.Criteria = new FilterExpression();
      querySdk.Criteria.AddCondition("crf8e_categoryname", ConditionOperator.BeginsWith, "America");
      querySdk.Criteria.AddCondition("crf8e_categorynumber", ConditionOperator.GreaterEqual, 1051010);

      var accountsCollection = await serviceClient.RetrieveMultipleAsync(querySdk);

      System.Console.WriteLine("-----SDK-----");
      accountsCollection.Entities.ToList().ForEach(x =>
      {
        Console.WriteLine($"{x.Attributes["crf8e_categoryname"]}, {x.Attributes["crf8e_categorynumber"]}");
      });


      // A FetchXML query
      string fetchXml = @"
        <fetch top='50' >
          <entity name='crf8e_iowa_category' >
            <attribute name='crf8e_categoryname' />
            <attribute name='crf8e_categorynumber' />
            <filter type='and' >
              <condition 
                attribute='crf8e_categoryname' 
                operator='like' 
                value='America%' />
              <condition 
                attribute='crf8e_categorynumber' 
                operator='ge' 
                value='1051010' />
            </filter>
            <order attribute='crf8e_categorynumber' />
          </entity>
        </fetch>";

      var queryFetchXml = new FetchExpression(fetchXml);
      EntityCollection results = serviceClient.RetrieveMultiple(queryFetchXml);

      System.Console.WriteLine("-----FetchXML-----");
      results.Entities.ToList().ForEach(x =>
      {
        Console.WriteLine($"{x.Attributes["crf8e_categoryname"]}, {x.Attributes["crf8e_categorynumber"]}");
      });


      //A SQL query
      string ConnectionString = @$"Server={dataverseEnvironment}; Authentication=Active Directory Service Principal; Encrypt=True; Database={dataverseDatabase}; User Id={clientId}; Password={clientSecret}";

      var cnn = new SqlConnection(ConnectionString);
      cnn.Open();
      string querySql = "SELECT [crf8e_categoryname], [crf8e_categorynumber] FROM [dbo].[crf8e_iowa_category] WHERE [crf8e_categoryname] like 'America%' AND [crf8e_categorynumber] >= 1051010 ORDER BY [crf8e_categorynumber] ASC";
      SqlCommand sqlCommand = new SqlCommand(querySql, cnn);
      SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

      System.Console.WriteLine("-----SQL query-----");
      while (sqlDataReader.Read())
      {
        System.Console.WriteLine($"{sqlDataReader.GetString(0)}, {sqlDataReader.GetString(1)}");
      }

      //SDK INSERT.
      var iowaCategory = new Entity("crf8e_iowa_category");
      iowaCategory["crf8e_categorynumber"] = "55555";
      iowaCategory["crf8e_categoryname"] = "Polish Vodka";

      serviceClient.Create(iowaCategory);

    }
  }
}
