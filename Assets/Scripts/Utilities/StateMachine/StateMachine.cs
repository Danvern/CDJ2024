using System;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
	void OnEnter();
	void OnExit();
	void FrameUpdate();
	void PhysicsUpdate();
}

public interface IPredicate
{
	public bool Evaluate();
}


public delegate void StateNotification();

public class TriggerPredicate : IPredicate
{
	private bool _triggered = false;

	public bool Evaluate()
	{
		if (_triggered)
		{
			_triggered = false;
			return true;
		}
		return false;
	}

	public bool IsTriggered()
	{
		return _triggered;
	}

	public void Trigger()
	{
		_triggered = true;
	}
}

public class FunctionPredicate : IPredicate
{
	private Func<bool> _function;

	public FunctionPredicate(Func<bool> func)
	{
		_function = func;
	}

	public bool Evaluate()
	{
		return _function();
	}
}

public interface ITransition
{
	public IState To { get; }
	public IPredicate Predicate { get; }
	public bool Condition();
}

public class Transition : ITransition
{
	public IState To { get; }
	public IPredicate Predicate { get; }

	public Transition(IState to, IPredicate predicate)
	{
		To = to;
		Predicate = predicate;
	}

	public bool Condition()
	{
		return Predicate.Evaluate();
	}
}

public class StateTransition
{
	public string target;
	public Func<bool> condition;

	public StateTransition(string target, Func<bool> condition)
	{
		this.target = target;
		this.condition = condition;
	}

	public bool Evaluate()
	{
		return condition();
	}
}

public class StateMachine
{
	private IState _currentState;
	private Dictionary<Type, List<ITransition>> _transitions = new();
	private List<ITransition> _currentTransitions = new();
	private List<ITransition> _anyTransitions = new();
	private static List<ITransition> EmptyTransitions = new();
	public bool LogState = false;

	public void FrameUpdate()
	{
		if (_currentState == null)
			return;
		var transition = GetTransition();
		if (transition != null)
			SetState(transition.To);

		_currentState.FrameUpdate();
	}

	public void PhysicsUpdate()
	{
		_currentState.PhysicsUpdate();
	}

	public void SetState(IState state)
	{
		if (state == _currentState)
			return;

		_currentState?.OnExit();
		_currentState = state;

		_transitions.TryGetValue(_currentState.GetType(), out _currentTransitions);
		if (_currentTransitions == null)
			_currentTransitions = EmptyTransitions;
		GetTransition(); // update all transitions so you do not get stuck triggers

		_currentState?.OnEnter();

		if(LogState)
			Debug.Log("entered state: " + _currentState.GetType());

		//Debug.Log("Changed state to: " + _currentState.GetType());
	}

	public void AddTransition(IState from, IState to, IPredicate predicate)
	{
		if (_transitions.TryGetValue(from.GetType(), out var transitions) == false)
		{
			transitions = new();
			_transitions[from.GetType()] = transitions;
		}

		transitions.Add(new Transition(to, predicate));
	}

	public void AddAnyTransition(IState state, IPredicate predicate)
	{
		_anyTransitions.Add(new Transition(state, predicate));
	}

	private ITransition GetTransition()
	{
		// loop all to update predicates
		ITransition valid = null;

		foreach (ITransition transition in _anyTransitions)
		{
			if (transition.Condition() && valid == null)
				valid = transition;
		}

		foreach (ITransition transition in _currentTransitions)
		{
			if (transition.Condition() && valid == null)
				valid = transition;
		}

		return valid;
	}
}
