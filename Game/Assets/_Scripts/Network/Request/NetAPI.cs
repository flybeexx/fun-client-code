using UnityEngine;
using System.Collections;

public class NetAPI
{
	private static NetAPI _instance;
	public static NetAPI Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new NetAPI();
			}

			return _instance;
		}
	}

	private Hashtable _manifest;
	public Hashtable Manifest
	{
		get
		{
			return _manifest;
		}
		set
		{
			_manifest = value;
		}
	}

	private string _configFileVersion;
	public string ConfigFileVersion
	{
		get
		{
			if (string.IsNullOrEmpty(_configFileVersion))
			{
				if (_manifest != null && _manifest.ContainsKey("app_version"))
				{
					Hashtable data = _manifest["app_version"] as Hashtable;
					if (data != null && data.ContainsKey("config_version"))
					{
						_configFileVersion = data["config_version"].ToString();
					}
				}
			}
			return _configFileVersion;
		}
	}
}
