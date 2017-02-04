/*
* temperature humidity brightness
* pumpState lightState ventState
*
*
*/
#include <SoftwareSerial.h>
#include <Servo.h>

#define DEBUG true
#define ESP_RX 6
#define ESP_TX 7
#define SERVO_PIN 9
#define RELAY_PIN 4

Servo servo; 
SoftwareSerial esp(ESP_RX, ESP_TX);
char server[] = "34.248.238.197";//"34.249.39.144";
char appKey[] = "e72278dc-8ad1-4df6-bde5-e4785cf2f236";//"983fee7d-5713-48b6-b6a1-8704a1c1fc9d";
char thing[]="HARThing";//TestT

void setup() {
  esp.begin(19200);
  Serial.begin(115200);
  //servo.attach(SERVO_PIN);
  pinMode(RELAY_PIN, OUTPUT);   
  while (! Serial);
  while (! esp);
  //Serial.println(WiFiCheck());
  setServer();
  Serial.println("Started...");
  sendData(String(90.0),"temperature");
  //Serial.println(getData("temperature"));
}

void loop() {
  if(DEBUG)while (esp.available() > 0)
    {
    Serial.write(esp.read());
    }
    while (Serial.available() > 0)
    {
    esp.write(Serial.read());
    }
    /*String buff="";
    while (true)
  {
    if (Serial.available()) {
      char c = Serial.read();
      if (c == '$')break;
      buff += c;
    }
  }
    if(buff!="")
    {
       sendData(String(float(buff.toInt())/10),"temperature");

    }*/
    if(!DEBUG){
    delay(500);
    String data=getData("ventState");
    if(data=="false")
    digitalWrite(RELAY_PIN, LOW);
    if(data=="true")
    digitalWrite(RELAY_PIN, HIGH);
    }
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

String WiFiCheck()
{
 esp.println(F("?sw"));
 String buf;
 delay(1000);
 while(esp.available())
 {
   Serial.write(esp.read());
 /* char c=esp.read();
  buf+=c;*/
 }
 return buf;
}
