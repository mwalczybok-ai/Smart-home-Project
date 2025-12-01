#include <Arduino.h>
#include "pitches.h" 


String inputString = "";

const int tempPin = 3;
const int lightPin = 0;
const int buzzerPin = 8;
int tempAdc;
int light;

float adcToC(int);
void alert();
void sendTelemetry();
void xmas();
void handleCommand(String command);
void checkTelemetryTimer();


void setup() {
  Serial.begin(9600);
  pinMode(buzzerPin, OUTPUT);
}

unsigned long lastSend = 0;

void loop() {
  
  checkTelemetryTimer();

  
  while(Serial.available()){
    char c = Serial.read();
    if (c == '\n') {           
      handleCommand(inputString);
      inputString = "";
      break;
    } else {
      inputString += c;
    }
  }

}

void checkTelemetryTimer()
{
  unsigned long now = millis();

  if (now - lastSend >= 500) {
    lastSend = now;
    sendTelemetry();
  }
}

void sendTelemetry(){

  light = analogRead(lightPin);
  tempAdc = analogRead(tempPin);

  Serial.print("{\"light\":");
  Serial.print(light);
  Serial.print(",");

  float temp = adcToC(tempAdc);

  Serial.print("\"temp\":");
  Serial.print(temp);
  Serial.println("}");
  
}

float adcToC(int adc) {

  float v = ((float)adc * 5)/1024;
  float t = (v-0.5)*100;

  return t;
}




void handleCommand(String command){

  if (command == "alert")
    alert();

  if (command == "xmas")
    xmas();

}


void alert()
{

  for (int i = 0; i<5; i++){
    checkTelemetryTimer();
      
    tone(buzzerPin, 4000, 1000); 
      

    delay(1200);
      
    noTone(buzzerPin); 
	  
  }
}


void xmas(){
  int melody[] = {
    NOTE_E5, NOTE_E5, NOTE_E5,
    NOTE_E5, NOTE_E5, NOTE_E5,
    NOTE_E5, NOTE_G5, NOTE_C5, NOTE_D5,
    NOTE_E5,
    NOTE_F5, NOTE_F5, NOTE_F5, NOTE_F5,
    NOTE_F5, NOTE_E5, NOTE_E5, NOTE_E5, NOTE_E5,
    NOTE_E5, NOTE_D5, NOTE_D5, NOTE_E5,
    NOTE_D5, NOTE_G5,
    NOTE_E5, NOTE_E5, NOTE_E5,
    NOTE_E5, NOTE_E5, NOTE_E5,
    NOTE_E5, NOTE_G5, NOTE_C5, NOTE_D5,
    NOTE_E5,
    NOTE_F5, NOTE_F5, NOTE_F5, NOTE_F5,
    NOTE_F5, NOTE_E5, NOTE_E5, NOTE_E5, NOTE_E5,
    NOTE_E5, NOTE_D5, NOTE_D5, NOTE_E5,
    NOTE_D5
  };

  int durations[] = {
    8, 8, 4,
    8, 8, 4,
    8, 8, 8, 8,
    2,
    8, 8, 8, 8,
    8, 8, 8, 16, 16,
    8, 8, 8, 8,
    4, 4,
    8, 8, 4,
    8, 8, 4,
    8, 8, 8, 8,
    2,
    8, 8, 8, 8,
    8, 8, 8, 16, 16,
    8, 8, 8, 8,
    4
  };

  int size = sizeof(durations) / sizeof(int);

  for (int note = 0; note < size; note++) {

  
    checkTelemetryTimer();

    //to calculate the note duration, take one second divided by the note type.
    //e.g. quarter note = 1000 / 4, eighth note = 1000/8, etc.
    int duration = 1200 / durations[note];
    tone(buzzerPin, melody[note], duration);

    //to distinguish the notes, set a minimum time between them.
    //the note's duration + 30% seems to work well:
    int pauseBetweenNotes = duration * 1.30;
    delay(pauseBetweenNotes);

    //stop the tone playing:
    noTone(buzzerPin);
  }
}