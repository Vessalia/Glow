using System;
using UnityEngine;

public class CharacterInputFacade : ICharacterInputReader
{
	private ICharacterInputReader _reader;

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

	public CharacterInputFacade(ICharacterInputReader reader)
	{
		_reader = reader;
		SubscribeAll(_reader);
	}

	public void Set(ICharacterInputReader reader)
	{
		UnsubscribeAll(_reader);
		_reader = reader;
		SubscribeAll(_reader);
	}

	private void SubscribeAll(ICharacterInputReader reader)
	{
		reader.MoveEvent += OnMove;
		reader.LookEvent += OnLook;
		reader.JumpStartedEvent += OnJumpStarted;
		reader.JumpCancelledEvent += OnJumpCancelled;
		reader.AttackStartedEvent += OnAttackStarted;
		reader.AttackCancelledEvent += OnAttackCancelled;
		reader.AimStartedEvent += OnAimStarted;
		reader.AimCancelledEvent += OnAimCancelled;
		reader.CrouchStartedEvent += OnCrouchStarted;
		reader.CrouchCancelledEvent += OnCrouchCancelled;
		reader.SprintStartedEvent += OnSprintStarted;
		reader.SprintCancelledEvent += OnSprintCancelled;
		reader.InteractEvent += OnInteract;
		reader.PauseEvent += OnPause;
	}

	private void UnsubscribeAll(ICharacterInputReader reader)
	{
		reader.MoveEvent -= OnMove;
		reader.LookEvent -= OnLook;
		reader.JumpStartedEvent -= OnJumpStarted;
		reader.JumpCancelledEvent -= OnJumpCancelled;
		reader.AttackStartedEvent -= OnAttackStarted;
		reader.AttackCancelledEvent -= OnAttackCancelled;
		reader.AimStartedEvent -= OnAimStarted;
		reader.AimCancelledEvent -= OnAimCancelled;
		reader.CrouchStartedEvent -= OnCrouchStarted;
		reader.CrouchCancelledEvent -= OnCrouchCancelled;
		reader.SprintStartedEvent -= OnSprintStarted;
		reader.SprintCancelledEvent -= OnSprintCancelled;
		reader.InteractEvent -= OnInteract;
		reader.PauseEvent -= OnPause;
	}

	private void OnMove(Vector2 v) => MoveEvent?.Invoke(v);
	private void OnLook(Vector2 v) => LookEvent?.Invoke(v);
	private void OnJumpStarted() => JumpStartedEvent?.Invoke();
	private void OnJumpCancelled() => JumpCancelledEvent?.Invoke();
	private void OnAttackStarted() => AttackStartedEvent?.Invoke();
	private void OnAttackCancelled() => AttackCancelledEvent?.Invoke();
	private void OnAimStarted() => AimStartedEvent?.Invoke();
	private void OnAimCancelled() => AimCancelledEvent?.Invoke();
	private void OnCrouchStarted() => CrouchStartedEvent?.Invoke();
	private void OnCrouchCancelled() => CrouchCancelledEvent?.Invoke();
	private void OnSprintStarted() => SprintStartedEvent?.Invoke();
	private void OnSprintCancelled() => SprintCancelledEvent?.Invoke();
	private void OnInteract() => InteractEvent?.Invoke();
	private void OnPause() => PauseEvent?.Invoke();

	public void Enable() => _reader.Enable();
	public void Disable() => _reader.Disable();
}

public class UIInputFacade : IUIInputReader
{
	private IUIInputReader _reader;

	public event Action<Vector2> NavigateEvent;
	public event Action SubmitEvent;
	public event Action CancelEvent;

	public UIInputFacade(IUIInputReader reader)
	{
		_reader = reader;
		SubscribeAll(_reader);
	}

	public void Set(IUIInputReader reader)
	{
		UnsubscribeAll(_reader);
		_reader = reader;
		SubscribeAll(_reader);
	}

	private void SubscribeAll(IUIInputReader reader)
	{
		reader.NavigateEvent += OnNavigate;
		reader.SubmitEvent += OnSubmit;
		reader.CancelEvent += OnCancel;
	}

	private void UnsubscribeAll(IUIInputReader reader)
	{
		reader.NavigateEvent -= OnNavigate;
		reader.SubmitEvent -= OnSubmit;
		reader.CancelEvent -= OnCancel;
	}

	private void OnNavigate(Vector2 v) => NavigateEvent?.Invoke(v);
	private void OnSubmit() => SubmitEvent?.Invoke();
	private void OnCancel() => CancelEvent?.Invoke();

	public void Enable() => _reader.Enable();
	public void Disable() => _reader.Disable();
}