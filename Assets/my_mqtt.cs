/* using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class my_mqtt : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
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

/// <summary>
/// Examples for the M2MQTT library (https://github.com/eclipse/paho.mqtt.m2mqtt),
/// </summary>
namespace ChuongGa
{
    /// <summary>
    /// Script for testing M2MQTT with a Unity UI
    /// </summary>
    public class my_mqtt: M2MqttUnityClient
    {
		public List<string> topics = new List<string>();
        [Tooltip("Set this to true to perform a testing cycle automatically on startup")]
        public bool autoTest = false;
		public ChuongGaManager switcher;
        [Header("User Interface")]
        public Text consoleInputField;
        public InputField addressInputField;
        /* public InputField portInputField; */
		public InputField usernameInputField;
        public InputField passwordInputField;
        public Button connectButton;
		public Text tempText;
		public Text humidText;
		public Toggle ledToggle;
		public Toggle pumpToggle;
        /* public Button disconnectButton;
        public Button testPublishButton;
        public Button clearButton; */

        private List<string> eventMessages = new List<string>();
        private bool updateUI = false;

		public class StatusData {
				public int temp;
				public int humid;
		}
		
		public class ToggleData {
				public string device;
				public string status;
		}
        public void TestPublish()
        {
            client.Publish("M2MQTT_Unity/test", System.Text.Encoding.UTF8.GetBytes("Test message"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            Debug.Log("Test message published");
            SetUiMessage("Test message published.");
        }

        public void SetBrokerAddress(string brokerAddress)
        {
            if (addressInputField && !updateUI)
            {
                this.brokerAddress = brokerAddress;
				this.brokerPort = 1883;
            }
        }
		
		public void SetUsername(string username)
        {
            if (usernameInputField && !updateUI)
            {
                this.mqttUserName = username;
            }
        }
		
		public void SetPassword(string pwd)
        {
            if (passwordInputField && !updateUI)
            {
                this.mqttPassword = pwd;
            }
        }

       /*  public void SetBrokerPort(string brokerPort)
        {
            if (portInputField && !updateUI)
            {
                int.TryParse(brokerPort, out this.brokerPort);
            }
        } */




        public void SetUiMessage(string msg)
        {
            if (consoleInputField != null)
            {
                consoleInputField.text = msg;
                updateUI = true;
            }
        }

        public void AddUiMessage(string msg)
        {
            if (consoleInputField != null)
            {
                consoleInputField.text += msg + "\n";
                updateUI = true;
            }
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
			ledToggle.onValueChanged.AddListener(delegate {
                ToggleValueChanged(ledToggle,"l");
            });
			pumpToggle.onValueChanged.AddListener(delegate {
                ToggleValueChanged(pumpToggle,"p");
            });
			switcher.SwitchLayer();
		}

        protected override void SubscribeTopics()
        {
            foreach (string topic in topics)
            {
                if (topic != "")
                {
                    client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

                }
            }
        }
         protected override void UnsubscribeTopics()
        {
            foreach (string topic in topics)
            {
                if (topic != "")
                {
                    client.Unsubscribe(new string[] { topic });
                }
            }

        }

        protected override void OnConnectionFailed(string errorMessage)
        {
            SetUiMessage("CONNECTION FAILED! " + errorMessage);
        }

        protected override void OnDisconnected()
        {
            SetUiMessage("Disconnected.");
        }

        protected override void OnConnectionLost()
        {
            SetUiMessage("CONNECTION LOST!");
        }

        private void UpdateUI()
        {/* 
            if (client == null)
            {
                if (connectButton != null)
                {
                    connectButton.interactable = true;
                    disconnectButton.interactable = false;
                    testPublishButton.interactable = false;
                }
            }
            else
            {
                if (testPublishButton != null)
                {
                    testPublishButton.interactable = client.IsConnected;
                }
                if (disconnectButton != null)
                {
                    disconnectButton.interactable = client.IsConnected;
                }
                if (connectButton != null)
                {
                    connectButton.interactable = !client.IsConnected;
                }
            }
            if (addressInputField != null && connectButton != null)
            {
                addressInputField.interactable = connectButton.interactable;
                addressInputField.text = brokerAddress;
            }
            if (portInputField != null && connectButton != null)
            {
                portInputField.interactable = connectButton.interactable;
                portInputField.text = brokerPort.ToString();
            }
            if (encryptedToggle != null && connectButton != null)
            {
                encryptedToggle.interactable = connectButton.interactable;
                encryptedToggle.isOn = isEncrypted;
            }
            if (clearButton != null && connectButton != null)
            {
                clearButton.interactable = connectButton.interactable;
            } */
			
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
            Debug.Log("Received: " + topic + msg);
            //StoreMessage(msg);
          /*   if (topic == "led")
                /* ProcessMessageStatus(msg); */

            if (topic == topics[0])
                ProcessMessage(msg); 
        }

        private void StoreMessage(string eventMsg)
        {
            eventMessages.Add(eventMsg);
        }

        private void ProcessMessage(string msg)
        {
            SetUiMessage("Received: " + msg);
			StatusData data =  JsonUtility.FromJson<StatusData>(msg);
       
			tempText.text = $"Nhi???t ?????\n{data.temp}";
			humidText.text = $"????? ???m\n{data.humid}";
        }
		
		

        protected override void Update()
        {
            base.Update(); // call ProcessMqttEvents()

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
		
		public void ToggleValueChanged(Toggle change,string t)
		{
			string status = "OFF";
			string device;
			string link;
			if (t== "l"){
				device = "LED";
				if (ledToggle.isOn) status = "ON";
				link = "/bkiot/1811524/led";
			}
			else {
				device = "PUMP";
				if (pumpToggle.isOn) status = "ON";
				link = "/bkiot/1811524/pump";
			}
			ToggleData data = new ToggleData();
			data.device = device;
			data.status = status;
			
			string json = JsonUtility.ToJson(data);
			
		
            client.Publish(link, System.Text.Encoding.UTF8.GetBytes(json), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
		}
    }
}

