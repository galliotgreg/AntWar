﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

using System.Runtime.InteropServices;
using System.Text;

public class MCEditorManager : MonoBehaviour {

    /// <summary>
    /// The static instance of the Singleton for external access
    /// </summary>
    public static MCEditorManager instance = null;

    //Templates & Prefabs
    [SerializeField]
    private ProxyABState statePrefab;
    [SerializeField]
    private ProxyABTransition transitionPrefab;
    [SerializeField]
    private Pin pinPrefab;
    [SerializeField]
    private ProxyABOperator operatorPrefab;
    [SerializeField]
    private ProxyABParam parameterPrefab;
    [SerializeField]
    private ProxyABAction actionPrefab;

	[SerializeField]
	private GameObject MC_Container;	// Parent for the MC
	Transform MCparent;

    private ABModel abModel;

    // Save function utils
    private int idNodeSyntTree;
    private int idNodeInputPin;
    private ABOperatorFactory opeFactory = new ABOperatorFactory();
    private ABParamFactory paramFactory = new ABParamFactory();
    private Dictionary<string, string> operatorDictionary = new Dictionary<string, string>();
    private Dictionary<string, string> paramDictionary = new Dictionary<string, string>();


    private Dictionary<ABState,ProxyABState> statesDictionnary;    
    private Dictionary<ABState, ProxyABAction> actionsDictionnary;

    private List<ProxyABState> proxyStates;
    private List<ProxyABAction> proxyActions;
    private List<ProxyABTransition> proxyTransitions;
    private List<Pin> pins; //ProxyABGateOperator    
    private List<ProxyABOperator> proxyOperators;
    private List<ProxyABParam> proxyParams;

    [SerializeField]
    private string MC_OrigFilePath = "Assets/Inputs/Test/GREG_TRANS_STATE_STATE_TEST.csv";/* siu_scoot_behavior_LOAD_SAVE_TEST.csv"; /*ref_table_Test.txt"; /*siu_scoot_behavior_LOAD_TEST.csv";*/

    /** START TEST SAVE**/
    ProxyABAction abAction = null;
    ProxyABAction abAction2 = null;
    ProxyABState abState = null;
    ProxyABState abState2 = null;
    ProxyABParam abParam = null;
    ProxyABOperator aBOperator = null;
    ProxyABOperator aBOperator2 = null;

    /** END TEST SAVE**/

    void Awake()
    {
        //Check if instance already exists and set it to this if not
        if (instance == null)
        {
            instance = this;
        }

        //Enforce the unicity of the Singleton
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }    

    public ABModel AbModel
    {
        get
        {
            return abModel;
        }
    }

    // Use this for initialization
    void Start()
    {
        /** INITIALISATION **/
        proxyStates = new List<ProxyABState>();
        proxyTransitions = new List<ProxyABTransition>();
        pins = new List<Pin>(); //ProxyABGateOperator
        proxyParams = new List<ProxyABParam>(); //ProxyABParam
        proxyOperators = new List<ProxyABOperator>();
        proxyActions = new List<ProxyABAction>();
        actionsDictionnary = new Dictionary<ABState, ProxyABAction>();
        statesDictionnary = new Dictionary<ABState, ProxyABState>();
        
        //usefull for save function
        opeFactory.CreateDictionnary();
        paramFactory.CreateDictionnary();

        operatorDictionary = opeFactory.TypeStringToString;
        paramDictionary = paramFactory.TypeStringToString;

		MCparent = (MC_Container == null ? this.transform : MC_Container.transform);

        /** LOAD MODEL AND CREATE PROXY OBJECTS **/
        SetupModel();
    }

    private void Update()
    {
        /**START TEST SAVE**/
        if (Input.GetKeyDown(KeyCode.S))
        {
            Save_MC();
        } else if (Input.GetKeyDown(KeyCode.O))
        {
            CreateOperator();
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            CreateParam();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            abState = Instantiate<ProxyABState>(statePrefab);
			proxyStates.Add (abState);
            abState2 = Instantiate<ProxyABState>(statePrefab);
			proxyStates.Add (abState2);
        } else if (Input.GetKeyDown(KeyCode.R))
        {
            CreateTransition(abState.GetComponentInChildren<Pin>(), abState2.GetComponentInChildren<Pin>());
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            abState = Instantiate<ProxyABState>(statePrefab);
            abAction = Instantiate<ProxyABAction>(actionPrefab);
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            CreateTransition(abState.GetComponentInChildren<Pin>(), abAction.GetComponentInChildren<Pin>());
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            abParam = Instantiate<ProxyABParam>(parameterPrefab);
            aBOperator = Instantiate<ProxyABOperator>(operatorPrefab);
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            CreateTransition(abParam.GetComponentInChildren<Pin>(), aBOperator.GetComponentInChildren<Pin>());
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            aBOperator = Instantiate<ProxyABOperator>(operatorPrefab);
            aBOperator2 = Instantiate<ProxyABOperator>(operatorPrefab);
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            CreateTransition(aBOperator.GetComponentInChildren<Pin>(), aBOperator2.GetComponentInChildren<Pin>());
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            aBOperator = Instantiate<ProxyABOperator>(operatorPrefab);
            abAction = Instantiate<ProxyABAction>(actionPrefab);
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            CreateTransition(aBOperator.GetComponentInChildren<Pin>(), abAction.GetComponentInChildren<Pin>());
        }

        /**END TEST SAVE**/
		/**Delete Transition**/
		else if (Input.GetKeyDown(KeyCode.D))
		{
			this.deleteSelectedTransition ();
		}
    }

