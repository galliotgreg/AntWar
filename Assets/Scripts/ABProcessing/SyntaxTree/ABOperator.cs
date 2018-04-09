﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ABOperator<T> : ABNode, IABOperator
{
    protected ABNode[] inputs;
	OperatorType opType;

    public ABNode[] Inputs
    {
        get
        {
            return inputs;
        }

        set
        {
            inputs = value;
        }
    }

    public virtual string ClassName
    {
        get
        {
            return GetType().ToString();
        }

        set
        {
            throw new System.NotSupportedException();
        }
    }

    public virtual string ViewName
    {
        get
        {
            string opeName = this.ToString();
            char splitter = '_';
            string[] newName = opeName.Split(splitter);
            string newOpeName = "";

            for (int i = 1; i < newName.Length - 1; i++)
            {
                newOpeName += newName[i];
            }            
            return newOpeName;
        }

        set
        {
            throw new System.NotSupportedException();
        }
    }
    public virtual string SymbolName
    {
        get
        {
            throw new System.NotSupportedException();
        }

        set
        {
            throw new System.NotSupportedException();
        }
    }

	public OperatorType OpType {
		get {
			return opType;
		}
		set {
			opType = value;
		}
	}

    public T EvaluateOperator(ABContext context){
		try{
			return Evaluate( context );
		}
		catch( SyntaxTree_MC_Exception opEx ){
			throw new Operator_MC_Exception ( this, opEx );
		}
		catch( System.Exception someEx ){
			throw new Operator_Exception ( this, context, someEx.Message );
		}
	}

	protected abstract T Evaluate(ABContext context);

	public System.Type getOutcomeType ()
	{
		return typeof(T);
		//return ((ABOperator<T>)this).GetType().GetGenericArguments () [0];
	}

	public virtual System.Type getIncomeType( int index ){
		int indexPlusStart = index + 3;

		string[] terms = this.GetType ().ToString ().Split ('_');
		try{ // Txt; Scal; 
			foreach( ParamType t in System.Enum.GetValues( typeof( ParamType ) ) ){
				if( getTypeName( t ) == terms[indexPlusStart] ){
					return ABModel.ParamTypeToType( t );
				}else if( getTypeName( t ) == terms[indexPlusStart] + "le" ){// ScalTab + le => ScalTable
					return ABModel.ParamTypeToType( t );
				}else if( getTypeName( t ) + "Star" == terms[indexPlusStart]  ){// Scal + Star => Star
					return ABStar<ABBool>.generateABStar( t );
				}
			}
		}catch(System.Exception ex){return ABModel.ParamTypeToType( ParamType.None );}
		return ABModel.ParamTypeToType( ParamType.None );
	}

	public bool acceptIncome( int index, System.Type income ){
		System.Type thisType = getIncomeType (index);

		if (thisType == income) {
			return true;
		} else {
			// Check Star Param
			// todo change to abstar.isstar
			if( thisType.IsGenericType && thisType.GetGenericTypeDefinition() == typeof( ABStar<> ) && thisType.GetGenericArguments().Length > 0 ){
				System.Type argType = thisType.GetGenericArguments()[0];

				if (argType == income) {
					// Simple Type
					return true;
				} else {
					// Complex Type
					if( income.GetGenericArguments().Length > 0 && income.GetGenericArguments()[0] == argType ){
						return true;
					}
				}
			}
		}

		return false;
	}

	public static string getTypeName( ParamType type ){
		if( type == ParamType.Scalar ){
			return "Scal";
		}
		else if( type == ParamType.Text ){
			return "Txt";
		}
		else if( type == ParamType.ScalarTable ){
			return "ScalTable";
		}
		else if( type == ParamType.TextTable ){
			return "TxtTable";
		}
		else{
			return type.ToString();
		}
	}

	public virtual IABOperator Clone ()
	{
		return ABOperatorFactory.CreateOperator (this.OpType);
	}
}
