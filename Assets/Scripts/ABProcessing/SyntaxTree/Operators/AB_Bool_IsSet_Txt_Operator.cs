﻿public class AB_Bool_IsSet_Txt_Operator : ABOperator<ABBool>
{
    public AB_Bool_IsSet_Txt_Operator()
    {

        this.Inputs = new ABNode[1];
    }

	protected override ABBool Evaluate(ABContext context)
    {
        ABText t1 = null;
        ABNode input1 = Inputs[0];
        t1 = OperatorHelper.Instance.getTextParam(context, input1);

        ABBool result = TypeFactory.CreateEmptyBool();
        if (t1 != null) {
            result.Value = true;
        }
        else {
            result.Value = false;
        }

        return result;
    }
}