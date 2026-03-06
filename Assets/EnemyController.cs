using UnityEngine;

[RequireComponent(typeof(CharacterMotor))]
public class EnemyController : MonoBehaviour
{
	[SerializeField] private float _chaseRange = 12f;
	[SerializeField] private float _attackRange = 1.8f;
	[SerializeField] private Transform _target;

	private CharacterMotor _motor;

	private void Awake()
	{
		_motor = GetComponent<CharacterMotor>();
	}

	private void Start()
	{
		if (_target == null)
		{
			var player = GameObject.FindGameObjectWithTag("Player");
			_target = player.transform;
		}
	}

	private void FixedUpdate()
	{
		if (_target == null)
		{
			_motor.ApplyIntent(CharacterIntent.Idle);
			return;
		}

		float dist = Vector3.Distance(transform.position, _target.position);

		var intent = new CharacterIntent();

		if (dist <= _attackRange)
		{
			intent.AttackPressed = true;
		}
		else if (dist <= _chaseRange)
		{
			// Move toward player in local space.
			Vector3 toTarget = (_target.position - transform.position).normalized;
			// Express in the motor's local frame (forward = +Z, right = +X).
			intent.MoveDirection = new Vector2(
				Vector3.Dot(toTarget, transform.right),
				Vector3.Dot(toTarget, transform.forward)
			);
		}

		_motor.ApplyIntent(intent);
	}
}
