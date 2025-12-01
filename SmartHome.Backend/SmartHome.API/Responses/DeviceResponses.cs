namespace SmartHome.API.Responses;

public record DeviceInfo(
	string deviceId, 
	long time, 
	float avgTemp, 
	float avgLight, 
	int light);

public record DeviceResponse(
	string Time,
	float AverageTemperature,
	float AverageLightLevel,
	int LightLevelAtTheMoment
);
