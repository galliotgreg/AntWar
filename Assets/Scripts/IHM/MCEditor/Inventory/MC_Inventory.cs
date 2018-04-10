﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MC_Inventory : MonoBehaviour {

	// Use this for initialization
	protected void Start () {
		
	}

	// Update is called once per frame
	protected void Update () {
		// Adjust content's cell size
		/*float maxHeight = 0;
		for (int i = 0; i < container.transform.childCount; i++) {
			float h = container.transform.GetChild (i).GetComponent<RectTransform> ().rect.height;
			if (h > maxHeight) {
				maxHeight = h;
			}
		}
		UnityEngine.UI.GridLayoutGroup grid = container.GetComponent<UnityEngine.UI.GridLayoutGroup> ();
		grid.cellSize = new Vector2 (grid.cellSize.x, maxHeight);*/
	}

	[SerializeField]
	GameObject container;

	[SerializeField]
	DropArea dropArea;

	#region Load
	[SerializeField]
	protected GameObject itemPrefab;

	protected void setItems (List<System.Object> items){
		// Clear items
		foreach(MC_InventoryItem item in container.GetComponentsInChildren<MC_InventoryItem>()){
			Destroy (item.gameObject);
		}

		// Create new items
		foreach( System.Object item in items ){
			GameObject newItem = Instantiate (itemPrefab);

			MC_InventoryItem inventoryItem = newItem.GetComponent<MC_InventoryItem> ();
			inventoryItem.Item = item;
			inventoryItem.transform.SetParent ( container.transform );

			// adjust position
			newItem.transform.position = container.transform.position;

			inventoryItem.Inventory = this;
			configItem ( inventoryItem );
		}
	}

	protected abstract void configItem ( MC_InventoryItem item );
	public abstract GameObject instantiateProxy ( MC_InventoryItem item );
	#endregion

	#region Drop
	public void DropItem ( GameObject proxy, MC_InventoryItem item ){
		if (dropArea.CanDrop) {
			MCEditorManager.positioningProxy (proxy.GetComponent<MCEditor_Proxy>());
			Drop ( proxy, item );
		} else {
			Destroy (proxy.gameObject);
		}
	}

	protected abstract void Drop( GameObject proxy, MC_InventoryItem item );
	#endregion
}
