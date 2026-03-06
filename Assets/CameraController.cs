using UnityEngine;


public class CameraController : MonoBehaviour
{
	[Header("Rig References")]
	[SerializeField] private Transform _pivot;     // child of Player, yaws
	[SerializeField] private Transform _arm;       // child of Pivot, pitches
	[SerializeField] private Transform _socket;    // child of Arm, holds offset
	[SerializeField] private Camera _camera;       // child of Socket

	[Header("Sensitivity")]
	[SerializeField] private float _yawSensitivity = 0.15f;
	[SerializeField] private float _pitchSensitivity = 0.15f;
	[SerializeField] private bool _invertPitch = false;

	[Header("Pitch Limits")]
	[SerializeField] private float _pitchMin = -40f;
	[SerializeField] private float _pitchMax = 70f;

	[Header("Collision")]
	[SerializeField] private float _collisionRadius = 0.2f;
	[SerializeField] private LayerMask _collisionMask;

	[Header("Look Target")]
	[SerializeField] private Transform _lookTarget;         // assign e.g. a chest bone, or leave null
	[SerializeField] private float _lookTargetDistance = 50f; // fallback: point this far along arm forward

	[Header("Aim")]
	[SerializeField] private Vector3 _defaultOffset = new Vector3(0.6f, 0f, -3.0f);
	[SerializeField] private Vector3 _aimOffset = new Vector3(0.3f, 0f, -1.5f);
	[SerializeField] private float _aimLerpSpeed = 10f;

	private ICharacterInputReader _input;

	// Current yaw and pitch angles (degrees).
	private float _yaw;
	private float _pitch;

	// Whether the player is holding the aim button.
	private bool _isAiming;

	// ── Public ─────────────────────────────────────────────────────────────────

	/// World-space point the camera is looking at. Expose this so other systems
	/// (enemy detection, aim reticle, hit-scan) can query it.
	public Vector3 LookPoint
	{
		get
		{
			if (_lookTarget != null) return _lookTarget.position;
			return _arm.position + _arm.forward * _lookTargetDistance;
		}
	}

	/// World-space forward direction of the camera, flattened to XZ.
	/// Feed this into CharacterMotor.ApplyIntent so movement is camera-relative.
	public Vector3 CameraForward
	{
		get
		{
			Vector3 flat = _arm.forward;
			flat.y = 0f;
			return flat.sqrMagnitude > 0.001f ? flat.normalized : transform.forward;
		}
	}

	public bool IsAiming => _isAiming;

	// ── Lifecycle ──────────────────────────────────────────────────────────────

	private void Awake()
	{
		// Initialise yaw from the character's current facing so the camera
		// doesn't snap on the first frame.
		_yaw = transform.eulerAngles.y;
		
	}

	private void OnEnable()
	{
		_input = InputManager.Instance.GetPlayerInput();

		_input.LookEvent += OnLook;
		_input.AimStartedEvent += OnAimStarted;
		_input.AimCancelledEvent += OnAimCancelled;
	}

	private void OnDisable()
	{
		_input.LookEvent -= OnLook;
		_input.AimStartedEvent -= OnAimStarted;
		_input.AimCancelledEvent -= OnAimCancelled;
	}

	private void LateUpdate()
	{
		// Run in LateUpdate so the camera settles after the motor has moved.
		ApplyRotation();
		ApplyOffset();
		ApplyCollision();
	}

	// ── Private ────────────────────────────────────────────────────────────────

	private void ApplyRotation()
	{
		// Pivot yaws in world space — it stays at the character's root position
		// (parented to Player) so it automatically follows the character.
		_pivot.rotation = Quaternion.Euler(0f, _yaw, 0f);

		// Arm pitches in local space relative to the pivot.
		_arm.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
	}

	private void ApplyOffset()
	{
		// Lerp the socket's local position between default and aim offsets.
		Vector3 targetOffset = _isAiming ? _aimOffset : _defaultOffset;
		_socket.localPosition = Vector3.Lerp(
			_socket.localPosition,
			targetOffset,
			_aimLerpSpeed * Time.deltaTime
		);
	}

	private void ApplyCollision()
	{
		Vector3 desiredPos = _socket.position;
		Vector3 origin = _arm.position;
		Vector3 direction = (desiredPos - origin).normalized;
		float desiredDist = Vector3.Distance(origin, desiredPos);

		Vector3 finalCameraPos;
		if (Physics.SphereCast(origin, _collisionRadius, direction,
							   out RaycastHit hit, desiredDist, _collisionMask))
		{
			finalCameraPos = origin + direction * (hit.distance - _collisionRadius);
		}
		else
		{
			finalCameraPos = desiredPos;
		}

		_camera.transform.position = finalCameraPos;

		// Orient the camera toward the look point rather than just copying the
		// arm's rotation — this is what actually makes the camera feel grounded.
		Vector3 toTarget = LookPoint - finalCameraPos;
		if (toTarget.sqrMagnitude > 0.001f)
			_camera.transform.rotation = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
	}

	// ── Input handlers ─────────────────────────────────────────────────────────

	private void OnLook(Vector2 delta)
	{
		_yaw += delta.x * _yawSensitivity;
		_pitch += delta.y * _pitchSensitivity * (_invertPitch ? 1f : -1f);
		_pitch = Mathf.Clamp(_pitch, _pitchMin, _pitchMax);
	}

	private void OnAimStarted() => _isAiming = true;
	private void OnAimCancelled() => _isAiming = false;
}