    private void SetupModel()
    {
        abModel = LoadMC();
        LoadProxyStates();
        LoadProxyTransitions();
    }
    
    ABModel LoadMC()
    {
        ABModel model = new ABModel();

        /**** START TODO ****/
        //TODO : Récuperer le ABModel en Utilisant le AppContextManager et remplacer path
		model = ABManager.instance.LoadABModelFromFile(MC_OrigFilePath);
        /**** END TODO ****/

        return model;
    }

	#region LOAD MODEL FUNCTIONS
    void LoadProxyStates() {

        ProxyABState proxyState;
        ProxyABAction proxyAction;

        foreach (ABState state in this.AbModel.States)
        {
			// Actions
            if (state.Action != null)
            {
				proxyAction = instantiateAction (state, calculateActionPosition (MCparent), actionPrefab, pinPrefab, MCparent );
				this.registerAction ( state, proxyAction );

				// Create SyntaxTrees
				List<Pin> pins = getPins( proxyAction.gameObject, Pin.PinType.ActionParam );
				if (state.Action.Parameters != null) {
					for (int i = 0; i < state.Action.Parameters.Length; i++) {
						IABGateOperator param = state.Action.Parameters [i];

						Pin start = pins [i];
						registerPin (start);

						// create syntaxTree for each Param
						foreach (ABNode node in param.Inputs) {
							Pin end = RecNodeSynthTree (node);
                            if (end != null)
                            {
							instantiateTransition ( start, end, false, transitionPrefab, pinPrefab, MCparent.transform );
                            }
                        }
					}
				}
            }
			// States
            else {
				proxyState = instantiateState (state, abModel.InitStateId == state.Id, calculateStatePosition (MCparent), statePrefab, pinPrefab, MCparent.transform);
				this.registerState ( state, proxyState );
            }
        }
    }

    //Load syntaxe Tree in a recursive way
	// return the outcome pin
    Pin RecNodeSynthTree(ABNode node)
    {
        if (node is IABOperator)
        {
			ProxyABOperator ope = instantiateOperator( (IABOperator)node, true, calculateOperatorPosition( MCparent ), operatorPrefab, pinPrefab, MCparent );
			registerOperator (ope);
		    
			// Creating params trees
			List<Pin> pins = getPins( ope.gameObject, Pin.PinType.OperatorIn );
			for(int i=0; i<((IABOperator)node).Inputs.Length; i++){
				ABNode inputNode = ((IABOperator)node).Inputs [i];
				Pin start = pins[i];

                if(inputNode == null)
                {
                    break;
                }

                Pin end = RecNodeSynthTree(inputNode);

                if(end != null)
                {
					instantiateTransition ( start, end, false, transitionPrefab, pinPrefab, MCparent.transform );
                }
            }

			// Outcome
			return getPins( ope.gameObject, Pin.PinType.OperatorOut )[0];
        }
        else if (node is IABParam)
        {
			ProxyABParam param = instantiateParam ( (IABParam)node, true, calculateParamPosition( MCparent ), parameterPrefab, pinPrefab, MCparent );
			registerParam ( param );

			return getPins (param.gameObject, Pin.PinType.Param) [0];
        }
		return null;
    }

    void LoadProxyTransitions()
    {
        ProxyABTransition proxyABTransition;

        for (int i = 0; i < AbModel.Transitions.Count; i++) {
			List<Pin> pinList = LoadPinsStates(i);
			proxyABTransition = instantiateTransition ( pinList[0], pinList[1], true, transitionPrefab, pinPrefab, MCparent );
			// todo ATTENTION
			proxyABTransition.Transition = AbModel.Transitions [i];

            if (AbModel.Transitions[i].Condition != null) {                
                Pin end = RecNodeSynthTree(AbModel.Transitions[i].Condition.Inputs[0]);
				instantiateTransition ( proxyABTransition.Condition, end, false, transitionPrefab, pinPrefab, MCparent );
            }
        }         
    }

    List<Pin> LoadPinsStates(int curTransition)
    {
        List<Pin> pinList = new List<Pin>();

        ProxyABState startState = statesDictionnary[AbModel.Transitions[curTransition].Start];        

		// Instantiate OutCome Pin in the state
		pinList.Add( instantiatePin( Pin.PinType.TransitionOut, calculatePinPosition( startState.AbState, startState.gameObject, true, curTransition ), pinPrefab, startState.transform ) );

        if (statesDictionnary.ContainsKey(AbModel.Transitions[curTransition].End)) {
            ProxyABState endState = statesDictionnary[AbModel.Transitions[curTransition].End];

			// Recovery income Pin
			pinList.Add( getPins ( endState.gameObject, Pin.PinType.TransitionIn )[0] );
        }
        else if (actionsDictionnary.ContainsKey(AbModel.Transitions[curTransition].End)) {

            ProxyABAction endState = actionsDictionnary[AbModel.Transitions[curTransition].End];

			pinList.Add( getPins ( endState.gameObject, Pin.PinType.TransitionIn )[0] );
        }
        
        return pinList;
    }
	#endregion

	#region PROXY CREATION FUNCTIONS

