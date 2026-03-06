using System;
using UnityEngine;

public interface ICharacterInputReader
{
	public event Action<Vector2> MoveEvent;
	public event Action<Vector2> LookEvent;

	public event Action JumpStartedEvent;
	public event Action JumpCancelledEvent;

	public event Action AttackStartedEvent;
	public event Action AttackCancelledEvent;

	public event Action AimStartedEvent;
	public event Action AimCancelledEvent;

	public event Action CrouchStartedEvent;
	public event Action CrouchCancelledEvent;

	public event Action SprintStartedEvent;
	public event Action SprintCancelledEvent;
	
	public event Action InteractEvent;
	public event Action PauseEvent;
	public event Action<Cycle> CycleEvent;

	public void Enable();
	public void Disable();
}

public interface IUIInputReader
{
	public event Action<Vector2> NavigateEvent;
	public event Action SubmitEvent;
	public event Action CancelEvent;

	public void Enable();
	public void Disable();
}
