using UnityEngine;
using UnityEngine.UI;
using UnityServiceLocator;

public class ManaBar : MonoBehaviour
{
	[SerializeField] Slider activeBar;
	[SerializeField] Image capImage;
	[SerializeField] Sprite[] capSprites;
	Material activeMaterial;
	float displayValue;
	float displayValueMax;

	public float UpdateDisplayValue(float value) => displayValue = value;
	public float UpdateDisplayValueMax(float value) => displayValueMax = value;

	void Awake()
	{
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
			activeBar.value = (displayValue / displayValueMax);
		else
			activeBar.value = (0);
		capImage.sprite = capSprites[Mathf.Min(Mathf.FloorToInt(activeBar.value * capSprites.Length), capSprites.Length - 1)];
    }	

	int CapIndex() {
		return Mathf.FloorToInt(displayValue / displayValueMax * 4);

	}
}
