
public interface ITransition
{
	public IState NextState { get; }
	public IPredicate Condition { get; }
}

public class Transition : ITransition
{
	public IState NextState { get; }

	public IPredicate Condition { get; }

	public Transition(IState nextState, IPredicate condition)
	{
		NextState = nextState;
		Condition = condition;
	}
}
