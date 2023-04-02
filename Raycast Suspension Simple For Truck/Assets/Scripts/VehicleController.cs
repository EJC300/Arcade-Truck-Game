using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class VehicleController : MonoBehaviour
{
    public Rigidbody rb => GetComponent<Rigidbody>();
    
    
    [SerializeField] private WheelScript[] wheels;
    [SerializeField] private VirtualEngine engine;
    [SerializeField] private float brakingForce;
    [SerializeField] private int downForce;
    private void Start()
    {
       rb.centerOfMass = -Vector3.up;
    }
    public void SteerWheel(float steerAmount)
    {
        /*
         * if steer wheel turn wheel that is steerable by steerAmount
         * 
         * 
         */
    }
    public void Gas(float throttle)
    {
        /*
         * 
         * apply engine force to driveWheels Traction Force
         * 
         */
        //engine.calculateEngineForce(throttle);
        //Test Engine
        engine.calculateEngineForce(Input.GetAxis("Vertical"));
    }

    

   
    public void ShiftUp()
    {
        engine.ChangeGear(1, 0);
    }
   
    
    
      
     public void ShiftDown()
      {
        engine.ChangeGear(-1, 0);


     }
      void ApplyDownForce()
    {
        rb.AddRelativeForce(Vector3.down * rb.velocity.magnitude * downForce);
    }
     
    public void Brakes(float brakeAmount)
    {
        /*
         * 
         * apply brakeForce to all wheels Traction Force
         * 
         */
    }
    private void FixedUpdate()
    {
        Gas(0);
        ApplyDownForce();
    }
}
