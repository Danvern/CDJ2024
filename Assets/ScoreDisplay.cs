using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityServiceLocator;

public class ScoreDisplay : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI text;
	float displayValue;

	public float UpdateDisplayValue(float value) => displayValue = value;
	void Awake()
	{
		ServiceLocator.ForSceneOf(this).Register(this);

	}
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	void LateUpdate()
    {
			text.text = displayValue.ToString();
    }
}
