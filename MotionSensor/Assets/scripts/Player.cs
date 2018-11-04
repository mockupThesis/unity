using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public Animator animator;
    public string brokerHostname = "mqtt.bayi.hu";
    public int brokerPort = 1883;
    public MqttComponent mqtt;

    private int frameCounter = 0;

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        mqtt = new MqttComponent(brokerHostname);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown("1"))
        {
            animator.Play("WAIT01", -1, 0f);
        }
        if (Input.GetKeyDown("2"))
        {
            animator.Play("WAIT02", -1, 0f);
        }
        if (Input.GetKeyDown("3"))
        {
            animator.Play("WAIT03", -1, 0f);
        }
        if (Input.GetKeyDown("4"))
        {
            animator.Play("WAIT04", -1, 0f);
        }

        if(frameCounter++ == 10)
        {
            PublishRightArm();
            PublishLeftArm();
            frameCounter = 0;
        }

    }

    public void PublishRightArm()
    {
        Transform rightUpperArm = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
        Vector3 euler = rightUpperArm.eulerAngles;
        int z = Mathf.RoundToInt(euler.z / 2);
        mqtt.Publish("sensor/RightUpperArmZ", z.ToString());

        int x = Mathf.RoundToInt(180 - euler.x / 2);
        mqtt.Publish("sensor/RightUpperArmX", x.ToString());

        Transform rightLowerArm = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
        euler = rightLowerArm.eulerAngles;
        z = Mathf.RoundToInt(euler.z / 2);
        mqtt.Publish("sensor/RightLowerArmZ", z.ToString());
    }

    public void PublishLeftArm()
    {
        Transform leftUpperArm = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        Vector3 euler = leftUpperArm.eulerAngles;
        int z = Mathf.RoundToInt(euler.z / 2);
        mqtt.Publish("sensor/LeftUpperArmZ", z.ToString());

        int x = Mathf.RoundToInt(180 - euler.x / 2);
        mqtt.Publish("sensor/LeftUpperArmX", x.ToString());

        Transform leftLowerArm = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
        euler = leftLowerArm.eulerAngles;
        z = Mathf.RoundToInt(euler.z / 2);
        mqtt.Publish("sensor/leftLowerArmZ", z.ToString());
    }

    public string FormatMessageAsQuaternion(Vector3 euler)
    {
        Quaternion quaternion = Quaternion.Euler(euler);
        string message = quaternion.w + "|" + quaternion.x + "|" + quaternion.y + "|" + quaternion.z;

        return message;
    }

    public string FormatMessageAsEuler(Vector3 euler)
    {
        string message = euler.x + "|" + euler.y + "|" + euler.z;
        return message;
    }
}
