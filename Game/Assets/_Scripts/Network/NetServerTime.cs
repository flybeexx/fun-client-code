using UnityEngine;
using System.Collections;

public class NetServerTime
{
	const int SYNC_TIME_INTERVAL = 60;

	/// <summary>
	/// The 'server time field' update time(client time).
	/// </summary>
	double m_UpdateTime;
	/// <summary>
	/// Mark if update.
	/// </summary>
	bool m_update;

	static NetServerTime _instance;

	public static NetServerTime Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new NetServerTime();
			}

			return _instance;
		}
	}

	public void StartSyncTime()
	{
		ResetSmoothTime();
		Oscillator.Instance.SecondEvent += Process;
	}

	public void StopSyncTime()
	{
		Oscillator.Instance.SecondEvent -= Process;
		ResetSmoothTime();
	}

	void Process(int timestamp)
	{
		if (m_update && timestamp - (int)_serverTime > SYNC_TIME_INTERVAL)
		{
			m_update = false;

			//MessageHub.inst.GetPortByAction(PortType.PLAYER_PING).SendLoader(null, null, false); 
		}
	}

	/// <summary>
	/// server time in int
	/// </summary>
	/// <value>The server timestamp.</value>
	public int ServerTimestamp
	{
		get {return (int)(_serverTime +Time.realtimeSinceStartup - m_UpdateTime);}
	}

	/// <summary>
	/// server time in double
	/// </summary>
	/// <value>The update time.</value>
	public double UpdateTime
	{
		get {return Time.realtimeSinceStartup - m_UpdateTime + _serverTime;}
	}

	/// <summary>
	/// local time in int
	/// </summary>
	/// <value>The local timestamp.</value>
	public int LocalTimestamp
	{
		get {return (int)((System.DateTime.UtcNow.Ticks 
			             - System.DateTime.Parse("01/01/1970 00:00:00").Ticks)/10000000);}
	}

	/// <summary>
	/// local time in double
	/// </summary>
	/// <value>The local timestamp.</value>
	public double LocalUpdateTime
	{
		get {return (System.DateTime.UtcNow.Ticks 
			             - System.DateTime.Parse("01/01/1970 00:00:00").Ticks)/10000000.0;}
	}

	#region Server Time

	private double _serverTime;

	public void SetServerTime(double serverTime)
	{
		if (serverTime <=  _serverTime) { return; }

		_serverTime = serverTime;

		m_UpdateTime = Time.realtimeSinceStartup;
		/*
		if (serverTime < _serverTime)
		{
			m_UpdateTime -= (_serverTime - serverTime);
		}
		
		_serverTime = serverTime;
*/
		m_update = true;
	}

	#endregion

	#region Smoothed Server Time

	private float _lastUpdateTime = -1.0f;

	private double _smoothServerTime = -1.0;
	public double SmoothServerTime
	{
		get
		{
			if(_lastUpdateTime != Time.time)
			{
				UpdateSmoothServerTime();
				_lastUpdateTime = Time.time;
			}

			return _smoothServerTime;
		}
	}

	private void UpdateSmoothServerTime()
	{
		double serverTime = UpdateTime;
      
		if(_smoothServerTime == -1)
		{
			_smoothServerTime = serverTime;
		}
		
		_smoothServerTime = LerpTime(_smoothServerTime,serverTime);
	}


	private void ResetSmoothTime()
	{
		_smoothServerTime = -1.0;
		_lastUpdateTime = -1.0f;
	}
	
	public double LerpTime(double originTime,double targetTime, double lerpFactor = 0.95f)
	{
		double lerpTime;
		
		if(originTime >= targetTime)
		{
			lerpTime = originTime;
		}
		else
		{
			lerpTime = originTime + (targetTime - originTime) * lerpFactor;
		}
		
		return lerpTime;
	}

	#endregion

    public int TodayLeftTimeUTC()
    {
        return Util.ONE_DAY - ServerTimestamp % Util.ONE_DAY;
    }

    public bool IsToday(double time)
    {
        return ServerTimestamp / Util.ONE_DAY == (int)time / Util.ONE_DAY;
    }

}
