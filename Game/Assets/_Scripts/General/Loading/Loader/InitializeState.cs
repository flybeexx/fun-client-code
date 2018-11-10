using UnityEngine;
using System.Collections;

public class InitializeState : BaseState
{
	#region state interface

	public override void OnEnter()
	{
		base.OnEnter();
        base.Wait();

		LoadManifest();
	}

	public override void OnProcess()
	{
		base.OnProcess();

		// process here
	}

	public override void OnExit()
	{
		base.OnExit();
	}

	#endregion

	private void LoadManifest()
	{
		Hashtable htRequest = new Hashtable();
		htRequest.Add("os", NativeManager.Instance.GetOS());
		htRequest.Add("client_version", 1);

		RequestManager.Instance.SendRequest(PortType.MANIFEST, htRequest, delegate(bool ret, object data) {

			if (ret)
			{
				Debug.Log("recv ### " + SerializeUtil.Object2Json(data));

				NetAPI.Instance.Manifest = data as Hashtable;

                base.Notify();
			}
		});
	}
}
