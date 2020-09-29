using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothMouseLook : MonoBehaviour
{
	public float sensitivity = 15F;

	private readonly float _minimumX = -360F;
	private readonly float _maximumX = 360F;
	private readonly float _minimumY = -90F;
	private readonly float _maximumY = 90F;

	private float _rotationX = 0F;
	private float _rotationY = 0F;

	private readonly List<float> _rotArrayX = new List<float>();
	private readonly List<float> _rotArrayY = new List<float>();

	[Range(0.1f, 1f)]
	public float secondsAveraged = 0.2f;

	private Quaternion _originalRotation;

	void Update()
	{
		float deltaTime = Time.smoothDeltaTime;
		deltaTime = deltaTime == 0 ? 0.1f : deltaTime;
		int frameCounter = (int)(secondsAveraged / deltaTime);
		frameCounter = Math.Min(2, frameCounter);

		float rotAverageY = 0f;
		float rotAverageX = 0f;

		_rotationY += Input.GetAxis("Mouse Y") * sensitivity;
		_rotationX += Input.GetAxis("Mouse X") * sensitivity;

		_rotationY = ClampAngle(_rotationY, _minimumY, _maximumY);
		_rotationX = ClampAngle(_rotationX, _minimumX, _maximumX);

		_rotArrayY.Add(_rotationY);
		_rotArrayX.Add(_rotationX);

		if (_rotArrayY.Count >= frameCounter)
		{
			_rotArrayY.RemoveAt(0);
		}
		if (_rotArrayX.Count >= frameCounter)
		{
			_rotArrayX.RemoveAt(0);
		}

		for (int j = 0; j < _rotArrayY.Count; j++)
		{
			rotAverageY += _rotArrayY[j];
		}
		for (int i = 0; i < _rotArrayX.Count; i++)
		{
			rotAverageX += _rotArrayX[i];
		}

		rotAverageY /= _rotArrayY.Count;
		rotAverageX /= _rotArrayX.Count;

		Quaternion yQuaternion = Quaternion.AngleAxis(rotAverageY, Vector3.left);
		Quaternion xQuaternion = Quaternion.AngleAxis(rotAverageX, Vector3.up);

		transform.localRotation = _originalRotation * xQuaternion * yQuaternion;
	}

	void Start()
	{
		Rigidbody rb = GetComponent<Rigidbody>();
		if (rb)
        {
            rb.freezeRotation = true;
        }

        _originalRotation = transform.localRotation;
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