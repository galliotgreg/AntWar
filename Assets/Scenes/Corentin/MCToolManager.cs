﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.IO;

public class MCToolManager : MonoBehaviour
{
    #region SINGLETON
    /// <summary>
    /// The static instance of the Singleton for external access
    /// </summary>
    public static MCToolManager instance = null;

    /// <summary>
    /// Enforce Singleton properties
    /// </summary>
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
        neverCalculated = true;
        cast_name = AppContextManager.instance.ActiveCast.Name + "_behavior";

    }
    #endregion

    public enum ToolType
    {
        Selection,
        Hand,
        Undo,
        Redo,
        Copier,
        Coller,
        None
    };
    public ConfirmationPanel confirmPan;
    public List<GameObject> SelectedNodes = new List<GameObject>();
    public GameObject getTarget;
    [SerializeField]
    ToolType currentTool = ToolType.None;

    bool isMouseDragging;
    [SerializeField]
    bool inventory = false;

    [SerializeField]
    private Button btn_Selection;
    [SerializeField]
    private Button btn_Main;
    [SerializeField]
    private Button btn_Undo;
    [SerializeField]
    private Button btn_Redo;
    [SerializeField]
    private Button btn_Copier;
    [SerializeField]
    private Button btn_Coller;

    [SerializeField]
    DropArea centerZone;

    private bool selectionEnCours;

    private Vector3 mpos;
    private bool neverCalculated;
    List<Vector3> DistanceList = new List<Vector3>();
    List<GameObject> CopyNodes = new List<GameObject>();

    int id = -1;
    int idmax = 0;
    string cast_name;
    public bool saved = false;
    public bool hasBeenAdded = false;
    public MCEditor_Proxy[] allUnits;
    string sourceFilePath;

    bool found_not_connex = false;

    #region PROPERTIES
    public ToolType CurrentTool
    {
        get
        {
            return currentTool;
        }
        set
        {
            currentTool = value;
        }
    }
    #endregion

    private void Start()
    {
        currentTool = ToolType.None;
        btn_Selection.onClick.AddListener(() => { CurrentTool = ToolType.Selection; CancelInventory(); SelectionSquare.instance.enabled = true; });
        btn_Main.onClick.AddListener(() => { CurrentTool = ToolType.Hand; CancelInventory(); neverCalculated = true; });
        btn_Undo.onClick.AddListener(() => { CurrentTool = ToolType.Undo; CancelInventory(); ToggleUndoConfirmationPanel(); });
        btn_Redo.onClick.AddListener(() => { CurrentTool = ToolType.Redo; CancelInventory(); ToolRedo(); });
        btn_Copier.onClick.AddListener(() => { CurrentTool = ToolType.Copier; CancelInventory(); ToolCopier(); });
        btn_Coller.onClick.AddListener(() => { CurrentTool = ToolType.Coller; CancelInventory(); ToolColler(); });
        DeleteTemporary_Backup();
        sourceFilePath = Application.dataPath + @"/Inputs/TemporaryBackup/" + cast_name;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt))
        {
            if (Input.GetKeyDown(KeyCode.S)) //Shortcut Selection ctrl + alt + S
            {
                CurrentTool = ToolType.Selection;
                btn_Selection.GetComponent<ChangeCursorToDefault>().OnClickChangeCursor();
                SelectionSquare.instance.enabled = true;
                isMouseDragging = false;
                CancelInventory();

            }
        }
        else if (Input.GetKey(KeyCode.LeftControl)) //Shortcut Main ctrl+m
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                CurrentTool = ToolType.Hand;
                btn_Main.GetComponent<ChangeCursorToHand>().OnClickChangeCursor();
                neverCalculated = true;
                SelectionSquare.instance.enabled = false;
                SelectionSquare.instance.MultipleSelection = false;
                selectionEnCours = false;
                CancelInventory();
            }
        
            if (Input.GetKeyDown(KeyCode.Z)) //Shortcut Undo ctrl+z
            {
                btn_Undo.GetComponent<ChangeCursorToDefault>().OnClickChangeCursor();
                CurrentTool = ToolType.Undo;
                CancelInventory();
                ToggleUndoConfirmationPanel();
            }
        

            if (Input.GetKeyDown(KeyCode.Y)) //Shortcut Redo ctrl+y
            {
                btn_Redo.GetComponent<ChangeCursorToDefault>().OnClickChangeCursor();
                CurrentTool = ToolType.Redo;
                CancelInventory();
                ToolRedo();
            }

            if (Input.GetKeyDown(KeyCode.C)) //shortcut Copier ctrl+c
            {
                btn_Copier.GetComponent<ChangeCursorToDefault>().OnClickChangeCursor();
                CurrentTool = ToolType.Copier;
                CancelInventory();
                ToolCopier();
            }
            if (Input.GetKeyDown(KeyCode.V)) //shortcut Coller ctrl+v
            {
                btn_Coller.GetComponent<ChangeCursorToDefault>().OnClickChangeCursor();
                CurrentTool = ToolType.Coller;
                CancelInventory();
                ToolColler();
            }
        }

        allUnits = FindObjectsOfType<MCEditor_Proxy>();
        //Mouse Button Press Down
        if (Input.GetMouseButtonDown(0) && !selectionEnCours)
        {
            RaycastHit hitInfo;
            getTarget = ReturnClickedObject(out hitInfo);
            if (getTarget.name == "PinPrefab(Clone)" || getTarget.name == "PinInPrefab(Clone)" || getTarget.name == "transition(Clone)" || getTarget.name == "pinOut(Clone)")
            {
                SelectionSquare.instance.enabled = false;
                isMouseDragging = false;
            }
            if (centerZone.CanDrop)
            {
                //Hit background
                if (getTarget.name == "STUBS_backgroundCollider")
                {
                    inventory = false;
                    //current tool activated
                    if (CurrentTool == ToolType.Selection)
                    {
                        selectionEnCours = true;
                        SelectionSquare.instance.MultipleSelection = true;

                        SelectionSquare.instance.enabled = true;

                        SelectedNodes = SelectionSquare.instance.selectedUnits;
                    }
                    if (CurrentTool == ToolType.Hand)
                    {
                        SelectionSquare.instance.enabled = false;
                        isMouseDragging = false;
                    }
                }

                if (getTarget.name != "STUBS_backgroundCollider")
                {
                    //hit something selectable (node, state, action...)
                    if (getTarget.tag == "Selectable")
                    {
                        inventory = false;
                        //current tool activated
                        if (CurrentTool == ToolType.Selection)
                        {

                            SelectionSquare.instance.MultipleSelection = false;
                            SelectionSquare.instance.enabled = true;
                            SelectedNodes = SelectionSquare.instance.selectedUnits;

                        }
                        if (CurrentTool == ToolType.Hand)
                        {
                            SelectionSquare.instance.enabled = false;
                            isMouseDragging = true;

                            if (neverCalculated)
                            {
                                DistanceList.Clear();
                                mpos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -GameObject.Find("Camera").GetComponent<Camera>().transform.position.z);
                                mpos = GameObject.Find("Camera").GetComponent<Camera>().ScreenToWorldPoint(mpos);
                                neverCalculated = false;

                                foreach (GameObject b in SelectedNodes)
                                {
                                    DistanceList.Add(b.transform.position - mpos);

                                }
                            }
                        }
                    }
                }
            }
            else
            {
                // Disable square when inventory is selected
                SelectionSquare.instance.enabled = false;
                SelectionSquare.instance.MultipleSelection = false;
                isMouseDragging = false;
                selectionEnCours = false;
            }
        }

        // Stop Selection when the mouse goes into the inventory 
        // Attention : it is necessary to remove the raycast target property of the selection square 
        // Attention : it is necessary to implement the "stop drawing" the selection square 
        /*if (Input.GetMouseButton (0) && selectionEnCours && !centerZone.CanDrop) { 
	      isMouseDragging = false; 
	      selectionEnCours = false; 
	      SelectionSquare.instance.enabled = false; 
	    }*/

        if (Input.GetMouseButtonUp(0))
        {
            isMouseDragging = false;
            selectionEnCours = false;
            if (saved)
                hasBeenAdded = false;
            saved = false;
        }

        if (CurrentTool == ToolType.Hand)
        {
            if (isMouseDragging)
            {
                ToolMain();
            }
            else
            {
                ToolMain_ConsolidateNodes();
                checkSave();
            }
        }

        // Delete selected nodes when DELETE is pressed
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            DeleteNodes();
            SelectedNodes.Clear();
            checkSave();
        }
    }
    void DeleteTemporary_Backup()
    {
        string backupPath = Application.dataPath + @"/Inputs/TemporaryBackup";
        string[] filesPath = Directory.GetFiles(backupPath, "*.csv", SearchOption.TopDirectoryOnly);
        foreach (string filepath in filesPath)
        {
            File.Delete(filepath);
            File.Delete(filepath + ".meta");
        }

    }
    //Method to Return Clicked Object
    GameObject ReturnClickedObject(out RaycastHit hit)
    {
        GameObject target = null;
        Ray ray = GameObject.Find("Camera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray.origin, ray.direction * 10, out hit))
        {
            //Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
            target = hit.collider.gameObject;
        }
        return target;
    }

    #region TOOL : HAND
    private void ToolMain()
    {

        /*//Mouse moving
        if (isMouseDragging)
        {*/
        //tracking mouse pos
        Vector3 currentScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -GameObject.Find("Camera").GetComponent<Camera>().transform.position.z);

        //converting screen pos => world pos.
        Vector3 currentPosition = GameObject.Find("Camera").GetComponent<Camera>().ScreenToWorldPoint(currentScreenSpace);

        if (SelectedNodes != null && SelectedNodes.Count == 0)
        {
            getTarget.transform.position = currentPosition;
        }

        else
        {
            if (SelectedNodes != null && SelectedNodes.Contains(getTarget))
            {
                int i = 0;
                foreach (GameObject b in SelectedNodes)
                {
                    //update targets current postions.
                    if (DistanceList[i].x < 0 || DistanceList[i].y < 0)
                    {
                        DistanceList[i].Set(DistanceList[i].x * -1.0f, DistanceList[i].y * -1.0f, DistanceList[i].z);
                    }
                    b.transform.position = currentPosition + DistanceList[i];
                    i++;

                }
            }
        }
        //}
    }

    /// <summary>
    /// After dropping the nodes, adjust the selected nodes to the grid
    /// </summary>
    private void ToolMain_ConsolidateNodes()
    {
        if (SelectedNodes != null && SelectedNodes.Count == 0)
        {
            MCEditorManager.positioningProxy(getTarget.GetComponent<MCEditor_Proxy>());
        }
        else
        {
            if (SelectedNodes != null)
            {
                foreach (GameObject b in SelectedNodes)
                {
                    MCEditorManager.positioningProxy(b.GetComponent<MCEditor_Proxy>());
                }
            }
        }
    }
    #endregion

    #region COPIER/COLLER
    void ToolCopier()
    {
        CopyNodes = SelectedNodes;
    }

    void ToolColler()
    {
        Vector3 CopyPos = new Vector3();
        Vector3 Offset = new Vector3(2.0f, -2.0f, 0.0f);

        foreach (GameObject b in CopyNodes)
        {
            CopyPos = b.transform.position + Offset;
            if (b.GetComponent<ProxyABParam>())
            {
                IABParam newParam = b.GetComponent<ProxyABParam>().AbParam.Clone();
                ProxyABParam result = MCEditor_Proxy_Factory.instantiateParam(newParam, false);
                result.gameObject.transform.position = b.gameObject.transform.position + Offset;
                MCEditorManager.positioningProxy(result.GetComponent<MCEditor_Proxy>());
            }
            if (b.GetComponent<ProxyABOperator>())
            {
                IABOperator newOperator = b.GetComponent<ProxyABOperator>().AbOperator.Clone();
                ProxyABOperator result = MCEditor_Proxy_Factory.instantiateOperator(newOperator, false);
                result.gameObject.transform.position = b.gameObject.transform.position + Offset;
                MCEditorManager.positioningProxy(result.GetComponent<MCEditor_Proxy>());
            }
        }
        //CopyNodes.Clear();
        //SelectedNodes.Clear();
        hasBeenAdded = false;
        checkSave(); 
    }


    public void checkSave()
    {
        foreach(MCEditor_Proxy b in allUnits)
        {
            if (!b.isConnected())
            {
                found_not_connex = true;
                break;
            }
            found_not_connex = false;

        }
        if (found_not_connex)
        {
            btn_Undo.GetComponent<Image>().color = Color.red;
            btn_Redo.GetComponent<Image>().color = Color.red;

        }
        else
        {
            btn_Undo.GetComponent<Image>().color = Color.white;
            btn_Redo.GetComponent<Image>().color = Color.white;
            TemporarySave();
        }

    }

    public void ToggleUndoConfirmationPanel()
    {
        if (found_not_connex)
        {
            confirmPan.TogglePanel(3);
        }
        else ToolUndo();
    }

    #endregion

    #region DELETE
    /// <summary>
    /// Deletes the selected nodes
    /// </summary>
    void DeleteNodes()
    {
        if (SelectedNodes != null && SelectedNodes.Count == 0)
        {
            MCEditorManager.instance.deleteSelectedProxies(new List<MCEditor_Proxy>() { getTarget.GetComponent<MCEditor_Proxy>() });
            getTarget = null;
        }
        else
        {
            if (SelectedNodes != null)
            {
                List<MCEditor_Proxy> selectedProxies = new List<MCEditor_Proxy>();
                foreach (GameObject b in SelectedNodes)
                {
                    selectedProxies.Add(b.GetComponent<MCEditor_Proxy>());
                }
                MCEditorManager.instance.deleteSelectedProxies(selectedProxies);
                SelectedNodes.Clear();
            }
        }
        checkSave();
    }
    #endregion

    #region UNDO
    public void ToolUndo()
    {
        /*if (id == idmax)
        {
            idmax++;
            MCEditorManager.instance.Temporary_Save_MC_Behavior(cast_name, idmax.ToString());
        }*/
        if (id == 0)
        {
            string destinationFolderPath = AppContextManager.instance.ActiveSpecieFolderPath;
            string sourcePosition = sourceFilePath + "_POSITION_" + id.ToString() + ".csv";
            string sourceBehavior = sourceFilePath + "_" + id.ToString() + ".csv";
            List<MCEditor_Proxy> allProxies = new List<MCEditor_Proxy>();
            MCEditor_Proxy initToDestroy = null;
            foreach (MCEditor_Proxy b in allUnits)
            {
                if (b.GetComponent<ProxyABState>() && b.GetComponent<ProxyABState>().AbState.Id == MCEditorManager.instance.AbModel.InitStateId)
                {
                    initToDestroy = b;
                }
                else
                {
                    allProxies.Add(b.GetComponent<MCEditor_Proxy>());
                }
            }
            MCEditorManager.instance.deleteSelectedProxies(allProxies);
            MCEditorManager.instance.forcedeleteProxy((ProxyABState)initToDestroy);

            SelectedNodes.Clear();
            MCEditorManager.instance.TemporarySetupModel(cast_name, id.ToString());
        }
        btn_Undo.GetComponent<Image>().color = Color.white;
        btn_Redo.GetComponent<Image>().color = Color.white;
        if (!IdExists(id - 1))
        {
            return;
        }

        if (id > 0)
        {


            id--;
            string destinationFolderPath = AppContextManager.instance.ActiveSpecieFolderPath;
            string sourcePosition = sourceFilePath + "_POSITION_" + id.ToString() + ".csv";
            string sourceBehavior = sourceFilePath + "_" + id.ToString() + ".csv";
            /*//Backup Behavior Files
            File.Delete(destinationFolderPath + cast_name + ".csv");
            File.Delete(destinationFolderPath + cast_name + ".csv.meta");
            File.Copy(sourceBehavior, destinationFolderPath + cast_name + ".csv");

            //Backup Position Files
            File.Delete(destinationFolderPath + cast_name + "_POSITION.csv");
            File.Delete(destinationFolderPath + cast_name + "_POSITION.csv.meta");
            File.Copy(sourcePosition, destinationFolderPath + cast_name + "_POSITION.csv");
            */
            List<MCEditor_Proxy> allProxies = new List<MCEditor_Proxy>();
            MCEditor_Proxy initToDestroy = null;
            foreach (MCEditor_Proxy b in allUnits)
            {
                if (b.GetComponent<ProxyABState>() && b.GetComponent<ProxyABState>().AbState.Id == MCEditorManager.instance.AbModel.InitStateId)
                {
                    initToDestroy = b;
                }
                else
                {
                    allProxies.Add(b.GetComponent<MCEditor_Proxy>());
                }
            }
            MCEditorManager.instance.deleteSelectedProxies(allProxies);
            MCEditorManager.instance.forcedeleteProxy((ProxyABState)initToDestroy);

            SelectedNodes.Clear();
            //MCEditorManager.instance.SetupModel();
            MCEditorManager.instance.TemporarySetupModel(cast_name, id.ToString());
            //Debug.Log("LOAD: " + cast_name + "_" + id.ToString());
        }

    }
    #endregion

    #region REDO
    private void ToolRedo()
    {
        if (!IdExists(id + 1))
        {
            return;
        }

        string destinationFolderPath = AppContextManager.instance.ActiveSpecieFolderPath;

        id++;
        if (id <= idmax)
        {

            string sourcePosition = sourceFilePath + "_POSITION_" + id.ToString() + ".csv";
            string sourceBehavior = sourceFilePath + "_" + id.ToString() + ".csv";

            /*File.Delete(destinationFolderPath + cast_name + "_POSITION.csv");
            File.Delete(destinationFolderPath + cast_name + "_POSITION.csv.meta");
            File.Copy(sourcePosition, destinationFolderPath + cast_name + "_POSITION.csv");

            File.Delete(destinationFolderPath + cast_name + ".csv");
            File.Delete(destinationFolderPath + cast_name + ".csv.meta");
            File.Copy(sourceBehavior, destinationFolderPath + cast_name + ".csv");*/

            List<MCEditor_Proxy> allProxies = new List<MCEditor_Proxy>();
            MCEditor_Proxy initToDestroy = null;
            foreach (MCEditor_Proxy b in allUnits)
            {
                if (b.GetComponent<ProxyABState>() && b.GetComponent<ProxyABState>().AbState.Id == MCEditorManager.instance.AbModel.InitStateId)
                {
                    initToDestroy = b;
                }
                else
                {
                    allProxies.Add(b.GetComponent<MCEditor_Proxy>());
                }
            }
            MCEditorManager.instance.deleteSelectedProxies(allProxies);
            MCEditorManager.instance.forcedeleteProxy((ProxyABState)initToDestroy);

            SelectedNodes.Clear();
            //MCEditorManager.instance.SetupModel();
            MCEditorManager.instance.TemporarySetupModel(cast_name, id.ToString());
            //Debug.Log("LOAD: " + cast_name + "_" + id.ToString());
        }
    }

    private bool IdExists(int testedId)
    {
        string csvpath = Application.dataPath + @"\Inputs/TemporaryBackup/" + cast_name + "_" + testedId + ".csv";
        return File.Exists(csvpath);
    }
    #endregion

    #region TEMPO SAVE

    public void TemporarySave()
    {
        if (!hasBeenAdded)
        {

            id++;
            if (idmax <= id)
            {
                idmax = id;
            }
            for (int id_delete = id; id_delete <= idmax; id_delete++)
            {
                string sourceBehavior = Application.dataPath + @"\Inputs\TemporaryBackup\" + cast_name + "_" + id_delete.ToString() + ".csv";
                string sourcePosition = Application.dataPath + @"\Inputs\TemporaryBackup\" + cast_name + "_POSITION_" + id_delete.ToString() + ".csv";
                File.Delete(sourceBehavior);
                File.Delete(sourcePosition);
            }


            MCEditorManager.instance.Temporary_Save_MC_Behavior(cast_name, id.ToString());
            //Debug.Log("SAVE: " + cast_name + "   " + id.ToString());
            saved = true;
            hasBeenAdded = true;
        }
    }

    #endregion

    public void Inventory()
    {
        inventory = true;
    }
    public void CancelInventory()
    {
        inventory = false;
    }
}

