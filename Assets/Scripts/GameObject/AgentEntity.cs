﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentEntity : MonoBehaviour
{
    [SerializeField]
    private int id;

	[SerializeField]
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

    // Use this for initialization
    void Awake()
    {
        behaviour = this.GetComponent<AgentBehavior>();
        context = this.GetComponent<AgentContext>();
		context.Entity = this;
    }

    void Start()
    {
        ABManager.instance.RegisterAgent(this);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
