# 1 "E:\\Projekte\\MobiFlightFC\\FirmwareSource\\mobiflight_uno\\mobiflight_uno.ino"
# 1 "E:\\Projekte\\MobiFlightFC\\FirmwareSource\\mobiflight_uno\\mobiflight_uno.ino"
/**

 * Includes Core Arduino functionality 

 **/
# 4 "E:\\Projekte\\MobiFlightFC\\FirmwareSource\\mobiflight_uno\\mobiflight_uno.ino"
char foo;



# 9 "E:\\Projekte\\MobiFlightFC\\FirmwareSource\\mobiflight_uno\\mobiflight_uno.ino" 2


//#define DEBUG 0





//#define MODULETYPE MTYPE_MEGA
//#define MODULETYPE MTYPE_MICRO
# 64 "E:\\Projekte\\MobiFlightFC\\FirmwareSource\\mobiflight_uno\\mobiflight_uno.ino"
# 65 "E:\\Projekte\\MobiFlightFC\\FirmwareSource\\mobiflight_uno\\mobiflight_uno.ino" 2
# 66 "E:\\Projekte\\MobiFlightFC\\FirmwareSource\\mobiflight_uno\\mobiflight_uno.ino" 2
# 67 "E:\\Projekte\\MobiFlightFC\\FirmwareSource\\mobiflight_uno\\mobiflight_uno.ino" 2
# 68 "E:\\Projekte\\MobiFlightFC\\FirmwareSource\\mobiflight_uno\\mobiflight_uno.ino" 2
# 69 "E:\\Projekte\\MobiFlightFC\\FirmwareSource\\mobiflight_uno\\mobiflight_uno.ino" 2
# 70 "E:\\Projekte\\MobiFlightFC\\FirmwareSource\\mobiflight_uno\\mobiflight_uno.ino" 2
# 71 "E:\\Projekte\\MobiFlightFC\\FirmwareSource\\mobiflight_uno\\mobiflight_uno.ino" 2
# 72 "E:\\Projekte\\MobiFlightFC\\FirmwareSource\\mobiflight_uno\\mobiflight_uno.ino" 2
# 73 "E:\\Projekte\\MobiFlightFC\\FirmwareSource\\mobiflight_uno\\mobiflight_uno.ino" 2
# 74 "E:\\Projekte\\MobiFlightFC\\FirmwareSource\\mobiflight_uno\\mobiflight_uno.ino" 2
# 75 "E:\\Projekte\\MobiFlightFC\\FirmwareSource\\mobiflight_uno\\mobiflight_uno.ino" 2
# 76 "E:\\Projekte\\MobiFlightFC\\FirmwareSource\\mobiflight_uno\\mobiflight_uno.ino" 2
# 77 "E:\\Projekte\\MobiFlightFC\\FirmwareSource\\mobiflight_uno\\mobiflight_uno.ino" 2
# 78 "E:\\Projekte\\MobiFlightFC\\FirmwareSource\\mobiflight_uno\\mobiflight_uno.ino" 2

const byte MEM_OFFSET_NAME = 0;
const byte MEM_LEN_NAME = 48;
const byte MEM_OFFSET_SERIAL = MEM_OFFSET_NAME + MEM_LEN_NAME;
const byte MEM_LEN_SERIAL = 11;
const byte MEM_OFFSET_CONFIG = MEM_OFFSET_NAME + MEM_LEN_NAME + MEM_LEN_SERIAL;

// 1.0.1 : Nicer firmware update, more outputs (20)
// 1.1.0 : Encoder support, more outputs (30)
// 1.2.0 : More outputs (40), more inputs (40), more led segments (4), more encoders (20), steppers (10), servos (10)
// 1.3.0 : Generate New Serial
// 1.4.0 : Servo + Stepper support
// 1.4.1 : Reduce velocity
// 1.5.0 : Improve servo behaviour
// 1.6.0 : Set name
// 1.6.1 : Reduce servo noise
// 1.7.0 : New Arduino IDE, new AVR, Uno Support
// 1.7.1 : More UNO stability
// 1.7.2 : "???"
// 1.7.3 : Servo behaviour improved, fixed stepper bug #178, increased number of buttons per module (MEGA)
const char version[8] = "1.7.3";
# 117 "E:\\Projekte\\MobiFlightFC\\FirmwareSource\\mobiflight_uno\\mobiflight_uno.ino"
char type[20] = "MobiFlight Uno";
char serial[MEM_LEN_SERIAL] = "0987654321";
char name[MEM_LEN_NAME] = "MobiFlight Uno";
int eepromSize = 1024;
const int MEM_LEN_CONFIG = 256;


