using Mono.Cecil.Cil;
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

		[SerializeField, Range(1f, 10f)] private float _lanternInteractionRange;

		[SerializeField] private Collider _lanternInteraction;


		private float _lanternIntensityTimer = 0;

		private void OnValidate()
		{
			if (_lanternInteraction != null)
			{
				var pos = Vector3.zero;
				pos.z = _lanternInteractionRange / 2;
				_lanternInteraction.transform.localPosition = pos;
				_lanternInteraction.transform.localScale = new Vector3(1, 1, _lanternInteractionRange);
			}
		}

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

			if (t >= 1 && !_lanternInteraction.gameObject.activeSelf)
				_lanternInteraction.gameObject.SetActive(true);
			else if (t < 1 && _lanternInteraction.gameObject.activeSelf)
				_lanternInteraction.gameObject.SetActive(false);
		}

		void OnDestroy()
		{
			intent.Dispose();
		}
	}
}
