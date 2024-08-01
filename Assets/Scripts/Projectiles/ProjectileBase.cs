using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
	[SerializeField] ProjectileDamageData projectileDamageData;
	ProjectileDamageLogic damageLogic;

    // Start is called before the first frame update
    void Start()
    {
        damageLogic = new ProjectileDamageLogic(projectileDamageData);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        damageLogic.CheckCollisons(transform.position);
    }
}
