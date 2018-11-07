using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameEngine : MonoBehaviour
{
	public const string FIRST_LOAD_SCENE = "BootScene";
	private static GameEngine _instance;
	public static GameEngine Instance
	{
		get
		{
			if (_instance == null)
			{
				Debug.Log("GameEngine not yet created. It needs to be the first GameObject created.");
			}
			return _instance;
		}
	}

	void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Debug.LogError("There should be only one GameEngine object");
			Destroy(gameObject);
			return;
		}

		if (BlackCamera.Instance != null)
		{
			BlackCamera.Instance.SetActive(false);
		}

		_instance = this;

		DontDestroyOnLoad(gameObject);
	}

	void Start()
	{
		Initialize();
	}

	private void Initialize()
	{
		InitializeManager();
	}

	private void InitializeManager()
	{
		NetworkManager.Instance.Initialize();
		MessageManager.Instance.Initialize();
		RequestManager.Instance.Initialize();

		DB.DBManager.Instance.Initialize();

		LoaderManager.Instance.Initialize();
		PreloadManager.Instance.Initialize();
	}

	private void DisposeManager()
	{
		NetworkManager.Instance.Dispose();
		MessageManager.Instance.Dispose();
		RequestManager.Instance.Dispose();

		DB.DBManager.Instance.Dispose();

		LoaderManager.Instance.Dispose();
		PreloadManager.Instance.Dispose();
	}

	private void DisposeCoroutines()
	{
		MonoBehaviour[] gos = Object.FindObjectsOfType<MonoBehaviour>();
		foreach(var go in gos)
		{
			go.StopAllCoroutines();
		}

		StopAllCoroutines();
	}

	private void DisposeGameScenes()
	{
		for (int i = 0; i < SceneManager.sceneCount; i++)
		{
			SceneManager.UnloadScene(i);
		}

		Resources.UnloadUnusedAssets();
	}

	public static bool IsAvailable
	{
		get { return _instance != null; }
	}

	public void RestartGame()
	{
		DisposeCoroutines();
		DisposeGameScenes();

		DisposeManager();

		Destroy(gameObject);
		_instance = null;

		SceneManager.LoadScene(FIRST_LOAD_SCENE);
	}
}