char configBuffer[MEM_LEN_CONFIG] = "";

int configLength = 0;
boolean configActivated = false;

bool powerSavingMode = false;
byte pinsRegistered[13];
const unsigned long POWER_SAVING_TIME = 60*15; // in seconds

CmdMessenger cmdMessenger = CmdMessenger(Serial);
unsigned long lastCommand;

MFOutput outputs[8];
byte outputsRegistered = 0;

MFButton buttons[8];
byte buttonsRegistered = 0;

MFSegments ledSegments[1];
byte ledSegmentsRegistered = 0;

MFEncoder encoders[2];
byte encodersRegistered = 0;

MFStepper *steppers[2]; //
byte steppersRegistered = 0;

MFServo servos[2];
byte servosRegistered = 0;

enum
{
  kTypeNotSet, // 0 
  kTypeButton, // 1
  kTypeEncoder, // 2
  kTypeOutput, // 3
  kTypeLedSegment, // 4
  kTypeStepper, // 5
  kTypeServo, // 6
};

// This is the list of recognized commands. These can be commands that can either be sent or received. 
// In order to receive, attach a callback function to these events
enum
{
  kInitModule, // 0
  kSetModule, // 1
  kSetPin, // 2
  kSetStepper, // 3
  kSetServo, // 4
  kStatus, // 5, Command to report status
  kEncoderChange, // 6  
  kButtonChange, // 7
  kStepperChange, // 8
  kGetInfo, // 9
  kInfo, // 10
  kSetConfig, // 11
  kGetConfig, // 12
  kResetConfig, // 13
  kSaveConfig, // 14
  kConfigSaved, // 15
  kActivateConfig, // 16
  kConfigActivated, // 17
  kSetPowerSavingMode, // 18  
  kSetName, // 19
  kGenNewSerial, // 20
  kResetStepper, // 21
  kSetZeroStepper, // 22
  kTrigger, // 23
  kResetBoard // 24
};

// Callbacks define on which received commands we take action
void attachCommandCallbacks()
{
  // Attach callback methods
  cmdMessenger.attach(OnUnknownCommand);
  cmdMessenger.attach(kInitModule, OnInitModule);
  cmdMessenger.attach(kSetModule, OnSetModule);
  cmdMessenger.attach(kSetPin, OnSetPin);
  cmdMessenger.attach(kSetStepper, OnSetStepper);
  cmdMessenger.attach(kSetServo, OnSetServo);
  cmdMessenger.attach(kGetInfo, OnGetInfo);
  cmdMessenger.attach(kGetConfig, OnGetConfig);
  cmdMessenger.attach(kSetConfig, OnSetConfig);
  cmdMessenger.attach(kResetConfig, OnResetConfig);
  cmdMessenger.attach(kSaveConfig, OnSaveConfig);
  cmdMessenger.attach(kActivateConfig, OnActivateConfig);
  cmdMessenger.attach(kSetName, OnSetName);
  cmdMessenger.attach(kGenNewSerial, OnGenNewSerial);
  cmdMessenger.attach(kResetStepper, OnResetStepper);
  cmdMessenger.attach(kSetZeroStepper, OnSetZeroStepper);
  cmdMessenger.attach(kTrigger, OnTrigger);
  cmdMessenger.attach(kResetBoard, OnResetBoard);




}

void OnResetBoard() {
  EEPROM.setMaxAllowedWrites(1000);
  EEPROM.setMemPool(0, eepromSize);

  configBuffer[0]='\0';
  //readBuffer[0]='\0'; 
  generateSerial(false);
  clearRegisteredPins();
  lastCommand = millis();
  loadConfig();
  _restoreName();
}

// Setup function
void setup()
{
  Serial.begin(115200);
  attachCommandCallbacks();
  OnResetBoard();
  cmdMessenger.printLfCr();
}

