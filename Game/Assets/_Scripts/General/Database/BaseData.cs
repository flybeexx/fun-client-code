using UnityEngine;
using System.Collections;

namespace DB
{
	public class BaseData
	{
		public struct Params
		{
			public const string KEY = "base_data";
		}

		public virtual long PrimaryKey
		{
			get
			{
				return 0;
			}
		}

		protected long _updateTime;

		public bool CheckUpdateTime(long checkTime)
		{
			return checkTime >= _updateTime;
		}

		public bool CheckAndResetUpdateTime(long checkTime)
		{
			if (CheckUpdateTime(checkTime))
		    {
				_updateTime = checkTime;
				return true;
			}
			return false;
		}

		public static bool CheckAndResetUpdateTime(long checkTime, ref long updateTime)
		{
			if (checkTime >= updateTime)
			{
				updateTime = checkTime;
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

		public bool CheckOrgDataAndUpdateTime(object orgData, long updateTime, out Hashtable htData)
		{
			htData = null;

			return CheckAndResetUpdateTime(updateTime) && CheckAndParseOrgData(orgData, out htData);
		}

		public bool CheckOrgDataAndUpdateTime(object orgData, long updateTime, out ArrayList arrData)
		{
			arrData = null;
			
			return CheckAndResetUpdateTime(updateTime) && CheckAndParseOrgData(orgData, out arrData);
		}

		public virtual bool Decode(Hashtable data, long updateTime)
		{
			return false;
		}
	}
}
