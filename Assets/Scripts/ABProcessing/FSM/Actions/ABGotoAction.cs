﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ABGotoAction : ABAction
{
    public ABGotoAction()
    {
        this.type = ActionType.Goto;
        this.parameters = new IABGateOperator[1];
        this.parameters[0] = new AB_VecGate_Operator();
    }

	#region implemented abstract members of ABAction

	public override ABAction CloneEmpty ()
	{
		return ABActionFactory.CreateAction (ActionType.Goto);
	}

	#endregion
}
