#include <DHT_U.h>
#include <Wire.h>
#include <BH1750.h>
#include <SoftwareSerial.h>
#include <DHT.h>
#include <Servo.h>

#define ESP_TX 7
#define ESP_RX 6
#define LAMP_PIN 5
#define RELAY_PIN 4
#define SERVO_PIN 3
#define DHT_PIN 2
#define WATERMETER A3
#define DHTTYPE DHT11

#define SENSOR_POLL_INTERVAL 3000
#define VENT_OPEN 35
#define VENT_CLOSE 110


#define IS_LOCAL false
#define DEBUG false
#define NOPE false

DHT dht(DHT_PIN, DHTTYPE);
BH1750 lightMeter;
SoftwareSerial esp(ESP_RX, ESP_TX);
Servo vent;

// Connection parametrs
char server[] = "52.3.52.231";
char appKey[] = "776a7a6c-75fe-4274-bd98-4dbab8c4a853";
char thing[] = "DreamHouseThing";

long last_sensors_poll = 0;

// DataModel
int Temperature = 0;
int Brightness = 0;
int Humidity = 0;
bool VentState = false;
bool PumpState = false;
int LampBrightness = 0;

void setup() {
  Serial.begin(115200);
  esp.begin(19200);
  dht.begin();
  digitalWrite(WATERMETER, 1);
  lightMeter.begin();
  vent.attach(SERVO_PIN);
  pinMode(LAMP_PIN, OUTPUT);
  pinMode(RELAY_PIN, OUTPUT);
  setServer();

  while (! Serial);
  while (! esp);
  last_sensors_poll = millis();
//if(!IS_LOCAL)getData("VentState");
  if(DEBUG)Serial.println("DEBUG_MODE");
  delay(2000);
  Serial.println("Start");
    esp.println(F("Mjson:"));
}

void loop() {
  if(!NOPE)
  if (IS_LOCAL)
  {
    localWork();
    //    if ((millis() - last_sensors_poll) > SENSOR_POLL_INTERVAL) {
    //      Serial.println(getData("PumpState"));
    //      Serial.println(getData("LampBrightness"));
    //      Serial.println(getData("VentState"));
    //       last_sensors_poll = millis();
    //    }
  } else {
    if (DEBUG)
    {
      while (esp.available() > 0)
      {
        Serial.write(esp.read());
      }
      while (Serial.available() > 0)
      {
        esp.write(Serial.read());
      }
    } else {


      if ((millis() - last_sensors_poll) > SENSOR_POLL_INTERVAL) {
        Humidity = analogRead(WATERMETER);
        Temperature = dht.readTemperature();
        Brightness = lightMeter.readLightLevel();

        //SendData
        sendAllData();
        //        sendData(String(Temperature), "Temperature");
        //        sendData(String(Humidity), "Humidity");
        //        sendData(String(Brightness), "Brightness");
      //GetValues
//      String Stmp = getData("VentState");
//      if (Stmp.indexOf("t") > 0)VentState = true;
//      else if (Stmp.indexOf("f") > 0)VentState = false;
//      Stmp="";
//      Stmp = getData("PumpState");
//      if (Stmp.indexOf("t") > 0)PumpState = true;
//      else if (Stmp.indexOf("f") > 0)PumpState = false;
//
//      Stmp="";
//      Stmp = getData("LampBrightness");
//      LampBrightness = Stmp.toInt();
      autoMode();
      //CheckData
      LampBrightness = LampBrightness < 0 ? 0 : LampBrightness;
      LampBrightness = LampBrightness > 255 ? 255 : LampBrightness;
      //LampBrightness = map(LampBrightness, 0, 100, 0, 255);

      analogWrite(LAMP_PIN, LampBrightness);
      digitalWrite(RELAY_PIN, PumpState ? 1 : 0);
      vent.write(VentState ? VENT_OPEN : VENT_CLOSE);
        last_sensors_poll = millis();
      }
    }
  }
}


void setServer()
{
  esp.println();
  esp.println((String("h" + String(server))));
  esp.println(F("p80"));
}

void sendData(String data, String param)
{
  clearBuf();
  String json = String("{\"" + param + "\": " + data + "}");
  esp.print(F("aPUT /Thingworx/Things/"));
  esp.print(thing);
  esp.print(F("/Properties/"));
  esp.print(param);
  esp.println(F(" HTTP/1.1"));
  esp.print(F("aHost: "));
  esp.println(server);
  esp.println(F("aContent-Type: application/json"));
  esp.print(F("aContent-Length: "));
  esp.println(json.length());
  esp.print(F("aappKey: "));
  esp.println(appKey);
  esp.println(F("a"));
  esp.println("a" + json);
  esp.println(F("a"));
  esp.println(F("C"));
  
  if (DEBUG) {
    Serial.print(millis());
    Serial.print(F("  "));
    Serial.print(param);
    Serial.print(F(" sended:  "));
    Serial.println(data);
  }
}

