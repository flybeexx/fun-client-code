using UnityEngine;
using System.Collections;

public class BlackCamera : MonoBehaviour 
{
	public static GameObject Instance = null;

	void Awake () 
	{
		DontDestroyOnLoad (this.gameObject);
		Instance = gameObject;
	}

	void OnDestory()
	{
		Instance = null;
	}
}
