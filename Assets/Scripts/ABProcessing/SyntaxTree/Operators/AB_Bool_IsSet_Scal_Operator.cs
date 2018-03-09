﻿public class AB_Bool_IsSet_Scal_Operator : ABOperator<ABBool>
{
    public AB_Bool_IsSet_Scal_Operator()
    {

        this.Inputs = new ABNode[1];
    }

	protected override ABBool Evaluate(ABContext context)
    {
        ABScalar t1 = null;
        ABNode input1 = Inputs[0];
        t1 = OperatorHelper.Instance.getScalarParam(context, input1);

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