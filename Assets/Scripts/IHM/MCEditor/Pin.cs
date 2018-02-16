﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 GameObject de branchement de transition entre deux états
 */  
public class Pin : DragSelectableProxyGameObject {

    bool isGateOperator = false;
    bool isActionChild = false;

	[SerializeField]
	GameObject transitionPrefab;

    public bool IsGateOperator {
        get {
            return isGateOperator;
        }

        set {
            isGateOperator = value;
        }
    }

    public bool IsActionChild
    {
        get
        {
            return isActionChild;
        }

        set
        {
            isActionChild = value;
        }
    }

    // Use this for initialization
    void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		base.Update ();

		handleSelectedState ();
	}

	#region implemented abstract members of SelectableProxyGameObject
	protected override void select ()
	{
		if (MCEditorManager.instance.Transition_Pin_Start != null ) {
			MCEditorManager.instance.createTransition_setEndPin (this);
		} else {
			MCEditorManager.instance.createTransition_setStartPin ( this );
		}
	}

	protected override void unselect ()
	{
		if (MCEditorManager.instance.Transition_Pin_Start == this) {
			MCEditorManager.instance.createTransition_setStartPin ( null );
		}
	}

	ProxyABTransition auxTransition;
	bool selectNow = false;

	protected void handleSelectedState(){
		if (Selected) {
			if (!selectNow) {
				auxTransition = Instantiate (transitionPrefab).GetComponent<ProxyABTransition> ();
				auxTransition.Collider.enabled = false;

				selectNow = true;
			}

			if (auxTransition != null) {
				// Move
				auxTransition.LineRenderer.SetPosition( 0, this.transform.position );
				Vector3 mouse = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				mouse.z = this.transform.position.z;

				auxTransition.LineRenderer.SetPosition( 1, mouse );
			}
		} else {
			selectNow = false;
			if (auxTransition != null) {
				Destroy (auxTransition.gameObject);
			}
		}
	}
	#endregion
}
