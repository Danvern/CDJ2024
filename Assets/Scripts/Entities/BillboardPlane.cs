using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardPlane : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		transform.rotation = Camera.main.transform.rotation * Quaternion.Euler(-270, 180, 0);
        
    }
}
