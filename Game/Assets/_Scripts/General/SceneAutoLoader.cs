using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneAutoLoader : MonoBehaviour
{
	public string sceneName = "";

	void Start ()
	{
		SceneManager.LoadSceneAsync(sceneName);
	}
}
