/*
* temperature
*
*
*
*/
#include <SoftwareSerial.h>
#include <Servo.h>

#define ESP_RX 6
#define ESP_TX 7
#define SERVO_PIN 9

Servo servo; 
SoftwareSerial esp(ESP_RX, ESP_TX);
char server[] = "34.249.39.144";
char appKey[] = "983fee7d-5713-48b6-b6a1-8704a1c1fc9d";

void setup() {
  esp.begin(19200);
  Serial.begin(115200);
  servo.attach(SERVO_PIN);
  while (! Serial);
  setServer();
  
  //sendData(String(22.0),"temperature");
  //Serial.println(getData("temperature"));
}

void loop() {
  /*while (esp.available() > 0)
    {
    Serial.write(esp.read());
    }
    while (Serial.available() > 0)
    {
    esp.write(Serial.read());
    }*/
}

void setServer()
{
  esp.println();
  esp.println((String("h" + String(server))));
  esp.println(F("p80"));//F() хранение строки в Flash памяти
}

void sendData(String data, String param)
{
  clearBuf();
  String json = String("{\"" + param + "\": " + data + "}");
  esp.print(F("aPUT /Thingworx/Things/TestT/Properties/"));
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
  esp.print(F("aGET /Thingworx/Things/TestT/Properties/"));
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
    if (i >= 4000000)
    {
      Data = "err";
      break;
    }
  }

  Data = Data.substring(Data.lastIndexOf(param) + param.length() + 1);
  Data = Data.substring(1, Data.length() - 3);
  return Data;
}
