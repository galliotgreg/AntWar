﻿public class AB_Scal_Size_BoolTab_Operator : ABOperator<ABScalar>
{
    public AB_Scal_Size_BoolTab_Operator() {

        this.Inputs = new ABNode[1];
    }

	protected override ABScalar Evaluate(ABContext context)
    {
        ABTable<ABBool> tab = null;
        ABNode input1 = Inputs[0];

        tab = OperatorHelper.Instance.getTabBoolParam(context, input1);

        //Build then return Result
        ABBool[] values = new ABBool[tab.Values.Length];
        if (tab.Values.Length == 0) {
            return TypeFactory.CreateEmptyScalar();
        }

        int length = 0;
        length = tab.Values.Length;
        ABScalar result = TypeFactory.CreateEmptyScalar();
        result.Value = length;
        return result;
    }
}
