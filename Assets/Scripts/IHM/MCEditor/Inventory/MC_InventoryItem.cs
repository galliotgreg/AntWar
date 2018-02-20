﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MC_InventoryItem : MonoBehaviour, IDragObjectActivator {

	// Use this for initialization
	void Start () {
		RectTransform rect = GetComponent<RectTransform>();
		rect.localScale = new Vector3 (1, 1, 1);
	}

	bool click = false;
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			if (hover) {
				activateClick ();
			}
		}
	}

	protected ABNode item;

	public ABNode Item {
		get {
			return item;
		}
		set {
			item = value;
		}
	}

	MC_Inventory inventory;

	public MC_Inventory Inventory {
		get {
			return inventory;
		}
		set {
			inventory = value;
		}
	}

	[SerializeField]
	UnityEngine.UI.Text text;

	public UnityEngine.UI.Text Text {
		get {
			return text;
		}
		set {
			text = value;
		}
	}

	[SerializeField]
	DragObject dragObjectComponent;

	void activateClick(){
		GameObject proxy = inventory.instantiateProxy (this);

		dragObjectComponent.startDrag (proxy, this);
	}

	bool hover = false;
	void OnMouseEnter(){
		hover = true;
	}
	void OnMouseExit(){
		hover = false;
	}

	#region IDragObjectActivator implementation

	public void endDrag (GameObject droppedObject)
	{
		inventory.DropItem ( droppedObject, this );
	}

	#endregion
}
