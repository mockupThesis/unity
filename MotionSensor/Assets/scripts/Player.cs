using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IControllable {

    public Animator animator;
    public string brokerHostname = "mqtt.bayi.hu";
    public int brokerPort = 1883;
    public MqttComponent mqtt;

    private int frameCounter = 0;
    public bool controlMode = false;

    private Dictionary<HumanBodyBones, Quaternion> quaternions;

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        animator.speed = 0.3f;
        quaternions = new Dictionary<HumanBodyBones, Quaternion>();
        mqtt = new MqttComponent(brokerHostname, this);
	}

    public void ModeChanged()
    {
        controlMode = !controlMode;
        Debug.Log("mode changed" + controlMode);
    }
	
	// Update is called once per frame
	void Update () {
        if (controlMode == false)
        {

            if (Input.GetKeyDown("1"))
            {
                animator.Play("REFLESH00", -1, 0f);
            }
            if (Input.GetKeyDown("2"))
            {
                animator.Play("HANDUP00_R", -1, 0f);
            }
            if (Input.GetKeyDown("3"))
            {
                animator.Play("WAIT03", -1, 0f);
            }
            if (Input.GetKeyDown("4"))
            {
                animator.Play("LOSE00", -1, 0f);
            }

            if (frameCounter++ == 5)
            {
                PublishRightArm();
                PublishLeftArm();
                PublishHead();
                frameCounter = 0;
            }

        }
    }

    public void OnHandUp()
    {
        animator.Play("HANDUP00_R", -1, 0f);
    }

    public void OnStretching()
    {
        animator.Play("WAIT03", -1, 0f);
    }

    public void OnRelax()
    {
        animator.Play("LOSE00", -1, 0f);
    }

    void LateUpdate()
    {
        if (controlMode == true)
        {

            if (Input.GetKey(KeyCode.B))
            {
                Transform transform = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
                transform.eulerAngles = new Vector3(0, 0, 90);
            }

            foreach (HumanBodyBones bone in quaternions.Keys)
            {
                Transform boneTransform = animator.GetBoneTransform(bone);
                Quaternion quaternion = quaternions[bone];
                boneTransform.eulerAngles = quaternion.eulerAngles;
            }
        }
    }

    public void control(HumanBodyBones bone, Quaternion quaternion)
    {
        quaternions[bone] = quaternion;
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
        euler = rightLowerArm.localEulerAngles;
        z = Mathf.RoundToInt(180 - euler.z / 2);
        mqtt.Publish("sensor/RightLowerArmZ", z.ToString());
    }

    public void PublishLeftArm()
    {
        Transform leftUpperArm = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        Vector3 euler = leftUpperArm.eulerAngles;
        int z = Mathf.RoundToInt(180 - euler.z / 2);
        mqtt.Publish("sensor/LeftUpperArmZ", z.ToString());

        int x = Mathf.RoundToInt(180 - euler.x / 2);
        mqtt.Publish("sensor/LeftUpperArmX", x.ToString());

        Transform leftLowerArm = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
        euler = leftLowerArm.localEulerAngles;
        z = Mathf.RoundToInt(euler.z / 2);
        mqtt.Publish("sensor/LeftLowerArmZ", z.ToString());
    }

    public void PublishHead()
    {
        Transform head = animator.GetBoneTransform(HumanBodyBones.Head);
        Vector3 euler = head.localEulerAngles;

        /*int y = Mathf.RoundToInt(euler.y / 2);
        mqtt.Publish("sensor/HeadY", y.ToString());*/

        int z = Mathf.RoundToInt(euler.z / 2);
        mqtt.Publish("sensor/HeadZ", z.ToString());
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