	public static List<Pin> getPins( GameObject obj, Pin.PinType pinType ){
		List<Pin> result = new List<Pin> ();

		Pin[] pins = obj.GetComponentsInChildren<Pin>();

		foreach (Pin p in pins) {
			if (p.Pin_Type == pinType) {
				result.Add ( p );
			}
		}

		return result;
	}

	#region ACTION
	public static ProxyABAction instantiateAction( ABState state, Vector3 position, ProxyABAction prefab, Pin pinPrefab, Transform parent ){
		ProxyABAction result = Instantiate<ProxyABAction>( prefab, parent );
		result.IsLoaded = true;
		result.transform.position = position;

		Text actionName = result.GetComponentInChildren<Text>();
		actionName.text = state.Name;

		result.GetComponent<ProxyABAction>().AbState = state;

		// Create Pins
		if (state.Action.Parameters != null) {
			foreach (IABGateOperator param in state.Action.Parameters) {
				Pin start = instantiatePin (Pin.PinType.ActionParam, calculatePinPosition (result), pinPrefab, result.transform);
			}
		}

		// Income
		instantiatePin (Pin.PinType.TransitionIn, calculatePinPosition (result), pinPrefab, result.transform);

		return result;
	}

	public void registerAction( ABState state, ProxyABAction action ){
		proxyActions.Add(action);
		actionsDictionnary.Add(state, action);
	}

	public static Vector3 calculateActionPosition( Transform parent ){
		return new Vector3(UnityEngine.Random.Range(-5, 5),UnityEngine.Random.Range(-5, 5), parent.position.z);
	}
	#endregion

	#region STATE
	public static ProxyABState instantiateState( ABState state, bool init, Vector3 position, ProxyABState prefab, Pin pinPrefab, Transform parent ){
		ProxyABState result = Instantiate<ProxyABState>(prefab, parent);
		result.IsLoaded = true;
		result.transform.position = position;

		Text stateName = result.GetComponentInChildren<Text>();
		stateName.text = state.Name;

		result.GetComponent<ProxyABState>().AbState = state;

		// Income Pin
		if (!init) {
			instantiatePin (Pin.PinType.TransitionIn, calculatePinPosition (result), pinPrefab, result.transform);
		}

		return result;
	}

	public void registerState( ABState state, ProxyABState proxyState ){
		proxyStates.Add( proxyState );
		statesDictionnary.Add(state, proxyState);
	}

	public static Vector3 calculateStatePosition( Transform parent ){
		return new Vector3(UnityEngine.Random.Range(-5, 5),UnityEngine.Random.Range(-5, 5), parent.position.z);
	}
	#endregion

	#region OPERATOR
	public static ProxyABOperator instantiateOperator( IABOperator operatorObj, bool isLoaded, Vector3 position, ProxyABOperator prefab, Pin pinPrefab, Transform parent ){
		ProxyABOperator result = Instantiate<ProxyABOperator> (prefab, parent);
		result.IsLoaded = isLoaded;
		result.transform.position = position;
		result.AbOperator = operatorObj;
		SetNodeName( result.gameObject, (ABNode)operatorObj );

		// Create Pins
		foreach(ABNode inputNode in operatorObj.Inputs)
		{
			Pin start = instantiatePin (Pin.PinType.OperatorIn, calculatePinPosition (result), pinPrefab, result.transform);
		}

		// Outcome pin
		instantiatePin( Pin.PinType.OperatorOut, calculatePinPosition (result), pinPrefab, result.transform );

		return result;
	}

	public static Vector3 calculateOperatorPosition( Transform parent ){
		return new Vector3(UnityEngine.Random.Range(-5, 5),UnityEngine.Random.Range(-5, 5), parent.position.z);
	}

	public void registerOperator( ProxyABOperator proxyOperator ){
		proxyOperators.Add( proxyOperator );
	}
	#endregion

	#region PARAM
	public static ProxyABParam instantiateParam( IABParam paramObj, bool isLoaded, Vector3 position, ProxyABParam prefab, Pin pinPrefab, Transform parent ){
		ProxyABParam result = Instantiate<ProxyABParam> (prefab, parent);
		result.IsLoaded = isLoaded;
		result.transform.position = position;

		// Set text
		Text paramName = result.GetComponentInChildren<Text>();            
		paramName.text = GetParamValue( (ABNode)paramObj );

		// Outcome pin
		instantiatePin( Pin.PinType.Param, calculatePinPosition (result), pinPrefab, result.transform );

		return result;
	}

	public static Vector3 calculateParamPosition( Transform parent ){
		return new Vector3(UnityEngine.Random.Range(-5, 5),UnityEngine.Random.Range(-5, 5), parent.position.z);
	}

	public void registerParam( ProxyABParam proxyParam ){
		proxyParams.Add( proxyParam );
	}
	#endregion

	#endregion

	#region PIN
	/*public Pin CreatePinState(ABState state, Transform state_transform, bool isAction, bool isStart, [Optional] int curTransition){
		if (isAction) {
			// Action
			ProxyABAction action = state_transform.GetComponent<ProxyABAction> ();
			Pin pin = instantiatePin (Pin.PinType.ActionParam, calculatePinPosition (action), pinPrefab, action.transform);
			return pin;
		} else {
			// State
			// todo verify isstart : in/out
			Pin pin = instantiatePin ( Pin.PinType.TransitionOut, calculatePinPosition ( state, state_transform.gameObject, isStart, curTransition ), pinPrefab, state_transform );
			return pin;
		}
	}*/

