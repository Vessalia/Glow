using UnityEngine;

public class EscortController : MonoBehaviour
{
	[SerializeField]
	private CharacterMotor _motor;

	private StateMachine _brain;

	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		var intent = new CharacterIntent();

		_motor.ApplyIntent(intent);
	}
}
