using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityServiceLocator;

public class ManaBar : MonoBehaviour
{
	[SerializeField] Image activeBar;
	Material activeMaterial;
	float displayValue;
	float displayValueMax;

	public float UpdateDisplayValue(float value) => displayValue = value;
	public float UpdateDisplayValueMax(float value) => displayValueMax = value;

	void Awake()
	{
		activeBar.material = new Material(activeBar.material);
		activeMaterial = activeBar.material;
		ServiceLocator.ForSceneOf(this).Register(this);

	}

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void LateUpdate()
    {
		if (displayValueMax > 0)
			activeMaterial.SetFloat("_Fullness", displayValue / displayValueMax);
		else
			activeMaterial.SetFloat("_Fullness", 0);
    }	

	int CapIndex() {
		return Mathf.FloorToInt(displayValue / displayValueMax * 4);

	}
}
