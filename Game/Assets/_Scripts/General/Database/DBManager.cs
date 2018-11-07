using UnityEngine;
using System.Collections;

namespace DB
{
	public class DBManager
	{
		#region DB defines

		public BaseDB DB_BaseTest;

		#endregion

        private bool _initialized = false;
		private const string DATA_SET = "set";
		private const string DATA_DEL = "del";

		private static DBManager _instance;
		public static DBManager Instance
		{
			get
			{
				if(_instance == null)
				{
					_instance = new DBManager();
				}
				return _instance;
			}
		}

		public void Initialize()
		{
			if (!_initialized)
			{
				DB_BaseTest = new BaseDB();
            }
        }

		public void Dispose()
		{
			DB_BaseTest = null;
		}

		public void LoadDatas(object orgData, long updateTime)
		{
			UpdateDatas(orgData, updateTime);
		}

		public void ReciveDatas(object orgData, long updateTime)
		{
			if (orgData == null) return;

			Hashtable data = orgData as Hashtable;
			if (data == null) return;
#if UNITY_EDITOR
			try
			{
#endif
				UpdateDatas(data[DATA_SET], updateTime);
				RemoveDatas(data[DATA_DEL], updateTime);
#if UNITY_EDITOR
			}
			catch (UnityException e)
			{
				Debug.Log(e.StackTrace);
                Debug.Log("Error : " + SerializeUtil.Object2Json(orgData));
			}
#endif
		}

		public void UpdateDatas(object orgData, long updateTime = 0)
		{
			Hashtable datas = orgData as Hashtable;

			if (datas == null) return;

			foreach (string key in datas.Keys)
			{
				switch (key)
				{
				case BaseData.Params.KEY:
					DB_BaseTest.UpdateDatas(datas[key], updateTime);
					break;
				}
			}
		}

		public void RemoveDatas(object orgData, long updateTime)
		{
			Hashtable datas = orgData as Hashtable;
			
			if (datas == null) return;

			foreach (string key in datas.Keys)
			{
				switch (key)
				{
				case BaseData.Params.KEY:
					DB_BaseTest.RemoveDatas(datas[key], updateTime);
					break;
				}
			}
		}
	}

	public class DatabaseTools
	{
		private static int _tmpStaticInt;
		private static long _tmpStaticLong;
		private static bool _tmpStaticBool;
		private static float _tmpStaticFloat;
		private static double _tmpStaticDouble;

		public static bool UpdateData(Hashtable inData, string key, ref int outData)
		{
			if (inData.ContainsKey(key) && inData[key] != null)
			{
				if(int.TryParse(inData[key].ToString(), out _tmpStaticInt))
				{
					if(_tmpStaticInt != outData)
					{
						outData = _tmpStaticInt;
						return true;
					}
				}
			}
			return false;
		}

		public static bool UpdateData(Hashtable inData, string key, ref long outData)
		{
			if (inData.ContainsKey(key) && inData[key] != null
			    && long.TryParse(inData[key].ToString(), out _tmpStaticLong)
			    && _tmpStaticLong != outData)
			{
				outData = _tmpStaticLong;
				return true;
			}
			return false;
		}

		public static bool UpdateData(Hashtable inData, string key, ref bool outData)
		{
			if (inData.ContainsKey(key) && inData[key] != null
				&& bool.TryParse(inData[key].ToString(), out _tmpStaticBool)
			    && _tmpStaticBool != outData)
			{
				outData = _tmpStaticBool;
				return true;
			}
			return false;
		}
		
		public static bool UpdateData(Hashtable inData, string key, ref float outData)
		{
			if (inData.ContainsKey(key) && inData[key] != null
			    && float.TryParse(inData[key].ToString(), out _tmpStaticFloat)
			    && _tmpStaticFloat != outData)
			{
				outData = _tmpStaticFloat;
				return true;
			}
			return false;
		}
		
		public static bool UpdateData(Hashtable inData, string key, ref double outData)
		{
			if (inData.ContainsKey(key) && inData[key] != null
			    && double.TryParse(inData[key].ToString(), out _tmpStaticDouble)
			    && _tmpStaticDouble != outData)
			{
				outData = _tmpStaticDouble;
				return true;
			}
			return false;
		}

		public static bool UpdateData(Hashtable inData, string key, ref string outData)
		{
			if (inData.ContainsKey(key)
			    && inData[key] != null
			    && (string.Compare(inData[key].ToString(), outData) != 0))
			{
				outData = inData[key].ToString();
				return true;
			}
			return false;
		}

		public static bool CheckAndParseOrgData(object orgData, out Hashtable data)
		{
			if (orgData == null)
			{
				data = null;
				return false;
			}
			
			data = orgData as Hashtable;
			
			return data != null;
		}
		
		public static bool CheckAndParseOrgData(object orgData, out ArrayList data)
		{
			if (orgData == null)
			{
				data = null;
				return false;
			}
			
			data = orgData as ArrayList;
			
			return data != null;
		}
	}
}
