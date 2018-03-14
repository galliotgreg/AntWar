﻿
using UnityEngine;

class ScalSizeRefTab_Op_TEST : MonoBehaviour{

    [SerializeField]
    private string symbol = "SsizeR[]";
    [SerializeField]
    private int nbTests = 100;
    // Use this for initialization
    void Start() {
        for (int i = 0; i < nbTests; i++) {
            RandomizeTest();
        }
    }

    private void RandomizeTest() {
        int size = Random.Range(1, 100);
        object[][] tab = new object[size][];

        for (int i = 0; i < tab.Length; i++) {
            tab[i] = new object[size];
        }


        //Build test operator
        ABContext ctx = new ABContext();
        AB_Scal_Size_RefTab_Operator ope = (AB_Scal_Size_RefTab_Operator)ABOperatorFactory.CreateOperator(symbol);
        ABParam<ABTable<ABRef>> arg = (ABParam<ABTable<ABRef>>)ABParamFactory.CreateRefTableParam("const", tab);
        ope.Inputs[0] = arg;

        //Test
        int testValue = (int)ope.EvaluateOperator(ctx).Value;
        int expected = size;
        if (testValue == expected) {
            Debug.Log(this.GetType().Name + " OK");
        }
        else {
            Debug.LogError(this.GetType().Name + " KO ! result for (" + size.ToString() + ") should be '" + expected + "' but it is '" + testValue + "'");
        }
    }

    // Update is called once per frame
    void Update() {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        Destroy(this);
#endif
    }
}

