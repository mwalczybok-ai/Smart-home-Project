namespace SmartHome.API.Models;

public class HubSettings
{
	public string DeviceId { get; set; } = string.Empty;
	public string HubName { get; set; } = string.Empty;
	public string SharedAccessKey { get; set; } = string.Empty;
}