	public static Pin instantiatePin( Pin.PinType pinType, Vector3 position, Pin prefab, Transform parent ){
		Pin result = Instantiate<Pin> (prefab, parent);
		result.Pin_Type = pinType;
		result.transform.position = position;

		return result;
	}

	public void registerPin( Pin pin ){
		pins.Add (pin);
	}

	public static Vector3 calculatePinPosition( ProxyABAction parent ){
		float radius = parent.transform.localScale.y / 2;
		return new Vector3 (parent.transform.position.x, parent.transform.position.y + radius, parent.transform.position.z);
	}

	public static Vector3 calculatePinPosition( ProxyABState parent ){
		float radius = parent.transform.localScale.y / 2;
		return new Vector3 (parent.transform.position.x, parent.transform.position.y + radius, parent.transform.position.z);
	}

	public static Vector3 calculatePinPosition( ABState state, GameObject stateParent, bool transitionOut, int curTransition = 0 ){
		float radius = stateParent.transform.localScale.y / 2;
		if (transitionOut) {
			return new Vector3 (
				stateParent.transform.position.x + (radius * Mathf.Cos (curTransition * (2 * Mathf.PI) / Math.Max (1, state.Outcomes.Count))),
				stateParent.transform.position.y + (radius * Mathf.Sin (curTransition * (2 * Mathf.PI) / Math.Max (1, state.Outcomes.Count))),
				stateParent.transform.position.z
			);
		} else {
			return new Vector3(
				stateParent.transform.position.x + (radius * Mathf.Cos (curTransition * (2 * Mathf.PI) / Math.Max (1, state.Outcomes.Count))),
				stateParent.transform.position.y + (radius * Mathf.Sin (curTransition * (2 * Mathf.PI) / Math.Max (1, state.Outcomes.Count))),
				stateParent.transform.position.z
			);
		}
	}

	public static Vector3 calculatePinPosition( ProxyABTransition parent ){
		return parent.transform.position;
	}

	public static Vector3 calculatePinPosition( ProxyABOperator parent ){
		int childCount = parent.transform.childCount;
		float radius = parent.transform.localScale.y / 2;
		return new Vector3(parent.transform.position.x + (radius * Mathf.Cos(childCount * (2 * Mathf.PI) / 4)),
							parent.transform.position.y + (radius* Mathf.Sin(childCount * (2 * Mathf.PI) / 4)),
							parent.transform.position.z
		);
	}

	public static Vector3 calculatePinPosition( ProxyABParam parent ){
		int childCount = parent.transform.childCount;
		float radius = parent.transform.localScale.y / 2;
		return new Vector3(parent.transform.position.x + (radius * Mathf.Cos(childCount * (2 * Mathf.PI) / 4)),
			parent.transform.position.y + (radius* Mathf.Sin(childCount * (2 * Mathf.PI) / 4)),
			parent.transform.position.z
		);
	}
	#endregion

	#region TRANSITION
	public static ProxyABTransition instantiateTransition( Pin start, Pin end, bool createCondition, ProxyABTransition transitionPrefab, Pin pinPrefab, Transform parent ){
		ProxyABTransition proxyABTransition = Instantiate<ProxyABTransition>(transitionPrefab, parent);

		proxyABTransition.StartPosition = start;
		proxyABTransition.EndPosition = end;

		if (createCondition) {
			addConditionPin ( proxyABTransition, pinPrefab );
		}

		// TODO register?
		return proxyABTransition;
	}

	public static void addConditionPin( ProxyABTransition proxyABTransition, Pin pinPrefab ){
		Pin conditionPin = instantiatePin( Pin.PinType.Condition, calculatePinPosition( proxyABTransition ), pinPrefab, proxyABTransition.transform );
		proxyABTransition.Condition = conditionPin;
	}
	#endregion

	#region SAVE FUNCTION
    void Save_Ope_Param(int idNodeInput, int idNodeInputPin, ABNode node, StringBuilder syntTreeContent)
    {
        int idParentnode = idNodeSyntTree;       
        if (idNodeSyntTree == 0)
        {
            if (node is IABOperator)
            {
                string type = operatorDictionary[((IABOperator)node).GetType().ToString()];

                syntTreeContent.AppendLine(idNodeSyntTree + ",operator{" + type + "},");
                idNodeSyntTree++;
                idNodeInputPin = 0;
                foreach (ABNode input in ((IABOperator)node).Inputs)
                {
                    /**Recursive function**/
                    Save_Ope_Param(idParentnode, idNodeInputPin, input, syntTreeContent);
                    idNodeInputPin++;
                }
            }
            else if (node is IABParam)
            {
                syntTreeContent.AppendLine(idNodeSyntTree + ",param{" + ((IABParam)node).Identifier + "},");
                idNodeSyntTree++;
            }
        }
        else
        {            
            if (node is IABOperator)
            {
                string type = "";
                if (!operatorDictionary.ContainsKey(((IABOperator)node).GetType().ToString()))
                {
                    Debug.LogError(((IABOperator)node).GetType().ToString() + " n'est pas dans la le dictionnaire des opérateurs. Vérifier l'orthographe Dans le fichier ABOperatorFactory");
                } else
                {
                    type = operatorDictionary[((IABOperator)node).GetType().ToString()];
                }
                syntTreeContent.AppendLine(idParentnode + ",operator{" + type + "}" + "," + idNodeInput + "->" + idNodeInputPin);
                idNodeSyntTree++;
                idNodeInputPin = 0;
                foreach (ABNode input in ((IABOperator)node).Inputs)
                {
                    /**Recursive function**/
                    Save_Ope_Param(idParentnode, idNodeInputPin, input, syntTreeContent);
                    idNodeInputPin++;
                }
            }
            else if (node is IABParam)
            {
                string value = GetParamValue(node);
                string type = "";


                if (!paramDictionary.ContainsKey(GetParamType(node)))
                {
                    Debug.LogError(GetParamType(node) + " n'est pas dans la le dictionnaire des parameters. Vérifier l'orthographe Dans le fichier ABParamFactory");
                } else
                {
                    type = paramDictionary[GetParamType(node)];
                }

                if (((IABParam)node).Identifier != "const")
                {
                    syntTreeContent.AppendLine(idNodeSyntTree + ",param{" + type + ":" + ((IABParam)node).Identifier + "}" + "," + idNodeInput + "->" + idNodeInputPin);
                }
                else
                {
                    syntTreeContent.AppendLine(idNodeSyntTree + ",param{" + ((IABParam)node).Identifier + " " + type + "=" + value + "}" + "," + idNodeInput + "->" + idNodeInputPin);
                    idNodeSyntTree++;
                }
            }
        }              
    }

