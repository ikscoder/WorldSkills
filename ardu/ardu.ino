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

#define SENSOR_POLL_INTERVAL 1500

#define IS_LOCAL false
#define DEBUG false

DHT dht(DHT_PIN, DHTTYPE);
BH1750 lightMeter;
SoftwareSerial esp(ESP_RX, ESP_TX);
Servo vent;

// Connection parametrs
char server[] = "tvsn.cloud.thingworx.com";
char appKey[] = "";
char thing[] = "DreamHouseThing";

long last_sensors_poll = 0;

// DataModel
int Temperature;
int Brightness;
int Humidity;
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
}

void loop() {
  if (IS_LOCAL)
  {
    localWork();
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
      //GetValues
      String Stmp = getData("VentState");
      if (Stmp.equals("true"))VentState = true;
      if (Stmp.equals("false"))VentState = false;
      Stmp = getData("PumpState");
      if (Stmp.equals("true"))PumpState = true;
      if (Stmp.equals("false"))PumpState = false;
      Stmp = getData("LampBrightness");
      LampBrightness = Stmp.toInt();
      //CheckData
      LampBrightness = LampBrightness < 0 ? 0 : LampBrightness;
      LampBrightness = LampBrightness > 100 ? 100 : LampBrightness;
      LampBrightness = map(LampBrightness, 0, 100, 0, 255);

      analogWrite(LAMP_PIN, LampBrightness);
      digitalWrite(RELAY_PIN, PumpState ? 1 : 0);
      vent.write(VentState ? 40 : 170);

      if ((millis() - last_sensors_poll) > SENSOR_POLL_INTERVAL) {
        Humidity = analogRead(WATERMETER);
        Temperature = dht.readTemperature();
        Brightness = lightMeter.readLightLevel();

        //SendData
        sendData(String(Temperature), "Temperature");
        sendData(String(Humidity), "Humidity");
        sendData(String(Brightness), "Brightness");

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
  Serial.print(millis());
  Serial.print(F("  "));
  Serial.print(param);
  Serial.print(F(" sended:  "));
  Serial.println(data);
}

void clearBuf()
{
  esp.println(F("i"));
}

String getData(String param)
{
  String Data = "";
  clearBuf();
  esp.println(F("Mjson:"));
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

  long i = 0;
  while (true)
  {
    if (esp.available()) {
      char c = esp.read();
      if (c == '$')break;
      Data += c;
    }
    i++;
    if (i >= 2000000)
    {
      Data = "err";
      break;
    }
  }

  Data = Data.substring(Data.lastIndexOf(param) + param.length() + 1);
  Data = Data.substring(1, Data.length() - 3);
  Serial.print(millis());
  Serial.print(F("   Getting "));
  Serial.print(param);
  Serial.print(F(": "));
  Serial.println(Data);
  return Data;
}
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
