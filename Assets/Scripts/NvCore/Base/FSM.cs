using UnityEngine;
using System.Collections.Generic;


public class FSM<T> where T : new()
{
	private T owner;
	private Dictionary<string, FSMState<T>> allStates = new Dictionary<string, FSMState<T>>();

	private FSMState<T> currentState;
	private FSMState<T> previousState;
	private FSMState<T> globalState;

	private string changingToState = null;

	// for debug
	private List<string> stackHistory = new List<string>();

	public void RegisteState(FSMState<T> state)
	{
		string id = state.ID;
		if(allStates.ContainsKey(id))
		{
			//DebugUtil.Log("fsm state already registed: " + id);
			return;
		}
		allStates.Add(id, state);
		state.OnRegisted(owner);
	}

	public void Configure(T o)
	{
		currentState = null;
		previousState = null;
		globalState = null;
		owner = o;
	}

	public void SetGlobalState(string v)
	{
		if (globalState != null)
		{
			globalState.Exit();
		}

		if(allStates.ContainsKey(v))
		{
			globalState = allStates[v];
			globalState.Enter();
		}
	}

	public void Update()
	{
		if (globalState != null)
			globalState.Execute();
		if (currentState != null)
			currentState.Execute();
	}

	public FSMState<T> GetState(string id)
	{
		if (id == null || !allStates.ContainsKey(id))
		{
			return null;
		}
		return allStates[id];
	}

	public bool ChangeState(string id, object param = null)
	{
		if(id == null || !allStates.ContainsKey(id))
		{
			//DebugUtil.Log("fsm: state not exist: " + id);
			return false;
		}

		FSMState<T> newState = allStates[id];
		if(newState == currentState)
		{
//			DebugUtil.Log("fsm: try to switch to same state " + id);
			return true;
		}

		// currently changing to the target state.
		if (changingToState == id)
		{
			return true;
		}

		changingToState = id;
		previousState = currentState;
		if (currentState != null)
			currentState.Exit();
		currentState = newState;
		if (currentState != null)
		{
			currentState.SetParam(param);
			currentState.Enter();
		}
		stackHistory.Add(id);
		changingToState = null;
		return true;
	}

	public FSMState<T> GetCurrentState()
	{
		return currentState;
	}

	public void RevertToPreviousState()
	{
		if (previousState != null)
			ChangeState(previousState.ID);
	}

	public bool IsInState(System.Type type)
	{
		return currentState != null && currentState.Type == type;
	}
}
