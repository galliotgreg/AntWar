﻿public class AB_Scal_MaxId_ScalTab_Operator : ABOperator<ABScalar>
{
    public AB_Scal_MaxId_ScalTab_Operator()
    {
        this.Inputs = new ABNode[1];
    }

	protected override ABScalar Evaluate(ABContext context)
    {
        ABTable<ABScalar> tab = null;
        ABNode input1 = Inputs[0];

        tab = OperatorHelper.Instance.getTabScalarParam(context, input1);

        //Build then return Result
        ABScalar[] values = new ABScalar[tab.Values.Length];
        if (tab.Values.Length == 0)
        {
            return TypeFactory.CreateEmptyScalar();
        }

        int maxId = 0;
        float maxValue = tab.Values[0].Value;
        for (int i = 1; i < tab.Values.Length; i++)
        {
            if (tab.Values[i].Value > maxValue)
            {
                maxValue = tab.Values[i].Value;
                maxId = i;
            }
        }
        ABScalar result = TypeFactory.CreateEmptyScalar();
        result.Value = maxId;
        return result;
    }
}
