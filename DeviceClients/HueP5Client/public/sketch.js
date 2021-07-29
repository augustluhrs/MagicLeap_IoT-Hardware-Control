/*
  Hue Hub light control over MQTT
  
  created 23 July 2020
  modified 23 Nov 2020
  by Tom Igoe
*/

// Fill in your hue hub IP credentials here:
//the env stuff doesn't work, will fix later
let url = process.env.URL;
let username = process.env.USERNAME; 
// slider for dimming the lights:
let dimmer;
// the number of the light in the hub:
let lightNumber = 5;
let secondLight = 4;
let leftLight = 4;
let rightLight = 5;
// The light state:
let lightState = {
   bri: 0,
   on: false
}

let secondState = {
    bri: 0,
    on: false
 }

let leftState = {
    on: false,
    bri: 0,
    hue: 0
}
let rightState = {
    on: false,
    bri: 0,
    hue: 0
}
// MQTT broker details:
let broker = {
   hostname: process.env.HOSTNAME,
   port: 443
};

// MQTT client:
let client;
// client credentials:
let creds = {
   clientID: 'p5HueClient',
   userName: process.env.HOSTUSERNAME,
   password: process.env.PASSWORD
}
// topic to subscribe to when you connect to the broker:
// let topic = 'lights';

// UI elements: 
// a pushbutton to send messages:
let sendButton;
let rightButton;
// divs for text from the broker:
let localDiv;
let remoteDiv;

function setup() {
   noLoop();
//   createCanvas(windowWidth, windowHeight);
   // a div for the Hue hub's responses:
   remoteDiv = createDiv('Hub response');
   // position it:
   remoteDiv.position(20, 130);
   // a slider to dim one light:
   dimmer = createSlider(0, 65000, 127)
   // position it:
   dimmer.position(10, 10);
   // set a behavior for it:
   dimmer.mouseReleased(changeBrightness);

   // Create an MQTT client:
   client = new Paho.MQTT.Client(broker.hostname, Number(broker.port), creds.clientID);
   // set callback handlers for the client:
   client.onConnectionLost = onConnectionLost;
   client.onMessageArrived = onMessageArrived;
   // connect to the MQTT broker:
   client.connect(
       {
           onSuccess: onConnect,       // callback function for when you connect
           userName: creds.userName,   // username
           password: creds.password,   // password
           useSSL: true                // use SSL
       }
   );
   // create the send button:
   sendButton = createButton('toggle left');
   sendButton.position(20, 40);
   sendButton.mousePressed(sendMqttMessage);
   rightButton = createButton('toggle right');
   rightButton.position(120, 40);
   rightButton.mousePressed(rightMqttMessage);
   // create a div for local messages:
   localDiv = createDiv('local messages will go here');
   localDiv.position(20, 70);
   // create a div for the response:
   remoteDiv = createDiv('waiting for messages');
   remoteDiv.position(20, 100);
   connect();
}

/*
this function makes the HTTP GET call to get the light data:
HTTP GET http://your.hue.hub.address/api/username/lights/
*/
function connect() {
   url = "http://" + url + '/api/' + username + '/lights/';
   httpDo(url, 'GET', getLights);
}

/*
this function uses the response from the hub
to create a new div for the UI elements
*/
function getLights(result) {
   remoteDiv.html(result);
}

function changeBrightness() {
    if (leftState.on){
        leftState.hue = dimmer.value();
    }
    if (rightState.on){
        rightState.hue = dimmer.value();
    }
   

//    if (lightState.bri > 0) {
//       lightState.on = true;
//    } else {
//       lightState.on = false;
//    }
   // make the HTTP call with the JSON object:
    setLight(leftLight, leftState);
    setLight(rightLight, rightState);

    // setLight(secondLight, lightState); // same for test
}

