using System;
using uPLibrary.Networking.M2Mqtt.Messages;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using System.Text;


public class MqttComponent
{
	private IControllable controllable;
	private string brokerHostName;
	private MqttClient mqttclient;

	public MqttComponent (IControllable controllable, string brokerHostName)
	{
		this.controllable = controllable;
		this.brokerHostName = brokerHostName;

		Connect ();
		Publish ("sensor/status", "Unity client connected");
		mqttclient.MqttMsgPublishReceived += MqttMsgPublishReceived;
		string[] topics = { "sensor/RightUpperArm" };
		byte[] qosLevels = { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE };
		mqttclient.Subscribe (topics, qosLevels);
		mqttclient.Subscribe (new string[] { "sensor/RightLowerArm" }, qosLevels);
	}

	void MqttMsgPublishReceived (object sender, MqttMsgPublishEventArgs e)
	{
		string msg = System.Text.Encoding.UTF8.GetString (e.Message);
		Debug.Log ("Received message from " + e.Topic + " : " + msg);
		string[] msgParts = msg.Split ('|');
		Quaternion received = new Quaternion (float.Parse (msgParts [1]), float.Parse (msgParts [2]), float.Parse (msgParts [3]), float.Parse (msgParts [0]));

		if (e.Topic.Equals ("sensor/RightUpperArm")) {
			controllable.control (HumanBodyBones.RightUpperArm, received);
		} else {
			controllable.control (HumanBodyBones.RightLowerArm, received);
		}
	}

	private void Connect ()
	{
		mqttclient = new MqttClient (brokerHostName);
		string clientId = Guid.NewGuid ().ToString ();
		try {
			mqttclient.Connect (clientId);
			Debug.Log ("MQTT client connected successfully to: " + brokerHostName);
		} catch (Exception e) {
			Debug.LogError ("Conn error: " + e);
		}
	}

	private void Publish (string topic, string msg)
	{
		mqttclient.Publish (topic, Encoding.UTF8.GetBytes (msg), MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, false);
	}
		
}


