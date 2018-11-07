using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/*
	Electronic oscillator

	An electronic oscillator is an electronic circuit that produces a periodic,
	oscillating electronic signal, often a sine wave or a square wave.
	Oscillators convert direct current (DC) from a power supply to an alternating current signal.
	They are widely used in many electronic devices.

	From: http://en.wikipedia.org/wiki/Electronic_oscillator
*/

public class Oscillator : MonoBehaviour
{
	public event Action<double> UpdateEvent;
	public event Action<int> SecondEvent;
	public event Action ThreeSecondEvent;

	WaitForSeconds m_WaitOneSecond = new WaitForSeconds(1.0f);
	WaitForSeconds m_WaitThreeSecond = new WaitForSeconds(3.0f);

	static Oscillator m_Instance = null;
	public static Oscillator Instance
	{
		get
		{
			if(m_Instance == null && GameEngine.IsAvailable)
			{
				m_Instance = FindObjectOfType(typeof(Oscillator)) as Oscillator;

				if(m_Instance == null)
				{
					Debug.Log("<color=#00FFFF>Create the unique Oscillator instance.</color>");

					GameObject go = new GameObject("Oscillator");
					m_Instance = go.AddComponent<Oscillator>();
				}
			}
			return m_Instance;
		}
	}

	public void Dispose()
	{
		UpdateEvent = null;
		SecondEvent = null;
		ThreeSecondEvent = null;
		m_Instance = null;

		StopAllCoroutines();
		Destroy(this.gameObject);
	}
	
	void Start()
	{
		DontDestroyOnLoad(gameObject);
		StartCoroutine(TriggerSecondEvent());
		StartCoroutine(TriggerThreeSecondEvent());
	}

	void Update()
	{
		if (UpdateEvent != null)
		{
			UpdateEvent(ServerTime.SmoothServerTime);
		}
	}

	public NetServerTime ServerTime
	{
		get
		{
			return NetServerTime.Instance;
		}
	}

	IEnumerator TriggerSecondEvent()
	{
		while (true)
		{
			if (SecondEvent != null)
			{
				SecondEvent((int)ServerTime.SmoothServerTime);
			}
			yield return m_WaitOneSecond;
		}
	}

	IEnumerator TriggerThreeSecondEvent()
	{
		while (true)
		{
			if (ThreeSecondEvent != null)
			{
				ThreeSecondEvent();
			}
			yield return m_WaitThreeSecond;
		}
	}
}
