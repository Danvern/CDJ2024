using UnityEngine;

public delegate void CountdownNotification();

public class OwlCountdown
{
	public event CountdownNotification Expired;
	float _time = 0f;
	bool _expired = false;

	public OwlCountdown(float duration) => _time = duration;

	public void Update(float deltaTime)
	{
		if (_expired)
			return;

		if (_time <= 0)
		{
			Expired?.Invoke();
			_expired = true;
			_time = 0;
			return;
		}

		_time -= deltaTime;
	}

	public void Reset(float time)
	{
		_expired = false;
		_time = time;
	}

	public void AddTime(float time)
	{
		_time += time;
	}

	public float PercentageComplete(float max)
	{
		return 1f - (_time / max);
	}
}