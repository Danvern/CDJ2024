using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore;
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

	[SerializeField] GameObject PauseScreen;
	bool paused = false;
	bool playerWon = false;
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
		Unpause();
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
			manaBar.UpdateDisplayValue(DisplayMana);
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

		if (data.Prompt == VoicePrompt.Lose)
			Lose();
	}

	IEnumerator WaitAndRestart()
	{
		yield return new WaitForSeconds(3f);
		RestartLevel();
	}

	public void RestartLevel()
	{

		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		Time.timeScale = 1.0f;

	}


	public bool IsPaused() => paused;
	public void Pause()
	{
		if (paused) return;

		PauseScreen.SetActive(true);
		Time.timeScale = 0;
		paused = true;


	}
	public void Unpause()
	{
		if (!paused) return;

		PauseScreen.SetActive(false);
		Time.timeScale = 1.0f;
		paused = false;


	}
	public void TogglePause()
	{
		if (paused)
			Unpause();
		else
			Pause();
	}

	public void Lose()
	{
		WaitAndRestart();
	}
	public void Win()
	{
		if (!playerWon)
		{
			playerWon = true;
			Unpause();
			GetComponent<LoadScene>().Load();
		}
	}



	// Update is called once per frame
	void LateUpdate()
	{

	}


}
