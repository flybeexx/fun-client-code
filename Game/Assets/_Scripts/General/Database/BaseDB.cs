using System;
using System.Collections;
using System.Collections.Generic;

namespace DB
{
	public class BaseDB
	{
		protected Dictionary<long, BaseData> _tableData = new Dictionary<long, BaseData>();
		
		public event System.Action<BaseData> OnDataChanged;
		public event System.Action<BaseData> OnDataUpdated;
		public event System.Action<BaseData> OnDataRemoved;
		public event System.Action<BaseData> OnDataCreated;
		
		public void Clear()
		{
			_tableData.Clear ();
		}
		
		void Publish_OnDataChanged(BaseData data)
		{
			if (OnDataChanged != null)
			{
				OnDataChanged(data);
			}
		}
		
		void Publish_OnDataCreate(BaseData data)
		{
			if (OnDataCreated != null)
			{
				OnDataCreated(data);
			}
			Publish_OnDataChanged(data);
		}
		
		void Publish_OnDataUpdate(BaseData data)
		{
			if (OnDataUpdated != null)
			{
				OnDataUpdated(data);
			}
			Publish_OnDataChanged(data);
		}
		
		void Publish_OnDataRemove(BaseData data)
		{	
			if (OnDataRemoved != null)
			{
				OnDataRemoved(data);
			}
			Publish_OnDataChanged(data);
		}
		
		public void UpdateDatas(object orgDatas, long updateTime)
		{
			ArrayList datas = orgDatas as ArrayList;

			if (datas == null) return;

			for (int i = 0, length = datas.Count; i < length; i++)
			{
				Update(datas[i], updateTime);
			}
		}
		
		public void UpdateDatas(Hashtable orgDatas, long updateTime)
		{
			if (orgDatas == null) return;
			
			Update(orgDatas, updateTime);
		}
		
		bool Remove(long dataKey, long updateTime)
		{
			if (_tableData.ContainsKey(dataKey))
			{
				BaseData baseData = _tableData[dataKey];
				_tableData.Remove(dataKey);
				Publish_OnDataRemove(baseData);
				return true;
			}
			
			return false;
		}
		
		public void RemoveDatas(object orgDatas, long updateTime)
		{
			ArrayList datas = orgDatas as ArrayList;

			if (datas == null) return;
			
			long dataKey = 0;
			
			for (int i = 0, length = datas.Count; i < length; i++)
			{
				DB.DatabaseTools.UpdateData(datas[i] as Hashtable, GetDataKey(), ref dataKey);
				Remove(dataKey, updateTime);
			}
		}
		
		bool Add(object orgData, long updateTime)
		{
			Hashtable _datas = orgData as Hashtable;
			
			if (_datas == null) { return false; }
			
			BaseData newData = CreateData();
			
			if (newData.Decode(_datas, updateTime) && !_tableData.ContainsKey(newData.PrimaryKey))
			{
				_tableData.Add(newData.PrimaryKey, newData);
				Publish_OnDataCreate(newData);
				return true;
			}
			else
			{
				return false;
			}
		}
		
		bool Update(object orgData, long updateTime)
		{
			Hashtable _datas = orgData  as Hashtable;
			
			if (_datas == null) { return false; }
			
			long dataKey = 0;
			DB.DatabaseTools.UpdateData(_datas, GetDataKey(), ref dataKey);
			
			if (_tableData.ContainsKey(dataKey))
			{
				BaseData baseData = _tableData[dataKey];
				if(baseData.Decode(_datas, updateTime))
				{
					Publish_OnDataUpdate(baseData);
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				return Add(orgData, updateTime);
			}
		}

		protected virtual BaseData CreateData()
		{
			return null;
		}

		protected virtual string GetDataKey()
		{
			return "";
		}

		public BaseData GetData(long dataKey)
		{
			if (_tableData.ContainsKey(dataKey))
			{
				return _tableData[dataKey];
			}
			return null;
		}
	}
}
