using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;
public class CameraController : MonoBehaviour
{
	[Header("Rig References")]
	[SerializeField] private Transform _target;
	[SerializeField] private Transform _pivot;
	[SerializeField] private Transform _arm;       // child of Pivot
	[SerializeField] private Transform _socket;    // child of Arm
	[SerializeField] private Transform _camera;

	public float cameraMoveSpeed = 5f;
	public float cameraTurnSpeed = 10f;

	[Header("Sensitivity")]
    [SerializeField] private float _mouseYawSensitivity = 0.15f;
    [SerializeField] private float _mousePitchSensitivity = 0.15f;
    [SerializeField] private float _gamepadYawSensitivity = 180f;
    [SerializeField] private float _gamepadPitchSensitivity = 180f;
    [SerializeField] private bool _invertPitch = false;

	[Header("Pitch Limits")]
	[SerializeField] private float _pitchMin = -30f;
	[SerializeField] private float _pitchMax = 30f;


	[Header("Look Target")]
	[SerializeField] private Transform _lookTarget;         // assign e.g. a chest bone, or leave null
	[SerializeField] private float _lookTargetDistance = 50f; // fallback: point this far along arm forward

	private ICharacterInputReader _input;

	// Current yaw and pitch angles (degrees).
	private float _yaw;
	private float _pitch;

    private Vector2 _lookInput;
    private bool _isGamepad;
    // Whether the player is holding the aim button.
    private bool _isAiming;

	// ── Public ─────────────────────────────────────────────────────────────────

	public void Init()
	{
		_camera.position = _socket.position;
		_camera.rotation = _socket.rotation;
	}

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
		Init();
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

    private void Update()
    {
		_camera.SetPositionAndRotation(Vector3.Lerp(_camera.position, _socket.position, cameraMoveSpeed * Time.deltaTime), 
									   Quaternion.Slerp(_camera.rotation, _socket.rotation, cameraTurnSpeed * Time.deltaTime));
    }

    private void LateUpdate()
	{
		transform.position = _target.position;

		Vector2 sensitivity = GetSensitivity();
        _yaw += _lookInput.x * sensitivity.x;
        _pitch += _lookInput.y * sensitivity.y;
        _pitch = Mathf.Clamp(_pitch, _pitchMin, _pitchMax);

        ApplyRotation();
    }

	private Vector2 GetSensitivity()
	{
		return _isGamepad ? new Vector2(_gamepadYawSensitivity, (_invertPitch ? 1f : -1f) * _gamepadPitchSensitivity) * Time.deltaTime :
							new Vector2(_mouseYawSensitivity, (_invertPitch ? 1f : -1f) * _mousePitchSensitivity);
	}

	// ── Private ────────────────────────────────────────────────────────────────

	private void ApplyRotation()
	{
		_pivot.localRotation = Quaternion.Euler(_pitch, _yaw, 0f);
	}


    // ── Input handlers ─────────────────────────────────────────────────────────

    private void OnLook(Vector2 delta)
    {
        _lookInput = delta;
        _isGamepad = InputSystem.GetDevice<Gamepad>()?.wasUpdatedThisFrame ?? false;
    }

    private void OnAimStarted() => _isAiming = true;
	private void OnAimCancelled() => _isAiming = false;
}
