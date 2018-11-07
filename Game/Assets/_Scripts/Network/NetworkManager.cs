using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Net.Sockets;

public class NetworkManager
{
	private static NetworkManager _instance;
	public static NetworkManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new NetworkManager();
			}

			return _instance;
		}
	}

	public bool NetAvailable
	{
		get
		{
			if (_socket != null && _socket.Connected)
			{
				return true;
			}

			return false;
		}
	}
	
	private const float RECONNECT_DELAY_TIME = 3f;
	private const int MSG_HEAD_LENGTH = 2;
	private const int READ_BUFF_LENGTH = 1024;

	private Socket _socket;
	private bool _tryReconnect = true;
	private bool _isReading = false;
	private byte[] _readBuff;
	private List<byte> _dataCache;
	private string _serverIP;
	private int _serverPort;

	public void Initialize()
	{
		_readBuff = new byte[READ_BUFF_LENGTH];
		_dataCache = new List<byte>();

		Connect();
	}

	public void Dispose()
	{
		if (_socket != null)
		{
			_socket.Close();
			_socket = null;
		}

		if (_dataCache != null)
		{
			_dataCache.Clear();
			_dataCache = null;
		}

		_readBuff = null;
	}

	public void Connect(string ip = "127.0.0.1", int port = 3563)
	{
		try
		{
			_serverIP = ip;
			_serverPort = port;

			// 创建客户端连接对象
			_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			// 连接到服务器
			_socket.Connect(ip, port);
			// 开启异步消息接收, 消息到达后会直接写入缓冲区 readBuff
			_socket.BeginReceive(_readBuff, 0, READ_BUFF_LENGTH, SocketFlags.None, ReceiveCallback, _readBuff);
		}
		catch (Exception e)
		{
			_socket.Close();

			if (_tryReconnect)
			{
				Debug.Log("network error, " + e.Message);
				Reconnect(RECONNECT_DELAY_TIME);
			}
		}
	}

	public void SendRequest(string action, Hashtable postData, string type = "", string token = "")
	{
		Hashtable htRequest = new Hashtable();
		htRequest["Request"] = Util.Hash("method", action, "params", postData, "class", type, "req", token);

		Send(htRequest);
	}

	private void ReceiveCallback(IAsyncResult ar)
	{
		try
		{
			// 获取当前收到的消息长度
			int length = _socket.EndReceive(ar);
			Debug.Log("recv msg length: " + length.ToString());

			byte[] recvMsg = new byte[length];
			Buffer.BlockCopy(_readBuff, 0, recvMsg, 0, length);
			_dataCache.AddRange(recvMsg);

			if (!_isReading)
			{
				_isReading = true;
				OnRecvDataHandler();
			}

			if (_socket.Poll(-1, SelectMode.SelectRead))
			{
				if (_socket.Available > 0)
				{
					// 尾递归, 再次开启异步消息接收, 消息到达后会直接写入缓冲区 readBuff
					_socket.BeginReceive(_readBuff, 0, READ_BUFF_LENGTH, SocketFlags.None, ReceiveCallback, _readBuff);
				}
				else
				{
					_socket.Close();
				}
			}
		}
		catch 
		{
			Debug.Log("disconnected from server...");
			Reconnect(RECONNECT_DELAY_TIME);
		}
	}

	// 缓存中有数据处理
	private void OnRecvDataHandler()
	{
		// 长度解码
		byte[] result = Decode(ref _dataCache);
		
		// 长度解码返回空, 说明消息体不全, 等待下条消息过来补全
		if (result == null)
		{
			_isReading = false;
			return;
		}
		else
		{
			Debug.Log("recv from server: " + SerializeUtil.ByteArray2String(result));

			Util.ExecutePrompt(delegate(){
				MessageManager.Instance.DataParser(SerializeUtil.ByteArray2String(result));
			});
		}

		// 尾递归, 防止在消息处理过程中, 有其他消息到达而没有经过处理
		OnRecvDataHandler();
	}

	private void Reconnect(float delay)
	{
		Debug.Log(string.Format("reconnect server, {0}:{1}", _serverIP, _serverPort));

		Util.ExecuteInSecs(delay, Reconnect);
	}

	private void Reconnect()
	{
		if (!string.IsNullOrEmpty(_serverIP))
		{
			Connect(_serverIP, _serverPort);
		}
	}

	private byte[] Decode(ref List<byte> cache)
	{
		if (cache.Count < MSG_HEAD_LENGTH) return null;

		// 创建内存流对象, 并将缓存数据写入进去
		MemoryStream memoryStream = new MemoryStream(cache.ToArray());
		// 二进制读取流
		BinaryReader binaryReader = new BinaryReader(memoryStream);
		// 从缓存中读取int型消息体长度
		int length = binaryReader.ReadInt16();
		// 如果消息体长度 大于缓存中数据长度 说明消息没有读取完 等待下次消息到达后再次处理
		if (length > memoryStream.Length - memoryStream.Position)
		{
			return null;
		}

		// 读取正确长度的数据
		byte[] result = binaryReader.ReadBytes(length);
		// 清空缓存
		cache.Clear();
		// 将读取后的剩余数据写入缓存
		cache.AddRange(binaryReader.ReadBytes((int)(memoryStream.Length - memoryStream.Position)));

		binaryReader.Close();
		memoryStream.Close();

		return result;
	}

	private void Send(Hashtable postData)
	{
		byte[] byteArray = SerializeUtil.Json2ByteArray(SerializeUtil.Object2Json(postData));

		ByteArray sendMsg = new ByteArray();
		// 写入长度
		sendMsg.Write((short)byteArray.Length);
		// 写入消息体
		sendMsg.Write(byteArray);

		Debug.Log("Send Text = " + SerializeUtil.ByteArray2String(sendMsg.GetBuff(), MSG_HEAD_LENGTH));

		try
		{
			_socket.Send(sendMsg.GetBuff());
		}
		catch (Exception e)
		{
			Debug.Log("network error, " + e.Message);
			Reconnect(RECONNECT_DELAY_TIME);
		}

		sendMsg.Close();
	}
}
