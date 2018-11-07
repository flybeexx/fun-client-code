using System;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool<T> where T:class, new()
{
	BetterList<T> objectPool = new BetterList<T>();
	
	// allocate an object
	public T Allocate()
	{
		if (objectPool.size > 0)
		{
			return objectPool.Pop();
		}
		return new T();
	}
	
	// release an object
	public void Release(T o)
	{
		objectPool.Add(o);
	}
	
	public void Clear()
	{
		objectPool.Clear();
	}
}
