using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityServiceLocator;

public class UIController : MonoBehaviour, IEntityObserver
{
	public float DisplayHealth { get; set; } = 0;
	public float DisplayHealthMax { get; set; } = 0;
	public float DisplayMana { get; set; } = 0;
	public float DisplayManaMax { get; set; } = 0;
	HealthIconBar healthIconBar;
	ManaBar manaBar;
	public void SetPrimaryPlayer(EntityMediator targetPlayer) => targetPlayer.AddObserver(this);

	// Start is called before the first frame update
	void Awake()
	{
		if (!ServiceLocator.Global.TryGet<UIController>(out _))
		ServiceLocator.Global.Register(this);
	}

	void Start()
	{
		healthIconBar = ServiceLocator.Global.Get<HealthIconBar>();
		manaBar = ServiceLocator.Global.Get<ManaBar>();
		UpdateElements();
	}

	void UpdateElements()
	{
		if (healthIconBar != null)
		{
			healthIconBar.UpdateDisplayValue(DisplayHealth);
			healthIconBar.UpdateDisplayValueMax(DisplayHealthMax);
		}
		if (manaBar != null)
		{
			manaBar.UpdateDisplayValue(DisplayManaMax);
			manaBar.UpdateDisplayValueMax(DisplayManaMax);

		}
	}

	public void OnNotify(EntityData data)
	{
		DisplayHealth = data.CurrentHealth;
		DisplayHealthMax = data.MaxHealth;
		DisplayMana = data.CurrentMana;
		DisplayManaMax = data.MaxMana;

		Debug.Log("CurrentPlayerHope:" + DisplayHealth);
		UpdateElements();
	}

	public void RestartLevel()
	{
	SceneManager.LoadScene(SceneManager.GetActiveScene().name);

	}



	// Update is called once per frame
	void LateUpdate()
	{

	}
}
