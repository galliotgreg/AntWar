﻿using System.Collections;
using System.Collections.Generic;

public class ABLayAction : ABAction {
    public ABLayAction()
    {
        this.type = ActionType.Lay;
        this.parameters = new IABGateOperator[1];
        this.parameters[0] = new AB_TxtGate_Operator();
    }

	#region implemented abstract members of ABAction

	public override ABAction CloneEmpty ()
	{
		return ABActionFactory.CreateAction (ActionType.Lay);
	}

	#endregion
}
