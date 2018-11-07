using UnityEngine;
using System.Collections;

public class BaseState : IState
{
	public virtual string Name
	{
		get
		{
			return this.ToString();
		}
	}

	private object _data;
	public virtual object Data
	{
		get
		{
			return _data;
		}
		set
		{
			_data = value;
		}
	}

	private StateMachine _currentStateMachine;
	public virtual StateMachine CurrentStateMachine
	{
		get
		{
			return _currentStateMachine;
		}
		set
		{
			_currentStateMachine = value;
		}
	}

	public virtual void OnEnter()
	{
		Debug.Log(Name + ": OnEnter");
	}

	public virtual void OnProcess()
	{
		Debug.Log(Name + ": OnProcess");
	}

	public virtual void OnExit()
	{
		Debug.Log(Name + ": OnExit");
	}

	public virtual void Dispose()
	{
		_data = null;
	}

	protected virtual void MoveNext()
	{
		if (_currentStateMachine != null)
		{
			_currentStateMachine.MoveNext();
		}
	}

	protected virtual void Wait()
	{
		if (_currentStateMachine != null)
		{
			_currentStateMachine.Wait();
		}
	}

	protected virtual void Notify()
	{
		if (_currentStateMachine != null)
		{
			_currentStateMachine.Notify();
		}
	}

	protected virtual IEnumerator MoveNext(float delay, System.Action callback = null)
	{
		yield return null;
		yield return new WaitForSeconds(delay);

		if (callback != null)
		{
			callback();
		}

		MoveNext();
	}

	protected virtual void WaitForMoveNext(float delay, System.Action callback = null)
	{
		Util.StartCoroutine(MoveNext(delay, callback));
	}
}
