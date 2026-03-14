using JetBrains.Annotations;
using System;
using UnityEngine;

namespace Assets.Scripts.Player
{
	[Serializable]
	public class PlayerMotor
	{
		[Header("Jump Parameters")]
		[Range(1, 50)]
		public float maxJumpHeight = 10; // how many world units high we can jump

		[Range(1f, 100f)]
		public float maxJumpDistance = 10f; // horizontal distance traveled until jump peak

		[Range(0.01f, 1f)]
		public float timeToMaxSpeedGround = 0.1f; // how long does it take to reach max speed

		public float Acceleration => MovementSpeed / timeToMaxSpeedGround;

		public float TimeToJumpPeak => maxJumpDistance / (MovementSpeed * (1f + 1f / Mathf.Sqrt(fallGravityScalar)));
		public float Gravity => (-2.0f * maxJumpHeight) / (TimeToJumpPeak * TimeToJumpPeak);
		public float InitialJumpVelocity => (2.0f * maxJumpHeight) / TimeToJumpPeak;

		[Header("Movement")]
		public float MovementSpeed = 6f;
		public float RotationSpeed = 720f;

		public float WalkSpeed => MovementSpeed / 2f;

		[SerializeField]
		[Range(0.5f, 10f)]
		public float fallGravityScalar = 1f;  // how intense we want gravity to scale once we are past the peak of the jump
		public float FallGravity => Gravity * fallGravityScalar;

		[SerializeField]
		public Transform cameraTransform;

		private Vector3 velocity;

		public bool IsGrounded { get; private set; }

		public void Tick(PlayerIntent intent, CharacterController cc, Animator animator)
		{
			Vector2 moveInput = intent.Move;
			bool jumpPressed = intent.Jump;

			Vector3 moveDir = CalculateMoveDirection(moveInput);
			ResolveVerticalVelocity(jumpPressed, intent);

			Vector2 velocityXZ = velocity.xz();
			velocityXZ = ResolveHorizontalVelocity(intent.RunHeld, moveDir, moveInput, velocityXZ);
			ResolveRotation(moveDir, cc.transform);

			velocity = new Vector3(velocityXZ.x, velocity.y, velocityXZ.y);
			IsGrounded = cc.isGrounded;

			UpdateAnimator(animator, intent.RunHeld, intent.LanternRaised);
			cc.Move(velocity * Time.deltaTime);
		}

		private void UpdateAnimator(Animator animator, bool isRunning, bool lanternRaised)
		{
			animator.SetFloat("Speed", velocity.xz().magnitude);
			animator.SetBool("IsRunning", isRunning);

			int lanternLayer = animator.GetLayerIndex("Lantern");
			animator.SetBool("IsLanternRaised", lanternRaised);
		}

		private Vector3 CalculateMoveDirection(Vector2 moveInput)
		{
			Vector3 input = new Vector3(moveInput.x, 0f, moveInput.y);
			if (input.sqrMagnitude <= 0.001f)
				return Vector3.zero;

			float yaw = cameraTransform.eulerAngles.y;
			Quaternion planeRotation = Quaternion.Euler(0f, yaw, 0f);

			Vector3 camForward = planeRotation * Vector3.forward;
			Vector3 camRight = planeRotation * Vector3.right;

			Vector3 moveDir = camForward * input.z + camRight * input.x;
			moveDir.Normalize();
			return moveDir;
		}

		private void ResolveRotation(Vector3 moveDir, Transform transform)
		{
			float effectiveRotationSpeed = RotationSpeed;

			if (moveDir.sqrMagnitude > 0.001f)
			{
				transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(moveDir), effectiveRotationSpeed * Time.deltaTime);
			}
		}

		private void ResolveVerticalVelocity(bool jumpPressed, PlayerIntent intent)
		{
			if (jumpPressed && (IsGrounded || intent.CanUseCoyote))
			{
				velocity.y = InitialJumpVelocity;
				return;
			}

			if (IsGrounded)
			{
				velocity.y = -1f;
			}
			else
			{
				float gravity = velocity.y > 0 ? Gravity : FallGravity;
				velocity.y += gravity * Time.deltaTime;
			}
		}

		private Vector2 ResolveHorizontalVelocity(bool isRunning, Vector3 moveDir, Vector2 moveInput, Vector2 velocityXZ)
		{
			float effectiveMovementSpeed = isRunning ? MovementSpeed : WalkSpeed;

			velocityXZ = ApplyGroundMovement(moveDir, velocityXZ, effectiveMovementSpeed);
			velocityXZ = Vector3.ClampMagnitude(velocityXZ, moveInput.magnitude * effectiveMovementSpeed);

			return velocityXZ;
		}

		private Vector2 ApplyGroundMovement(Vector3 moveDir, Vector2 velocityXZ, float effectiveMovementSpeed)
		{
			if (moveDir.sqrMagnitude > 0f)
			{
				velocityXZ = Vector2.MoveTowards(velocityXZ, moveDir.xz() * effectiveMovementSpeed, Acceleration * Time.deltaTime);
			}
			else
			{
				float accelerationScale = 1f / 4f;
				float magnitude = Mathf.MoveTowards(velocityXZ.magnitude, 0f, Acceleration * accelerationScale * Time.deltaTime);
				velocityXZ = velocityXZ.normalized * magnitude;
			}

			return velocityXZ;
		}
	}
}