    void Save_MC()
    {
        string csvpath = "Assets/Inputs/Test/siu_scoot_behavior_SAVE_LOAD_SAVE_TEST.csv";
        StringBuilder csvcontent = new StringBuilder();
        List<StringBuilder> syntTrees = new List<StringBuilder>();

        csvcontent.AppendLine("States,Name,Type");
        foreach (ABState state in abModel.States)
        {
            if(state.Action != null)
            {
                csvcontent.AppendLine(state.Id + "," + state.Name + "," + "trigger{"+state.Action.Type.ToString().ToLower()+"}");                
                if (state.Action.Parameters[0].Inputs[0] != null)
                {
                    StringBuilder syntTreeContent = new StringBuilder();
                    syntTreeContent.AppendLine("Syntax Tree,output,");
                    syntTreeContent.AppendLine("1,"+state.Name +"->0"+",");
                    syntTreeContent.AppendLine("Nodes,Type,output (Node -> Input)");

                    idNodeSyntTree = 0;
                    foreach(ABNode node in state.Action.Parameters[0].Inputs)
                    {
                        Save_Ope_Param(idNodeSyntTree, idNodeInputPin, node, syntTreeContent);
                    }
                    syntTreeContent.AppendLine(",,");
                    syntTrees.Add(syntTreeContent);
                }
            }
            else
            {
                if (state.Id == 0)
                {
                    csvcontent.AppendLine(state.Id + "," + state.Name + ",Init");
                } else
                {
                    csvcontent.AppendLine(state.Id + "," + state.Name + ",Inter");
                }                
            }               
        }
        csvcontent.AppendLine(",,");
        csvcontent.AppendLine("Transitions,Start State,End State");
        foreach(ABTransition trans in abModel.Transitions)
        {
            csvcontent.AppendLine(trans.Id + "," + trans.Start.Name + "," + trans.End.Name);
            if (trans.Condition != null)
            {
                if (trans.Condition.Inputs[0] != null)
                {
                    StringBuilder syntTreeContent = new StringBuilder();
                    syntTreeContent.AppendLine("Syntax Tree,output,");
                    syntTreeContent.AppendLine("1," + trans.Id + ",");
                    syntTreeContent.AppendLine("Nodes,Type,output (Node -> Input)");

                    idNodeSyntTree = 0;
                    foreach (ABNode node in trans.Condition.Inputs)
                    {
                        Save_Ope_Param(idNodeSyntTree, idNodeInputPin, node, syntTreeContent);
                    }
                    syntTreeContent.AppendLine(",,");
                    syntTrees.Add(syntTreeContent);
                }
            }
        }
        csvcontent.AppendLine(",,");
        File.Delete(csvpath);
        File.AppendAllText(csvpath, csvcontent.ToString());

        foreach (StringBuilder content in syntTrees)
        {
            File.AppendAllText(csvpath, content.ToString());
        }
        Debug.Log("Save MC");
    }

	string GetParamType(ABNode node)
	{
		string type = "";        
		if (node is ABParam<ABText>)
		{
			type = ((ABParam<ABText>)node).Value.ToString();
		}
		else if (node is ABParam<ABVec>)
		{
			type = ((ABParam<ABVec>)node).Value.ToString();
		}
		else if (node is ABParam<ABBool>)
		{
			type = ((ABParam<ABBool>)node).Value.ToString();
		}
		else if (node is ABParam<ABRef>)
		{
			type = ((ABParam<ABRef>)node).Value.ToString();
		}
		else if (node is ABParam<ABColor>)
		{
			type = ((ABParam<ABColor>)node).Value.ToString();
		}
		else if (node is ABParam<ABScalar>)
		{
			type = ((ABParam<ABScalar>)node).Value.ToString();
		}

		else if (node is ABParam<ABTable<ABVec>>)
		{
			type = ((ABParam<ABTable<ABVec>>)node).Value.ToString();
		}
		else if (node is ABParam<ABTable<ABBool>>)
		{
			type = ((ABParam<ABTable<ABBool>>)node).Value.ToString();
		}
		else if (node is ABParam<ABTable<ABScalar>>)
		{
			type = ((ABParam<ABTable<ABScalar>>)node).Value.ToString();
		}
		else if (node is ABParam<ABTable<ABText>>)
		{
			type = ((ABParam<ABTable<ABText>>)node).Value.ToString();
		}
		else if (node is ABParam<ABTable<ABColor>>)
		{
			type = ((ABParam<ABTable<ABColor>>)node).Value.ToString();
		}
		else if (node is ABParam<ABTable<ABRef>>)
		{
			type = ((ABParam<ABTable<ABRef>>)node).Value.ToString();
		}

		return type;
	}

