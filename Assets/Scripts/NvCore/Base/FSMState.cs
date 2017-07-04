using UnityEngine;

public abstract class FSMState<T>
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

	public virtual void Enter() { }
	public virtual void Execute() { }
	public virtual void Exit() { }

	public virtual void SetParam(object par)
	{
		this.param = par;
	}

	public virtual void OnRegisted(T o) 
	{
		this.owner = o;
	}

	public virtual void OnUnregisted() { }

	public string ID { get { return id; } }
	public System.Type Type { get { return type; } }
}
