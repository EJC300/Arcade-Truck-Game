using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RaycastWheelSimple : MonoBehaviour
{

    /*
     * This script is a simple raycast wheel script to experiment with before I start prototyping Road Kings.
     * 
     * There is nothing overly complicated here. No antiRollBars no complicated wheel friction models just a simple kart style physics. Press the gas and go!
     * 
     * 
     * E.J. Curtis 3:08pm, 9/20/22
     * 
     */


    private Rigidbody rb => GetComponentInParent<Rigidbody>();
    [SerializeField] private Transform wheelGraphic=>transform.GetChild(0);
    [SerializeField] private int springStrength;
    [SerializeField] private int dampStrength;
    [SerializeField] private float springLength;
    [SerializeField] private float wheelDrag;
    [SerializeField] private float wheelFriction;
    [SerializeField] private LayerMask layer;
    private float currentWheelForce;
    private float rollingWheelForce;
    private float brakeForce;
    
    RaycastHit hit;
    float wheelForce;
    float lastWheelForce;
    [SerializeField] private bool braking;
   [SerializeField] private bool steer;
    private bool isGrounded => Physics.Raycast(transform.position, -transform.up, out hit, springLength,layer);

    private void Start()
    {
        rb.centerOfMass = -Vector3.down;
    }

    private Vector3 SuspensionForce()
    {
        if (isGrounded)
        {
            float spring = (springLength - hit.distance) / hit.distance;
            Vector3 upForce = transform.up * spring * springStrength;
            upForce.z = 0;
            upForce.x = 0;
            return ( upForce);

        }


        return Vector3.zero;
    }

    void Brakes()
    {
        


        if (transform.InverseTransformDirection(rb.velocity).z > 0.0f && isGrounded && Input.GetKey(KeyCode.S))
        {
            if (wheelForce > 0.00f)
            {
                currentWheelForce -= 200;

               
            }
        }
       
      
    }
    private void SteerWheel()
    {
        if(steer)
        {
            transform.localEulerAngles = Vector3.up * 45 *  Input.GetAxis("Horizontal");
        }
      
    }

    private void WheelSpeedForce()
    {
        lastWheelForce = wheelForce;
        if (Input.GetKey(KeyCode.W))
        {

            currentWheelForce += 3;

        }
        
        wheelForce = currentWheelForce / 2 * Mathf.PI * 0.5f;
        

      
       
      
      
       
     
        
        Debug.Log(rollingWheelForce);
        if (isGrounded)
        {
            rollingWheelForce = (currentWheelForce - lastWheelForce) / Time.time; 
            rb.AddForceAtPosition(transform.forward * rollingWheelForce * 400, hit.point);
        }
        else
        {
            rollingWheelForce = 0;
        }
    }

    private Vector3 SetWheelDragY()
    {
        Vector3 WheelPointVelocity = rb.GetPointVelocity(hit.point);
        Vector3 wheelVeloY = transform.TransformDirection(WheelPointVelocity);
        wheelVeloY.x = 0;
        wheelVeloY.y = 0;

      return  transform.InverseTransformDirection(wheelVeloY) * -wheelDrag;


    }
    private Vector3 SetWheelDragX()
    {
        Vector3 WheelPointVelocity = rb.GetPointVelocity(hit.point);
        Vector3 wheelVeloY = transform.TransformDirection(WheelPointVelocity);
        wheelVeloY.z = 0;
        wheelVeloY.y = 0;

        return transform.InverseTransformDirection(wheelVeloY) * -wheelDrag;


    }


    private Vector3 DampForce()
    {
        Vector3 velocityAtTouch = rb.GetPointVelocity(hit.point);
        Vector3 upSpeed = transform.TransformDirection(velocityAtTouch);
        upSpeed.z = 0;
        upSpeed.x = 0;
        Vector3 dampedForce = upSpeed * -dampStrength;

        if (isGrounded)
        {
            return dampedForce;
        }
        return Vector3.zero;
    }
    void WheelGraphics()
    {
        if(isGrounded)
        {
            wheelGraphic.transform.position = transform.position + Vector3.up * -(hit.distance-0.5f);
        }
        else
        {
            wheelGraphic.transform.position = transform.position + Vector3.up *( springLength - 0.5f);
        }
    }
    void Suspension()
    {
        if (isGrounded)
        {
            rb.AddForceAtPosition(SuspensionForce() + DampForce() + SetWheelDragX() + SetWheelDragY(), transform.position);
        }
    }
    private void FixedUpdate()
    {
       
        Suspension();

        WheelGraphics();
        SteerWheel();
        Brakes();
        WheelSpeedForce();

    }
}