	static string GetParamValue(ABNode node)
	{
		string text = "";
		if (node is ABParam<ABText>)
		{
			text = ((ABParam<ABText>)node).Value.Value.ToString();
		}
		else if (node is ABParam<ABVec>)
		{
			text = ((ABParam<ABVec>)node).Value.X.ToString() +";"+ ((ABParam<ABVec>)node).Value.Y.ToString();
		}
		else if (node is ABParam<ABBool>)
		{
			text = ((ABParam<ABBool>)node).Value.Value.ToString();
		}
		else if (node is ABParam<ABRef>)
		{
			text = ((ABParam<ABRef>)node).Value.ToString();
		}
		else if (node is ABParam<ABColor>)
		{
			text = ((ABParam<ABColor>)node).Value.Value.ToString();
		}
		else if (node is ABParam<ABScalar>)
		{
			text = ((ABParam<ABScalar>)node).Value.Value.ToString();
		}
		return text;
	}
	#endregion

    #region EDITOR FUNCTIONS
    void CreateTransition(Pin start, Pin end)
	{
		ProxyABTransition trans = instantiateTransition ( start, end, false, transitionPrefab, pinPrefab, MCparent.transform );

		ProxyABAction startActionParent;
		ProxyABState startStateParent;
		ProxyABAction endActionParent;
		ProxyABState endStateParent;

		ProxyABOperator startOpeParent;
		ProxyABParam startParamParent;
		ProxyABParam endParamParent;
		ProxyABOperator endOpeParent;

		// Action; State; Param; Operator; Transition

		// IN
		// action 	<- state | operator | param(gate)
		// state 	<- state
		// operator	<- param | operator
		// param 	<- 
		// transition <- operator | param(gate)

		// OUT
		// action 	->
		// state 	-> action | state
		// operator	-> action | operator | transition
		// param	-> operator | action(gate) | transition(gate)
		// transition -> 

		int transitionId = -1;

		// ACTION
		// action 	<- state | operator | param(gate)
		// action 	->
		if (start.Pin_Type == Pin.PinType.ActionParam)
        {
            startActionParent = start.GetComponentInParent<ProxyABAction>();

			if (end.Pin_Type == Pin.PinType.OperatorIn || end.Pin_Type == Pin.PinType.OperatorOut)
            {
                endOpeParent = end.GetComponentInParent<ProxyABOperator>();
				//start.IsGateOperator = true;
				start.Pin_Type = Pin.PinType.OperatorIn;
                startActionParent.AbState.Action.Parameters[0].Inputs[0] = (ABNode)endOpeParent.AbOperator;               
			} else if (end.Pin_Type == Pin.PinType.Param)
            {
                endParamParent = end.GetComponentInParent<ProxyABParam>();
                //start.IsGateOperator = true;
				start.Pin_Type = Pin.PinType.OperatorIn;
                startActionParent.AbState.Action.Parameters[0].Inputs[0] = (ABNode)endParamParent.AbParam;
            }
			else if (end.Pin_Type == Pin.PinType.ActionParam)
            {
                endActionParent = end.GetComponentInParent<ProxyABAction>();                
                AbModel.LinkStates(startActionParent.AbState.Name, endActionParent.AbState.Name);
				addConditionPin ( trans, pinPrefab );
				transitionId = AbModel.LinkStates(startActionParent.AbState.Name, endActionParent.AbState.Name);
                trans.Transition = AbModel.getTransition(transitionId);
            }
            else //State case
            {
                endStateParent = end.GetComponentInParent<ProxyABState>();
                AbModel.LinkStates(startActionParent.AbState.Name, endStateParent.AbState.Name);
				addConditionPin ( trans, pinPrefab );
				transitionId = AbModel.LinkStates(startActionParent.AbState.Name, endStateParent.AbState.Name);
                trans.Transition = AbModel.getTransition(transitionId);
            }

        }
		else if (start.Pin_Type == Pin.PinType.OperatorIn || start.Pin_Type == Pin.PinType.OperatorOut)
        {
            startOpeParent = start.GetComponentInParent<ProxyABOperator>();
			if (end.Pin_Type == Pin.PinType.OperatorIn || end.Pin_Type == Pin.PinType.OperatorOut)
            {
                endOpeParent = end.GetComponentInParent<ProxyABOperator>();
                //start.IsGateOperator = true;
				start.Pin_Type = Pin.PinType.OperatorIn;
                startOpeParent.Inputs[startOpeParent.Inputs.Length-1] = (ABNode)endOpeParent.AbOperator;
                ((ABNode)endOpeParent.AbOperator).Output = (ABNode)startOpeParent.AbOperator;
            }
			else if (end.Pin_Type == Pin.PinType.Param)
            {
                endParamParent = end.GetComponentInParent<ProxyABParam>();
                //start.IsGateOperator = true;
				start.Pin_Type = Pin.PinType.OperatorIn;
                startOpeParent.Inputs[startOpeParent.Inputs.Length-1] = (ABNode)endParamParent.AbParam;
            }
			else if (end.Pin_Type == Pin.PinType.ActionParam)
            {
                endActionParent = end.GetComponentInParent<ProxyABAction>();
                //end.IsGateOperator = true;
				end.Pin_Type = Pin.PinType.OperatorIn;
                endActionParent.AbState.Action.Parameters[0].Inputs[0] = (ABNode)startOpeParent.AbOperator;
            }
        }
		else if (start.Pin_Type == Pin.PinType.Param)
        {
            startParamParent = start.GetComponentInParent<ProxyABParam>();
			if (end.Pin_Type == Pin.PinType.OperatorIn || end.Pin_Type == Pin.PinType.OperatorOut)
            {
                endOpeParent = end.GetComponentInParent<ProxyABOperator>();
                //start.IsGateOperator = true;
				start.Pin_Type = Pin.PinType.OperatorIn;
                endOpeParent.Inputs[endOpeParent.Inputs.Length-1] = (ABNode)startParamParent.AbParam;
                ((ABNode)startParamParent.AbParam).Output = (ABNode)endOpeParent.AbOperator;//TODO ma geule
            }
			else if (end.Pin_Type == Pin.PinType.ActionParam)
            {
                endActionParent = end.GetComponentInParent<ProxyABAction>();
                //end.IsGateOperator = true;
				end.Pin_Type = Pin.PinType.OperatorIn;
                endActionParent.AbState.Action.Parameters[0].Inputs[0] = (ABNode)startParamParent.AbParam;
            }
        }
        else
        {
            startStateParent = start.GetComponentInParent<ProxyABState>();
			if (end.Pin_Type == Pin.PinType.ActionParam)
            {
                endActionParent = end.GetComponentInParent<ProxyABAction>();
                addConditionPin(trans, pinPrefab);
                transitionId = AbModel.LinkStates(startStateParent.AbState.Name, endActionParent.AbState.Name);
                trans.Transition = AbModel.getTransition(transitionId);
            }
            else
            {
                endStateParent = end.GetComponentInParent<ProxyABState>();
                addConditionPin(trans, pinPrefab);
                transitionId = AbModel.LinkStates(startStateParent.AbState.Name, endStateParent.AbState.Name);
                trans.Transition = AbModel.getTransition(transitionId);
            }
        }                                                               
    }

