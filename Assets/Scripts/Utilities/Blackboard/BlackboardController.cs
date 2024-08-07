using System;
using System.Collections.Generic;
using UnityEngine;
using UnityServiceLocator;

namespace BlackboardSystem
{
	public class BlackboardController : MonoBehaviour
	{
		[SerializeField] BlackboardData blackboardData;
		readonly Blackboard blackboard = new Blackboard();
		readonly Arbiter arbiter = new Arbiter();

		void Awake()
		{
			ServiceLocator.For(this).Register(this);
			blackboardData.SetValuesOnBlackboard(blackboard);
			//blackboard.Debug();
		}

		public Blackboard GetBlackboard() => blackboard;

		public void RegisterExpert(IExpert expert) => arbiter.RegisterExpert(expert);
		public void DeregisterExpert(IExpert expert) => arbiter.DeregisterExpert(expert);

		void Update()
		{
			// Execute all agreed actions from the current iteration
			foreach (var action in arbiter.BlackboardIteration(blackboard))
			{
				action();
			}
		}
	}
}