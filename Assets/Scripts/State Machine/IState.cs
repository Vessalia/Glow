
using Mono.Cecil;
using System;

public interface IState
{
	public bool CanTransitionToSelf { get; }

	void OnEnter();
	void OnExit();
	void OnUpdate();
	void OnFixedUpdate();
}
