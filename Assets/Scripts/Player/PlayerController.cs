using UnityEngine;
using UnityEngine.UIElements.Experimental;

namespace Assets.Scripts.Player
{
	public class PlayerController : MonoBehaviour
	{
		[SerializeField] private Animator animator;
		[SerializeField] private CharacterController cc;

		private PlayerIntent intent;
		[SerializeField] public PlayerMotor motor;


		[SerializeField] private Light _lanternLight;
		[SerializeField, Range(0f, 1f)] private float _lanternIntensityMin;
		[SerializeField, Range(0f, 1f)] private float _lanternIntensityMax;

		[SerializeField, Min(0f)] private float _lanternDistanceMin;
		[SerializeField, Min(0f)] private float _lanternDistanceMax;

		[SerializeField, Min(0f)] private float _lanternIntensityTime;

		private float _lanternIntensityTimer = 0;

		void Awake()
		{
			intent = new(motor);
		}

		void Update()
		{
			intent.Tick();
			motor.Tick(intent, cc, animator);
			intent.LateTick();

			AdjustLanternIntensity(intent.LanternRaised);
		}

		private void AdjustLanternIntensity(bool lanternRaised)
		{
			float dt = Time.deltaTime * (1.0f / _lanternIntensityTime);
			if (intent.LanternRaised)
				_lanternIntensityTimer += dt;
			else
				_lanternIntensityTimer -= dt;

			_lanternIntensityTimer = Mathf.Clamp01(_lanternIntensityTimer);
			float t = Easing.InCubic(_lanternIntensityTimer);

			_lanternLight.intensity = Mathf.Lerp(_lanternIntensityMin, _lanternIntensityMax, t);
			_lanternLight.range = Mathf.Lerp(_lanternDistanceMin, _lanternDistanceMax, t);
		}

		void OnDestroy()
		{
			intent.Dispose();
		}
	}
}