    ProxyABAction CreateAction()
    {
        ProxyABAction action = Instantiate<ProxyABAction>(actionPrefab);
            return action;
    }

    ProxyABOperator CreateOperator()
    {
        ProxyABOperator ope;
        ope = Instantiate<ProxyABOperator>(operatorPrefab);
        proxyOperators.Add(ope);
        return ope;
    }

    ProxyABParam CreateParam()
    {
        ProxyABParam param;
        param = Instantiate<ProxyABParam>(parameterPrefab);
        proxyParams.Add(param);
        return param;

		//trans.Transition = AbModel.getTransition( transitionId );

        Debug.Log(AbModel.Transitions.Count.ToString());
    }

    void Select()
    {

    }

	void DeleteTransition( ProxyABTransition transition )
    {
		if (transition != null) {

            // Transition between Action/State and Action/State
            if (transition.Condition != null)
            {
				if ( (!(transition.StartPosition.Pin_Type == Pin.PinType.OperatorIn || transition.StartPosition.Pin_Type == Pin.PinType.OperatorOut)
					|| !(transition.StartPosition.Pin_Type == Pin.PinType.Param) )
					&& (!(transition.EndPosition.Pin_Type == Pin.PinType.OperatorIn || transition.EndPosition.Pin_Type == Pin.PinType.OperatorOut)
					|| !(transition.EndPosition.Pin_Type == Pin.PinType.Param)))
                {
                    AbModel.UnlinkStates(transition.Transition.Start.Name, transition.Transition.End.Name);
                }
            } else
            {
                RemoveTransitionSyntTree(transition);
            }
            // Unlink            
 		               
			// Remove Pin
			// Destroy( transition.Condition.gameObject );
			// Destroy Object
			Destroy (transition.gameObject);
		}
    }

    void Move()
    {

    }
     void Copy()
    {

    }

    void Cut()
    {

    }

    void Undo()
    {

    }

    void Redo()
    {

    }
	#endregion

