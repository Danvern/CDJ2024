using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityServiceLocator;

public class WinWhenDead : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void OnDestroy()
    {
        ServiceLocator.ForSceneOf(this).Get<UIController>().Win();
    }
}
