using System.Collections.Generic;
using UnityEngine;

public abstract class EntitySubject : MonoBehaviour
{
	List<IEntityObserver> observers = new();
	public void AddObserver(IEntityObserver observer) {
		observers.Add(observer); 
	}
	public void RemoveObserver(IEntityObserver observer) {
		observers.Remove(observer);
	}
	public void NotifyObservers(EntityData data) {
		observers.ForEach((observer) => observer.OnNotify(data));
	}

}

public struct EntityData
{
	enum UpdateType {Health}

	public float CurrentHealth;
	public float MaxHealth;
	public float CurrentMana;
	public float MaxMana;
	public int Score;
	public VoicePrompt Prompt;

}

public enum VoicePrompt 
{
	None,
	Idle,
	Hurt,
	HealingPickup,
	MagicPickup,
	SwordSwing,
	MagicCast,
	Combo,
	Dash,
	Win,
	Death,
	Lose,
	NextWave,
	SpawningIn,
	Boss1,
	Boss2,
	Boss3,


}