	#region DISPLAY FUNCTIONS
    private void RemoveTransitionSyntTree(ProxyABTransition transition)
    {
        Pin start = transition.StartPosition;
        Pin end = transition.EndPosition;

        ProxyABOperator proxyOpeStart = null;
        ProxyABOperator proxyOpeEnd = null;
        ProxyABParam proxyParam = null;

		if (start.Pin_Type == Pin.PinType.OperatorIn || start.Pin_Type == Pin.PinType.OperatorOut)
        {
            proxyOpeStart = start.GetComponentInParent<ProxyABOperator>();
			if (end.Pin_Type == Pin.PinType.Param)
            {
                proxyParam = end.GetComponentInParent<ProxyABParam>();
                UnlinkOperator_Param(proxyOpeStart, proxyParam);
            }
			else if (end.Pin_Type == Pin.PinType.OperatorIn || end.Pin_Type == Pin.PinType.OperatorOut)
            {
                proxyOpeEnd = end.GetComponentInParent<ProxyABOperator>();
                UnlinkOperator_Operator(proxyOpeStart, proxyOpeEnd);
            }
        }
		else if (end.Pin_Type == Pin.PinType.OperatorIn || end.Pin_Type == Pin.PinType.OperatorOut)
        {
            proxyOpeEnd = end.GetComponentInParent<ProxyABOperator>();
			if (start.Pin_Type == Pin.PinType.Param)
            {
                proxyParam = start.GetComponentInParent<ProxyABParam>();
                UnlinkOperator_Param(proxyOpeStart, proxyParam);
            }
			else if (start.Pin_Type == Pin.PinType.OperatorIn || start.Pin_Type == Pin.PinType.OperatorOut)
            {
                proxyOpeEnd = start.GetComponentInParent<ProxyABOperator>();
                UnlinkOperator_Operator(proxyOpeStart, proxyOpeEnd);
            }
        }        
    }

    private void UnlinkOperator_Operator(ProxyABOperator proxyOpeStart, ProxyABOperator proxyOpeEnd)
    {
        for (int i = 0; i < proxyOpeStart.Inputs.Length; i++)
        {
            if (proxyOpeStart.Inputs[i] == ((ABNode)(proxyOpeEnd.AbOperator)))
            {
                proxyOpeStart.Inputs[i] = null;
            }
        }
        for (int i = 0; i < proxyOpeEnd.Inputs.Length; i++)
        {
            if (proxyOpeEnd.Inputs[i] == ((ABNode)(proxyOpeStart.AbOperator)))
            {
                proxyOpeEnd.Inputs[i] = null;
            }
        }
    }

    private void UnlinkOperator_Param(ProxyABOperator proxyOpeStart, ProxyABParam proxyParam)
    {

        string idRemoveObject = proxyParam.AbParam.Identifier;
        for (int i = 0; i < proxyOpeStart.AbOperator.Inputs.Length; i++)
        {
            ABNode node = proxyOpeStart.AbOperator.Inputs[i];
            if(node != null)
            {
                if (((IABParam)(node)).Identifier == idRemoveObject)
                {
                    node.Output = null;
                    proxyOpeStart.AbOperator.Inputs[i] = null;
                }
            }            
        }
    }

    void DisplayStates()
    {
        for (int i = 0; i < proxyStates.Count; i++)
        {
            proxyStates[i].transform.position = new Vector3(proxyStates[i].transform.position.x + UnityEngine.Random.Range(-5, 5), proxyStates[i].transform.position.y + UnityEngine.Random.Range(-5, 5), proxyStates[i].transform.position.z);
        }
    }

    void DisplayActions()
    {
        for (int i = 0; i < proxyActions.Count; i++)
        {
            proxyActions[i].transform.position = new Vector3(proxyActions[i].transform.position.x + UnityEngine.Random.Range(-5, 5), proxyActions[i].transform.position.y + UnityEngine.Random.Range(-5, 5), proxyActions[i].transform.position.z);
        }
    }

    void DisplayOperators()
    {

        for (int i = 0; i < proxyOperators.Count; i++)
        {
            proxyOperators[i].transform.position = new Vector3(proxyOperators[i].transform.position.x + UnityEngine.Random.Range(-5, 5), proxyOperators[i].transform.position.y + UnityEngine.Random.Range(-5, 5), 0);
        }
    }

    void DisplayParameters()
    {
        for (int i = 0; i < proxyParams.Count; i++)
        {
            proxyParams[i].transform.position = new Vector3(proxyOperators[i].transform.position.x + UnityEngine.Random.Range(-5, 5), proxyParams[i].transform.position.y + UnityEngine.Random.Range(-5, 5), proxyParams[i].transform.position.z);
        }
    }

    public static void SetNodeName(GameObject proxy, ABNode node)
    {
        Text operatorName = proxy.GetComponentInChildren<Text>();
		operatorName.text = getNodeName( node );
    }

	public static string getNodeName( ABNode node ){
		string opeName = node.ToString();
		char splitter = '_';
		string[] newName = opeName.Split(splitter);
		string newOpeName = "";

		for (int i = 1; i < newName.Length - 1; i++)
		{
			newOpeName += newName[i];
		}

		return newOpeName;
	}
	#endregion

	#region Transition Create Delete
	Pin transition_Pin_Start = null;
	ProxyABTransition transition_Selected = null;

	public Pin Transition_Pin_Start {
		get {
			return transition_Pin_Start;
		}
	}
	public ProxyABTransition Transition_Selected {
		get {
			return transition_Selected;
		}
	}

	public void createTransition_setStartPin( Pin pin ){
		this.transition_Pin_Start = pin;
	}
	public void createTransition_setEndPin( Pin pin ){
		if (this.transition_Pin_Start != null && this.transition_Pin_Start != pin) {
			this.CreateTransition ( this.transition_Pin_Start, pin );
			this.transition_Pin_Start = null;
		}
	}

	// delete
	public void selectTransition( ProxyABTransition transition ){
		this.transition_Selected = transition;
	}
	void deleteSelectedTransition(){
		if (this.transition_Selected != null) {
			this.DeleteTransition (this.transition_Selected);
			this.transition_Selected = null;
		}
	}
	#endregion
}
