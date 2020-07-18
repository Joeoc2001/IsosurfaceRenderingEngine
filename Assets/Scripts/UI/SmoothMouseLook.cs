using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothMouseLook : MonoBehaviour
{
	public float sensitivity = 15F;

	readonly float minimumX = -360F;
	readonly float maximumX = 360F;
	readonly float minimumY = -90F;
	readonly float maximumY = 90F;

	float rotationX = 0F;
	float rotationY = 0F;

	private readonly List<float> rotArrayX = new List<float>();
	private readonly List<float> rotArrayY = new List<float>();

	[Range(0.1f, 1f)]
	public float secondsAveraged = 0.2f;

	Quaternion originalRotation;

	void Update()
	{
		float deltaTime = Time.smoothDeltaTime;
		deltaTime = deltaTime == 0 ? 0.1f : deltaTime;
		int frameCounter = (int)(secondsAveraged / deltaTime);
		frameCounter = Math.Min(2, frameCounter);

		float rotAverageY = 0f;
		float rotAverageX = 0f;

		rotationY += Input.GetAxis("Mouse Y") * sensitivity;
		rotationX += Input.GetAxis("Mouse X") * sensitivity;

		rotationY = ClampAngle(rotationY, minimumY, maximumY);
		rotationX = ClampAngle(rotationX, minimumX, maximumX);

		rotArrayY.Add(rotationY);
		rotArrayX.Add(rotationX);

		if (rotArrayY.Count >= frameCounter)
		{
			rotArrayY.RemoveAt(0);
		}
		if (rotArrayX.Count >= frameCounter)
		{
			rotArrayX.RemoveAt(0);
		}

		for (int j = 0; j < rotArrayY.Count; j++)
		{
			rotAverageY += rotArrayY[j];
		}
		for (int i = 0; i < rotArrayX.Count; i++)
		{
			rotAverageX += rotArrayX[i];
		}

		rotAverageY /= rotArrayY.Count;
		rotAverageX /= rotArrayX.Count;

		Quaternion yQuaternion = Quaternion.AngleAxis(rotAverageY, Vector3.left);
		Quaternion xQuaternion = Quaternion.AngleAxis(rotAverageX, Vector3.up);

		transform.localRotation = originalRotation * xQuaternion * yQuaternion;
	}

	void Start()
	{
		Rigidbody rb = GetComponent<Rigidbody>();
		if (rb)
			rb.freezeRotation = true;
		originalRotation = transform.localRotation;
	}

	public static float ClampAngle(float angle, float min, float max)
	{
		angle %= 360;
		if ((angle >= -360F) && (angle <= 360F))
		{
			if (angle < -360F)
			{
				angle += 360F;
			}
			if (angle > 360F)
			{
				angle -= 360F;
			}
		}
		return Mathf.Clamp(angle, min, max);
	}
}