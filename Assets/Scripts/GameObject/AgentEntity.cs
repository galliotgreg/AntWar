﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentEntity : MonoBehaviour
{
    [SerializeField]
    private int id;

	private HomeScript home;
    [SerializeField]
    private AgentBehavior behaviour;
    [SerializeField]
    private AgentContext context;

	/// <summary>
	/// Name of the ABM the agent engages in
	/// </summary>
	[SerializeField]
	private string behaviorModelIdentifier;

	#region Properties
	public HomeScript Home {
		get {
			return home;
		}
		set {
			home = value;
			this.context.Home = value.gameObject;
		}
	}

    public AgentBehavior Behaviour
    {
        get
        {
            return behaviour;
        }
    }

    public AgentContext Context
    {
        get
        {
            return context;
        }
    }

	public string BehaviorModelIdentifier {
		get {
			return behaviorModelIdentifier;
		}
		set {
			behaviorModelIdentifier = value;
		}
	}

    public int Id
    {
        get
        {
            return id;
        }

        set
        {
            id = value;
        }
    }

	[SerializeField]
    public PlayerAuthority Authority
    {
        get
        {
			return home.Authority;
        }
    }

	[SerializeField]
    public string CastName
    {
        get
        {
			return this.context.Model.Cast;
        }
    }
	#endregion

	public void setAction( ABAction action, IABType[] actionParams ){
		this.behaviour.CurAction = action;
		this.behaviour.CurActionParams = actionParams;
	}

	public AgentComponent[] getAgentComponents (){
		return this.GetComponentsInChildren<AgentComponent> ();
	}

    // Use this for initialization
    void Awake()
    {
        behaviour = this.GetComponent<AgentBehavior>();
        context = this.GetComponent<AgentContext>();
		context.Entity = this;
    }

    void Start()
    {
        if (ABManager.instance != null)
        {
            ABManager.instance.RegisterAgent(this);
        } else
        {
            Debug.LogWarning("WARNING ! : ABManager not found");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

	void OnDestroy(){
		ABManager.instance.UnregisterAgent(this);
	}
}