void generateSerial(bool force)
{
  EEPROM.readBlock<char>(MEM_OFFSET_SERIAL, serial, MEM_LEN_SERIAL);
  if (!force&&serial[0]=='S'&&serial[1]=='N') return;
  randomSeed(analogRead(0));
  sprintf(serial,"SN-%03x-", (unsigned int) random(4095));
  sprintf(&serial[7],"%03x", (unsigned int) random(4095));
  EEPROM.writeBlock<char>(MEM_OFFSET_SERIAL, serial, MEM_LEN_SERIAL);
}

void loadConfig()
{
  resetConfig();
  EEPROM.readBlock<char>(MEM_OFFSET_CONFIG, configBuffer, MEM_LEN_CONFIG);




  for(configLength=0;configLength!=MEM_LEN_CONFIG;configLength++) {
    if (configBuffer[configLength]!='\0') continue;
    break;
  }
  readConfig(configBuffer);
  _activateConfig();
}

void _storeConfig()
{
  EEPROM.writeBlock<char>(MEM_OFFSET_CONFIG, configBuffer, MEM_LEN_CONFIG);
}

void SetPowerSavingMode(bool state)
{
  // disable the lights ;)
  powerSavingMode = state;
  PowerSaveLedSegment(state);






  //PowerSaveOutputs(state);
}

void updatePowerSaving() {
  if (!powerSavingMode && ((millis() - lastCommand) > (POWER_SAVING_TIME * 1000))) {
    // enable power saving
    SetPowerSavingMode(true);
  } else if (powerSavingMode && ((millis() - lastCommand) < (POWER_SAVING_TIME * 1000))) {
    // disable power saving
    SetPowerSavingMode(false);
  }
}

// Loop function
void loop()
{
  // Process incoming serial data, and perform callbacks   
  cmdMessenger.feedinSerialData();
  updatePowerSaving();

  // if config has been reset
  // and still is not activated
  // do not perform updates
  // to prevent mangling input for config (shared buffers)
  if (!configActivated) return;

  readButtons();
  readEncoder();

  // segments do not need update
  updateSteppers();
  updateServos();
}

bool isPinRegistered(byte pin) {
  return pinsRegistered[pin] != kTypeNotSet;
}

bool isPinRegisteredForType(byte pin, byte type) {
  return pinsRegistered[pin] == type;
}

void registerPin(byte pin, byte type) {
  pinsRegistered[pin] = type;
}

void clearRegisteredPins(byte type) {
  for(int i=0; i!=13;++i)
    if (pinsRegistered[i] == type)
      pinsRegistered[i] = kTypeNotSet;
}

void clearRegisteredPins() {
  for(int i=0; i!=13;++i)
    pinsRegistered[i] = kTypeNotSet;
}

//// OUTPUT /////
void AddOutput(uint8_t pin = 1, String name = "Output")
{
  if (outputsRegistered == 8) return;
  if (isPinRegistered(pin)) return;

  outputs[outputsRegistered] = MFOutput(pin);
  registerPin(pin, kTypeOutput);
  outputsRegistered++;



}

void ClearOutputs()
{
  clearRegisteredPins(kTypeOutput);
  outputsRegistered = 0;



}

//// BUTTONS /////
void AddButton(uint8_t pin = 1, String name = "Button")
{
  if (buttonsRegistered == 8) return;

  if (isPinRegistered(pin)) return;

  buttons[buttonsRegistered] = MFButton(pin, name);
  buttons[buttonsRegistered].attachHandler(btnOnRelease, handlerOnRelease);
  buttons[buttonsRegistered].attachHandler(btnOnPress, handlerOnRelease);

  registerPin(pin, kTypeButton);
  buttonsRegistered++;



}

void ClearButtons()
{
  clearRegisteredPins(kTypeButton);
  buttonsRegistered = 0;



}

//// ENCODERS /////
void AddEncoder(uint8_t pin1 = 1, uint8_t pin2 = 2, String name = "Encoder")
{
  if (encodersRegistered == 2) return;
  if (isPinRegistered(pin1) || isPinRegistered(pin2)) return;

  encoders[encodersRegistered] = MFEncoder();
  encoders[encodersRegistered].attach(pin1, pin2, name);
  encoders[encodersRegistered].attachHandler(encLeft, handlerOnEncoder);
  encoders[encodersRegistered].attachHandler(encLeftFast, handlerOnEncoder);
  encoders[encodersRegistered].attachHandler(encRight, handlerOnEncoder);
  encoders[encodersRegistered].attachHandler(encRightFast, handlerOnEncoder);

  registerPin(pin1, kTypeEncoder); registerPin(pin2, kTypeEncoder);
  encodersRegistered++;



}

