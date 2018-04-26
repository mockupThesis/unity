using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IControllable
{

	void control (HumanBodyBones bone, Quaternion quaternion);
}
