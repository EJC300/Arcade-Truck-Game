using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWheel : MonoBehaviour
{

    //This is a good method with lots to grow on. I can add a tireslip model that effects grip using the unity built in math functions
    //Build 0.1
    //todo: Better grip simulation
    Rigidbody rb =>  GetComponentInParent<Rigidbody>();
    [SerializeField] private bool steer;
    Transform graphic => transform.GetChild(0);
    RaycastHit hit;
    [SerializeField] LayerMask layer;
    [SerializeField] private float slipY;
    [SerializeField] private float gripX;
    [SerializeField] private float gripY;
    [SerializeField] private int engineForce;
    [SerializeField] private float suspensionDist;
    [SerializeField] AnimationCurve slipCurve;
    [SerializeField] private float suspensionStrength;
    [SerializeField] private float mass;
    [SerializeField] private float suspensionDamp;
    [SerializeField] private AnimationCurve engineTorque;
    private float wheelSpeed;
    public bool isGrounded => Physics.Raycast(transform.position, -transform.up, out hit, suspensionDist, layer);
    float SlipY()
    {
        Vector3 steerDir = transform.forward;
        Vector3 velo = rb.GetPointVelocity(transform.position);


        float steerVelo = Vector3.Dot(steerDir, velo);
        steerVelo = slipCurve.Evaluate(Mathf.Clamp01(steerVelo));
        return steerVelo * slipY;
    }
    Vector3 rollingMomentum()
    {
      
        Vector3 velo = rb.GetPointVelocity(transform.position);
        float forwardVelo = Vector3.Dot(velo, transform.forward);
        float accel = forwardVelo / Time.time;
        float momentum = accel / mass;
        return transform.forward * momentum * rb.mass;
    }
    
    Vector3 WheelSteerForce()
    {

        Vector3 steerDir = transform.right;
        Vector3 velo = rb.GetPointVelocity(transform.position);
        

        float steerVelo = Vector3.Dot(steerDir, velo);

        float changeInVelo = (-steerVelo) *  (gripX);
        changeInVelo = changeInVelo * slipCurve.Evaluate(rb.velocity.normalized.z);
        float accel = ( changeInVelo) / Time.fixedDeltaTime;
        




        return steerDir * mass * accel;
    }

    private void Start()
    {
        rb.centerOfMass = Vector3.down;
    }
    void SteerWheel()
    {
        if(steer)
        {
            transform.localEulerAngles = Vector3.up * 45 * Input.GetAxis("Horizontal");
        }
    }
    Vector3 WheelEngineForceTest()
    {
        float wheelTorque = rb.GetPointVelocity(graphic.transform.position).sqrMagnitude /2 * Mathf.PI * 0.33f * Mathf.Rad2Deg ;
        float lastWheelTorque = wheelTorque;
        wheelSpeed = wheelTorque - lastWheelTorque;
        Debug.Log(wheelSpeed);
        return graphic.transform.forward * engineTorque.Evaluate(wheelTorque/wheelTorque) * engineForce * Input.GetAxis("Vertical");
    }
    Vector3 WheelForwardForce()
    {
        Vector3 forwardDir = graphic.transform.forward;
        Vector3 velo = rb.GetPointVelocity(transform.position);
     

        float forwardVelo = Vector3.Dot(forwardDir, velo);

     

        float changeInVelo = -forwardVelo * Mathf.Abs (gripY -SlipY());

        float accel = changeInVelo / Time.fixedDeltaTime;





        return forwardDir * mass * accel;
    }
    Vector3 SuspensionForce()
    {
        if (isGrounded)
        {

            Vector3 velo = rb.GetPointVelocity(transform.position);
            float offset = suspensionDist - hit.distance;
            float supsensionUpForce = offset * suspensionStrength;

            float suspVelo = Vector3.Dot(velo, transform.up);

            float suspensionDampForce = suspVelo * suspensionDamp;

            Vector3 supsensionTotalForce = transform.up * (supsensionUpForce - suspensionDampForce);

            return supsensionTotalForce;
        }
        return Vector3.zero;
    }
    public bool CheckAccelerating()
    {
        Vector3 currentVelocity = rb.velocity;


        Vector3 lastVelocity = currentVelocity;

        return (currentVelocity.magnitude >= 0.05f && currentVelocity.magnitude < 1.5f);
    }
    
    private void FixedUpdate()
    {
      
        
        SteerWheel();
        if (isGrounded)
        {
            Vector3 velo = new Vector3( rb.velocity.x,0,rb.velocity.z);


            Vector3 suspensionForce = SuspensionForce();
            float incline = Vector3.Angle(rb.transform.position, hit.normal);
            if (incline < 4 || rb.velocity.magnitude < 0.5f)
            {

                suspensionForce.x = 0;
                suspensionForce.z = 0;
            }
            rb.AddForceAtPosition(suspensionForce + WheelSteerForce() + WheelForwardForce() + WheelEngineForceTest(), graphic.position);
            
            graphic.transform.position = transform.position + Vector3.up * -(hit.distance - 0.5f);
           
           
           
        }
        else
        {
         
            graphic.transform.position = transform.position + Vector3.up * (suspensionDist - 0.5f);
        }
       
    }
}
