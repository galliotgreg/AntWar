﻿public class AB_RefTab_WhereNotEquals_RefTab_Txt_Txt_Operator : ABOperator<ABTable<ABRef>>
{
    public AB_RefTab_WhereNotEquals_RefTab_Txt_Txt_Operator()
    {

        this.Inputs = new ABNode[3];
    }

	protected override ABTable<ABRef> Evaluate(ABContext context)
    {
        //Get s1
        ABTable<ABRef> tab = null;
        ABNode input1 = Inputs[0];
        tab = OperatorHelper.Instance.getTabRefParam(context, input1);

        //Get s2
        ABText text = null;
        ABNode input2 = Inputs[1];
        text = OperatorHelper.Instance.getTextParam(context, input2);

        //Get s2
        ABText b1 = null;
        ABNode input3 = Inputs[2];
        b1 = OperatorHelper.Instance.getTextParam(context, input3);

        //Return
        ABTable<ABRef> result = TypeFactory.CreateEmptyTable<ABRef>();

        for (int i = 0; i < tab.Values.Length; i++) {

            if (tab.Values[i].GetAttr(text.Value) != null && ((ABText)tab.Values[i].GetAttr(text.Value)).Value != b1.Value) {
                result.Values[i] = tab.Values[i];
            }
        }

        return result;
    }
}
