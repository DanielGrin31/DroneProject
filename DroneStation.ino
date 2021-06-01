#include <SPI.h>
#include <nRF24L01.h>
#include <RF24.h>
#include <LiquidCrystal_I2C.h>
#define joy1X A0
#define joy1Y A1
#define joy2Y A2
#define joy2X A3
const byte address[6] = "NRF24";
const String mode="Joystick";
 RF24 radio(9, 8);  // CE, CSN
LiquidCrystal_I2C lcd(0x27,16,2);  // set the LCD address to 0x27 for a 16 chars and 2 line display
int x1Reading=0,y1Reading=0,x2Reading=0,y2Reading=0;
int x1Map=0,y1Map=0,y2Map=0,x2Map=0;

void setup()
{
  Serial.begin(9600);
  radio.begin();
  radio.openReadingPipe(0, address);
    radio.openWritingPipe(address);
  radio.startListening();
  radio.setChannel(2);
  lcd.init();                      // initialize the lcd 
  // Print a message to the LCD.
  lcd.backlight();
  lcd.clear();
  
}



void loop()
{
    if (radio.available())
  {
    char text[32] = {0};
    radio.read(&text, sizeof(text));
    if(text=="Mode"){
      String message="";
      message="Mode:"+mode;
      radio.stopListening();
      radio.write(&text,sizeof(text));
      delay(1000);
      radio.startListening();
    }
    else {
      String acc = splitString(text, ':', 0);
      String altitude = splitString(text, ':', 1);
      String x=splitString(acc,'|',0);
      String y=splitString(acc,'|',1);
      String z=splitString(acc,'|',2);
      lcd.clear();
    lcd.setCursor(0,0);
    lcd.print("X:"+x+" Y:"+y+" Z:"+z);
    lcd.setCursor(0,1);
    lcd.print("Altitude: "+String(altitude));
    }
  }
  if(mode=="Joystick"){
      joystickInput();
      int fd=4;
      if(y2Map>2){
        fd=0;
      }
      else if(y2Map<2){
        fd=1;
      }
      else if(x2Map<2){
        fd=2;
      }
      else if(x2Map>2){
        fd=3;
      }
      String joystickInfo="tl:"+String(y1Map)+"fd:"+String(fd);
      radio.stopListening();
      radio.write(&joystickInfo,sizeof(joystickInfo));
      radio.startListening();

  }
  if(Serial.available()){
      radio.stopListening();
      String stopmsg="Stop";
      radio.write(&stopmsg,sizeof(stopmsg));
      radio.startListening();
  }
    delay(100);  
}
void joystickInput()
{
    x1Reading =( analogRead(joy1X));
    x2Reading =( analogRead(joy2X));
    y1Reading =( analogRead(joy1Y));
    y2Reading =( analogRead(joy2Y));
    x1Map = map(x1Reading, 0,1023,0, 5);
    y1Map = map(y1Reading, 0,1023,0, 5);
    x2Map = map(x2Reading, 0,1023,0, 5);
    y2Map = map(y2Reading, 0,1023,0, 5);
}


String splitString(String data, char separator, int index)
{
    int found = 0;
    int strIndex[] = { 0, -1 };
    int maxIndex = data.length() - 1;

    for (int i = 0; i <= maxIndex && found <= index; i++) {
        if (data.charAt(i) == separator || i == maxIndex) {
            found++;
            strIndex[0] = strIndex[1] + 1;
            strIndex[1] = (i == maxIndex) ? i+1 : i;
        }
    }
    return found > index ? data.substring(strIndex[0], strIndex[1]) : "";
}
