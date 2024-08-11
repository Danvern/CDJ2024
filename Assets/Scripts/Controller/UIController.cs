using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityServiceLocator;

public class UIController : MonoBehaviour, IEntityObserver
{
	public float DisplayHealth { get; set; } = 0;
	public float DisplayHealthMax { get; set; } = 0;
	HealthIconBar healthIconBar;
	public void SetPrimaryPlayer(EntityMediator targetPlayer) => targetPlayer.AddObserver(this);

	// Start is called before the first frame update
	void Awake()
	{
		ServiceLocator.Global.Register(this);
	}

	void Start() => healthIconBar = ServiceLocator.Global.Get<HealthIconBar>();

	public void OnNotify(EntityData data)
	{
		DisplayHealth = data.CurrentHealth;
		DisplayHealthMax = data.MaxHealth;

		Debug.Log("CurrentPlayerHope:" + DisplayHealth);
		healthIconBar.UpdateDisplayValue(DisplayHealth);
		healthIconBar.UpdateDisplayValueMax(DisplayHealthMax);

	}


	// Update is called once per frame
	void Update()
	{

	}
}