void sendAllData()
{
  clearBuf();
  int  B=map(Brightness,0,1150,0,100);
  String json = String(String("{\"Temperature\": ") + Temperature + String(",\"Brightness\": ") + B + String(",\"Humidity\": ") + Humidity + String("}"));
  esp.print(F("aPUT /Thingworx/Things/"));
  esp.print(thing);
  esp.print(F("/Properties/*"));
  esp.println(F(" HTTP/1.1"));
  esp.print(F("aHost: "));
  esp.println(server);
  esp.println(F("aContent-Type: application/json"));
  esp.print(F("aContent-Length: "));
  esp.println(json.length());
  esp.print(F("aappKey: "));
  esp.println(appKey);
  esp.println(F("a"));
  esp.println("a" + json);
  esp.println(F("a"));
  esp.println(F("C"));
}

void clearBuf()
{
  esp.println(F("i"));
}

String getData(String param)
{
  String Data = "";
  clearBuf();

  esp.print(F("aGET /Thingworx/Things/"));
  esp.print(thing);
  esp.print(F("/Properties/"));
  esp.print(param);
  esp.println(F(" HTTP/1.1"));
  esp.print(F("aHost: "));
  esp.println(server);
  esp.println(F("aAccept: application/json"));
  esp.print(F("aappKey: "));
  esp.println(appKey);
  esp.println(F("a"));
  esp.println(F("C"));
 //Serial.print("In");
 clearBuf();
  long i =millis();
  while (true)
  {
    if (esp.available()) {
      char c = esp.read();
      if (c == '$')break;
      Data += c;
    }
    //i++;
    if (millis()-i>10000)
    {
      Data = "err";
      break;
    }
  }
  // Serial.println("Out");
//Serial.println(Data);
    Data = Data.substring(Data.lastIndexOf(param) + param.length() + 1);
    Data = Data.substring(1, Data.length() - 3);
  if (false) {
    Serial.print(millis());
    Serial.print(F("   Getting "));
    Serial.print(param);
    Serial.print(F(": "));
    Serial.println(Data);
  }
  return Data;
}

void autoMode()
{
       if (Temperature>45)VentState = true;
      else if (Temperature<40)VentState = false;
            if (Humidity>980)PumpState = false;
      else if (Humidity>920)PumpState = true;

      LampBrightness=(1024-Brightness)/4;
}

//void getAllData()
//{
//  String Data = "";
//  clearBuf();
//  Serial.println("Send");
//  esp.println(F("Mjson:"));
//  esp.print(F("aGET /Thingworx/Things/"));
//  esp.print(thing);
//  esp.print(F("/Properties/"));
//  esp.println(F(" HTTP/1.1"));
//  esp.print(F("aHost: "));
//  esp.println(server);
//  esp.println(F("aAccept: application/json"));
//  esp.print(F("aappKey: "));
//  esp.println(appKey);
//  esp.println(F("a"));
//  esp.println(F("C"));
//
//  Serial.println("Wait");
//  long i=millis();
//    while (true)
//  {
//    if (esp.available()) {
//      char c = esp.read();
//      //if (c == '$')break;
//      Data += c;
//    }
//    //i++;
//    if (millis()-i>10000)
//    {
//      Data = "err";
//      break;
//    }
//  }
//   // Data = Data.substring(Data.lastIndexOf(param) + param.length() + 1);
//    //Data = Data.substring(1, Data.length() - 3);
//    Serial.println("Get");
//  //Data=Data.substring(Data.lastIndexOf("rows"));
//  Serial.println(Data);
//  String d1="",d2="",d3="";
//  d1=Data.substring(Data.lastIndexOf("VentState")+10,Data.length()-(Data.lastIndexOf("VentState")+10)+2);
//  d2=Data.substring(Data.lastIndexOf("PumpState")+10,Data.length()-(Data.lastIndexOf("PumpState")+10)+2);
//  d3=Data.substring(Data.lastIndexOf("LampBrightness")+10,Data.length()-(Data.lastIndexOf("LampBrightness")+10)+Data.indexOf(Data.lastIndexOf("LampBrightness")+10));
//  if (true) {
//    //Serial.println(Data);
//    Serial.println(d1);
//    Serial.println(d2);
//    Serial.println(d3);
//  }
//}
//------LOCAL-----------------------------------------------------------------------------------------------------------------------------
void localWork()
{
  String buf = "";
  while (Serial.available())
  {
    char c = Serial.read();
    buf += c;
  }
  if (buf.indexOf("Led=") >= 0) {
    buf = buf.substring(buf.indexOf("Led=") + 4);
    Serial.print(buf);
    buf = buf.substring(0, buf.length() - 2);
    int value = buf.toInt();
    analogWrite(LAMP_PIN, value);
  }

  if (buf.indexOf("PumpOn") >= 0) {
    digitalWrite(RELAY_PIN, 1);
  }
  if (buf.indexOf("PumpOff") >= 0)
  {
    digitalWrite(RELAY_PIN, 0);
  }
  if (buf.indexOf("VentOn") >= 0) {
    vent.write(40);
  }
  if (buf.indexOf("VentOff") >= 0)
  {
    vent.write(170);
  }

  if ((millis() - last_sensors_poll) > SENSOR_POLL_INTERVAL) {
    Humidity = analogRead(WATERMETER);
    Temperature = dht.readTemperature();
    Brightness = lightMeter.readLightLevel();

    Serial.print("Temperature=");
    if (!isnan(Temperature)) {
      Serial.print(Temperature);
    } else {
      Serial.print("--");
    }
    Serial.print(" Brightness=");
    Serial.print(Brightness);
    Serial.print(" Humidity=");
    Serial.println(Humidity);
    last_sensors_poll = millis();
  }
}