/*
this function makes an HTTP PUT call to change the properties of the lights:
HTTP PUT http://your.hue.hub.address/api/username/lights/lightNumber/state/
and the body has the light state:
{
  on: true/false,
  bri: brightness
}
*/
function setLight(whichLight, data) {
   var path = url + whichLight + '/state/';
//    var path2 = url + secondLight + '/state/';

   var content = JSON.stringify(data);				 // convert JSON obj to string
   httpDo(path, 'PUT', content, 'text', getLights); //HTTP PUT the change
//    httpDo(path2, 'PUT', content, 'text', getLights); //HTTP PUT the change
}

// called when the client connects
function onConnect() {
   localDiv.html('client is connected');
   client.subscribe("lights");
   client.subscribe("lights/leftToggle");
   client.subscribe("lights/rightToggle");
   client.subscribe("lights/hueSlider");
   hueTest();
}

// called when the client loses its connection
function onConnectionLost(response) {
   if (response.errorCode !== 0) {
       localDiv.html('onConnectionLost:' + response.errorMessage);
   }
}

// called when a message arrives
function onMessageArrived(message) {

    let topic = message.destinationName;
    console.log(topic);
    remoteDiv.html('I got a message:' + message.payloadString);
    let  incomingNumber = parseInt(message.payloadString);

    //turn the left light on
    if (topic == "lights/leftToggle"){
        leftState.on = !leftState.on;
        leftState.bri = 254;
        setLight(leftLight, leftState);
    }
    //turn the right light on
    if (topic == "lights/rightToggle"){
        rightState.on = !rightState.on;
        rightState.bri = 254;
        setLight(rightLight, rightState);
    }
     //set the lights hue
     if (topic == "lights/hueSlider"){
        // rightState.on = !rightState.on;
        // rightState.bri = 254;
        if (leftState.on){
            leftState.hue = incomingNumber;
            setLight(leftLight, leftState);
        }
        if (rightState.on){
            rightState.hue = incomingNumber;
            setLight(rightLight, rightState);
        }
    }


    // // use it to set the light:
    // lightState.hue = incomingNumber;
    // lightState.bri = 254;
    // if (lightState.bri > 0) {
    //     lightState.on = true;
    // } else {
    //     lightState.on = false;
    // }
    // // make the HTTP call with the JSON object:
    // setLight(lightNumber, lightState);
}

// called when you want to send a message:
function sendMqttMessage() {
   // if the client is connected to the MQTT broker:
   if (client.isConnected()) {
       // make a string with a random number form 0 to 15:
    //    let msg = String(round(random(254)));
       //send the val of the dimmer
       let msg = String(round(dimmer.value()));
       // start an MQTT message:
       message = new Paho.MQTT.Message(msg);
       // choose the destination topic:
       message.destinationName = "lights/leftToggle";
       // send it:
       client.send(message);
       // print what you sent:
       localDiv.html('I sent: ' + message.payloadString);
   }
}

function rightMqttMessage() {
    // if the client is connected to the MQTT broker:
    if (client.isConnected()) {
        // make a string with a random number form 0 to 15:
     //    let msg = String(round(random(254)));
        //send the val of the dimmer
        let msg = String(round(dimmer.value()));
        // start an MQTT message:
        message = new Paho.MQTT.Message(msg);
        // choose the destination topic:
        message.destinationName = "lights/rightToggle";
        // send it:
        client.send(message);
        // print what you sent:
        localDiv.html('I sent: ' + message.payloadString);
    }
 }

 function hueTest() {
     // if the client is connected to the MQTT broker:
    if (client.isConnected()) {
        // make a string with a random number form 0 to 15:
     //    let msg = String(round(random(254)));
        //send the val of the dimmer
        let msg = String(round(dimmer.value()));
        // start an MQTT message:
        message = new Paho.MQTT.Message(msg);
        // choose the destination topic:
        message.destinationName = "lights/hueSlider";
        // send it:
        client.send(message);
        // print what you sent:
        localDiv.html('I sent: ' + message.payloadString);
    }
 }