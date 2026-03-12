using System;
using Unity.VisualScripting;
using UnityEngine;


[Serializable]
public class CharacterMotor
{
	[Header("Movement")]
	[SerializeField] private float _moveSpeed = 6f;
	[SerializeField] private float _acceleration = 20f;
	[SerializeField] private float _gravity = -20f;

	[Header("Jumping")]
	[SerializeField] private float _jumpHeight = 1.8f;
	[SerializeField] private float _jumpHoldBonus = 4f;   // extra upward force while held
	[SerializeField] private float _jumpHoldMaxTime = 0.25f;

	[Header("Looking")]
	[SerializeField] private Transform _cam;

	[SerializeField]
	private CharacterController _cc;

	private Vector3 _velocity;          // accumulated velocity (world space)
	private float _currentYVelocity;
	private float _jumpHoldTimer;
	private bool _isJumping;

	// ── Public API ─────────────────────────────────────────────────────────────

	/// Called by PlayerController or an AI brain each FixedUpdate.
	public void ApplyIntent(CharacterIntent intent)
	{
		HandleMovement(intent.MoveDirection);
		HandleJump(intent.JumpPressed, intent.JumpHeld);
		ApplyGravity();
		ApplyVelocity();
		HandleInteract(intent.InteractPressed);
	}

	// ── Private movement helpers ───────────────────────────────────────────────

	private void HandleMovement(Vector2 moveDir)
	{
		Vector3 worldMove = Vector3.zero;
		if (_cam != null)
		{
			Vector3 camRight = _cam.right;
			camRight.y = 0;
			camRight.Normalize();

			Vector3 camForward = _cam.forward;
			camForward.y = 0;
			camForward.Normalize();

			worldMove = camRight * moveDir.x + camForward * moveDir.y;
		}
		else
			worldMove = _cc.transform.right * moveDir.x + _cc.transform.forward * moveDir.y;

		worldMove = Vector3.ClampMagnitude(worldMove, 1f) * _moveSpeed;
		// Smooth acceleration.
		_velocity.x = Mathf.MoveTowards(_velocity.x, worldMove.x, _acceleration * Time.fixedDeltaTime);
		_velocity.z = Mathf.MoveTowards(_velocity.z, worldMove.z, _acceleration * Time.fixedDeltaTime);
	}

	private void HandleJump(bool pressed, bool held)
	{
		if (_cc.isGrounded)
		{
			_currentYVelocity = -2f; // keep grounded firmly
			_isJumping = false;
		}

		if (pressed && _cc.isGrounded)
		{
			// v = sqrt(2 * |gravity| * h)
			_currentYVelocity = Mathf.Sqrt(2f * Mathf.Abs(_gravity) * _jumpHeight);
			_jumpHoldTimer = _jumpHoldMaxTime;
			_isJumping = true;
		}

		// Variable height: keep adding upward force while button held.
		if (_isJumping && held && _jumpHoldTimer > 0f)
		{
			_currentYVelocity += _jumpHoldBonus * Time.fixedDeltaTime;
			_jumpHoldTimer -= Time.fixedDeltaTime;
		}
		else if (!held)
		{
			_jumpHoldTimer = 0f;
		}
	}

	private void ApplyGravity()
	{
		if (!_cc.isGrounded)
			_currentYVelocity += _gravity * Time.fixedDeltaTime;

		_velocity.y = _currentYVelocity;
	}

	private void ApplyVelocity()
	{
		_cc.Move(_velocity * Time.fixedDeltaTime);
	}

	private void HandleInteract(bool pressed)
	{
		if (!pressed) return;

		// Simple sphere-cast for interactables in front of the player.
		if (Physics.SphereCast(_cc.transform.position, 0.5f, _cc.transform.forward,
							   out RaycastHit hit, 2f))
		{
			hit.collider.GetComponent<IInteractable>()?.Interact(_cc.gameObject);
		}
	}

	// ── Utility ────────────────────────────────────────────────────────────────

	private static float ClampAngle(float angle, float min, float max)
	{
		if (angle > 180f) angle -= 360f;
		return Mathf.Clamp(angle, min, max);
	}
}

/// Anything in the world that can be interacted with implements this.
public interface IInteractable
{
	void Interact(GameObject instigator);
}
