using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleArcadeWheel : MonoBehaviour
{
    
    /*
     * This is another experimental script for raycast wheels.
     * I have implemented a simple wheel slip system where the slip overwhelms the wheel if forward or sidways traction is lost causing spinnout or drifting
     * The challenging part I have in all raycast cars is getting the car to not lerch forward by the smallest amount. I may need to cheat a little on keeping the vehicle still when at 
     * a mininum speed.
     * 
     */


    Rigidbody rb => GetComponentInParent<Rigidbody>();
    private float wheelForce;
    [SerializeField] private bool steer;
    [SerializeField] private Transform graphic=>transform.GetChild(0);

    [SerializeField] private float minSpeed;

    [SerializeField] private int stiffness;

    [SerializeField] private int dampening;

    [SerializeField] private float springLength;

    [SerializeField] private float maxLength;

    [SerializeField] private float minLength;

    [SerializeField] private float wheelRadius;

    [SerializeField] private LayerMask layer;

    [SerializeField] private float rollingResistance;
    
    [SerializeField] private float maxlateralSlip;

    [SerializeField] private float minlateralSlip;

    [SerializeField] private float maxlongitudalSlip;


    private Vector3 lastVelocity;
    

    private RaycastHit hit;
    private bool grounded => Physics.Raycast(transform.position, -transform.up, out hit, springLength + wheelRadius, layer);

    private float GetAcceleration()
    {
        
        Vector3 currentVelo = rb.velocity;

        lastVelocity = currentVelo;
        return (currentVelo - lastVelocity).magnitude / Time.time;
    }
    private Vector3 SuspensionForce()
    {
        float totalSuspensionDist = (springLength + wheelRadius);
        float proportionalDist =  ( totalSuspensionDist -hit.distance) / hit.distance;
        float suspension = Mathf.Lerp(proportionalDist, minLength, maxLength);
        proportionalDist = Mathf.Clamp(suspension, minLength, maxLength);
    
        return transform.up * proportionalDist * stiffness;
        
    }
    private void Awake()
    {
        rb.centerOfMass = Vector3.down;
    }
    private Vector3 DampForce()
    {
        Vector3 velocityAtTouch = rb.GetPointVelocity(hit.point);
        Vector3 upSpeed = transform.TransformDirection(velocityAtTouch);
        upSpeed.x = 0;
        upSpeed.z = 0;
        Vector3 dampedForce = upSpeed * -dampening;

        if (grounded)
        {
            return dampedForce;
        }
        return Vector3.zero;
    }
    private void SteerWheel()
    {
        if (steer)
        {
            transform.localEulerAngles = Vector3.up * 45 * Input.GetAxis("Horizontal");
        }

    }
    private Vector3 WheelSlipY()
    {
        Vector3 velocityAtTouch = rb.GetPointVelocity(hit.point);
        Vector3 forwardVelocity = transform.InverseTransformDirection(velocityAtTouch);
        forwardVelocity.y = 0;
        forwardVelocity.x = 0;
        float sideWays = Vector3.Dot(forwardVelocity, -Vector3.right);
        Vector3 friction = Vector3.Lerp(forwardVelocity,-forwardVelocity * maxlongitudalSlip,sideWays);
        return (friction);
    }
    private Vector3 WheelSlipX()
    {
        Vector3 velocityAtTouch = rb.GetPointVelocity(hit.point);
        Vector3 forwardVelocity = transform.InverseTransformDirection(velocityAtTouch);
        forwardVelocity.y = 0;
        forwardVelocity.z = 0;
        float sideWays = Vector3.Dot(forwardVelocity, -Vector3.forward);
        Vector3 friction = Vector3.Lerp(forwardVelocity * minlateralSlip, -forwardVelocity * maxlateralSlip, sideWays);
        return ( friction);
    }

    private void WheelGraphics()
    {
        if(grounded)
        {
            graphic.transform.position = transform.position + -Vector3.up * (hit.distance - wheelRadius); 
        }
        else
        {
            graphic.transform.position = transform.position + -Vector3.up * (maxLength - wheelRadius);
        }
    }

    private Vector3 SetWheelDragY()
    {
        Vector3 WheelPointVelocity = rb.GetPointVelocity(hit.point);
        Vector3 wheelVeloY = transform.TransformDirection(WheelPointVelocity);
        wheelVeloY.x = 0;
        wheelVeloY.y = 0;

        return transform.InverseTransformDirection(wheelVeloY) * -rollingResistance;


    }
    private Vector3 SetWheelDragX()
    {
        Vector3 WheelPointVelocity = rb.GetPointVelocity(hit.point);
        Vector3 wheelVeloY = transform.TransformDirection(WheelPointVelocity);
        wheelVeloY.z = 0;
        wheelVeloY.y = 0;

        return transform.InverseTransformDirection(wheelVeloY) * -rollingResistance;


    }

    private void LimitForwardSpeed()
    {
    }
    private void WheelSuspension()
    {
        if(grounded)
        {
            rb.AddForceAtPosition((SuspensionForce() + DampForce()) + WheelSlipX() + WheelSlipY() + SetWheelDragX() + SetWheelDragY(), hit.point);
        }
    }
    private void FixedUpdate()
    {
      
        WheelGraphics();
        WheelSuspension();
        SteerWheel();
        LimitForwardSpeed();
    }
}
