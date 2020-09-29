using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothWASDMove : MonoBehaviour
{
    private readonly List<Vector3> _velocities = new List<Vector3>();

    [Range(1f, 100f)]
    public float speed = 20f;

    [Range(0.1f, 1f)]
    public float secondsAveraged = 0.2f;

    void Update()
    {
        Vector3 dir = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            dir += transform.rotation * Vector3.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            dir += transform.rotation * Vector3.left;
        }
        if (Input.GetKey(KeyCode.S))
        {
            dir += transform.rotation * Vector3.back;
        }
        if (Input.GetKey(KeyCode.D))
        {
            dir += transform.rotation * Vector3.right;
        }
        _velocities.Add(dir.normalized * speed);

        float deltaTime = Time.smoothDeltaTime;
        deltaTime = deltaTime == 0 ? 0.1f : deltaTime;
        int frameCounter = (int)(secondsAveraged / deltaTime);
        frameCounter = Math.Min(2, frameCounter);
        if (_velocities.Count >= frameCounter)
        {
            _velocities.RemoveAt(0);
        }

        Vector3 averageVelocity = Vector3.zero;
        for (int j = 0; j < _velocities.Count; j++)
        {
            averageVelocity += _velocities[j];
        }
        averageVelocity /= _velocities.Count;

        transform.localPosition += averageVelocity * Time.deltaTime;
    }
}
