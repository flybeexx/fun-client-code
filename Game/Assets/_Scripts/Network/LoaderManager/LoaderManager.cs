using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoaderManager
{
	private static LoaderManager _instance;
	public static LoaderManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new LoaderManager();
			}

			return _instance;
		}
	}

	public void Initialize()
	{
	}

	public void Dispose()
	{
	}	
}
