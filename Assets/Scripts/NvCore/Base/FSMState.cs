using UnityEngine;

abstract public class FSMState<T>
{
	protected T owner;
	protected object param;

	protected string id;
	protected System.Type type;

	public FSMState()
	{
		id = this.GetType().Name;
		type = this.GetType();
	}

	virtual public void Enter() { }
	virtual public void Execute() { }
	virtual public void Exit() { }

	virtual public void SetParam(object par)
	{
		this.param = par;
	}

	virtual public void OnRegisted(T o) 
	{
		this.owner = o;
	}

	virtual public void OnUnregisted() { }

	public string ID { get { return id; } }
	public System.Type Type { get { return type; } }
}