void ClearEncoders()
{
  clearRegisteredPins(kTypeEncoder);
  encodersRegistered = 0;



}

//// OUTPUTS /////

//// SEGMENTS /////
void AddLedSegment(int dataPin, int csPin, int clkPin, int numDevices, int brightness)
{
  if (ledSegmentsRegistered == 1) return;

  if (isPinRegistered(dataPin) || isPinRegistered(clkPin) || isPinRegistered(csPin)) return;

  ledSegments[ledSegmentsRegistered].attach(dataPin,csPin,clkPin,numDevices,brightness); // lc is our object

  registerPin(dataPin, kTypeLedSegment);
  registerPin(csPin, kTypeLedSegment);
  registerPin(clkPin, kTypeLedSegment);
  ledSegmentsRegistered++;



}

void ClearLedSegments()
{
  clearRegisteredPins(kTypeLedSegment);
  for (int i=0; i!=ledSegmentsRegistered; i++) {
    ledSegments[ledSegmentsRegistered].detach();
  }
  ledSegmentsRegistered = 0;



}

void PowerSaveLedSegment(bool state)
{
  for (int i=0; i!= ledSegmentsRegistered; ++i) {
    ledSegments[i].powerSavingMode(state);
  }

  for (int i=0; i!= outputsRegistered; ++i) {
    outputs[i].powerSavingMode(state);
  }
}
//// STEPPER ////
void AddStepper(int pin1, int pin2, int pin3, int pin4, int btnPin1)
{
  if (steppersRegistered == 2) return;
  if (isPinRegistered(pin1) || isPinRegistered(pin2) || isPinRegistered(pin3) || isPinRegistered(pin4) /* || isPinRegistered(btnPin1) */) {



    return;
  }
  steppers[steppersRegistered] = new MFStepper(pin1, pin2, pin3, pin4 /*, btnPin1*/ ); // is our object 
  steppers[steppersRegistered]->setMaxSpeed(600 /* 300 already worked, 467, too?*/);
  steppers[steppersRegistered]->setAcceleration(900);
  registerPin(pin1, kTypeStepper); registerPin(pin2, kTypeStepper); registerPin(pin3, kTypeStepper); registerPin(pin4, kTypeStepper);
  // autoreset is not released yet
  // registerPin(btnPin1, kTypeStepper);
  steppersRegistered++;




}

void ClearSteppers()
{
  for (int i=0; i!=steppersRegistered; i++)
  {
    delete steppers[steppersRegistered];
  }
  clearRegisteredPins(kTypeStepper);
  steppersRegistered = 0;



}

//// SERVOS /////
void AddServo(int pin)
{
  if (servosRegistered == 2) return;
  if (isPinRegistered(pin)) return;

  servos[servosRegistered].attach(pin, true);
  registerPin(pin, kTypeServo);
  servosRegistered++;
}

void ClearServos()
{
  for (int i=0; i!=servosRegistered; i++)
  {
    servos[servosRegistered].detach();
  }
  clearRegisteredPins(kTypeServo);
  servosRegistered = 0;



}

//// EVENT HANDLER /////
void handlerOnRelease(byte eventId, uint8_t pin, String name)
{
  cmdMessenger.sendCmdStart(kButtonChange);
  cmdMessenger.sendCmdArg(name);
  cmdMessenger.sendCmdArg(eventId);
  cmdMessenger.sendCmdEnd();
};

//// EVENT HANDLER /////
void handlerOnEncoder(byte eventId, uint8_t pin, String name)
{
  cmdMessenger.sendCmdStart(kEncoderChange);
  cmdMessenger.sendCmdArg(name);
  cmdMessenger.sendCmdArg(eventId);
  cmdMessenger.sendCmdEnd();
};

/**

 ** config stuff

 **/
