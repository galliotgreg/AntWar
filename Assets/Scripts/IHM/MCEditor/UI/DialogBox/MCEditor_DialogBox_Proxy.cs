﻿using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public abstract class MCEditor_DialogBox_Proxy : MCEditor_DialogBox {
		
	protected MCEditor_Proxy proxy;

	[SerializeField]
	UnityEngine.UI.Text title;

	#region PROPERTIES
	public MCEditor_Proxy Proxy {
		get {
			return proxy;
		}
		protected set {
			proxy = value;
		}
	}

	private string Title{
		set{
			title.text = value;
		}
	}
	#endregion

	// Use this for initialization
	protected void Start () {
		base.Start ();
	}

	// Update is called once per frame
	protected void Update () {
		base.Update ();

		if (Input.GetKeyDown (KeyCode.Return) || Input.GetKeyDown (KeyCode.KeypadEnter)) {
			confirm ();
		}
	}

    protected bool ValidateChars(string newName)
    {
        string pattern = "[^a-zA-Z0-9\\(\\)_]";
        Regex r = new Regex(pattern);
        Match m = r.Match(newName);
        if (m.Success)
        {
            return false;
        }
        return true;
    }

    #region implemented abstract members of MCEditor_DialogBox
    protected override void deactivate (){
		deactivateProxy ();
	}

	public override void confirm ()
	{
		confirmProxy ();
		close ();
	}

	protected abstract void confirmProxy ();
	protected abstract void deactivateProxy ();
	#endregion

	#region INSTANTIATE
	protected static MCEditor_DialogBox_Proxy instantiate ( MCEditor_Proxy proxy, MCEditor_DialogBox_Proxy prefab, Vector3 position, Transform parent ){
		MCEditor_DialogBox_Proxy result = (MCEditor_DialogBox_Proxy)MCEditor_DialogBox.instantiate (prefab, position, parent);
		result.config ( proxy );
		return result;
	}

	private void config( MCEditor_Proxy proxy ){
		this.Proxy = proxy;
		this.Title = dialogTitle();
	}

	protected abstract string dialogTitle ();
	#endregion
}
