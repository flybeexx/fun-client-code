using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DebugMenu : MonoBehaviour
{
	private bool _display;
	private Vector3 _scale = Vector3.one;
	private float _updateInterval = 1.0f;
	private float _lastInterval;
	private int _frames = 0;
	private float _ms = 0.0f;
	private float _fps = 0.0f;
	private System.Action[] _tabFunc;
	private int _selectTab = 0;

	private string _serverIP = "127.0.0.1";
	private string _serverPort = "3563";
	private string _sendMsg = "Say Sth...";

	private const float DEBUG_MENU_WIDTH = 800f;
	private const float DEBUG_MENU_HEIGHT = 600f;

	void Start()
	{
		_lastInterval = Time.realtimeSinceStartup;
		_frames = 0;
		_tabFunc = new System.Action[]{Tab1, Tab2, Tab3, Tab4};

		Debug.Log("Screen DPI: " + Screen.dpi + ", Screen dimensions: " + Screen.currentResolution.width + "x" + Screen.currentResolution.height);
	}

	void Update ()
	{
		if (Input.GetKeyUp(KeyCode.F1))
		{
			_display = !_display;
		}

		++_frames;

		float timeNow = Time.realtimeSinceStartup;
		if (timeNow > _lastInterval + _updateInterval)
		{
			_fps = _frames / (timeNow - _lastInterval);
			_ms = 1000.0f / Mathf.Max (_fps, 0.00001f);
			_frames = 0;
			_lastInterval = timeNow;
		}
	}

	void OnGUI()
	{
		if (!_display) return;

		float rate = Screen.height / DEBUG_MENU_HEIGHT;
		_scale.x = rate;
		_scale.y = rate;
		_scale.z = 1;

		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, _scale);
		}

		Matrix4x4 guiMat = GUI.matrix;
		
		GUILayout.BeginArea(new Rect (5, 5, DEBUG_MENU_WIDTH, DEBUG_MENU_HEIGHT));
		GUILayout.Label("Debug Menu");
		GUILayout.Label(string.Format("{0:F2}ms, {1:F2}FPS", _ms, _fps));

		List<string> buttons = new List<string>();
		buttons.Add("Tab1");
		buttons.Add("Tab2");
		buttons.Add("Tab3");
		buttons.Add("Tab4");

		string[] tabs = buttons.ToArray();
		_selectTab = GUILayout.SelectionGrid(_selectTab, tabs, tabs.Length, GUILayout.Height(60));
		if (_tabFunc != null && _selectTab >= 0 && _selectTab < _tabFunc.Length)
		{
			_tabFunc[_selectTab]();
		}
		
		GUILayout.EndArea();
		GUI.matrix = guiMat;
	}

	private void Tab1()
	{
		GUILayout.BeginVertical();

		GUILayout.BeginHorizontal();
		GUILayout.Box("IP", GUILayout.Width(DEBUG_MENU_WIDTH / _tabFunc.Length));
		_serverIP = GUILayout.TextField(_serverIP);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Box("Port", GUILayout.Width(DEBUG_MENU_WIDTH / _tabFunc.Length));
		_serverPort = GUILayout.TextField(_serverPort);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		_sendMsg = GUILayout.TextArea(_sendMsg, GUILayout.Width(DEBUG_MENU_WIDTH / _tabFunc.Length * 3), GUILayout.Height(DEBUG_MENU_HEIGHT / _tabFunc.Length));

		GUILayout.BeginVertical();
		if (GUILayout.Button("Connect", GUILayout.Width(DEBUG_MENU_WIDTH / _tabFunc.Length), GUILayout.Height(DEBUG_MENU_HEIGHT / (_tabFunc.Length * 2))))
		{
			int inputServerPort = 0;
			int.TryParse(_serverPort, out inputServerPort);
			
			NetworkManager.Instance.Connect(_serverIP, inputServerPort);
		}
		if (GUILayout.Button("Send Msg", GUILayout.Width(DEBUG_MENU_WIDTH / _tabFunc.Length), GUILayout.Height(DEBUG_MENU_HEIGHT / (_tabFunc.Length * 2))))
		{
			RequestManager.Instance.SendRequest("network:Test", Util.Hash("words", _sendMsg), delegate(bool ret, object data) {
				Debug.Log("Callback invoking... for response from server.");
			});
		}
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();

		GUILayout.EndVertical();
	}

	private void Tab2()
	{

	}

	private void Tab3()
	{

	}

	private void Tab4()
	{

	}
}
