
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIInputReader : IUIInputReader, GameInputActions.IUIActions
{
	public event Action<Vector2> NavigateEvent;
	public event Action SubmitEvent;
	public event Action CancelEvent;

	private Action OnEnable;
	private Action OnDisable;

	public UIInputReader(GameInputActions actions)
	{
		OnEnable = () => actions.UI.Enable();
		OnDisable = () => actions.UI.Disable();

		actions.UI.AddCallbacks(this);
	}

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

	public void Enable() => OnEnable?.Invoke();

	public void Disable() => OnDisable?.Invoke();
}