using UnityEngine;
using System;
using System.Collections;

public class Util
{
	// delay to execute in main thread
	public static void ExecuteInSecs(float delay, System.Action onFinished)
	{
		Loom.QueueOnMainThread(onFinished, delay);
	}

	// execute in main thread
	public static void ExecutePrompt(System.Action callback)
	{
		Loom.QueueOnMainThread(callback);
	}

	public static Coroutine StartCoroutine(string methodName)
	{
		if (Loom.Instance != null)
		{
			return Loom.Instance.StartCoroutine(methodName);
		}
		return null;
	}

	public static Coroutine StartCoroutine(IEnumerator routine)
	{
		if (Loom.Instance != null)
		{
			return Loom.Instance.StartCoroutine(routine);
		}
		return null;
	}

	public static Coroutine StartCoroutine(string methodName, object value)
	{
		if (Loom.Instance != null)
		{
			return Loom.Instance.StartCoroutine(methodName, value);
		}
		return null;
	}

	public static Hashtable Hash(params object[] args)
	{
		Hashtable hashTable = new Hashtable(args.Length / 2);
		if (args.Length % 2 != 0)
		{
			Debug.LogError("Error: Hash requires an even number of arguments!");
			return null;
		}
		else
		{
			int i = 0;
			while (i < args.Length - 1)
			{
				hashTable.Add(args[i], args[i + 1]);
				i += 2;
			}
			return hashTable;
		}
	}

	public const int ONE_DAY = 24 * 60 * 60;
	public static readonly System.DateTime time19700101 = new System.DateTime(1970, 1, 1);
	
	public static double ClientSideUnixUTCSeconds{ get { return System.DateTime.UtcNow.Subtract(time19700101).TotalSeconds; } }
	
	public static double ClientSideUnixUTCMillSeconds{ get { return System.DateTime.UtcNow.Subtract(time19700101).TotalMilliseconds; } }
	
	public static System.DateTime ServerTime2DateTime(long systemTime)
	{
		return (new System.DateTime(time19700101.Ticks + systemTime * 10000000)).ToLocalTime();
	}
	
	public static long DateTime2ServerTime(System.DateTime dateTime)
	{
		TimeSpan timeSpan = dateTime - time19700101;
		return (long)timeSpan.TotalSeconds;
	}

	public static string FormatTime(int time, bool hadDay = false)
	{
		if (time < 0)
		{
			time = 0;
		}
		
		int hours = 0;
		int minutes = 0;
		int seconds = 0;
		
		hours = time / 3600;
		int day = hours / 24;
		if (hadDay)
		{
			hours = hours % 24;
		}

		string dayStr = day > 0 ? day.ToString() + "d " : "";
		string hoursStr = hours > 0 ? hours.ToString() + ":" : "";
		
		minutes = (time % 3600) / 60;
		string minutesStr = minutes >= 10 ? (minutes.ToString() + ":") : ((hours > 0) ? ("0" + minutes.ToString() + ":") : ((minutes >= 0) ? (minutes.ToString() + ":") : ""));
		
		seconds = time % 60;
		string secondsStr = seconds >= 10 ? (seconds.ToString()) : (string.Format("0{0}", seconds));
		string reuslt = string.Format("{0}{1}{2}", hoursStr, minutesStr, secondsStr);
		if (hadDay)
		{
			reuslt = string.Format("{0}{1}{2}{3}", dayStr, hoursStr, minutesStr, secondsStr);
		}
		return reuslt;
	}
	
	public static string FormatTimeYYYYMMDD(long systemTime)
	{
		System.DateTime tmpDate = ServerTime2DateTime(systemTime);
		return string.Format("{2}/{0}/{1}", tmpDate.Month.ToString("00"), tmpDate.Day.ToString("00"), tmpDate.Year.ToString("00"));
	}

	public static string FormatThousands(string src)
	{
		string dst = "";
		
		if (src.IndexOf(".") > 0)
		{
			src = src.Substring(0, src.IndexOf(".") + 2);
			int comma = 0;
			bool bPassedDot = false;
			int fromRight = 0;
			for (int a = src.Length - 1; a >= 0; a--)
			{
				dst = src.Substring(a, 1) + dst;
				if (bPassedDot)
					comma++;
				if (src.Substring(a, 1) == ".")
					bPassedDot = true;
				if (comma % 3 == 0 && fromRight > 0 && a > 0 && src.Substring(a - 1, 1) != "-")
					dst = "," + dst;
				if (bPassedDot)
					fromRight++;
			}
		}
		else
		{
			int comma = 0;
			int fromRight = 0;
			for (int a = src.Length - 1; a >= 0; a--)
			{
				dst = src.Substring(a, 1) + dst;
				comma++;
				if (comma % 3 == 0 && fromRight > 0 && a > 0 && src.Substring(a - 1, 1) != "-")
					dst = "," + dst;
				fromRight++;
			}
		}
		
		return (dst);
	}
	
	public static string FormatShortThousands(int src)
	{
		string dst = "";
		if (src >= 1000000)
		{
			dst = FormatThousands(((float)(src / 1000000.0f)).ToString());
			dst += "M";
		}
		else if (src >= 1000)
		{
			float fSrc = src / 1000.0f;
			dst = string.Format("{0:0.#}K", fSrc);
		}
		else
		{
			dst = src.ToString();
		}
		return (dst);
	}
	
	public static string FormatShortThousands(long src)
	{
		string dst = "";
		if (src >= 1000000000)
		{
			dst = FormatThousands(((float)(src / 1000000000.0f)).ToString());
			dst += "B";
		}
		else if (src >= 1000000)
		{
			dst = FormatThousands(((float)(src / 1000000.0f)).ToString());
			dst += "M";
		}
		else if (src >= 1000)
		{
			float fSrc = src / 1000.0f;
			dst = string.Format("{0:0.#}K", fSrc);
		}
		else
		{
			dst = src.ToString();
		}
		return (dst);
	}
}
