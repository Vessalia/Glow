using System;
using UnityEngine;

namespace Assets.Scripts.Player
{
	public class PlayerIntent : IDisposable
	{
		public Vector2 Move { get; private set; }
		public Vector2 Look { get; private set; }

		public bool RunHeld { get; private set; }

		public bool Jump { get; private set; }
		public bool JumpHeld { get; private set; }

		public bool LanternRaised { get; private set; }

		public bool CanUseCoyote => coyoteTimer > 0f;

		private PlayerMotor motor;

		[Header("Jump Feel")]
		public float coyoteTime = 0.1f;
		public float jumpBufferTime = 0.1f;

		private float coyoteTimer;
		private float jumpBufferTimer;
		private bool jumpConsumed;


		public PlayerIntent(PlayerMotor motor)
		{
			this.motor = motor;

			var _input = InputManager.Instance.GetPlayerInput();
			_input.MoveEvent += OnMove;
			_input.LookEvent += OnLook;
			_input.SprintStartedEvent += OnRunStart;
			_input.SprintCancelledEvent += OnRunEnd;
			_input.AimStartedEvent += RaiseLantern;
			_input.AimCancelledEvent += LowerLantern;
		}

		~PlayerIntent() => Dispose();

		public void Dispose()
		{
			var _input = InputManager.Instance.GetPlayerInput();
			_input.MoveEvent -= OnMove;
			_input.LookEvent -= OnLook;
			_input.SprintStartedEvent -= OnRunStart;
			_input.SprintCancelledEvent -= OnRunEnd;
		}

		public void Tick()
		{
			ReadRawInput();
			UpdateJumpIntent();
		}

		public void LateTick()
		{
			Jump = false;
		}

		void ReadRawInput()
		{
			//var moveAction = InputSystem.actions.FindAction("Move");
			//var lookAction = InputSystem.actions.FindAction("Look");
			//var jumpAction = InputSystem.actions.FindAction("Jump");
			//var lockOnAction = InputSystem.actions.FindAction("LockOn");

			//Move = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;
			//Look = lookAction != null ? lookAction.ReadValue<Vector2>() : Vector2.zero;

			//bool jumpPressedRaw = jumpAction != null && jumpAction.WasPressedThisFrame();
			//JumpHeld = jumpAction != null && jumpAction.IsPressed();

			//if (jumpPressedRaw)
			//	jumpBufferTimer = jumpBufferTime;
		}

		void UpdateJumpIntent()
		{
			float dt = Time.deltaTime;

			if (motor != null && motor.IsGrounded)
			{
				coyoteTimer = coyoteTime;
				jumpConsumed = false;
			}
			else
			{
				coyoteTimer -= dt;
			}

			jumpBufferTimer -= dt;


			bool hasBufferedJump = jumpBufferTimer > 0f;

			if (!jumpConsumed && CanUseCoyote && hasBufferedJump)
			{
				Jump = true;
				jumpConsumed = true;
				jumpBufferTimer = 0f;
			}
		}

		private void OnMove(Vector2 move) => Move = move;
		private void OnLook(Vector2 look) => Look = look;

		private void OnRunStart() => RunHeld = true;
		private void OnRunEnd() => RunHeld = false;

		private void RaiseLantern() => LanternRaised = true;
		private void LowerLantern() => LanternRaised = false;
	}
}
