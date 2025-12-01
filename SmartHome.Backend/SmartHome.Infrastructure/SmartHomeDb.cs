using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace SmartHome.Infrastructure;

public class SmartHomeDb
{
	private CosmosClient _cosmosClient;

	private string AccountEndpoint { get; init; } 
	private string AccountKey { get; init; }
	
	public SmartHomeDb(IOptions<CosmoDbSettings> settings)
	{
	
		AccountEndpoint = settings.Value.AccountEndpoint;
		AccountKey = settings.Value.AccountKey;
		
		string connectionString = GetConnectionString();
		
		_cosmosClient =  new CosmosClient(connectionString,
			new CosmosClientOptions { ApplicationRegion = Regions.FranceCentral, });
	}

	private string GetConnectionString()
	{
		return
			$"AccountEndpoint={AccountEndpoint};" +
			$"AccountKey={AccountKey};";
	}

	public Container GetContainer()
	{
		return _cosmosClient.GetContainer("test", "container200myvmesete");
	}
	
	
	 
}