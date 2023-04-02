using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class PhysicsLimiter : MonoBehaviour
{
    Rigidbody rb => GetComponent<Rigidbody>();
    [SerializeField] private float maxSleepAngle = 15f;
    [SerializeField] private int parkingDrag = 100000;
    [SerializeField] private TestWheel[] wheels;
    bool isGrounded()
    {
       int groundedCount = 0;
        foreach (TestWheel wheel in wheels)
        {
            if (wheel.isGrounded)
            {
                groundedCount++;
            }
        }
        return groundedCount >= 4;
    }
    private bool isAccelerating()
    {
        float currentSpeed = rb.velocity.magnitude;

        float lastSpeed = rb.velocity.magnitude;

        return ((currentSpeed - lastSpeed) / Time.deltaTime) >= 0.5f;
    }

    private bool isDeccelerating()
    {
        float currentSpeed = rb.velocity.magnitude;

        float lastSpeed = rb.velocity.magnitude;

        return ((currentSpeed - lastSpeed) / Time.deltaTime) <= 0.5f;
    }

    private bool isBeyondSleepAngle()
    {
        return transform.eulerAngles.x >= maxSleepAngle;
    }
    void LimitPhysics()
    {
    
       if(Mathf.Abs(rb.velocity.magnitude) < 0.5f &&!isAccelerating() && isGrounded() && !isBeyondSleepAngle())
        {
            rb.Sleep();
        }
     
        Debug.Log(rb.velocity.magnitude);
    }
    private void FixedUpdate()
    {
        rb.AddTorque(Vector3.forward * rb.angularVelocity.x * 1000);
    }
}
