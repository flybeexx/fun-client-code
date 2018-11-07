using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;

public class MessageManager
{
	private ObjectPool<MessagePack> _msgPackPool;
	private ConcurrentQueue<MessagePack> _recordQueue;
	private ConcurrentDictionary<string, MessagePort> _eventDict;

	private static MessageManager _instance;
	public static MessageManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new MessageManager();
			}

			return _instance;
		}
	}

	public void Initialize()
	{
		Oscillator.Instance.UpdateEvent += OnMessageProcess;

		_msgPackPool = new ObjectPool<MessagePack>();
		_recordQueue = new ConcurrentQueue<MessagePack>();
		_eventDict = new ConcurrentDictionary<string, MessagePort>();
	}

	public void Dispose()
	{
		Oscillator.Instance.UpdateEvent -= OnMessageProcess;

		if (_msgPackPool != null)
		{
			_msgPackPool.Clear();
			_msgPackPool = null;
		}

		_recordQueue = null;

		for (var keyValue = _eventDict.GetEnumerator(); keyValue.MoveNext();)
		{
			keyValue.Current.Value.Dispose();
		}
		_eventDict.Clear();
		_eventDict = null;
	}

	public MessagePort GetPortByAction(string action)
	{
		if (!_eventDict.ContainsKey(action))
		{
			_eventDict[action] = new MessagePort(action);
		}
		
		return _eventDict[action];
	}

	public void DataParser(string json)
	{
		Hashtable data = SerializeUtil.Json2Object(json) as Hashtable;
		Hashtable recvData = data["Response"] as Hashtable;
		Debug.Log("Data Parser: " + json);

		if (CheckRecvData(recvData))
		{
			MessagePack msgPack = PopMessage();
			msgPack.Return = true;

			if (recvData.ContainsKey("method") && recvData["method"] != null)
			{
				msgPack.Action = recvData["method"].ToString();
			}

			if (recvData.ContainsKey("resp") && recvData["resp"] != null)
			{
				msgPack.Callback = RequestManager.Instance.GetRespCallbackByToken(recvData["resp"].ToString());
			}
			
			if (recvData.ContainsKey("time") && recvData["time"] != null)
			{
				long timeStamp = 0;
				long.TryParse(recvData["time"].ToString(), out timeStamp);
				if (timeStamp > 0)
				{
					msgPack.TimeStamp = timeStamp;
				}
			}
			
			if (recvData.ContainsKey("data"))
			{
				msgPack.Data = recvData["data"];
			}
			
			if (recvData.ContainsKey("payload"))
			{
				msgPack.Payload = recvData["payload"];
			}
			
			if (recvData.ContainsKey("ok") && recvData["ok"].ToString() == "0")
			{
				msgPack.Return = false;
			}

			MessageHandler(msgPack);
		}
	}

	public MessagePack PopMessage()
	{
		return _msgPackPool.Allocate();
	}

	public void ReleaseMessage(MessagePack msgPack)
	{
		if (msgPack == null) return;

		msgPack.Dispose();
		_msgPackPool.Release(msgPack);
	}

	private void OnMessageProcess(double time)
	{
		if (_recordQueue == null) return;

		MessagePack currentRecord;
		if (_recordQueue.TryDequeue(out currentRecord))
		{
			DispatchMessage(currentRecord);
		}
	}

	private bool CheckRecvData(Hashtable recvData)
	{
		if (recvData != null && recvData["ok"] != null)
		{
			switch (recvData["ok"].ToString())
			{
			case MessagePack.Type.NORMAL:
				return true;

			case MessagePack.Type.MAINTENANCE:
				break;
			}
		}

		return false;
	}

	private void MessageHandler(MessagePack msgPack)
	{
		if (msgPack != null)
		{
			_recordQueue.Enqueue(msgPack);
		}
		else
		{
			Debug.LogError("MessageHandler, has a null msg data to handle.");
		}
	}

	private void DispatchMessage(MessagePack msgPack)
	{
		if (msgPack.TimeStamp > 0)
		{
			NetServerTime.Instance.SetServerTime(msgPack.TimeStamp / 1000.0);
			
			if (msgPack.Data != null)
			{
				//DB.DBManager.inst.ReciveDatas(msgInfo.Data, msgInfo.TimeStamp);
			}
		}
		
		if (!string.IsNullOrEmpty(msgPack.Action))
		{
			GetPortByAction(msgPack.Action).PushMessage(msgPack.Payload);
		}
		
		if (msgPack.Callback != null)
		{
			msgPack.Callback(msgPack.Return, msgPack.Payload);
		}
		
		ReleaseMessage(msgPack);
	}
}
