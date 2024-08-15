using UnityEngine;
using UnityEngine.SceneManagement;
using UnityServiceLocator;

public class UIController : MonoBehaviour, IEntityObserver
{
	public float DisplayHealth { get; set; } = 0;
	public float DisplayHealthMax { get; set; } = 0;
	public float DisplayMana { get; set; } = 0;
	public float DisplayManaMax { get; set; } = 0;
	public float DisplayScore { get; set; } = 0;
	ScoreDisplay scoreDisplay;
	HealthIconBar healthIconBar;
	ManaBar manaBar;
	public void SetPrimaryPlayer(EntityMediator targetPlayer) => targetPlayer.AddObserver(this);

	// Start is called before the first frame update
	void Awake()
	{
		ServiceLocator.ForSceneOf(this).Register(this);
	}

	void Start()
	{
		healthIconBar = ServiceLocator.ForSceneOf(this).Get<HealthIconBar>();
		manaBar = ServiceLocator.ForSceneOf(this).Get<ManaBar>();
		scoreDisplay = ServiceLocator.ForSceneOf(this).Get<ScoreDisplay>();
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
		if (scoreDisplay)
		{
			scoreDisplay.UpdateDisplayValue(DisplayScore);

		}
	}

	public void OnNotify(EntityData data)
	{
		DisplayHealth = data.CurrentHealth;
		DisplayHealthMax = data.MaxHealth;
		DisplayMana = data.CurrentMana;
		DisplayManaMax = data.MaxMana;
		DisplayScore = data.Score;

		//Debug.Log("CurrentPlayerHope:" + DisplayHealth);
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
