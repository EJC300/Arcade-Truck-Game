using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Wheel
{
    [SerializeField] private float wheelTorque;
    [SerializeField] private int springStrength;
    [SerializeField] private int dampStrength;
    [SerializeField] private float springLength;
    [SerializeField] private float wheelDrag;
    public Transform wheelGraphic;
}
