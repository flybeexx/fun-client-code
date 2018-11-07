using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class NativeManager : MonoBehaviour
{
	[DllImport("__Internal")]
	private static extern string GetIOSDeviceUDID();

	private static NativeManager _instance;
	public static NativeManager Instance
	{
		get
		{
			if (_instance == null)
			{
				GameObject go = new GameObject("NativeManager");
				go.AddComponent<DontDestroy>();

				_instance = go.AddComponent<NativeManager>();
			}

			return _instance;
		}
	}

	public string GetOS()
	{
		if (Application.isEditor)
		{
#if UNITY_ANDROID
			return "editor_android";
#else
			return "editor_ios";
#endif
		}

		switch(Application.platform)
		{
		case RuntimePlatform.Android:
			return "android";
		case RuntimePlatform.IPhonePlayer:
			return "ios";
		}

		return "editor";
	}
	
	public string GetDeviceUDID()
	{
#if UNITY_IOS
		return GetIOSDeviceUDID();
#endif

		return SystemInfo.deviceUniqueIdentifier;
	}
}
