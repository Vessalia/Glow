using System;
using System.Collections.Generic;

public class StateMachine
{
	private StateNode current;
	private Dictionary<Type, StateNode> nodes = new();
	private HashSet<ITransition> anyTransitions = new();

	public void Update()
	{
		if (CanTransition(out ITransition transition))
			ChangeState(transition.NextState);

		current.State?.OnUpdate();
	}

	public void FixedUpdate()
	{
		current.State?.OnFixedUpdate();
	}

	public void SetState(IState state)
	{
		current = nodes[state.GetType()];
		current.State?.OnEnter();
	}

	public void AddTransition(IState from, IState to, IPredicate pred)
	{
		GetOrAddNode(from).AddTransition(to, pred);
	}

	public void AddAnyTransition(IState to, IPredicate pred)
	{
		anyTransitions.Add(new Transition(GetOrAddNode(to).State, pred));
	}

	private StateNode GetOrAddNode(IState state)
	{
		if (nodes.TryGetValue(state.GetType(), out var node))
			return node;

		var newNode = new StateNode(state);
		nodes.Add(state.GetType(), newNode);
		return newNode;
	}

	private void ChangeState(IState state)
	{
		if (state == current.State && !state.CanTransitionToSelf)
			return;

		var prev = current;
		var next = nodes[state.GetType()];

		prev.State?.OnExit();
		next.State?.OnEnter();

		current = next;
	}

	private bool CanTransition(out ITransition transition)
	{
		transition = null;

		foreach (var t in anyTransitions)
		{
			if (t.Condition.Evaluate())
			{
				transition = t;
				return true;
			}
		}

		foreach (var t in current.Transitions)
		{
			if (t.Condition.Evaluate())
			{
				transition = t;
				return true;
			}
		}

		return false;
	}

	private class StateNode
	{
		public IState State { get; }
		public HashSet<ITransition> Transitions { get; }

		public StateNode(IState state)
		{
			State = state;
			Transitions = new();
		}

		public StateNode AddTransition(IState state, IPredicate pred)
		{
			Transitions.Add(new Transition(state, pred));
			return this;
		}
	}
}
