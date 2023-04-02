using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelScript : MonoBehaviour
{
    Rigidbody rb => GetComponentInParent<Rigidbody>();
    [SerializeField] private bool steer;
    Transform graphic => transform.GetChild(0);

    public float LongForce { get => longForce; set => longForce = value; }
    public float LateralForce { get => lateralForce; set => lateralForce = value; }

    private RaycastHit hit;
    [SerializeField] AnimationCurve gripCurve;
    [SerializeField] LayerMask layer;
    [SerializeField] private float rollingCST;
    [SerializeField] private float slip;
    [SerializeField] private float grip;
    [SerializeField] private float suspensionDist;
    [SerializeField] private float wheelRadius;
    [SerializeField] private float suspensionStrength;
    [SerializeField] private float suspensionDamp;
    [SerializeField] private float wheelDrag;
    [SerializeField] private float maxGrip;
    [SerializeField] float rearAxelDistance;
    [SerializeField] float frontAxelDistance;
    private float alpha;
    [SerializeField] private float lateralCST;
   [SerializeField] private bool isDriveWheel;
    private float currentGrip;
    private float tractionForce;
    private float normalForce;
    private float rollingResitance;
    private float longForce;
    private float lateralForce;
    private float engineForce;
    private float brakeForce;
    private float momentum;
    private bool isBraking;
    private float wheelSpeed;
    public bool isGrounded => Physics.Raycast(transform.position, -transform.up, out hit,suspensionDist + wheelRadius, layer);
    
    public float TractionForce { get => tractionForce; set => tractionForce = value; }
    public float RollingResitance { get => rollingResitance; set => rollingResitance = value; }
    public float NormalForce { get => normalForce; set => normalForce = value; }
    public float EngineForce { get => engineForce; set => engineForce = value; }
    public float BrakeForce { get => brakeForce; set => brakeForce = value; }
    public bool IsBraking { get => isBraking; set => isBraking = value; }
    public float Momentum { get => momentum; set => momentum = value; }
    public float WheelSpeed { get => wheelSpeed; set => wheelSpeed = value; }
    public bool IsDriveWheel { get => isDriveWheel; set => isDriveWheel = value; }

    private Vector3 SuspsensionForce()
    {
        if (isGrounded)
        {

            Vector3 velo = rb.GetPointVelocity(transform.position);
            float offset = suspensionDist - hit.distance;
            float supsensionUpForce = offset * suspensionStrength;

            float suspVelo = Vector3.Dot( transform.up, velo);

            float suspensionDampForce = suspVelo * suspensionDamp;

            Vector3 supsensionTotalForce = hit.normal * (supsensionUpForce - suspensionDampForce);
       
            return supsensionTotalForce;
        }
        return Vector3.zero;
    }
    private Vector3 LateralVelocity()
    {
        Vector3 velo = rb.GetPointVelocity(transform.position);
        if (isGrounded)
        {
            alpha = Mathf.Atan(rearAxelDistance + frontAxelDistance * longForce) - Mathf.Sign(longForce);
            currentGrip = Mathf.Lerp(currentGrip, maxGrip, Time.time) * gripCurve.Evaluate(wheelSpeed);

            LateralForce = -Vector3.Dot( transform.right,velo) * ((maxGrip - alpha) * lateralCST);
        }
        return transform.right * lateralForce;
    }
   private Vector3 LongitudalVelocity()
    {
        Vector3 velo = rb.GetPointVelocity(transform.position);

        if (isGrounded)
        {

            normalForce = rb.mass * Vector3.Dot( -transform.up,velo) + Vector3.Dot(rb.velocity,-transform.up);
            rollingResitance = rb.velocity.z * -rollingCST;

            if(isBraking)
            {
                TractionForce = normalForce + longForce * -brakeForce;
            }
            else if(!isBraking)
            {
                TractionForce = engineForce;
                Debug.Log(tractionForce);
            }
            else
            {
                TractionForce = Vector3.Dot(transform.forward, velo) / rb.mass;
            }
            LongForce = tractionForce + normalForce + rollingResitance + (-Vector3.Dot(transform.forward,velo) * wheelDrag);
        }
        WheelSpeed = velo.z / (2*Mathf.PI / (60 * wheelRadius));
        return transform.forward * longForce * Time.deltaTime;
    }
    private void WheelSuspensionAnimation()
    {
        if (isGrounded)
        {
            graphic.transform.position = transform.position + Vector3.up * -(hit.distance - wheelRadius);
        }
        else
        {
            graphic.transform.position = transform.position + Vector3.up * (suspensionDist - wheelRadius);
        }
    }
    private void ApplyForces()
    {
        rb.AddForceAtPosition(SuspsensionForce() + LateralVelocity() + LongitudalVelocity() , transform.position);
    }
    private void Update()
    {
        WheelSuspensionAnimation();
    }
    private void FixedUpdate()
    {
        ApplyForces();
       
    }
}
