
public class InputManager
{
	private static GameInputActions _actions;
	private static InputManager _instance;
	public static InputManager Instance
	{
		get
		{
			if( _actions == null)
				_actions = new GameInputActions();

			if (_instance == null)
				_instance = new InputManager(new PlayerInputReader(_actions), new UIInputReader(_actions));
			
			return _instance;
		}
	}

	private ICharacterInputReader _playerInput;
	private IUIInputReader _uiInput;

	public InputManager(ICharacterInputReader playerInput, IUIInputReader uiInput)
	{
		this._playerInput = playerInput;
		this._uiInput = uiInput;

		EnablePlayerInput();
	}

	public void SetPlayerInput(ICharacterInputReader playerInput) => _playerInput = playerInput;
	public void SetUIInput(IUIInputReader uiInput) => _uiInput = uiInput;

	public ICharacterInputReader GetPlayerInput() => _playerInput;
	public IUIInputReader GetUIInput() => _uiInput;

	public void EnablePlayerInput()
	{
		_uiInput.Disable();
		_playerInput.Enable();
	}

	public void EnableUIInput()
	{
		_playerInput.Disable();
		_uiInput.Enable();
	}
}
