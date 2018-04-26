using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using System;
using System.Text;
using uPLibrary.Networking.M2Mqtt.Messages;

public class RobotMovement : MonoBehaviour
{

	public Animator animator;
	MqttClient mqttclient;

	public string brokerHostname = "mqtt.bayi.hu";
	public int brokerPort = 1883;
	private Quaternion received;

	void Start ()
	{
		Connect ();
		Publish ("sensor/status", "Unity client connected");
		mqttclient.MqttMsgPublishReceived += MqttMsgPublishReceived;
		string[] topics = { "sensor/quat" };
		byte[] qosLevels = { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE };
		mqttclient.Subscribe (topics, qosLevels);
	}

	void Update ()
	{
	

	}

	void LateUpdate ()
	{
		Transform transform = animator.GetBoneTransform (HumanBodyBones.RightUpperArm);
		if (Input.GetKey (KeyCode.B)) {
			transform.eulerAngles = new Vector3 (0, 0, 90);
		}
		transform.eulerAngles = received.eulerAngles;
	}

	void MqttMsgPublishReceived (object sender, MqttMsgPublishEventArgs e)
	{
		string msg = System.Text.Encoding.UTF8.GetString (e.Message);
		Debug.Log ("Received message from " + e.Topic + " : " + msg);
		string[] msgParts = msg.Split ('|');
		received = new Quaternion (float.Parse (msgParts [1]), float.Parse (msgParts [2]), float.Parse (msgParts [3]), float.Parse (msgParts [0]));
	}

	private void Connect ()
	{
		mqttclient = new MqttClient (brokerHostname);
		string clientId = Guid.NewGuid ().ToString ();
		try {
			mqttclient.Connect (clientId);
			Debug.Log ("MQTT client connected successfully to: " + brokerHostname);
		} catch (Exception e) {
			Debug.LogError ("Conn error: " + e);
		}
	}

	private void Publish (string topic, string msg)
	{
		mqttclient.Publish (topic, Encoding.UTF8.GetBytes (msg), MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, false);
	}

}
