using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Build 0 for raycast car

public class RaycastWheel : MonoBehaviour
{
    Rigidbody rb => GetComponentInParent<Rigidbody>();

    Transform wheelGraphic => transform.GetChild(0);
    
    [SerializeField] private float suspensionHeight;

    [SerializeField] private float maxSuspensionHeight;

    [SerializeField] private float minSuspensionHeight;

    [SerializeField] private float suspensionStiffness;

    [SerializeField] private float suspensionDamp;

    [SerializeField] private float wheelRadius;

    [SerializeField] private float wheelDrag;

    [SerializeField] private float cRR;

    [SerializeField] private float sideWaysDrag;

    [SerializeField] private float grip;

    [SerializeField] private int tireMass;

    private float lastSpeed;

    [SerializeField] private Vector3 wheelVelocity => rb.GetPointVelocity(transform.position)/2*Mathf.PI* wheelRadius;

    [SerializeField] LayerMask layer;
    RaycastHit hit;
    bool Wheelhit => Physics.Raycast(transform.position, -transform.up, out hit, suspensionHeight + wheelRadius, layer);

    void Suspension()
    {
        Vector3 worldVelocity = rb.GetPointVelocity(transform.position);
        float upVelocity = Vector3.Dot(worldVelocity, transform.up);
        float compressionRatio = (suspensionHeight - hit.distance);
        compressionRatio = Mathf.Clamp(compressionRatio, minSuspensionHeight, maxSuspensionHeight);
        float springForce = (suspensionStiffness * compressionRatio) -(upVelocity * suspensionDamp);
        rb.AddForceAtPosition(springForce * transform.up, transform.position);

    }

    void LongForce()
    {
        float forwardVelocity = Vector3.Dot(wheelVelocity, transform.forward);
        float drag = -forwardVelocity * wheelDrag;
        float rollingResitance = -forwardVelocity * cRR;
        lastSpeed = forwardVelocity / Time.time;
        float traction = forwardVelocity * lastSpeed + drag + rollingResitance;

        rb.AddForceAtPosition(transform.forward * traction,transform.position);
    }
    void lateralForce()
    {
        float sideWaysVelocity = Vector3.Dot(wheelVelocity, transform.right);
        float accel = -sideWaysVelocity / Time.time;
        float sideWaysGrip = -sideWaysVelocity * grip;
        float gripBySpeed = Mathf.Lerp(accel, grip, wheelVelocity.magnitude);
        float totalLateralGrip = sideWaysGrip + gripBySpeed * (accel * tireMass) * sideWaysDrag;

        rb.AddForceAtPosition(transform.right * totalLateralGrip,transform.position);
    }
    private void FixedUpdate()
    {
        if(Wheelhit)
        {
            Suspension();
          
            LongForce();
            lateralForce();
            wheelGraphic.transform.position = transform.position + Vector3.up * -(hit.distance - wheelRadius);



        }
        else
        {

            wheelGraphic.transform.position = transform.position + Vector3.up * (suspensionHeight - wheelRadius);
        }
    }

}
