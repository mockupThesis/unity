using System;
using uPLibrary.Networking.M2Mqtt.Messages;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using System.Text;


public class MqttComponent
{
	private string brokerHostName;
	private MqttClient mqttclient;
    IControllable controllable;

    public MqttComponent (string brokerHostName, IControllable controllable)
	{
		this.brokerHostName = brokerHostName;
        this.controllable = controllable;

		Connect ();
		Publish ("sensor/status", "Unity client connected");
		mqttclient.MqttMsgPublishReceived += MqttMsgPublishReceived;
		string[] topics = { "sensor/RightUpperArm" };
		byte[] qosLevels = { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE };
		mqttclient.Subscribe (topics, qosLevels);
        mqttclient.Subscribe(new string[] { "sensor/RightLowerArm" }, qosLevels);
       // mqttclient.Subscribe(new string[] { "sensor/LeftUpperArm" }, qosLevels);
       // mqttclient.Subscribe(new string[] { "sensor/LeftLowerArm" }, qosLevels);
    }

	void MqttMsgPublishReceived (object sender, MqttMsgPublishEventArgs e)
	{
		string msg = System.Text.Encoding.UTF8.GetString (e.Message);
		Debug.Log ("Received message from " + e.Topic + " : " + msg);
		string[] msgParts = msg.Split ('|');
		Quaternion received = new Quaternion (float.Parse (msgParts [1]), float.Parse (msgParts [2]), float.Parse (msgParts [3]), float.Parse (msgParts [0]));

        string bone = e.Topic.Split('/')[1];
        
        HumanBodyBones boneEnum = (HumanBodyBones) Enum.Parse(typeof(HumanBodyBones), bone);
        Debug.Log("Received: " + boneEnum + " Quaternion: " + received);
        controllable.control (boneEnum, received);
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

	public void Publish (string topic, string msg)
	{
		mqttclient.Publish (topic, Encoding.UTF8.GetBytes (msg), MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, false);
        Debug.Log("Published message to: " + topic + " : " + msg);
    }
		
}


