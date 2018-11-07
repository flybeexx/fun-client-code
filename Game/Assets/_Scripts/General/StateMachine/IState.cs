using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public interface IState
{
	string Name { get; }
	object Data { get; set; }
	StateMachine CurrentStateMachine { get; set; }

	void OnEnter();
	void OnProcess();
	void OnExit();

	void Dispose();
}
