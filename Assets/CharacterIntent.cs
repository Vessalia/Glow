using UnityEngine;

public enum Cycle
{
	Prev = -1,
	None,
	Next
}

public struct CharacterIntent
{
	public Vector2 MoveDirection;
	public Vector2 LookDelta;

	public bool JumpPressed;
	public bool JumpHeld;

	public bool InteractPressed;
	public bool AttackPressed;
	public bool AttackHeld;   
	
	public bool IsCrouching;
	public bool IsSprinting;
	
	public Cycle CycleTo;

	public static readonly CharacterIntent Idle = new CharacterIntent();
}
