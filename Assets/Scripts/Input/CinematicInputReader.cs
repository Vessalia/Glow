
using System;
using UnityEngine;

// This is just for example for another class using the interface
public class CinematicInputReader : MonoBehaviour, ICharacterInputReader
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

	private bool _enabled = false;

	public void Disable() => _enabled = false;
	public void Enable() => _enabled = true;

	public void DoAIForCinematics()
	{
		if (!_enabled)
			return;

		Vector3 walkTarget = new Vector3(100, 0, 100); // some location
		var direction = (walkTarget - this.transform.position).normalized;
		MoveEvent(direction);
	}
}
