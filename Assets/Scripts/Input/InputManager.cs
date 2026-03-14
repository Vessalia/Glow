
using UnityEngine;

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

	private CharacterInputFacade _playerInput;
	private UIInputFacade _uiInput;

	public InputManager(ICharacterInputReader playerInput, IUIInputReader uiInput)
	{
		_playerInput = new(playerInput);
		_uiInput = new(uiInput);

		EnablePlayerInput();
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}

	public void SetPlayerInput(ICharacterInputReader playerInput) => _playerInput.Set(playerInput);
	public void SetUIInput(IUIInputReader uiInput) => _uiInput.Set(uiInput);

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
