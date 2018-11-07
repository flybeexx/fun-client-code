using UnityEngine;
using System.Collections;

public class AccountManager
{
	private const string ACCOUNT_ID = "account_id";

	private static AccountManager _instance;
	public static AccountManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new AccountManager();
			}

			return _instance;
		}
	}

	public string AccountId
	{
		get
		{
			return GetAccount();
		}
	}

	public string GetAccount()
	{
		if (!PlayerPrefs.HasKey(ACCOUNT_ID))
		{
			PlayerPrefs.SetString(ACCOUNT_ID, NativeManager.Instance.GetDeviceUDID());
		}

		return PlayerPrefs.GetString(ACCOUNT_ID);
	}
}
