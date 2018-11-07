using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class StateMachine
{
	private bool _wait;
	private bool _executed;
	private IState _current;
	private Dictionary<string, IState> _states;
	private SubState _nextSubState;

	public IState CurrentState
	{
		get { return _current; }
	}

	public StateMachine()
	{
		_current = null;
		_states = new Dictionary<string, IState>();

		Oscillator.Instance.UpdateEvent += OnTimeUpdateEvent;
	}

	public void Dispose()
	{
		if (_current != null)
		{
			_current.OnExit();
		}

		if (_states != null)
		{
			foreach (KeyValuePair<string, IState> kv in _states)
		    {
				kv.Value.Dispose();
			}

			_states.Clear();
			_states = null;
		}

		_current = null;
		_nextSubState = SubState.INVALID;
		_wait = false;
		_executed = false;

		Oscillator.Instance.UpdateEvent -= OnTimeUpdateEvent;
	}

	public void RegisterState(IState state)
	{
		if (state != null && !_states.ContainsKey(state.Name))	
		{
			state.CurrentStateMachine = this;
			_states.Add(state.Name, state);
		}
		else
		{
			Debug.Log("RegisterState::Error: " + (state != null ? state.Name : "state is null"));
		}
	}

    /// <summary>
    /// Start the state machine
    /// </summary>
    public void Start()
    {
        MoveNext();
    }

	public void MoveNext(IState state, object data = null)
	{
		if (state != null)
		{
			MoveNext(state.Name, data);
		}
	}

	public void MoveNext(string name, object data = null)
	{
		if (_wait || !_states.ContainsKey(name)) return;

		if (_current != null)
		{
			if (_current.Name == name)
			{
				return;
			}

			_current.OnExit();
		}

		_current = _states[name] as IState;
		_current.Data = data;

        EnterStateHandler();
	}

	public void MoveNext()
	{
		if (_wait || _states.Count <= 0) return;

		bool stateChanged = false;

		if (_current == null)
		{
			_current = _states.Values.First();
			stateChanged = true;
		}
		else if (_current == _states.Values.Last())
		{
			Dispose();
		}
		else
		{
			_current.OnExit();

			for (var v = _states.Values.GetEnumerator(); v.MoveNext();)
			{
				if (_current == v.Current)
				{
					if (v.MoveNext())
					{
						_current = v.Current;
						stateChanged = true;
					}

					break;
				}
			}
		}

		if (!stateChanged) return;

        EnterStateHandler();
	}

	public void ResetState(IState state, object data)
	{
		if (state != null)
		{
			ResetState(state.Name, data);
		}
	}

	public void ResetState(string name, object data)
	{
		if (_wait)
		{
			Debug.Log("State::Waited");
			return;
		}
		
		if (!_states.ContainsKey(name))
		{
			return;
		}

		IState state = _states[name] as IState;
		if (state != null)
		{
			state.Data = data;
		}
	}

    /// <summary>
    /// Invoke Notify at the same sub-state while Wait function invoked.
    /// </summary>
	public void Notify()
	{
		_wait = false;
	}

    /// <summary>
    /// Wait the current state/sub-state until Notify invoked.
    /// </summary>
	public void Wait()
	{
		_wait = true;
	}

    private void EnterStateHandler()
    {
        if (!_wait)
        {
            _current.OnEnter();
            _nextSubState = SubState.PROCESS;
        }
        else
        {
            _nextSubState = SubState.ENTER;
        }
    }

	private void OnTimeUpdateEvent(double time)
	{
        if (_wait) return;

        if (_current != null && !_executed)
		{
            _executed = true;

            switch (_nextSubState)
            {
                case SubState.ENTER:
                    _nextSubState = SubState.PROCESS;
                    _current.OnEnter();
                    break;

                case SubState.PROCESS:
                    _nextSubState = SubState.EXIT;
                    _current.OnProcess();
                    break;

                case SubState.EXIT:
                    _nextSubState = SubState.INVALID;
                    MoveNext();
                    break;
            }

            _executed = false;
		}
	}

	protected enum SubState
	{
		ENTER,
		PROCESS,
		EXIT,
		INVALID,
	}
}
