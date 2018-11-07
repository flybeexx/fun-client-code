using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using Pathfinding.Serialization.JsonFx;

public class SerializeUtil
{
	/// <summary>
	/// 对象序列化
	/// </summary>
	public static byte[] Encode(object value)
	{
		// 创建编码解码的内存流对象
		MemoryStream memoryStream = new MemoryStream();
		// 二进制流序列化对象
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		// 将obj对象序列化成二进制数据写入到内存流
		binaryFormatter.Serialize(memoryStream, value);

		byte[] result = new byte[memoryStream.Length];
		// 将流数据拷贝到结果数组
		Buffer.BlockCopy(memoryStream.GetBuffer(), 0, result, 0, (int)memoryStream.Length);
		memoryStream.Close();
		return result;
	}

	/// <summary>
	/// 反序列化对象
	/// </summary>
	public static object Decode(byte[] value)
	{
		// 创建编码解码的内存流对象, 并将需要反序列化的数据写入其中
		MemoryStream memoryStream = new MemoryStream(value);
		// 二进制流序列化对象
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		// 将流数据反序列化为obj对象
		object result = binaryFormatter.Deserialize(memoryStream);
		memoryStream.Close();
		return result;
	}

	#region about JSON
	
	private static JsonReaderSettings _rSetting = null;
	private static JsonWriterSettings _wSetting = null;
	
	private static bool _jsonInited = false;
	
	private static void CheckJsonLib()
	{
		if (!_jsonInited)
		{
			_rSetting = new JsonReaderSettings();
			_rSetting.UseStringInsteadOfNumber = true;
			
			_wSetting = new JsonWriterSettings();
		}
		
		_jsonInited = true;
	}
	
	public static System.Object Json2Object(string json, bool useStringInsteadOfNumber = true)
	{
		System.Object jsonObj = null;
		
		if (json != null)
		{
			bool success = true;
			
			CheckJsonLib();
			
			_rSetting.UseStringInsteadOfNumber = useStringInsteadOfNumber;
			
			JsonReader jReader = new JsonReader(json, _rSetting);
			jsonObj = jReader.Deserialize();
			
			if (!success)
			{
				jsonObj = null;
				Debug.LogError("json decode error with following json:\n" + json);
			}
			
			return jsonObj;
		}
		
		return null;	
	}
	
	public static string Object2Json(System.Object obj)
	{
		CheckJsonLib();
		
		StringBuilder sb = new StringBuilder();
		using (JsonWriter jWriter = new JsonWriter(sb, _wSetting))
		{
			jWriter.Write(obj);
		}
		
		return sb.ToString();
	}
	
	public static byte[] Json2ByteArray(string json)
	{
		return Encoding.UTF8.GetBytes(json);
	}

	public static string ByteArray2String(byte[] bytes, int index = 0)
	{
		return Encoding.UTF8.GetString(bytes, index, bytes.Length - index);
	}
	
	public static bool SerializeString(string aString, StringBuilder builder)
	{
		char[] charArray = aString.ToCharArray();
		for (int i = 0; i < charArray.Length; i++)
		{
			char c = charArray [i];
			if (c == '"')
			{
				builder.Append("\\\"");
			}
			else if (c == '\\')
			{
				builder.Append("\\\\");
			}
			else if (c == '\b')
			{
				builder.Append("\\b");
			}
			else if (c == '\f')
			{
				builder.Append("\\f");
			}
			else if (c == '\n')
			{
				builder.Append("\\n");
			}
			else if (c == '\r')
			{
				builder.Append("\\r");
			}
			else if (c == '\t')
			{
				builder.Append("\\t");
			}
			else
			{
				int codePoint = Convert.ToInt32(c);
				if ((codePoint >= 32) && (codePoint <= 126))
				{
					builder.Append(c);
				}
				else
				{
					builder.Append("\\u" + Convert.ToString(codePoint, 16).PadLeft(4, '0'));
				}
			}
		}

		return true;
	}
	
	public static string RemoveBBCode(string source)
	{
		return Regex.Replace(source,  @"\[[^\]]+]", "");
	}
	
	public static void SetClipboardText(string str)
	{
		UniCopipe.Value = str;
	}
	
	#endregion
}
