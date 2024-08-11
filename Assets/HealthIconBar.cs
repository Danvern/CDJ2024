using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class HealthIconBar : MonoBehaviour
{
	List<Image> iconBehaviors;
	[SerializeField] List<Sprite> iconBank = new List<Sprite>();
	float displayValue;
	float displayValueMax;

	public float UpdateDisplayValue(float value) => displayValue = value;

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
			int choiceIndex = Mathf.FloorToInt(((displayValue / HealthPerIcon()) % HealthPerIcon()) * (iconBank.Count - 1));
			container.sprite = iconBank[index];
			index++;
		});
    }

	float HealthPerIcon() {
		return displayValueMax / iconBehaviors.Count;

	}
}
