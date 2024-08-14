using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityServiceLocator;

public class HealthIconBar : MonoBehaviour
{
	List<Image> iconBehaviors;
	[SerializeField] List<Sprite> iconBank = new List<Sprite>();
	float displayValue;
	float displayValueMax;

	public float UpdateDisplayValue(float value) => displayValue = value;
	public float UpdateDisplayValueMax(float value) => displayValueMax = value;

	void Awake()
	{
	ServiceLocator.ForSceneOf(this).Register(this);
	iconBehaviors = GetComponentsInChildren<Image>().ToList();

	}

    // Start is called before the first frame update
    void Start()
    {
        iconBehaviors = GetComponentsInChildren<Image>().ToList();
    }

    // Update is called once per frame
    void LateUpdate()
    {
		if (iconBehaviors.Count == 0 || iconBank.Count <= 2) return;

		int index = 0;
        iconBehaviors.ForEach((container) => 
		{
			int choiceIndex = Mathf.Clamp(Mathf.FloorToInt((displayValue - index * HealthPerIcon()) / HealthPerIcon() * (iconBank.Count - 1)), 0, iconBank.Count - 1);
			container.sprite = iconBank[choiceIndex];
			index++;
		});
    }

	float HealthPerIcon() {
		return displayValueMax / iconBehaviors.Count;

	}
}
