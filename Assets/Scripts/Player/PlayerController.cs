using UnityEngine;

namespace Assets.Scripts.Player
{
	public class PlayerController : MonoBehaviour
	{
		[SerializeField] private Animator animator;
		[SerializeField] private CharacterController cc;

		private PlayerIntent intent;
		[SerializeField] public PlayerMotor motor;


		void Awake()
		{
			intent = new(motor);
		}

		void Update()
		{
			intent.Tick();
			motor.Tick(intent, cc, animator);
			intent.LateTick();
		}

		void OnDestroy()
		{
			intent.Dispose();
		}
	}
}
