using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.Devices;
using Microsoft.Extensions.Options;
using SmartHome.API.Models;
using SmartHome.API.Responses;
using SmartHome.Infrastructure;

namespace SmartHome.API.Controllers;

[ApiController]
[Route("[controller]")]
public class DeviceController : ControllerBase
{
	
	private readonly Container _container;
	private readonly Hub _hub;
	
	public DeviceController(IOptions<HubSettings> hubSettings, SmartHomeDb smartHomeDb)
	{
		SmartHomeDb cosmos = smartHomeDb;
		_container = cosmos.GetContainer();
		_hub = new Hub(hubSettings.Value);
	}
	

	[HttpGet("/info/last")]
	public async Task<ActionResult<DeviceResponse>> GetLastDeviceData()
	{
		IQueryable<DeviceInfo> queryable = _container
			.GetItemLinqQueryable<DeviceInfo>()
			.Where(d => d.deviceId == _hub.DeviceId)
			.OrderByDescending(d => d.time)
			.Take(1);

		using FeedIterator<DeviceInfo> linqFeed = queryable.ToFeedIterator();


		FeedResponse<DeviceInfo> response = await linqFeed.ReadNextAsync();
			
		DeviceInfo info = response.FirstOrDefault()!;
		DeviceResponse deviceResponse = new(
			MillisecondsToString(info.time),
			info.avgTemp,
			info.avgLight,
			info.light
		);
		
		return deviceResponse;
		
	}
	
	[HttpGet("/info/last10")]
	public async Task<ActionResult<List<DeviceResponse>>> GetLast10DeviceData()
	{
		IQueryable<DeviceInfo> queryable = _container
			.GetItemLinqQueryable<DeviceInfo>()
			.Where(d => d.deviceId == _hub.DeviceId)
			.OrderByDescending(d => d.time)
			.Take(10);

		using FeedIterator<DeviceInfo> linqFeed = queryable.ToFeedIterator();

		List<DeviceResponse> responseList = new();
		while (linqFeed.HasMoreResults)
		{
			FeedResponse<DeviceInfo> response = await linqFeed.ReadNextAsync();

			foreach (DeviceInfo item in response)
			{
				responseList.Add(new DeviceResponse(
					MillisecondsToString(item.time),
					item.avgTemp,
					item.avgLight,
					item.light
					));
			}
		}
		
		return responseList;
	}

	private string MillisecondsToString(long milliseconds)
	{
		DateTime dateTime = DateTimeOffset
			.FromUnixTimeMilliseconds(milliseconds)
			.ToLocalTime()
			.DateTime;

		return dateTime.ToString("dd/MM/yyyy HH:mm:ss");
	}
	
	
	
	

	[HttpGet("/signals/alert")]
	public async Task<ActionResult<string>> TurnOnAlert()
	{
		CloudToDeviceMethodResult response = await _hub.DeviceMethodAlert();

		if (response.Status == 200)
			return Ok(response.GetPayloadAsJson()); 

		return StatusCode(response.Status, response.GetPayloadAsJson());
	}
	
	[HttpGet("/signals/xmas")]
	public async Task<ActionResult<string>> XmasMelody()
	{
		CloudToDeviceMethodResult response = await _hub.DeviceMethodXmas();
		
		if (response.Status == 200)
			return Ok(response.GetPayloadAsJson()); 
		
		return StatusCode(response.Status, response.GetPayloadAsJson());
	}
	
	
	
}