# 547 "E:\\Projekte\\MobiFlightFC\\FirmwareSource\\mobiflight_uno\\mobiflight_uno.ino"
void OnSetConfig()
{




  lastCommand = millis();
  String cfg = cmdMessenger.readStringArg();
  int cfgLen = cfg.length();
  int bufferSize = MEM_LEN_CONFIG - (configLength+cfgLen);

  if (bufferSize>1) {
    cfg.toCharArray(&configBuffer[configLength], bufferSize);
    configLength += cfgLen;
    cmdMessenger.sendCmd(kStatus,configLength);
  } else
    cmdMessenger.sendCmd(kStatus,-1);



}

void resetConfig()
{
  ClearButtons();
  ClearEncoders();
  ClearOutputs();
  ClearLedSegments();
  ClearServos();
  ClearSteppers();
  configLength = 0;
  configActivated = false;
}

void OnResetConfig()
{
  resetConfig();
  cmdMessenger.sendCmd(kStatus, "OK");
}

void OnSaveConfig()
{
  _storeConfig();
  cmdMessenger.sendCmd(kConfigSaved, "OK");
}

void OnActivateConfig()
{
  readConfig(configBuffer);
  _activateConfig();
  cmdMessenger.sendCmd(kConfigActivated, "OK");
}

void _activateConfig() {
  configActivated = true;
}

void readConfig(String cfg) {
  char readBuffer[MEM_LEN_CONFIG+1] = "";
  char *p = __null;
  cfg.toCharArray(readBuffer, MEM_LEN_CONFIG);

  char *command = strtok_r(readBuffer, ".", &p);
  char *params[6];
  if (*command == 0) return;

  do {
    switch (atoi(command)) {
      case kTypeButton:
        params[0] = strtok_r(__null, ".", &p); // pin
        params[1] = strtok_r(__null, ":", &p); // name
        AddButton(atoi(params[0]), params[1]);
      break;

      case kTypeOutput:
        params[0] = strtok_r(__null, ".", &p); // pin
        params[1] = strtok_r(__null, ":", &p); // Name
        AddOutput(atoi(params[0]), params[1]);
      break;

      case kTypeLedSegment:
        params[0] = strtok_r(__null, ".", &p); // pin Data
        params[1] = strtok_r(__null, ".", &p); // pin Cs
        params[2] = strtok_r(__null, ".", &p); // pin Clk
        params[3] = strtok_r(__null, ".", &p); // brightness
        params[4] = strtok_r(__null, ".", &p); // numModules
        params[5] = strtok_r(__null, ":", &p); // Name
        // int dataPin, int clkPin, int csPin, int numDevices, int brightness
        AddLedSegment(atoi(params[0]), atoi(params[1]), atoi(params[2]), atoi(params[4]), atoi(params[3]));
      break;

      case kTypeStepper:
        // AddStepper(int pin1, int pin2, int pin3, int pin4)
        params[0] = strtok_r(__null, ".", &p); // pin1
        params[1] = strtok_r(__null, ".", &p); // pin2
        params[2] = strtok_r(__null, ".", &p); // pin3
        params[3] = strtok_r(__null, ".", &p); // pin4
        params[4] = strtok_r(__null, ".", &p); // btnPin1
        params[5] = strtok_r(__null, ":", &p); // Name
        AddStepper(atoi(params[0]), atoi(params[1]), atoi(params[2]), atoi(params[3]), atoi(params[4]));
      break;

      case kTypeServo:
        // AddServo(int pin)
        params[0] = strtok_r(__null, ".", &p); // pin1
        params[1] = strtok_r(__null, ":", &p); // Name
        AddServo(atoi(params[0]));
      break;

      case kTypeEncoder:
        // AddEncoder(uint8_t pin1 = 1, uint8_t pin2 = 2, String name = "Encoder")
        params[0] = strtok_r(__null, ".", &p); // pin1
        params[1] = strtok_r(__null, ".", &p); // pin2
        params[2] = strtok_r(__null, ":", &p); // Name
        AddEncoder(atoi(params[0]), atoi(params[1]), params[2]);
      break;

      default:
        // read to the end of the current command which is
        // apparently not understood
        params[0] = strtok_r(__null, ":", &p); // read to end of unknown command
    }
    command = strtok_r(__null, ".", &p);
  } while (command!=__null);
}

// Called when a received command has no attached function
void OnUnknownCommand()
{
  lastCommand = millis();
  cmdMessenger.sendCmd(kStatus,"n/a");
}

