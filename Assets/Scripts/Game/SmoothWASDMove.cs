using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothWASDMove : MonoBehaviour
{
    private readonly List<Vector3> velocities = new List<Vector3>();

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
        velocities.Add(dir.normalized * speed);

        int frameCounter = (int)(Universe.Instance.FPSAverage * secondsAveraged);
        frameCounter = Math.Min(2, frameCounter);
        if (velocities.Count >= frameCounter)
        {
            velocities.RemoveAt(0);
        }

        Vector3 averageVelocity = Vector3.zero;
        for (int j = 0; j < velocities.Count; j++)
        {
            averageVelocity += velocities[j];
        }
        averageVelocity /= velocities.Count;

        transform.localPosition += averageVelocity * Time.deltaTime;
    }
}
