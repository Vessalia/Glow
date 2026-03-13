using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[SerializeField]
	private CharacterMotor _motor;

	// Persistent state — updated by events, consumed each FixedUpdate.
	private Vector2 _moveInput;
	private Vector2 _lookInput;

	private bool _jumpPressed;      // one-frame, cleared after motor consumes it
	private bool _jumpHeld;

	private bool _attackPressed;    // one-frame
	private bool _attackHeld;

	private bool _isCrouching;      // held state
	private bool _isSprinting;      // held state

	private bool _interactPressed;  // one-frame

	private Cycle _cycleTo;       // one-frame

	private bool _isAiming;

	// ── Unity lifecycle ────────────────────────────────────────────────────────

	private void OnEnable()
	{
		var input = InputManager.Instance.GetPlayerInput();
        input.MoveEvent += OnMove;
		input.LookEvent += OnLook;

		input.JumpStartedEvent += OnJumpStarted;
		input.JumpCancelledEvent += OnJumpCancelled;

		input.AttackStartedEvent += OnAttackStarted;
		input.AttackCancelledEvent += OnAttackCancelled;

		input.CrouchStartedEvent += OnCrouchStarted;
		input.CrouchCancelledEvent += OnCrouchCancelled;

		input.SprintStartedEvent += OnSprintStarted;
		input.SprintCancelledEvent += OnSprintCancelled;

		input.InteractEvent += OnInteract;
		input.CycleEvent += OnCycle;

		input.AimStartedEvent += OnAimStarted;
		input.AimCancelledEvent += OnAimCancelled;
	}

	private void OnDisable()
	{
		var input = InputManager.Instance.GetPlayerInput();
        input.MoveEvent -= OnMove;
		input.LookEvent -= OnLook;

		input.JumpStartedEvent -= OnJumpStarted;
		input.JumpCancelledEvent -= OnJumpCancelled;

		input.AttackStartedEvent -= OnAttackStarted;
		input.AttackCancelledEvent -= OnAttackCancelled;

		input.CrouchStartedEvent -= OnCrouchStarted;
		input.CrouchCancelledEvent -= OnCrouchCancelled;

		input.SprintStartedEvent -= OnSprintStarted;
		input.SprintCancelledEvent -= OnSprintCancelled;

		input.InteractEvent -= OnInteract;
		input.CycleEvent -= OnCycle;

		input.AimStartedEvent -= OnAimStarted;
		input.AimCancelledEvent -= OnAimCancelled;
	}

	private void FixedUpdate()
	{
		var intent = new CharacterIntent
		{
			MoveDirection = _moveInput,
			LookDelta = _lookInput,
			JumpPressed = _jumpPressed,
			JumpHeld = _jumpHeld,
			AttackPressed = _attackPressed,
			AttackHeld = _attackHeld,
			IsCrouching = _isCrouching,
			IsSprinting = _isSprinting,
			InteractPressed = _interactPressed,
			CycleTo = _cycleTo,
			IsAiming = _isAiming,
		};

		_motor.ApplyIntent(intent);

		// Clear one-frame flags after the motor has consumed them.
		_jumpPressed = false;
		_attackPressed = false;
		_interactPressed = false;
		_cycleTo = Cycle.None;
    }

	// ── Event handlers ─────────────────────────────────────────────────────────

	private void OnMove(Vector2 value) => _moveInput = value;
	private void OnLook(Vector2 value) => _lookInput = value;

	private void OnJumpStarted() { _jumpPressed = true; _jumpHeld = true; }
	private void OnJumpCancelled() => _jumpHeld = false;

	private void OnAttackStarted() { _attackPressed = true; _attackHeld = true; }
	private void OnAttackCancelled() => _attackHeld = false;

	private void OnCrouchStarted() => _isCrouching = true;
	private void OnCrouchCancelled() => _isCrouching = false;

	private void OnSprintStarted() => _isSprinting = true;
	private void OnSprintCancelled() => _isSprinting = false;

	private void OnInteract() => _interactPressed = true;
	private void OnCycle(Cycle cycle) => _cycleTo = cycle;

	private void OnAimStarted() => _isAiming = true;
	private void OnAimCancelled() => _isAiming = false;
}
