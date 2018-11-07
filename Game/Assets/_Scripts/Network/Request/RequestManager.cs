using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;

public class RequestManager
{
	private ObjectPool<MessagePack> _msgPackPool;
	private ConcurrentDictionary<string, MessagePack> _recordDict;

	private static RequestManager _instance;
	public static RequestManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new RequestManager();
			}
			
			return _instance;
		}
	}

	public void Initialize()
	{
		_msgPackPool = new ObjectPool<MessagePack>();
		_recordDict = new ConcurrentDictionary<string, MessagePack>();
	}
	
	public void Dispose()
	{
		if (_msgPackPool != null)
		{
			_msgPackPool.Clear();
			_msgPackPool = null;
		}

		_recordDict.Clear();
		_recordDict = null;
	}

	public void SendRequest(string action, Hashtable postData, System.Action<bool, object> callback = null)
	{
		MessagePack msgPack = PopRequest();

		if (msgPack != null)
		{
			msgPack.Action = action;
			msgPack.Callback = callback;
			msgPack.Token = Util.ClientSideUnixUTCMillSeconds.ToString();

			_recordDict.TryAdd(msgPack.Token, msgPack);

			NetworkManager.Instance.SendRequest(action, postData, token: msgPack.Token);
		}
	}

	public System.Action<bool, object> GetRespCallbackByToken(string token)
	{
		MessagePack msgPack;
		_recordDict.TryRemove(token, out msgPack);

		if (msgPack == null) return null;

		return msgPack.Callback;
	}

	public MessagePack PopRequest()
	{
		return _msgPackPool.Allocate();
	}
	
	public void ReleaseRequest(MessagePack msgPack)
	{
		if (msgPack == null) return;
		
		msgPack.Dispose();
		_msgPackPool.Release(msgPack);
	}
}
