using UnityEngine;
using System;
using System.Collections;

public class MessagePort
{
	private string _action;
	public string Action
	{
		get
		{
			return _action;
		}
	}

	private Action<object> _messageEvent;

	public MessagePort(string action)
	{
		_action = action;
	}

	public void AddEvent(Action<object> func)
	{
		_messageEvent += func;
	}

	public void RemoveEvent(Action<object> func)
	{
		_messageEvent -= func;
	}

	public void PushMessage(object data)
	{
		if (_messageEvent != null)
		{
			_messageEvent(data);
		}
		Debug.Log("PushMessage invoking...");
	}

	public void Dispose()
	{
		_action = null;
		_messageEvent = null;
	}
}
