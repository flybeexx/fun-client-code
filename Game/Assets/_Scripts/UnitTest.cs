using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class UnitTest : MonoBehaviour
{
	void Start ()
	{

	}

	void RemoveObjects(Object[] objects)
	{
		if (objects == null) return;

		for (int i = 0; i < objects.Length; i++)
		{
			DestroyImmediate(objects[i]);
		}
	}
}
