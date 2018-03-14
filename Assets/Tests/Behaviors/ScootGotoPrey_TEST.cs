﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScootGotoPrey_TEST : MonoBehaviour {
    private const String expected = "goto prey";

    public ABManager behaviorManager;
    public AgentEntity agent;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        ABInstance behaviorInstance = behaviorManager.FindABInstance(agent.Id);
        ABContext context = behaviorManager.CreateABContextFromAgentContext(agent.Context);
		int finalStateId = behaviorInstance.Evaluate(context).getLastTracingInfo().State.Id;
        String finalStateName = behaviorInstance.Model.GetStateName(finalStateId);

        if (finalStateName == expected)
        {
            Debug.Log(this.GetType().Name + " OK");
        } else
        {
            Debug.LogError(this.GetType().Name + " KO ! final state should be '" + expected + "' but it is '" + finalStateName + "'");
        }

        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        Destroy(this);
#endif
    }
}
