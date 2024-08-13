using System;
using TMPro;
using UnityEngine;

public class PickupReward : MonoBehaviour
{
	[SerializeReference] GenericPickup stats;
	[field: SerializeField] public AnimationCurve ScaleCurve { get; private set; }
	[field: SerializeField] TextMeshProUGUI text;
	OwlCountdown countdown;
	Action consumptionFunction;
	public event Action OnPickupUsed;

	void Start()
	{
		if (stats == null)
			return;
		stats = Instantiate(stats);
		if (stats.Duration > 0)
		{
			countdown = new OwlCountdown(stats.Duration);
			countdown.Expired += () => { Destroy(gameObject); };
		}
		consumptionFunction = () => { OnPickupUsed?.Invoke(); if (gameObject) Destroy(gameObject); };
		stats.OnPickupUsed += consumptionFunction;
		if (text != null)
			text.text = stats.ToString();
	}

	void Update()
	{
		if (countdown == null)
			return;
		countdown.Update(Time.deltaTime);
		if (ScaleCurve != null)
		{
			float s = ScaleCurve.Evaluate(countdown.PercentageComplete(stats.Duration));
			transform.localScale = new Vector3(s, s, s);
		}
	}

	void OnDestroy()
	{
		stats.OnPickupUsed -= consumptionFunction;
	}

	void OnTriggerEnter(Collider collision)
	{
		if (stats == null)
		{
			Debug.LogWarning("Reward has no logic!!");
			Destroy(gameObject);
			return;
		}

		var visitable = collision.gameObject.GetComponents<IVisitable>();
		foreach (IVisitable component in visitable)
		{
			component.Accept(stats);
		}
	}
}

public interface IPickup : IVisitor
{
	event Action OnPickupUsed;

	float Duration { get; }
}

public abstract class GenericPickup : ScriptableObject, IPickup
{
	public event Action OnPickupUsed;
	[field: SerializeField] public float Duration { get; private set; } = -1f;
	public virtual void Visit(Entity visitable) { }
	public virtual void Visit(EntityHealthLogic visitable) { }
	public virtual void Visit(IMovementLogic visitable) { }
	public virtual void Visit(EntityMediator visitable) { }
	public void Consume() => OnPickupUsed?.Invoke();
}
