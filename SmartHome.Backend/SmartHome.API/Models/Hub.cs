using Microsoft.Azure.Devices;

namespace SmartHome.API.Models;

public class Hub
{
	
	
	public string DeviceId { get; init; }
	private string HubName { get; init; }
	private string SharedAccesKey { get; init; }
	public static bool AlertStatus { get; set; }

	private readonly ServiceClient _serviceClient;
	
	public Hub(HubSettings settings)
	{
		DeviceId = settings.DeviceId;
		HubName = settings.HubName;
		SharedAccesKey = settings.SharedAccessKey;
		
		_serviceClient = GetServiceClient();
	}
	
	
	
	private string GetConnectionString()
	{
		return
			$"HostName={HubName}.azure-devices.net;" +
			$"SharedAccessKeyName=service;" + 
			$"SharedAccessKey={SharedAccesKey}";
	}

	private ServiceClient GetServiceClient()
	{
		string connectionString = GetConnectionString();
		return ServiceClient.CreateFromConnectionString(connectionString);
	}

	public async Task<CloudToDeviceMethodResult> DeviceMethodAlert()
	{
		if (AlertStatus)
			return new CloudToDeviceMethodResult{Status = 400};

		CloudToDeviceMethod method = new("turnOnAlert");
		method.ResponseTimeout = TimeSpan.FromSeconds(10);
		CloudToDeviceMethodResult response = await _serviceClient.InvokeDeviceMethodAsync(DeviceId, method);

		if (response.Status == 200)
		{
			AlertStatus = true;
			await Task.Delay(7000);
		}
		
		AlertStatus = false;
		return response;

	}
	
	public async Task<CloudToDeviceMethodResult> DeviceMethodXmas()
	{
		if (AlertStatus)
			return new CloudToDeviceMethodResult{Status = 400};
		
		CloudToDeviceMethod method = new("xmasMelody");
		method.ResponseTimeout = TimeSpan.FromSeconds(10);
		CloudToDeviceMethodResult response = await _serviceClient.InvokeDeviceMethodAsync(DeviceId, method);
			
		if (response.Status == 200)
		{
			AlertStatus = true;
			await Task.Delay(14000);
		}
		
		AlertStatus = false;
		return response;

	}
	
	
	
	
}