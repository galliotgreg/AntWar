﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCEditor_DialogBoxManager : MonoBehaviour {

	#region SINGLETON
	// The static instance of the Singleton for external access
	public static MCEditor_DialogBoxManager instance = null;

	// Enforce Singleton properties
	void Awake()
	{
		//Check if instance already exists and set it to this if not
		if (instance == null)
		{
			instance = this;
		}

		//Enforce the unicity of the Singleton
		else if (instance != this)
		{
			Destroy(gameObject);
		}
	}
	#endregion

	[SerializeField]
	RectTransform container;

	// States
	[SerializeField]
	MCEditor_DialogBox_State_Name stateName_Prefab;
	// Actions
	[SerializeField]
	MCEditor_DialogBox_Action_Name actionName_Prefab;
	// Params
	[SerializeField]
	MCEditor_DialogBox_Param_String paramText_Prefab;
	[SerializeField]
	MCEditor_DialogBox_Param_Scalar paramScalar_Prefab;
	[SerializeField]
	MCEditor_DialogBox_Param_Bool paramBool_Prefab;
	[SerializeField]
	MCEditor_DialogBox_Param_Color paramColor_Prefab;
	[SerializeField]
	MCEditor_DialogBox_Param_Vec paramVec_Prefab;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public MCEditor_DialogBox_Param instantiateValue( ProxyABParam param, Vector2 position ){
		Vector3 pos3D = new Vector3 ( position.x + 1, position.y + 1, container.position.z );
		if (param.AbParam is ABTextParam) {
			return MCEditor_DialogBox_Param_String.instantiate( param, paramText_Prefab, pos3D, container);
		}
		else if (param.AbParam is ABScalParam) {
			return MCEditor_DialogBox_Param_Scalar.instantiate( param, paramScalar_Prefab, pos3D, container);
		}
		else if (param.AbParam is ABBoolParam) {
			return MCEditor_DialogBox_Param_Bool.instantiate( param, paramBool_Prefab, pos3D, container);
		}
		else if (param.AbParam is ABColorParam) {
			return MCEditor_DialogBox_Param_Color.instantiate( param, paramColor_Prefab, pos3D, container);
		}
		else if (param.AbParam is ABVecParam) {
			return MCEditor_DialogBox_Param_Vec.instantiate( param, paramVec_Prefab, pos3D, container);
		}
		return null;
	}

	public MCEditor_DialogBox_State_Name instantiateStateName( ProxyABState state, Vector2 position ){
		Vector3 pos3D = new Vector3 ( position.x + 1, position.y + 1, container.position.z );
		return (MCEditor_DialogBox_State_Name)MCEditor_DialogBox_State.instantiate( state, stateName_Prefab, pos3D, container );
	}
	public MCEditor_DialogBox_Action_Name instantiateActionName( ProxyABAction action, Vector2 position ){
		Vector3 pos3D = new Vector3 ( position.x + 1, position.y + 1, container.position.z );
		return (MCEditor_DialogBox_Action_Name)MCEditor_DialogBox_Action.instantiate( action, actionName_Prefab, pos3D, container );
	}
}
