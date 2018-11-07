using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class ByteArray
{
	MemoryStream _memoryStream = new MemoryStream();
	
	BinaryWriter _binaryWriter;
	BinaryReader _binaryReader;

	public void Close()
	{
		_binaryWriter.Close();
		_binaryReader.Close();
		_memoryStream.Close();
	}
	
	/// <summary>
	/// 支持传入初始数据的构造
	/// </summary>
	public ByteArray(byte[] buff)
	{
		_memoryStream = new MemoryStream(buff);
		_binaryWriter = new BinaryWriter(_memoryStream);
		_binaryReader = new BinaryReader(_memoryStream);
	}
	
	/// <summary>
	/// 获取当前数据 读取到的下标位置
	/// </summary>
	public int Position
	{
		get { return (int)_memoryStream.Position; }
	}
	
	/// <summary>
	/// 获取当前数据长度
	/// </summary>
	public int Length
	{
		get { return (int)_memoryStream.Length; }
	}

	/// <summary>
	/// 当前是否还有数据可以读取
	/// </summary>
	public bool Readable
	{
		get { return _memoryStream.Length > _memoryStream.Position; }
	}
	
	/// <summary>
	/// 默认构造
	/// </summary>
	public ByteArray()
	{
		_binaryWriter = new BinaryWriter(_memoryStream);
		_binaryReader = new BinaryReader(_memoryStream);
	}
	
	public void Write(int value)
	{
		_binaryWriter.Write(value);
	}

	public void Write(short value)
	{
		_binaryWriter.Write(value);
	}

	public void Write(byte value)
	{
		_binaryWriter.Write(value);
	}

	public void Write(bool value)
	{
		_binaryWriter.Write(value);
	}

	public void Write(string value)
	{
		_binaryWriter.Write(value);
	}

	public void Write(byte[] value)
	{
		_binaryWriter.Write(value);
	}
	
	public void Write(double value)
	{
		_binaryWriter.Write(value);
	}

	public void Write(float value)
	{
		_binaryWriter.Write(value);
	}

	public void Write(long value)
	{
		_binaryWriter.Write(value);
	}

	public void Read(out int value)
	{
		value = _binaryReader.ReadInt32();
	}

	public void Read(out byte value)
	{
		value = _binaryReader.ReadByte();
	}

	public void Read(out bool value)
	{
		value = _binaryReader.ReadBoolean();
	}

	public void Read(out string value)
	{
		value = _binaryReader.ReadString();
	}

	public void Read(out byte[] value, int length)
	{
		value = _binaryReader.ReadBytes(length);
	}
	
	public void Read(out double value)
	{
		value = _binaryReader.ReadDouble();
	}

	public void Read(out float value)
	{
		value = _binaryReader.ReadSingle();
	}

	public void Read(out long value)
	{
		value = _binaryReader.ReadInt64();
	}
	
	public void Reposition()
	{
		_memoryStream.Position = 0;
	}
	
	/// <summary>
	/// 获取数据
	/// </summary>
	public byte[] GetBuff()
	{
		byte[] result = new byte[_memoryStream.Length];
		Buffer.BlockCopy(_memoryStream.GetBuffer(), 0, result, 0, (int)_memoryStream.Length);
		return result;
	}
}
