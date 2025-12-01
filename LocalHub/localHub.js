import pkg from 'azure-iot-device';
import { Mqtt } from "azure-iot-device-mqtt";
const { Message, Client } = pkg;

import { ReadlineParser } from "@serialport/parser-readline";
import { SerialPort } from "serialport";

import dotenv from 'dotenv'
dotenv.config({ path: './.env' })



let serialPort;
//arduino part
async function subscribeOnDevice(){
     serialPort = new SerialPort({ path: "/dev/ttyACM0", baudRate: 9600});
    const parser = serialPort.pipe(new ReadlineParser({ delimiter: "\r\n"}));


    let lightList = [];
    let tempList = [];

    setInterval(() => {
        sendTelemetry(lightList, tempList);
        lightList =[];
        tempList =[];
    }, 20000)


    parser.on("data", line => {
        console.log("Arduino:", line);

        // line.trim();
        let {light, temp} = JSON.parse(line);

        lightList.push(light);
        tempList.push(temp);

    });

}


async function sendTelemetry(light, temp){

    if (temp.length === 0 || light.length === 0){
        console.log("\n\tThere is no temperature or light data\n\tTelemetry has not been sent\n")
        return;
    }

    let averageTemp = temp.reduce((a, b) => a + b) / temp.length;
    let averageLight = light.reduce((a, b) => a + b) / light.length;

    const payload = {
        "deviceId": deviceId,
        "time": Date.now(),
        "avgTemp": averageTemp,
        "avgLight": averageLight,
        "light": light[light.length - 1],
    }

    let msg = new Message(JSON.stringify(payload));

    await client.sendEvent(msg);
    console.log("\n\tTelemetry sent \n");

}





//device method
async function turnOnAlert(req, res){
    serialPort.write("alert\n",
        (err) => {
            if (err) {
                console.error("Error sending:", err)
                return
            } else {
                console.log("Command sent");
            }
        });

    console.log("Turning on alert \n");


    res.send(200, {result: "ok"},
        function(err) {
            if (err) {
                console.error('An error occurred when sending a method response:\n' + err.toString());
            } else {
                console.log('Response to method \'' + req.methodName + '\' sent successfully.');
            }
        });

}
//device method
async function xmasMelody(req,res){
    serialPort.write("xmas\n",
        (err) => {
            if (err) {
                console.error("Error sending:", err)
                return
            } else {
                console.log("Command sent");
            }
        });

    console.log("Turning on xmas \n");


    res.send(200, {result: "ok"},
        function(err) {
            if (err) {
                console.error('An error occurred when sending a method response:\n' + err.toString());
            } else {
                console.log('Response to method \'' + req.methodName + '\' sent successfully.');
            }
        });

}








const deviceId = process.env.DEVICE_ID;
const sasToken = process.env.SAS_TOKEN;
const ioTHubName = process.env.HUB_NAME;

const connectionString = `HostName=${ioTHubName}.azure-devices.net;DeviceId=${deviceId};SharedAccessKey=${sasToken};`;

const client = Client.fromConnectionString(connectionString, Mqtt);


async function sendToAzureHubCallback(err) {
    if (err)
        console.error('Could not connect: ' + err);

    console.log("Connected to Azure IoT Hub");


    await subscribeOnDevice();


    client.onDeviceMethod("turnOnAlert", turnOnAlert);
    client.onDeviceMethod("xmasMelody", xmasMelody);



}


console.log('Connecting to Azure IoT Hub...');
client.on('error', ()=>console.log("error"))
client.on('message', (e)=>{
    console.log(e)
    console.log(
    Buffer.from(e.data,"hex").toString("utf-8"))
    }
);

await client.open(sendToAzureHubCallback);
