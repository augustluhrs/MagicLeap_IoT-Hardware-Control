/*
The MIT License (MIT)

Copyright (c) 2018 Giovanni Paolo Vigano'

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using M2MqttUnity;
using TMPro;
using Microsoft.MixedReality.Toolkit.UI;

/// <summary>
/// Examples for the M2MQTT library (https://github.com/eclipse/paho.mqtt.m2mqtt),
/// </summary>
namespace M2MqttUnity.Examples
{
    /// <summary>
    /// Script for testing M2MQTT with a Unity UI
    /// </summary>
    public class MQTT_UIFunctions : M2MqttUnityClient
    {
        [Tooltip("Set this to true to perform a testing cycle automatically on startup")]
        public bool autoTest = false;


        [Header("User Interface")]
        //public InputField consoleInputField;
        //public Toggle encryptedToggle;
        //public InputField addressInputField;
        //public InputField portInputField;
        public TextMeshPro mqttStatusText;
        //public GameObject leftToggle;
        //public GameObject rightToggle;
        public GameObject hueSlider;
        private bool hueUpdate = false;

        //public Button connectButton;
        //public Button disconnectButton;
        //public Button testPublishButton;
        //public Button clearButton;

        private List<string> eventMessages = new List<string>();
        private bool updateUI = false;


        //UI EVENT FUNCTIONS
        public void LeftLightToggle()
        {
            client.Publish("lights/leftToggle", System.Text.Encoding.UTF8.GetBytes("248"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);

        }

        public void RightLightToggle()
        {
            client.Publish("lights/rightToggle", System.Text.Encoding.UTF8.GetBytes("248"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);

        }

        public void HueSliderUpdate()
        {
            //hueUpdate = true;
            float hue = hueSlider.GetComponent<PinchSlider>().SliderValue;
            hue *= 65000;
            client.Publish("lights/hueSlider", System.Text.Encoding.UTF8.GetBytes(hue.ToString()), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
        }

        public void HueSliderStop()
        {
            hueUpdate = false;
        }

        public void TestPublish()
        {
            client.Publish("lights", System.Text.Encoding.UTF8.GetBytes("248"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            Debug.Log("Test message to lights published");
            AddUiMessage("Light message published.");
        }

        //public void SetBrokerAddress(string brokerAddress)
        //{
        //    if (addressInputField && !updateUI)
        //    {
        //        this.brokerAddress = brokerAddress;
        //    }
        //}

        //public void SetBrokerPort(string brokerPort)
        //{
        //    if (portInputField && !updateUI)
        //    {
        //        int.TryParse(brokerPort, out this.brokerPort);
        //    }
        //}

        //public void SetEncrypted(bool isEncrypted)
        //{
        //    this.isEncrypted = isEncrypted;
        //}


        public void SetUiMessage(string msg)
        {
            if (mqttStatusText != null)
            {
                mqttStatusText.text = msg + "\n";
                updateUI = true;
            }
            //if (consoleInputField != null)
            //{
            //    consoleInputField.text = msg;
            //    updateUI = true;
            //}
        }

        public void AddUiMessage(string msg)
        {

            if (mqttStatusText != null)
            {
                mqttStatusText.text += msg + "\n";
                updateUI = true;
            }
            //if (consoleInputField != null)
            //{
            //    consoleInputField.text += msg + "\n";
            //    updateUI = true;
            //}
        }

        protected override void OnConnecting()
        {
            base.OnConnecting();
            SetUiMessage("Connecting to broker on " + brokerAddress + ":" + brokerPort.ToString() + "...\n");
        }

        protected override void OnConnected()
        {
            base.OnConnected();
            SetUiMessage("Connected to broker on " + brokerAddress + "\n");

            if (autoTest)
            {
                TestPublish();
            }
        }

        protected override void SubscribeTopics()
        {
            client.Subscribe(new string[] { "lights" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            client.Subscribe(new string[] { "lights/leftToggle" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            client.Subscribe(new string[] { "lights/rightToggle" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            client.Subscribe(new string[] { "lights/hueSlider" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

        }

        protected override void UnsubscribeTopics()
        {
            client.Unsubscribe(new string[] { "lights" });
        }

        protected override void OnConnectionFailed(string errorMessage)
        {
            AddUiMessage("CONNECTION FAILED! " + errorMessage);
        }

        protected override void OnDisconnected()
        {
            AddUiMessage("Disconnected.");
        }

        protected override void OnConnectionLost()
        {
            AddUiMessage("CONNECTION LOST!");
        }

        private void UpdateUI()
        {
            //if (client == null)
            //{
                //if (connectButton != null)
                //{
                //    connectButton.interactable = true;
                //    disconnectButton.interactable = false;
                //    testPublishButton.interactable = false;
                //}
            //}
            //else
            //{
                //if (testPublishButton != null)
                //{
                //    testPublishButton.interactable = client.IsConnected;
                //}
                //if (disconnectButton != null)
                //{
                //    disconnectButton.interactable = client.IsConnected;
                //}
                //if (connectButton != null)
                //{
                //    connectButton.interactable = !client.IsConnected;
                //}
            //}
            //if (addressInputField != null && connectButton != null)
            //{
            //    addressInputField.interactable = connectButton.interactable;
            //    addressInputField.text = brokerAddress;
            //}
            //if (portInputField != null && connectButton != null)
            //{
            //    portInputField.interactable = connectButton.interactable;
            //    portInputField.text = brokerPort.ToString();
            //}
            //if (encryptedToggle != null && connectButton != null)
            //{
            //    encryptedToggle.interactable = connectButton.interactable;
            //    encryptedToggle.isOn = isEncrypted;
            //}
            //if (clearButton != null && connectButton != null)
            //{
            //    clearButton.interactable = connectButton.interactable;
            //}
            updateUI = false;
        }

        protected override void Start()
        {
            SetUiMessage("Ready.");
            updateUI = true;
            base.Start();
        }

        protected override void DecodeMessage(string topic, byte[] message)
        {
            string msg = System.Text.Encoding.UTF8.GetString(message);
            Debug.Log("Received: " + msg);
            StoreMessage(msg);
            if (topic == "lights")
            {
                if (autoTest)
                {
                    autoTest = false;
                    Disconnect();
                }
            }
        }

        private void StoreMessage(string eventMsg)
        {
            eventMessages.Add(eventMsg);
        }

        private void ProcessMessage(string msg)
        {
            AddUiMessage("Received: " + msg);
        }

        protected override void Update()
        {
            base.Update(); // call ProcessMqttEvents()

            //if (hueUpdate) //moving this here to try and limit the number of update messages that get sent
            //{
            //    float hue = hueSlider.GetComponent<PinchSlider>().SliderValue;
            //    hue *= 65000;
            //    client.Publish("lights/hueSlider", System.Text.Encoding.UTF8.GetBytes(hue.ToString()), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            //}

            if (eventMessages.Count > 0)
            {
                foreach (string msg in eventMessages)
                {
                    ProcessMessage(msg);
                }
                eventMessages.Clear();
            }
            if (updateUI)
            {
                UpdateUI();
            }
        }

        private void OnDestroy()
        {
            Disconnect();
        }

        private void OnValidate()
        {
            if (autoTest)
            {
                autoConnect = true;
            }
        }
    }
}

