using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualEngine : MonoBehaviour
{

  [SerializeField] private int maxRPM;
  [SerializeField] private AnimationCurve torqueCurve;
  [SerializeField] private float[] gearRatios;
    private int currentGearIndex; 
  private float currentGear;
   private float currentEngineForce;

  [SerializeField]  float wheelSpeed = 0;
    [SerializeField] private WheelScript[] wheels;

    public int CurrentGearIndex { get => currentGearIndex; set => currentGearIndex = value; }

    public void calculateEngineForce(float throttle)
    {
        foreach(WheelScript wheel in wheels)
        {
            if(wheel.IsDriveWheel && throttle > 0.5f)
            {

                wheelSpeed = wheel.WheelSpeed / 2*Mathf.PI/1.5f;
               
                wheelSpeed = Mathf.Clamp(wheelSpeed, 0, maxRPM);
                float torque = torqueCurve.Evaluate(wheelSpeed / maxRPM);
                currentEngineForce += (torque/ gearRatios[currentGearIndex]) * 1000;
                
                wheel.EngineForce = currentEngineForce;
              
            }
            if(throttle < 1.0f)
            {
                currentEngineForce = wheelSpeed;
            }
        }

    }

    private void ReverseEngine(float gas)
    {
        foreach (WheelScript wheel in wheels)
        {
            if (wheel.IsDriveWheel)
            {
                float wheelSpeed = wheel.WheelSpeed;
                float torque = torqueCurve.Evaluate(wheelSpeed / maxRPM) * 0.25f;
                currentEngineForce = ((torque /1) * 100) / (2 * Mathf.PI / 90);
                currentEngineForce = Mathf.Clamp(currentEngineForce, 0, maxRPM * 0.25f);
                wheel.EngineForce = -currentEngineForce;
            }
        }
    }
    public void ChangeGear(int gearIndex,float gas)
    {
        if(gearIndex >= 0 && gearIndex <= gearRatios.Length)
        {
            currentGear = gearRatios[currentGearIndex+ gearIndex];
        }
        if(gearIndex == -1)
        {
            ReverseEngine(gas);
        }
        
    }
 

    private void Update()
    {
      

    }

}
