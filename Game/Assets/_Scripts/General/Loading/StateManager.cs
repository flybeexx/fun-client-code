using UnityEngine;
using System.Collections;

public class StateManager
{
	private StateMachine _stateMachine;
	public StateMachine StateMachine
	{
		get
		{
			return _stateMachine;
		}
	}

	private static StateManager _instance;
	public static StateManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new StateManager();
			}

			return _instance;
		}
	}

	public void Initialize()
	{
		RegisterStates();

		// show loading screen here
	}

	public void Dispose()
	{
		if (_stateMachine != null)
		{
			_stateMachine.Dispose();
			_stateMachine = null;
		}
	}

	private void RegisterStates()
	{
		_stateMachine = new StateMachine();

		_stateMachine.RegisterState(new InitializeState());
		_stateMachine.RegisterState(new GameVersionState());
		_stateMachine.RegisterState(new LoadConfigState());
		_stateMachine.RegisterState(new LoadBundleState());
		_stateMachine.RegisterState(new InitUserDataState());
		_stateMachine.RegisterState(new PreopenBundleUpdateState());
		_stateMachine.RegisterState(new OTABundleUpdateState());

        _stateMachine.Start();
	}
}
