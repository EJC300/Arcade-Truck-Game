using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraThing : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Camera.main.transform.LookAt(this.transform);
    }
}
