using UnityEngine;
using System;
using System.Collections;

public class MessagePack
{
	private string _action;
	public string Action
	{
		get { return _action; }
		set { _action = value; }
	}

	private string _token;
	public string Token
	{
		get { return _token; }
		set { _token = value; }
	}

	private object _data;
	public object Data
	{
		get { return _data; }
		set { _data = value; }
	}

	private object _payload;
	public object Payload
	{
		get { return _payload; }
		set { _payload = value; }
	}

	private long _timeStamp;
	public long TimeStamp
	{
		get { return _timeStamp; }
		set { _timeStamp = value; }
	}

	private bool _return;
	public bool Return
	{
		get { return _return; }
		set { _return = value; }
	}

	private Action<bool, object> _callback;
	public Action<bool, object> Callback
	{
		get { return _callback; }
		set { _callback = value; }
	}
	
	public void Dispose()
	{
		_timeStamp = 0;
		_data = null;
		_action = null;
		_payload = null;

		_return = false;
		_callback = null;
	}

	public struct Type
	{
		public const string NORMAL = "1";
		public const string MAINTENANCE = "2";
	}
}