void OnGetInfo() {
  lastCommand = millis();
  cmdMessenger.sendCmdStart(kInfo);
  cmdMessenger.sendCmdArg(type);
  cmdMessenger.sendCmdArg(name);
  cmdMessenger.sendCmdArg(serial);
  cmdMessenger.sendCmdArg(version);
  cmdMessenger.sendCmdEnd();
}

void OnGetConfig()
{
  lastCommand = millis();
  cmdMessenger.sendCmdStart(kInfo);
  cmdMessenger.sendCmdArg(configBuffer);
  cmdMessenger.sendCmdEnd();
}

// Callback function that sets led on or off
void OnSetPin()
{
  // Read led state argument, interpret string as boolean
  int pin = cmdMessenger.readIntArg();
  int state = cmdMessenger.readIntArg();
  // Set led
  digitalWrite(pin, state > 0 ? 0x1 : 0x0);
  lastCommand = millis();
}

void OnInitModule()
{
  int module = cmdMessenger.readIntArg();
  int subModule = cmdMessenger.readIntArg();
  int brightness = cmdMessenger.readIntArg();
  ledSegments[module].setBrightness(subModule,brightness);
  lastCommand = millis();
}

void OnSetModule()
{
  int module = cmdMessenger.readIntArg();
  int subModule = cmdMessenger.readIntArg();
  char * value = cmdMessenger.readStringArg();
  byte points = (byte) cmdMessenger.readIntArg();
  byte mask = (byte) cmdMessenger.readIntArg();
  ledSegments[module].display(subModule, value, points, mask);
  lastCommand = millis();
}

void OnSetStepper()
{
  int stepper = cmdMessenger.readIntArg();
  long newPos = cmdMessenger.readLongArg();

  if (stepper >= steppersRegistered) return;
  steppers[stepper]->moveTo(newPos);
  lastCommand = millis();
}

void OnResetStepper()
{
  int stepper = cmdMessenger.readIntArg();

  if (stepper >= steppersRegistered) return;
  steppers[stepper]->reset();
  lastCommand = millis();
}

void OnSetZeroStepper()
{
  int stepper = cmdMessenger.readIntArg();

  if (stepper >= steppersRegistered) return;
  steppers[stepper]->setZero();
  lastCommand = millis();
}

void OnSetServo()
{
  int servo = cmdMessenger.readIntArg();
  int newValue = cmdMessenger.readIntArg();
  if (servo >= servosRegistered) return;
  servos[servo].moveTo(newValue);
  lastCommand = millis();
}

void updateSteppers()
{
  for (int i=0; i!=steppersRegistered; i++) {
    steppers[i]->update();
  }
}

void updateServos()
{
  for (int i=0; i!=servosRegistered; i++) {
    servos[i].update();
  }
}

void readButtons()
{
  for(int i=0; i!=buttonsRegistered; i++) {
    buttons[i].update();
  }
}

void readEncoder()
{
  for(int i=0; i!=encodersRegistered; i++) {
    encoders[i].update();
  }
}

void OnGenNewSerial()
{
  generateSerial(true);
  cmdMessenger.sendCmdStart(kInfo);
  cmdMessenger.sendCmdArg(serial);
  cmdMessenger.sendCmdEnd();
}

void OnSetName() {
  String cfg = cmdMessenger.readStringArg();
  cfg.toCharArray(&name[0], MEM_LEN_NAME);
  _storeName();
  cmdMessenger.sendCmdStart(kStatus);
  cmdMessenger.sendCmdArg(name);
  cmdMessenger.sendCmdEnd();
}

void _storeName() {
  char prefix[] = "#";
  EEPROM.writeBlock<char>(MEM_OFFSET_NAME, prefix, 1);
  EEPROM.writeBlock<char>(MEM_OFFSET_NAME+1, name, MEM_LEN_NAME-1);
}

void _restoreName() {
  char testHasName[1] = "";
  EEPROM.readBlock<char>(MEM_OFFSET_NAME, testHasName, 1);
  if (testHasName[0] != '#') return;

  EEPROM.readBlock<char>(MEM_OFFSET_NAME+1, name, MEM_LEN_NAME-1);
}

void OnTrigger()
{
  for(int i=0; i!=buttonsRegistered; i++) {
    buttons[i].trigger();
  }
}