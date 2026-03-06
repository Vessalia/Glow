using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputReader", menuName = "Scriptable Objects/InputReader")]
public class InputReader : ScriptableObject, GameInputActions.IPlayerActions, GameInputActions.IUIActions
{
	// ── Player events ──────────────────────────────────────────────────────────
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

	// ── UI events ─────────────────────────────────────────────────────────────
	public event Action<Vector2> NavigateEvent;
	public event Action SubmitEvent;
	public event Action CancelEvent;

	private GameInputActions _actions;

	// ── Lifecycle ─────────────────────────────────────────────────────────────

	private void OnEnable()
	{
		if (_actions == null)
		{
			_actions = new GameInputActions();
			_actions.Player.SetCallbacks(this);
			_actions.UI.SetCallbacks(this);
		}

		EnablePlayerInput();   // start in gameplay mode
	}

	private void OnDisable()
	{
		_actions.Player.Disable();
		_actions.UI.Disable();
	}

	// ── Map switching (call from your GameStateManager / pause logic) ──────────

	public void EnablePlayerInput()
	{
		_actions.Player.Enable();
		_actions.UI.Disable();
	}

	public void EnableUIInput()
	{
		_actions.Player.Disable();
		_actions.UI.Enable();
	}

	// ── IPlayerActions callbacks ───────────────────────────────────────────────

	public void OnMove(InputAction.CallbackContext ctx)
	{
		MoveEvent?.Invoke(ctx.ReadValue<Vector2>());
	}

	public void OnLook(InputAction.CallbackContext ctx)
	{
		LookEvent?.Invoke(ctx.ReadValue<Vector2>());
	}

	public void OnJump(InputAction.CallbackContext ctx)
	{
		if (ctx.started) JumpStartedEvent?.Invoke();
		if (ctx.canceled) JumpCancelledEvent?.Invoke();
	}

	public void OnInteract(InputAction.CallbackContext ctx)
	{
		if (ctx.started) InteractEvent?.Invoke();
	}

	public void OnOpenPause(InputAction.CallbackContext ctx)
	{
		if (ctx.started) PauseEvent?.Invoke();
	}

	public void OnAttack(InputAction.CallbackContext ctx)
	{
		if (ctx.started) AttackStartedEvent?.Invoke();
		if (ctx.canceled) AttackCancelledEvent?.Invoke();
	}

	public void OnCrouch(InputAction.CallbackContext ctx)
	{
		if (ctx.started) { CrouchStartedEvent?.Invoke(); }
		if (ctx.canceled) { CrouchCancelledEvent?.Invoke(); }
	}

	public void OnSprint(InputAction.CallbackContext ctx)
	{
		if (ctx.started) { SprintStartedEvent?.Invoke(); }
		if (ctx.canceled) { SprintCancelledEvent?.Invoke(); }
	}

	public void OnAim(InputAction.CallbackContext ctx)
	{
		if (ctx.started) AimStartedEvent?.Invoke();
		if (ctx.canceled) AimCancelledEvent?.Invoke();
	}

	public void OnPrevious(InputAction.CallbackContext ctx)
	{
		if (ctx.started) CycleEvent?.Invoke(Cycle.Prev);
	}

	public void OnNext(InputAction.CallbackContext ctx)
	{
		if (ctx.started) CycleEvent?.Invoke(Cycle.Next);
	}

	// ── IUIActions callbacks ───────────────────────────────────────────────────

	public void OnNavigate(InputAction.CallbackContext ctx)
	{
		if (ctx.performed) NavigateEvent?.Invoke(ctx.ReadValue<Vector2>());
	}

	public void OnSubmit(InputAction.CallbackContext ctx)
	{
		if (ctx.started) SubmitEvent?.Invoke();
	}

	public void OnCancel(InputAction.CallbackContext ctx)
	{
		if (ctx.started) CancelEvent?.Invoke();
	}
}
