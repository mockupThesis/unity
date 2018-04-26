using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using System;
using System.Text;
using uPLibrary.Networking.M2Mqtt.Messages;

public class RobotMovement : MonoBehaviour, IControllable
{

	public Animator animator;

	public string brokerHostname = "mqtt.bayi.hu";
	public int brokerPort = 1883;

	private MqttComponent mqtt;

	private Dictionary<HumanBodyBones, Quaternion> quaternions;

	private Quaternion rightUpperArm;

	void Start ()
	{
		quaternions = new Dictionary <HumanBodyBones, Quaternion> ();
		mqtt = new MqttComponent (this, brokerHostname);
	}

	void Update ()
	{


	}

	void LateUpdate ()
	{
		
		if (Input.GetKey (KeyCode.B)) {
			Transform transform = animator.GetBoneTransform (HumanBodyBones.RightUpperArm);
			transform.eulerAngles = new Vector3 (0, 0, 90);
		}

		foreach (HumanBodyBones bone in quaternions.Keys) {
			Transform boneTransform = animator.GetBoneTransform (bone);
			Quaternion quaternion = quaternions [bone];
			boneTransform.eulerAngles = quaternion.eulerAngles;
		}

	}

	public void control (HumanBodyBones bone, Quaternion quaternion)
	{
		quaternions.Add (bone, quaternion);
	}

}
