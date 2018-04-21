﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIController : MonoBehaviour {

    /// <summary>
    /// The static instance of the Singleton for external access
    /// </summary>
    public static InGameUIController instance = null;
    public static OpenClosePanel instanceOpenClose = null;

    private static Color colorPlayer1 = Color.white;
    private static Color colorPlayer2 = new Color(195f / 255f, 91f / 255f, 169f / 255f); 

    private float startTime = 2.0f;
    private bool winState = false;
    private bool alreadyClosed = false;

    //Bool to ensure focus is on a Lumy 
    private bool unitSelected = false;
    private AgentScript self;
    private Color color; 
    GameManager gameManager;
    GameObject[] openClosePanelList;
    private int indiceFocus = 0;

    ActionType oldActiontype = ActionType.None; 
    #region UIVariables
    #region PlayerInfosPanel
    /// <summary>
    /// Resources 
    /// </summary>
    [Header("Player Infos Panel")]
    [SerializeField]
    private Text J1_Red_Resources;
    [SerializeField]
    private Text J1_Green_Resources;
    [SerializeField]
    private Text J1_Blue_Resources;
    [SerializeField]
    private Text J1_Pop;
    [SerializeField]
    private Text J1_Species;
    [SerializeField]
    private Text J1_PrysmeLife;
    [SerializeField]
    private Text J2_Red_Resources;
    [SerializeField]
    private Text J2_Green_Resources;
    [SerializeField]
    private Text J2_Blue_Resources;
    [SerializeField]
    private Text J2_Pop;
    [SerializeField]
    private Text J2_Species;
    [SerializeField]
    private Text J2_PrysmeLife;

    #endregion

    #region MainMenu
    [Header("Main Menu")]
    [SerializeField]
    private Button Menu_MainMenu;
    [SerializeField]
    private Button Menu_PersonnalizedMap;
    [SerializeField]
    private Button Menu_Caste;
    [SerializeField]
    private Button Menu_OptionsDebug;
    #endregion

    #region ValidationOnExit
    [Header("ValidationOnExit")]
    [SerializeField]
    private Button Confirm_Exit_MainMenu;
    [SerializeField]
    private Button Cancel_Exit_MainMenu; 
    [SerializeField]
    private GameObject Panel_Exit_MainMenu;
    [SerializeField]
    private Button Confirm_Exit_CasteMenu;
    [SerializeField]
    private Button Cancel_Exit_CasteMenu;
    [SerializeField]
    private GameObject Panel_Exit_CasteMenu;
    [SerializeField]
    private Button Confirm_Exit_PartiePersoMenu;
    [SerializeField]
    private Button Cancel_Exit_PartiePersoMenu;
    [SerializeField]
    private GameObject Panel_Exit_PartiePersoMenu;

    #endregion
    #region VictoryScreen
    [Header("Victory Screen")]
    [SerializeField]
    private GameObject victoryMenu;
    [SerializeField]
    private Text victory;
    [SerializeField]
    private Image J1_Icon;
    [SerializeField]
    private Image J2_Icon;
    [SerializeField]
    private Text winCondition;
    [SerializeField]
    private Text J1_Resources;
    [SerializeField]
    private Text J2_Resources;
    [SerializeField]
    private Text J1_unitCost;
    [SerializeField]
    private Text J2_unitCost;
    [SerializeField]
    private Text J1_EnemiesDetruis;
    [SerializeField]
    private Text J2_EnemiesDetruis;
    [SerializeField]
    private Text J1_AlliesCrees;
    [SerializeField]
    private Text J2_AlliesCrees;
    [SerializeField]
    private Text J1_AlliesDetruits;
    [SerializeField]
    private Text J2_AlliesDetruits;
    [SerializeField]
    private Text J1_Nuee;
    [SerializeField]
    private Text J2_Nuee;
    [SerializeField]
    private Image J1_ressourcesSlider;
    [SerializeField]
    private Image J2_ressourcesSlider;
    [SerializeField]
    private Image J1_unitCostSlider;
    [SerializeField]
    private Image J2_unitCostSlider;
    [SerializeField]
    private Image J1_enemiesDestroyedSlider;
    [SerializeField]
    private Image J2_enemiesDestroyedSlider;
    [SerializeField]
    private Image J1_unitCreatedSlider;
    [SerializeField]
    private Image J2_unitCreatedSlider;
    [SerializeField]
    private Image J1_unitDestoyedSlider;
    [SerializeField]
    private Image J2_unitDestoyedSlider;
    [SerializeField]
    private Button Caste_Menu;
    [SerializeField]
    private Button quitVictory;
    [SerializeField]
    private Text J1_scoreUnitCost;
    [SerializeField]
    private Text J2_scoreUnitCost;
    [SerializeField]
    private Text J1_ScoreFinal;
    [SerializeField]
    private Text J2_ScoreFinal; 

    #endregion

    [SerializeField]
    private Button Menu;
    [SerializeField]
    private GameObject subMenu;
    [SerializeField]
    private GameObject panelOptionsDebug;

    /// <summary>
    /// Timer 
    /// </summary>
    [Header("Timer")]
    [SerializeField]
    private Text timer;
    [SerializeField]
    private Button playPause;
    [SerializeField]
    private Sprite pause;
    [SerializeField]
    private Sprite play;

    #region ExitMenu
    /// <summary>
    /// Exit Menu
    /// </summary>
    [Header("Exit Menu")]
    [SerializeField]
    private GameObject exitMenu;
    [SerializeField]
    private Button quit_ExitMenu;
    [SerializeField]
    private Button cancel_ExitMenu;
    [SerializeField]
    private Canvas canvas;

    #endregion

    #region StatsLumy
    [Header("Stats Lumy")]
    [SerializeField]
    private Text vitalityText;
    [SerializeField]
    private Text strenghtText;
    [SerializeField]
    private Text staminaText;
    [SerializeField]
    private Text moveSpeedText;
    [SerializeField]
    private Text actionSpeedText;
    [SerializeField]
    private Text visionText;
    [SerializeField]
    private Text pickupRangeText;
    [SerializeField]
    private Text strikeRangeText;
    [SerializeField]
    private Text curPosText;
    [SerializeField]
    private Text trgPosText;
    [SerializeField]
    private Text LayTimeText;
    [SerializeField]
    private Text castText;
    [SerializeField]
    private Text item;
    [SerializeField]
    private GameObject unitGoJ1;
    [SerializeField]
    private GameObject unitGoJ2;
    [SerializeField]
    private GameObject contentParentJ1;
    [SerializeField]
    private GameObject contentParentJ2;
    [SerializeField]
    private Text unitCostRedText;
    [SerializeField]
    private Text unitCostGreenText;
    [SerializeField]
    private Text unitCostBlueText;
    [SerializeField]
    private Text alliesInSightText;
    [SerializeField]
    private Text ennemiesInSightText;
    [SerializeField]
    private Text ressourcesInSightText;
    [SerializeField]
    private Text tracesInSightText;
    #endregion


    Dictionary<string, int> castsJ1;
    Dictionary<string, int> castsJ2;

    /// <summary>
    /// Variables for CastUIDisplay
    /// </summary>
    private List<GameObject> castUiList = new List<GameObject>();
    private List<GameObject> castUiListJ2 = new List<GameObject>();
    private Dictionary<string, int> popJ1 = new Dictionary<string, int>();
    private Dictionary<string, int> popJ2 = new Dictionary<string, int>();


    private List<GameObject> queens = new List<GameObject>();

    private float newAmount = 0f;
    private float newAmountJ2 = 0f;
    private float[] newAmountColor = { 0, 0, 0 };
    private float[] newAmountColorJ2 = { 0, 0, 0 };
    private float depenseLigne = 0;
    private float gainLigne = 0;

    [Header("Prysme Text")]
    [SerializeField]
    private Text positiveRedText;
    [SerializeField]
    private Text negativeRedText;
    [SerializeField]
    private Text positiveBlueText;
    [SerializeField]
    private Text negativeBlueText;
    [SerializeField]
    private Text positiveGreenText;
    [SerializeField]
    private Text negativeGreenText;
    [SerializeField]
    private Text positiveRedTextJ2;
    [SerializeField]
    private Text negativeRedTextJ2;
    [SerializeField]
    private Text positiveBlueTextJ2;
    [SerializeField]
    private Text negativeBlueTextJ2;
    [SerializeField]
    private Text positiveGreenTextJ2;
    [SerializeField]
    private Text negativeGreenTextJ2;
    [SerializeField]
    private float waitingTime = 1f;

    #region Bottom Left panel
    [SerializeField]
    private GameObject statGrid;
    [SerializeField]
    private GameObject statPrysmeJ1;
    [SerializeField]
    private GameObject StatPrysmeJ2;
    #endregion
    //button valid debugg params
    [SerializeField]
    private Button valider;

    #region MiniMap

    [Header("Minimap")]
    //Icon minimap materials
    [SerializeField]
    private Material player1Material;
    [SerializeField]
    private Material player2Material;
    [SerializeField]
    private Material playerDefaultMaterial;
    #endregion

    private bool isDisplayingNegativeResJ1 =false;
    private bool isDisplayingPositiveResJ1 = false;
    private bool isDisplayingNegativeResJ2 = false;
    private bool isDisplayingPositiveResJ2 = false;
    private DateTime tsNegativeJ1;
    private DateTime tsPositiveJ1;
    private DateTime tsNegativeJ2;
    private DateTime tsPositiveJ2;

    private bool panelOpened = true;

    private bool statePlayPause = true;

    private bool isLumy; 

    #region Instance
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
    }
    #endregion
    #endregion
    

    #region Accesseur
    public bool UnitSelected
    {
        get
        {
            return unitSelected;
        }

        set
        {
            unitSelected = value;
        }
    }

    public AgentScript Self
    {
        get
        {
            return self;
        }

        set
        {
            self = value;
        }
    }

    public Color Color
    {
        get
        {
            return color;
        }

        set
        {
            color = value;
        }
    }

    public GameObject PanelOptionsDebug
    {
        get
        {
            return panelOptionsDebug;
        }

        set
        {
            panelOptionsDebug = value;
        }
    }

    public bool StatePlayPause
    {
        get
        {
            return statePlayPause;
        }

        set
        {
            statePlayPause = value;
        }
    }

    public bool IsLumy
    {
        get
        {
            return isLumy;
        }

        set
        {
            isLumy = value;
        }
    }

    public bool WinState
    {
        get
        {
            return winState;
        }

        set
        {
            winState = value;
        }
    }
    #endregion

    // Use this for initialization
    void Start()
    {
        Init();
        if(!isNotNull())
            return;
        popJ1 = new Dictionary<string, int>(GameObject.Find("p1_hive").GetComponent<HomeScript>().Population);
        popJ2 = new Dictionary<string, int>(GameObject.Find("p2_hive").GetComponent<HomeScript>().Population);
    }


    /// <summary>
    /// Init the Controller 
    /// </summary>
	private void Init() {
        //Get gameManager Instance 
        gameManager = GameManager.instance;

        openClosePanelList = GameObject.FindGameObjectsWithTag("Panel");

        Camera camera = NavigationManager.instance.GetCurrentCamera();
        canvas.worldCamera = camera;
        //Init all Listener

        //Exit Menu
        cancel_ExitMenu.onClick.AddListener(CloseExitMenu);
        quit_ExitMenu.onClick.AddListener(ExitGame);

        //Victory Menu 
        Caste_Menu.onClick.AddListener(GoToCasteMenu);
        quitVictory.onClick.AddListener(ExitGame);
        Menu_MainMenu.onClick.AddListener(GoToMainMenu);
        Menu_PersonnalizedMap.onClick.AddListener(GoToPersonnalizedMap);
        Menu_Caste.onClick.AddListener(GoToCasteMenu);
        Menu_OptionsDebug.onClick.AddListener(OpenOptionsDebug);
        Menu.onClick.AddListener(SwitchMenu);

        //Confirmation Menu 
        Confirm_Exit_CasteMenu.onClick.AddListener(validateGoToCasteMenu);
        Cancel_Exit_CasteMenu.onClick.AddListener(unvalidateGoToCasteMenu);
        Confirm_Exit_MainMenu.onClick.AddListener(validateGoToMainMenu);
        Cancel_Exit_MainMenu.onClick.AddListener(unvalidatedGoToMainMenu);
        Confirm_Exit_PartiePersoMenu.onClick.AddListener(validateGoToPersonnalizedMap);
        Cancel_Exit_PartiePersoMenu.onClick.AddListener(unvalidatedGoToPersonnalisedMap); 

        valider.onClick.AddListener(OptionManager.instance.setPlayerPreferencesDebug);
        playPause.onClick.AddListener(playPauseButton);

        //Player Species 
        J1_Species.text = SwapManager.instance.GetPlayer1Name();
        J2_Species.text = SwapManager.instance.GetPlayer2Name();

        GameObject[] lumys = GameObject.FindGameObjectsWithTag("Agent");

        foreach (GameObject lumy in lumys) {
            if (lumy.gameObject.name == "p1_queen" || lumy.gameObject.name == "p2_queen") {
                queens.Add(lumy);
            }
        }
    
    }

    private void playPauseButton()
    {
        if (winState == true)
        {
            return;
        }
        PauseGame(); 
    }
 
    public void PauseGame()
    {
        
        Image pp = playPause.GetComponent<Image>();
        if (statePlayPause)
        {
            pp.sprite = play;
        }
        else if (statePlayPause == false)
        {
            pp.sprite = pause;
        } 
        if(Time.timeScale == 0)
        {
            SoundManager.instance.PlayPlayGameSFX();
        }
        else
        {
            SoundManager.instance.PlayPauseGameSFX(); 
        }
        
        GameManager.instance.PauseGame();
        statePlayPause = !statePlayPause;
    }


    // Update is called once per frame
    void Update()
    {
        CheckWinCondition();
        CheckKeys(); 
        UpdateUI();
        SwitchFocus();
        playFX(); 
    }

    /// <summary>
    /// Play all FX for the Lumy Selected (based on self) 
    /// </summary>
    private void playFX()
    {
        playFXOnMove(); 
    }

    /// <summary>
    /// Check all the buttons on the Scene 
    /// </summary>
    private void CheckKeys()
    {
        //if (Input.GetKeyDown(KeyCode.Escape))
        // exitMenu.SetActive(!exitMenu.activeSelf);
        if (Input.GetMouseButtonDown(1))
        {
            foreach (GameObject panel in openClosePanelList)
            {
                if (panelOpened)
                {
                    panel.GetComponent<OpenClosePanel>().CloseGlobal();
                }
                else
                {
                    panel.GetComponent<OpenClosePanel>().OpenGlobal();
                }
            }
            panelOpened = !panelOpened;
        }
    }

    #region WinConditions
    /// <summary>
    /// Check if the winner variable is on a Win State 
    /// </summary>
    private void CheckWinCondition()
    {
        //Wait until Game is Initialized 
        //All the Hive to be instantiate for example
        if (startTime > 0)
        {
            startTime -= Time.deltaTime;
            return; 
        }

        //Check Win Conditions 
        GameManager.Winner winner = gameManager.WinnerPlayer;
        if (!winState)
        {
            if (winner != GameManager.Winner.None)
            {
                CloseAllOthersMsgPanel(victoryMenu); 
                winState = true;
                victoryMenu.SetActive(true);
                if (winner == GameManager.Winner.Player1E)
                {
                    victory.text = SwapManager.instance.GetPlayer1Name();
                    victory.color = colorPlayer1; 
                    winCondition.text = "Victoire Economique";
                }
                if (winner == GameManager.Winner.Player2E) {

                    victory.text = SwapManager.instance.GetPlayer2Name();
                    victory.color = colorPlayer2; 
                    winCondition.text = "Victoire Economique";
                }
                if (winner == GameManager.Winner.Player1Q) {
                    victory.text = SwapManager.instance.GetPlayer1Name();
                    victory.color = colorPlayer1;
                    winCondition.text = "Domination";
                }
                if (winner == GameManager.Winner.Player2Q) {
                    victory.text = SwapManager.instance.GetPlayer2Name();
                    victory.color = colorPlayer2;
                    winCondition.text = "Domination";
                }
                if (winner == GameManager.Winner.Equality)
                {
                    victory.text = "Egalité ! ";
                    victory.color = Color.white; 
                    winCondition.text = "";
                }

                GameObject[] swarmPictos = LumyPictFactory.instance.InstanciateAllPicts();
                //Get J1 swarm picto
                Material materialJ1 = swarmPictos[GameManager.instance.P1_specie.PictId].GetComponent<MeshRenderer>().material;
                Texture2D texturePictoJ1 = (Texture2D)materialJ1.mainTexture;
                Sprite spritePictoJ1 = Sprite.Create(texturePictoJ1, new Rect(0.0f, 0.0f, texturePictoJ1.width, texturePictoJ1.height), new Vector2(0.5f, 0.5f), 100.0f);

                J1_Icon.sprite = spritePictoJ1;

                //Get J2 swarm picto
                Material materialJ2 = swarmPictos[GameManager.instance.P2_specie.PictId].GetComponent<MeshRenderer>().material;
                Texture2D texturePictoJ2 = (Texture2D)materialJ2.mainTexture;
                Sprite spritePictoJ2 = Sprite.Create(texturePictoJ2, new Rect(0.0f, 0.0f, texturePictoJ2.width, texturePictoJ2.height), new Vector2(0.5f, 0.5f), 100.0f);

                J2_Icon.sprite = spritePictoJ2;

                //Score 
                J1_scoreUnitCost.text = "Détails : \r\n " +
                    "Unités(en vie) : " + gameManager.SumProdCost(PlayerAuthority.Player1).ToString() + " \r\n" + 
                    "Ressources : " + gameManager.sumResources(PlayerAuthority.Player1).ToString();
                J1_scoreUnitCost.color = colorPlayer1; 
                J2_scoreUnitCost.text = "Détails :  \r\n" +
                   "Unités(en vie) : " + gameManager.SumProdCost(PlayerAuthority.Player2).ToString() + " \r\n" + 
                   "Ressources : " + gameManager.sumResources(PlayerAuthority.Player2).ToString();
                J2_scoreUnitCost.color = colorPlayer2; 
                J1_ScoreFinal.text = "   SCORE : " + gameManager.Score(PlayerAuthority.Player1).ToString();
                J1_ScoreFinal.color = colorPlayer1;
                J2_ScoreFinal.text = "   SCORE : " + gameManager.Score(PlayerAuthority.Player2).ToString() ;
                J2_ScoreFinal.color = colorPlayer2; 

                //Destroy pictPrefabs
                foreach (GameObject pict in swarmPictos) {
                    Destroy(pict);
                }
               
                J1_Nuee.text = SwapManager.instance.GetPlayer1Name();
                J1_Nuee.color = colorPlayer1;
                J2_Nuee.text = SwapManager.instance.GetPlayer2Name();
                J2_Nuee.color = colorPlayer2;
                J1_unitCost.text = gameManager.SumProdCost(PlayerAuthority.Player1).ToString();
                J1_unitCost.color = colorPlayer1;
                J2_unitCost.text = gameManager.SumProdCost(PlayerAuthority.Player2).ToString();
                J2_unitCost.color = colorPlayer2;
                J1_Resources.text = gameManager.sumResources(PlayerAuthority.Player1).ToString();
                J1_Resources.color = colorPlayer1;
                J2_Resources.text = gameManager.sumResources(PlayerAuthority.Player2).ToString();
                J2_Resources.color = colorPlayer2; 
                J1_EnemiesDetruis.text = Unit_GameObj_Manager.instance.unitPlayer2Destroyed.ToString();
                J1_EnemiesDetruis.color = colorPlayer1; 
                J2_EnemiesDetruis.text = Unit_GameObj_Manager.instance.unitPlayer1Destroyed.ToString();
                J2_EnemiesDetruis.color = colorPlayer2;
                J1_AlliesCrees.text = Unit_GameObj_Manager.instance.unitPlayer1Created.ToString();
                J1_AlliesCrees.color = colorPlayer1; 
                J2_AlliesCrees.text = Unit_GameObj_Manager.instance.unitPlayer2Created.ToString();
                J2_AlliesCrees.color = colorPlayer2; 
                J1_AlliesDetruits.text = Unit_GameObj_Manager.instance.unitPlayer1Destroyed.ToString();
                J1_AlliesDetruits.color = colorPlayer1; 
                J2_AlliesDetruits.text = Unit_GameObj_Manager.instance.unitPlayer2Destroyed.ToString();
                J2_AlliesDetruits.color = colorPlayer2; 

                J1_ressourcesSlider.fillAmount = (gameManager.sumResources(PlayerAuthority.Player1) + SwapManager.instance.GetPlayerResources()) / (SwapManager.instance.GetPlayerStock() *3);
                J2_ressourcesSlider.fillAmount = (gameManager.sumResources(PlayerAuthority.Player2) + SwapManager.instance.GetPlayerResources()) / (SwapManager.instance.GetPlayerStock() *3);
                J1_unitCostSlider.fillAmount = (float)gameManager.Score(PlayerAuthority.Player1) / (float)(gameManager.Score(PlayerAuthority.Player1) + gameManager.Score(PlayerAuthority.Player2));
                J2_unitCostSlider.fillAmount = (float)gameManager.Score(PlayerAuthority.Player2) / (float)(gameManager.Score(PlayerAuthority.Player1) + gameManager.Score(PlayerAuthority.Player2));
                J1_enemiesDestroyedSlider.fillAmount = (float)Unit_GameObj_Manager.instance.unitPlayer2Destroyed / (float)Unit_GameObj_Manager.instance.unitPlayer2Created;
                J2_enemiesDestroyedSlider.fillAmount = (float)Unit_GameObj_Manager.instance.unitPlayer1Destroyed / (float)Unit_GameObj_Manager.instance.unitPlayer1Created;
                J1_unitCreatedSlider.fillAmount = (float)Unit_GameObj_Manager.instance.unitPlayer1Created / (float)(Unit_GameObj_Manager.instance.unitPlayer1Created + Unit_GameObj_Manager.instance.unitPlayer2Created);
                J2_unitCreatedSlider.fillAmount = (float)Unit_GameObj_Manager.instance.unitPlayer2Created / (float)(Unit_GameObj_Manager.instance.unitPlayer1Created + Unit_GameObj_Manager.instance.unitPlayer2Created);
                J1_unitDestoyedSlider.fillAmount = (float)Unit_GameObj_Manager.instance.unitPlayer1Destroyed / (float)Unit_GameObj_Manager.instance.unitPlayer1Created;
                J2_unitDestoyedSlider.fillAmount = (float)Unit_GameObj_Manager.instance.unitPlayer2Destroyed / (float)Unit_GameObj_Manager.instance.unitPlayer2Created;

                PauseGame();
            }
        }
       
    }
    #endregion

    /// <summary>
    /// Update the whole UI each Frames
    /// </summary>
    private void UpdateUI()
    {
        //Get Resources values in game Manager
        if (gameManager == null)
        {
            return; 
        }
        if(winState == true)
        {
            return; 
        }
        //*** UPPER UI *** 
        //Set the resources for each Players
        SetResources();
        //Set the Timer of the game 
        SetTimer(); 
        
        //Set the Population for each player
        J1_Pop.text = "" +gameManager.GetHome(PlayerAuthority.Player1).getPopulation().Count;    
        J2_Pop.text = "" + gameManager.GetHome(PlayerAuthority.Player2).getPopulation().Count;

        //Set the PrysmeLife for each player 
        //TODO Check if queens are not destroyed
        J1_PrysmeLife.text = queens[0].transform.GetChild(1).GetComponent<AgentScript>().Vitality.ToString() + " / " + queens[0].transform.GetChild(1).GetComponent<AgentScript>().VitalityMax.ToString();
        J2_PrysmeLife.text = queens[1].transform.GetChild(1).GetComponent<AgentScript>().Vitality.ToString() + " / " + queens[1].transform.GetChild(1).GetComponent<AgentScript>().VitalityMax.ToString();

        //Show the spent fluctuation
        spentResJ1();
        spentResJ2();

        //*** DOWN UI ***
        //All Units infos if a unit have been clicked on
        if (unitSelected == true)
        { 
            UnitStats(); //Show the unit stats
            DisplayInSight(); //Show the units vision elements 
            //unitCost();  //Show the ProdCost of the Unit --> Directly call from CameraRay ? 
        }
        else
        {             
            cleanUnitStats(); //Clean the screen for all stats and cost of a unit
        }
        
        //Show all units by cast 
        getAllUnit(PlayerAuthority.Player1);
        getAllUnit(PlayerAuthority.Player2);
        lumyMinimapIconColor();
    }

    private void playFXOnMove()
    {
        if(this.self == null)
        {
            return; 
        }
        ActionType curAction = this.self.gameObject.GetComponentInParent<AgentBehavior>().CurActionType;
        if (curAction == oldActiontype)
        {
            return; 
        }
       if(curAction == ActionType.Roaming || curAction == ActionType.Goto)
        {
            SoundManager.instance.PlayLumyMovementSFX();
            oldActiontype = curAction; 
        }
    }

  
    #region UpdateFunctionOfUI
    private void SetResources()
    {
        float[] res = gameManager.GetResources(PlayerAuthority.Player1);

        if (CheckRes(res))
        {
            J1_Red_Resources.text = "" + res[0];
            J1_Green_Resources.text = "" + res[1];
            J1_Blue_Resources.text = "" + res[2];
        }

        res = gameManager.GetResources(PlayerAuthority.Player2);
        if (CheckRes(res))
        {
            J2_Red_Resources.text = "" + res[0];
            J2_Green_Resources.text = "" + res[1];
            J2_Blue_Resources.text = "" + res[2];
        }
    }
    private void SetTimer() {
        if (gameManager.TimerLeft != null)
        {
            TimeSpan t = TimeSpan.FromSeconds(gameManager.TimerLeft);
            timer.text = t.Minutes + " : " + t.Seconds;
            if (t.Seconds < 10)
            {
                timer.text = t.Minutes + " : 0" + t.Seconds;
            }
            if (gameManager.TimerLeft <= 30)
            {
                timer.color = new Color(255, 0, 0, 255);
                timer.fontStyle = FontStyle.Bold;
            }
        }
    }
    //TODO Find a better Implementation than spentResJ1 & spentResJ2
    private void spendRes(PlayerAuthority player)
    {
        GameObject home;
        HomeScript hiveScript; 

        //Check which Player was sent in parameters 
        if(PlayerAuthority.Player1 == player)
        {
            home = GameManager.instance.P1_home;
            hiveScript = home.GetComponent<HomeScript>();
        }
        else if(PlayerAuthority.Player2 == player)
        {
            home = GameManager.instance.P2_home;
            hiveScript = home.GetComponent<HomeScript>(); 
        }
        else
        {
            return; 
        }
       
        float oldAmount = hiveScript.RedResAmout + hiveScript.BlueResAmout + hiveScript.GreenResAmout;
        float[] oldAmountColor = { hiveScript.RedResAmout, hiveScript.GreenResAmout, hiveScript.BlueResAmout };
        int depense = (int)(oldAmount - newAmount);
        int depenseB = (int)(oldAmountColor[2] - newAmountColor[2]);
        int depenseG = (int)(oldAmountColor[1] - newAmountColor[1]);
        int depenseR = (int)(oldAmountColor[0] - newAmountColor[0]);

        if (isDisplayingNegativeResJ1)
        {
            if (DateTime.Now > tsNegativeJ1)
            {
                isDisplayingNegativeResJ1 = false;
                negativeRedText.text = "-";
                negativeBlueText.text = "-";
                negativeGreenText.text = "-";
            }
        }
        else if (!isDisplayingNegativeResJ1)
        {
            if (depense < 0)
            {
                negativeRedText.text = depenseR.ToString();
                negativeBlueText.text = depenseB.ToString();
                negativeGreenText.text = depenseG.ToString();
                tsNegativeJ1 = DateTime.Now + TimeSpan.FromSeconds(waitingTime);
                isDisplayingNegativeResJ1 = true;
            }

        }
        if (isDisplayingPositiveResJ1)
        {
            if (DateTime.Now > tsPositiveJ1)
            {
                isDisplayingPositiveResJ1 = false;
                positiveRedText.text = "-";
                positiveBlueText.text = "-";
                positiveGreenText.text = "-";
            }
        }
        else if (!isDisplayingPositiveResJ1)
        {
            if (depense > 0)
            {
                positiveRedText.text = "+" + depenseR.ToString();
                positiveBlueText.text = "+" + depenseB.ToString();
                positiveGreenText.text = "+" + depenseG.ToString();
                tsPositiveJ1 = DateTime.Now + TimeSpan.FromSeconds(waitingTime);
                isDisplayingPositiveResJ1 = true;
            }

        }
        newAmount = oldAmount;
        newAmountColor[0] = oldAmountColor[0];
        newAmountColor[1] = oldAmountColor[1];
        newAmountColor[2] = oldAmountColor[2];
    }
    private void spentResJ1()
    {
        GameObject homeP1 = GameManager.instance.P1_home;

        HomeScript p1_hiveScript = homeP1.GetComponent<HomeScript>();
        float oldAmount = p1_hiveScript.RedResAmout + p1_hiveScript.BlueResAmout + p1_hiveScript.GreenResAmout;
        float[] oldAmountColor = { p1_hiveScript.RedResAmout, p1_hiveScript.GreenResAmout, p1_hiveScript.BlueResAmout };
        int depense = (int)(oldAmount - newAmount);
        int depenseB = (int)(oldAmountColor[2] - newAmountColor[2]);
        int depenseG = (int)(oldAmountColor[1] - newAmountColor[1]);
        int depenseR = (int)(oldAmountColor[0] - newAmountColor[0]);

        if (isDisplayingNegativeResJ1)
        {
            if (DateTime.Now > tsNegativeJ1)
            {
                isDisplayingNegativeResJ1 = false;
                negativeRedText.text = "-";
                negativeBlueText.text = "-";
                negativeGreenText.text = "-";
            }
        }
        else if (!isDisplayingNegativeResJ1)
        {
            if (depense < 0)
            {
                negativeRedText.text = depenseR.ToString();
                negativeBlueText.text = depenseB.ToString();
                negativeGreenText.text = depenseG.ToString();
                tsNegativeJ1 = DateTime.Now + TimeSpan.FromSeconds(waitingTime);
                isDisplayingNegativeResJ1 = true;
            }

        }
        if (isDisplayingPositiveResJ1)
        {
            if (DateTime.Now > tsPositiveJ1)
            {
                isDisplayingPositiveResJ1 = false;
                positiveRedText.text = "-";
                positiveBlueText.text = "-";
                positiveGreenText.text = "-";
            }
        }
        else if (!isDisplayingPositiveResJ1)
        {
            if (depense > 0)
            {
                positiveRedText.text = "+" + depenseR.ToString();
                positiveBlueText.text = "+" + depenseB.ToString();
                positiveGreenText.text = "+" + depenseG.ToString();
                tsPositiveJ1 = DateTime.Now + TimeSpan.FromSeconds(waitingTime);
                isDisplayingPositiveResJ1 = true;
            }

        }
        newAmount = oldAmount;
        newAmountColor[0] = oldAmountColor[0];
        newAmountColor[1] = oldAmountColor[1];
        newAmountColor[2] = oldAmountColor[2];
    }
    private void spentResJ2()
    {
        GameObject homeP2 = GameManager.instance.P2_home;

        HomeScript p2_hiveScript = homeP2.GetComponent<HomeScript>();
        float oldAmount = p2_hiveScript.RedResAmout + p2_hiveScript.BlueResAmout + p2_hiveScript.GreenResAmout;
        float[] oldAmountColorJ2 = { p2_hiveScript.RedResAmout, p2_hiveScript.GreenResAmout, p2_hiveScript.BlueResAmout };
        int depense = (int)(oldAmount - newAmountJ2);
        int depenseB = (int)(oldAmountColorJ2[2] - newAmountColorJ2[2]);
        int depenseG = (int)(oldAmountColorJ2[1] - newAmountColorJ2[1]);
        int depenseR = (int)(oldAmountColorJ2[0] - newAmountColorJ2[0]);

        if (isDisplayingNegativeResJ2)
        {
            if (DateTime.Now > tsNegativeJ2)
            {
                isDisplayingNegativeResJ2 = false;
                negativeRedTextJ2.text = "-";
                negativeBlueTextJ2.text = "-";
                negativeGreenTextJ2.text = "-";
            }
        }
        else if (!isDisplayingNegativeResJ2)
        {
            if (depense < 0)
            {
                negativeRedTextJ2.text = depenseR.ToString();
                negativeBlueTextJ2.text = depenseB.ToString();
                negativeGreenTextJ2.text = depenseG.ToString();
                tsNegativeJ2 = DateTime.Now + TimeSpan.FromSeconds(waitingTime);
                isDisplayingNegativeResJ2 = true;
            }

        }
        if (isDisplayingPositiveResJ2)
        {
            if (DateTime.Now > tsPositiveJ2)
            {
                isDisplayingPositiveResJ2 = false;
                positiveRedTextJ2.text = "-";
                positiveBlueTextJ2.text = "-";
                positiveGreenTextJ2.text = "-";
            }
        }
        else if (!isDisplayingPositiveResJ2)
        {
            if (depense > 0)
            {
                positiveRedTextJ2.text = "+" + depenseR.ToString();
                positiveBlueTextJ2.text = "+" + depenseB.ToString();
                positiveGreenTextJ2.text = "+" + depenseG.ToString();
                tsPositiveJ2 = DateTime.Now + TimeSpan.FromSeconds(waitingTime);
                isDisplayingPositiveResJ2 = true;
            }

        }
        newAmountJ2 = oldAmount;
        newAmountColorJ2[0] = oldAmountColorJ2[0];
        newAmountColorJ2[1] = oldAmountColorJ2[1];
        newAmountColorJ2[2] = oldAmountColorJ2[2];
    }
    private void UnitStats()
    {
        GameObject[] allUnits = GameObject.FindGameObjectsWithTag("Agent");
        foreach (GameObject agent in allUnits)
        {
            if (agent.gameObject.transform.GetChild(1).GetComponent<AgentScript>() != self)
            {
                agent.gameObject.transform.GetChild(1).GetComponent<AgentScript>().gameObject.transform.GetChild(0).gameObject.SetActive(false);
            }
            else if (agent.gameObject.transform.GetChild(1).GetComponent<AgentScript>() == self)
            {
                agent.gameObject.transform.GetChild(1).GetComponent<AgentScript>().gameObject.transform.GetChild(0).gameObject.SetActive(true);
            }
        }

        //Get Stats from Self
        string vitality = self.Vitality.ToString();
        string visionRange = self.VisionRange.ToString();
        string vitalityMax = self.VitalityMax.ToString();
        string strength = self.Strength.ToString();
        string pickRange = self.PickRange.ToString();
        string atkRange = self.AtkRange.ToString();
        string actSpeed = self.ActSpd.ToString();
        string moveSpeed = self.MoveSpd.ToString();
        string nbItemMax = self.NbItemMax.ToString();
        string nbItem = self.NbItem.ToString();
        string layTimeCost = self.LayTimeCost.ToString();
        string stamina = self.Stamina.ToString();
        string cast = self.Cast;

        //Set Color
        vitalityText.color = color;
        strenghtText.color = color;
        staminaText.color = color;
        moveSpeedText.color = color;
        actionSpeedText.color = color;
        visionText.color = color;
        pickupRangeText.color = color;
        strikeRangeText.color = color;
        item.color = color;
        LayTimeText.color = color;
        castText.color = color;

        //Set Text
        vitalityText.text = vitality + " / " + self.VitalityMax.ToString();
        strenghtText.text = strength;
        staminaText.text = stamina.ToString();
        moveSpeedText.text = moveSpeed;
        actionSpeedText.text = actSpeed;
        visionText.text = visionRange;
        pickupRangeText.text = pickRange;
        strikeRangeText.text = atkRange;
        item.text = nbItem + " / " + nbItemMax;
        LayTimeText.text = layTimeCost;
        castText.text = cast;

    }
    #region getAllUnits

    private Dictionary<string, int> getAllUnit(PlayerAuthority player)
    {
        if (PlayerAuthority.Player1 == player)
        {
            if (!CheckDicoEquality(popJ1, GameObject.Find("p1_hive").GetComponent<HomeScript>().Population))
            {
                DisplayUnits(GameObject.Find("p1_hive").GetComponent<HomeScript>().Population);
                popJ1 = new Dictionary<string, int>(GameObject.Find("p1_hive").GetComponent<HomeScript>().Population);
            }

        }
        if (PlayerAuthority.Player2 == player)
        {
            if (!CheckDicoEquality(popJ2, GameObject.Find("p2_hive").GetComponent<HomeScript>().Population))
            {
                DisplayUnitsJ2(GameObject.Find("p2_hive").GetComponent<HomeScript>().Population);
                popJ2 = new Dictionary<string, int>(GameObject.Find("p2_hive").GetComponent<HomeScript>().Population);
            }
        }
        return null;
    }


    private bool CheckDicoEquality(Dictionary<string, int> dico1, Dictionary<string, int> dico2)
    {
        // check keys are the same

        foreach (string str in dico1.Keys)
        {
            if (!dico2.ContainsKey(str))
            {
                return false;
            }
        }
        // check values are the same
        foreach (string str in dico1.Keys)
        {
            if (!dico1[str].Equals(dico2[str]))
            {
                return false;
            }
        }
        return true;
    }
    #endregion
    /// <summary>
    /// Set cost and stats elements in UI to white color and "-" 
    /// </summary>
    private void cleanUnitStats()
    {
        GameObject[] allUnits = GameObject.FindGameObjectsWithTag("Agent");

        if (self == null)
        {
            foreach (GameObject agent in allUnits)
            {
                agent.gameObject.transform.GetChild(1).GetComponent<AgentScript>().gameObject.transform.GetChild(0).gameObject.SetActive(true);
            }
        }

        //Clean Stats 
        vitalityText.color = Color.white;
        strenghtText.color = Color.white;
        staminaText.color = Color.white;
        moveSpeedText.color = Color.white;
        actionSpeedText.color = Color.white;
        visionText.color = Color.white;
        pickupRangeText.color = Color.white;
        strikeRangeText.color = Color.white;
        item.color = Color.white;
        LayTimeText.color = Color.white;
        castText.color = Color.white;

        vitalityText.text = "-";
        strenghtText.text = "-";
        staminaText.text = "-";
        moveSpeedText.text = "-";
        actionSpeedText.text = "-";
        visionText.text = "-";
        pickupRangeText.text = "-";
        strikeRangeText.text = "-";
        item.text = "-";
        LayTimeText.text = "-";
        castText.text = "-";

        //Clean Cost 
        unitCostRedText.color = Color.white;
        unitCostGreenText.color = Color.white;
        unitCostBlueText.color = Color.white;
        unitCostRedText.text = "-";
        unitCostGreenText.text = "-";
        unitCostBlueText.text = "-";

        //Clean Elements in sight
        alliesInSightText.color = Color.white;
        ressourcesInSightText.color = Color.white;
        ennemiesInSightText.color = Color.white;
        tracesInSightText.color = Color.white;
        alliesInSightText.text = "-";
        ressourcesInSightText.text = "-";
        ennemiesInSightText.text = "-";
        tracesInSightText.text = "-";
    }
    #endregion
    #region CheckRes
    /// <summary>
    /// Check is Resources from GameManager is ok
    /// </summary>
    /// <param name="res"></param>
    /// <returns></returns>
    private bool CheckRes(float[] res)
    {
        if (res == null)
        {
            Debug.LogError("Res null in GUI");
            return false;

        }
        if (res.Length != 3)
        {
            Debug.LogError("Not 3 Resources for player in UI");
            return false ;
        }
        return true;
    }
    #endregion
    #region Validator
    /// <summary>
    /// Check is UI game objects are not null 
    /// </summary>
    /// <returns>Return a boolean</returns>
    private bool isNotNull()
    {
        if (J1_Red_Resources == null)
        {
            Debug.LogError("InGame UI Error : Red Resource not set for J1");
            return false; 
        }
        if (J2_Red_Resources == null)
        {
            Debug.LogError("InGame UI Error : Red Resource not set for J2");
            return false;
        }
        if (J1_Green_Resources == null)
        {
            Debug.LogError("InGame UI Error : Green Resource not set for J1");
            return false;
        }
        if (J2_Green_Resources == null)
        {
            Debug.LogError("InGame UI Error : Green Resource not set for J2");
            return false;
        }
        if (J1_Blue_Resources == null)
        {
            Debug.LogError("InGame UI Error : Blue Resource not set for J1");
            return false;
        }
        if (J2_Blue_Resources == null)
        {
            Debug.LogError("InGame UI Error : Blue Resource not set for J2");
            return false;
        }
        if(timer == null)
        {
            Debug.LogError("InGame UI Error : Timer not set ");
            return false; 
        }
        if(exitMenu == null)
        {
            Debug.LogError("Ingame UI Error : Exit Menu not set");
            return false; 
        }
        if(quit_ExitMenu == null)
        {
            Debug.LogError("Ingame UI Error : Exit Button Menu not set");
            return false; 
        }
        if(cancel_ExitMenu == null)
        {
            Debug.LogError("Ingame UI Error : Cancel Button Menu not set");
            return false; 
        }
        if(canvas == null)
        {
            Debug.LogError("Ingame UI Error : Canvas not set");
            return false; 
        }
        if(victoryMenu == null)
        {
            Debug.LogError("Ingame UI Error : Victory Menu not Set");
            return false;
        }
        if(victory == null )
        {
            Debug.LogError("Ingame UI Error : Victory field not set");
            return false; 
        }
        if(J1_Resources == null)
        {
            Debug.LogError("Ingame UI Error : J1 resources not set");
            return false;
        }
        if(J2_Resources == null)
        {
            Debug.LogError("Ingame UI Error : J2 resources not set ");
            return false; 
        }
        if(Caste_Menu == null)
        {
            Debug.LogError("Ingame UI Error : Menu Victory Caste button not set");
            return false; 
        }
        if(quitVictory == null)
        {
            Debug.LogError("Ingame UI Error : Menu Victory quit buttton not set");
            return false; 
        }

        if(Menu_Caste == null)
        {
            Debug.LogError("InGame UI Error : Menu/Caste Menu button not set"); 
        }
        if (Menu_MainMenu == null)
        {
            Debug.LogError("InGame UI Error : Menu/Principal Menu button not set");
        }
        if (Menu_PersonnalizedMap == null)
        {
            Debug.LogError("InGame UI Error : Menu/Personnalized Menu button not set");
        }


        return true; 
    }
    #endregion
    #region BtnListener
    private void CloseExitMenu()
    {
        exitMenu.SetActive(false);
    }

    private void ExitGame()
    
    {
        if (!CheckExitGame())
        {
            return;
        }
        NavigationManager.instance.ActivateFadeToBlack();
        NavigationManager.instance.SwapScenesWithoutZoom("PartiePersoScene");
        alreadyClosed = true; 
    }
    private void GoToCasteMenu()
    {
        CloseAllOthersMsgPanel(Panel_Exit_CasteMenu);
        Panel_Exit_CasteMenu.SetActive(!Panel_Exit_CasteMenu.activeSelf); 
    }

    private void validateGoToCasteMenu()
    {
        if (!CheckExitGame())
        {
            return;
        }
        NavigationManager.instance.ActivateFadeToBlack();
        NavigationManager.instance.SwapScenesWithoutZoom("EditeurCastesScene");
        alreadyClosed = true;
    }

    private void unvalidateGoToCasteMenu ()
    {
        Panel_Exit_CasteMenu.SetActive(false); 
    }

    private void GoToMainMenu()
    {
        CloseAllOthersMsgPanel(Panel_Exit_MainMenu); 
        Panel_Exit_MainMenu.SetActive(!Panel_Exit_MainMenu.activeSelf); 
    }

    private void validateGoToMainMenu()
    {
        if (!CheckExitGame())
        {
            return;
        }
        NavigationManager.instance.ActivateFadeToBlack();
        NavigationManager.instance.SwapScenesWithoutZoom("MenuPrincipalScene");
        alreadyClosed = true;
    }

    private void unvalidatedGoToMainMenu()
    {
        Panel_Exit_MainMenu.SetActive(false); 
    }



    private void GoToPersonnalizedMap()
    {
        CloseAllOthersMsgPanel(Panel_Exit_PartiePersoMenu); 
        Panel_Exit_PartiePersoMenu.SetActive(!Panel_Exit_PartiePersoMenu.activeSelf); 
    }

    private void validateGoToPersonnalizedMap()
    {
        if (!CheckExitGame())
        {
            return;
        }
        NavigationManager.instance.ActivateFadeToBlack();
        NavigationManager.instance.SwapScenesWithoutZoom("PartiePersoScene");
        alreadyClosed = true;
    }

    private void unvalidatedGoToPersonnalisedMap()
    {
        Panel_Exit_PartiePersoMenu.SetActive(false); 
    }

    private void CloseAllOthersMsgPanel(GameObject Panel)
    {
        List<GameObject> panels = new List<GameObject>();
        panels.Add(Panel_Exit_CasteMenu);
        panels.Add(Panel_Exit_MainMenu);
        panels.Add(Panel_Exit_PartiePersoMenu);
        panels.Add(panelOptionsDebug);
        panels.Add(subMenu); 
        foreach (GameObject go in panels)
        {
            if (go != Panel)
            {
                go.SetActive(false);
            }
        }
    }
        public void OpenOptionsDebug() {
        if(winState == false)
        {
            if (OperatorHelper.Instance != null)
            {
                OperatorHelper.Instance.transform.parent = GameManager.instance.transform;
            }
            CloseAllOthersMsgPanel(panelOptionsDebug);
            PanelOptionsDebug.SetActive(!panelOptionsDebug.activeSelf);
            if (Time.timeScale == 1)
            {
                PauseGame();
            }
          

        }
    }

    private void CheckPause()
    {
        if(Time.timeScale == 0)
        {
            Time.timeScale = 1; 
        }
    }

    private bool CheckExitGame()
    {
        if (alreadyClosed)
        {
            return false;
        }
        if (OperatorHelper.Instance != null)
        {
            OperatorHelper.Instance.transform.parent = GameManager.instance.transform;
        }
        MessagesManager.instance.CloseMsgPanel();
        CheckPause();
        return true; 
    }


    void SwitchMenu() {
        if(winState == false)
        {
            CloseAllOthersMsgPanel(subMenu);
            subMenu.SetActive(!subMenu.activeSelf);
        }

    }
    #endregion

    #region DisplayUnits
private void DisplayUnits(Dictionary<string, int> units)
    {
        //Dictionary<string, int> units = getAllUnit(PlayerAuthority.Player1);
        //Clean list if element in it
        foreach (GameObject go in castUiList)
        {
            Destroy(go);
        }
        castUiList.Clear(); 
        //Create UI cast and add them to the list 
        foreach (KeyValuePair<string, int> unit in units)
        {
            if (unit.Value != 0)
            {
                GameObject go = Instantiate(unitGoJ1);
                castUiList.Add(go);
                go.transform.GetChild(0).GetComponent<Text>().color = Color.white;
                go.transform.GetChild(1).GetComponent<Text>().color = Color.white;
                go.transform.GetChild(0).GetComponent<Text>().text = unit.Key;
                go.transform.GetChild(1).GetComponent<Text>().text = unit.Value.ToString();
                //go.transform.SetParent(unitGoJ1.transform.parent.gameObject.transform);
                go.transform.SetParent(contentParentJ1.transform);
                go.transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    
    private void DisplayUnitsJ2(Dictionary<string, int> units) {
        //Dictionary<string, int> units = getAllUnit(PlayerAuthority.Player1);
        //Clean list if element in it
        foreach (GameObject go in castUiListJ2) {
            
            Destroy(go);
        }
        castUiListJ2.Clear();
        //Create UI cast and add them to the list 
        foreach (KeyValuePair<string, int> unit in units) {
            if (unit.Value != 0) {
                GameObject go = Instantiate(unitGoJ2);
                castUiListJ2.Add(go);
                go.transform.GetChild(0).GetComponent<Text>().color = new Color(102f/255f, 27f/255f, 109f/255f);
                go.transform.GetChild(1).GetComponent<Text>().color = new Color(102f / 255f, 27f / 255f, 109f / 255f);
                go.transform.GetChild(0).GetComponent<Text>().text = unit.Key;
                go.transform.GetChild(1).GetComponent<Text>().text = unit.Value.ToString();
                //go.transform.SetParent(unitGoJ2.transform.parent.gameObject.transform);
                go.transform.SetParent(contentParentJ2.transform);
                go.transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }
#endregion

    #region DisplayElementInSight
private void DisplayInSight() {

        if (self == null)
        {
            cleanUnitStats(); 
            return;
        }

        //Set Color
        alliesInSightText.color = Color;
        ressourcesInSightText.color = Color;
        ennemiesInSightText.color = Color;
        tracesInSightText.color = Color;

        //Set Text
        alliesInSightText.text = self.GetComponentInParent<AgentContext>().Allies.Length.ToString();
        ressourcesInSightText.text = self.GetComponentInParent<AgentContext>().Resources.Length.ToString();
        ennemiesInSightText.text = self.GetComponentInParent<AgentContext>().Enemies.Length.ToString();
        tracesInSightText.text = self.GetComponentInParent<AgentContext>().Traces.Length.ToString();


    }
    #endregion

    public void unitCost() {

       if (self == null) {
            cleanUnitStats();     
            return;
        }
        //Set Color
        unitCostRedText.color = Color;
        unitCostGreenText.color = Color;
        unitCostBlueText.color = Color;

        Dictionary<string, int> costs = self.ProdCost;
        foreach (KeyValuePair<string, int> cost in costs) {
            //Set color and count like stats for the lumy 
            string color = cost.Key;
            int count = cost.Value;
           
            if (color == "Red") {
                unitCostRedText.text = count.ToString();
            }
            if (color == "Green") {
                unitCostGreenText.text = count.ToString();
            }
            if (color == "Blue") {
                unitCostBlueText.text = count.ToString();
            }
        }

    }


    public void lumyMinimapIconColor()
    {
        AgentContext[] units = GameObject.FindObjectsOfType<AgentContext>();
        foreach (AgentContext lumy in units)
        {
            //Icons Size
            if (lumy.name == "p1_queen" || lumy.name == "p2_queen")
            {
                lumy.transform.GetChild(6).transform.localScale = new Vector3(3, 3, 3);
            }
            else
            {
                lumy.transform.GetChild(6).transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            }

            //Icons Color
            if (lumy.Home.gameObject.name == "p1_hive")
            {
                Renderer lumyRendIcoMinimap = lumy.transform.GetChild(6).GetComponent<Renderer>();

                lumyRendIcoMinimap.material = player1Material;
            }
            else if (lumy.Home.gameObject.name == "p2_hive")
            {
                Renderer lumyRendIcoMinimap = lumy.transform.GetChild(6).GetComponent<Renderer>();
                lumyRendIcoMinimap.material = player2Material;
            }
            else
            {
                Renderer lumyRendIcoMinimap = lumy.transform.GetChild(6).GetComponent<Renderer>();
                lumyRendIcoMinimap.material = playerDefaultMaterial;
            }
        }
    }

    /// <summary>
    /// Change the self returns in case of a Tab clicked. 
    /// Used for switching of focus on Lumy. 
    /// </summary>
    private void SwitchFocus()
    {
        if(winState == true)
        {
            return; 
        }
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Tab))
        {
            indiceFocus--;
            DecrementFocus();
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            indiceFocus++;
            IncrementFocus(); 
        }

    }

    private void updateFocus(AgentContext[] agentList)
    {
        //Enable Selection Shader
        InGameUIController.instance.Self.gameObject.transform.GetChild(2).gameObject.SetActive(true); //Enable Canvas
        //Enable MC
        MC_Debugger_Manager.instance.activateDebugger(agentList[indiceFocus].gameObject.GetComponent<AgentEntity>());
        //Change the color based on the player
        ColorPlayer(this.self);
        //Enable showing in the UI
        this.unitSelected = true;
        //Show The unitCost
        this.unitCost();
        //At the end increment the indiceFocus
    }

    private void DecrementFocus()
    {
        AgentContext[] agentList = FindObjectsOfType<AgentContext>();
        if (InGameUIController.instance.self != null)
        {
            //Disable Shader Selection
            InGameUIController.instance.Self.gameObject.transform.GetChild(2).gameObject.SetActive(false); //Enable Canvas
        }
        //Change the Self for the Focus
        if (indiceFocus < 0)
        {
            indiceFocus = agentList.Length;
            this.self = agentList[indiceFocus].Self.GetComponent<AgentScript>();
        }
        else
        {
            this.self = agentList[indiceFocus].Self.GetComponent<AgentScript>();
        }
        if (self.Cast == "prysme")
        {
            if (self.gameObject.GetComponentInParent<AgentContext>().Home.name == "p2_hive")
            {
                ShowStatPrysme(PlayerAuthority.Player2);
            }
            else
            {
                ShowStatPrysme(PlayerAuthority.Player1);
            }
        }
        else
        {
            ShowStatLumy();
        }
        updateFocus(agentList);
    }

    private void IncrementFocus()
    {
        AgentContext[] agentList = FindObjectsOfType<AgentContext>();
        if (InGameUIController.instance.self != null)
        {
            //Disable Shader Selection
            InGameUIController.instance.Self.gameObject.transform.GetChild(2).gameObject.SetActive(false); //Enable Canvas
        }
        //Change the Self for the Focus
        if (indiceFocus >= agentList.Length)
        {
            indiceFocus = 0;
            this.self = agentList[indiceFocus].Self.GetComponent<AgentScript>();
        }
        else
        {
            this.self = agentList[indiceFocus].Self.GetComponent<AgentScript>();
        }
        if (self.Cast == "prysme")
        {
            if (self.gameObject.GetComponentInParent<AgentContext>().Home.name == "p2_hive")
            {
                ShowStatPrysme(PlayerAuthority.Player2);
            }
            else
            {
                ShowStatPrysme(PlayerAuthority.Player1);
            }
        }
        else
        {
            ShowStatLumy();
        }
        updateFocus(agentList);
    }

    public void ColorPlayer(AgentScript agentScript)
    {
        if (agentScript.GetComponentInParent<AgentContext>().Home.gameObject.GetComponent<HomeScript>().Authority == PlayerAuthority.Player1)
        {
            this.color = colorPlayer1;
        }
        else if (agentScript.GetComponentInParent<AgentContext>().Home.gameObject.GetComponent<HomeScript>().Authority == PlayerAuthority.Player2)
        {
            this.color = colorPlayer2; 
        }
        else
        {
            this.color = Color.white; 
        }
    }

    #region Bottom Left Panel Actions
    public void ShowStatLumy()
    {
        statGrid.SetActive(true);
        statPrysmeJ1.SetActive(false);
        StatPrysmeJ2.SetActive(false);
    }
    public void ShowStatPrysme(PlayerAuthority player)
    {
        if (player == PlayerAuthority.Player1)
        {
            statGrid.SetActive(false);
            statPrysmeJ1.SetActive(true);
            StatPrysmeJ2.SetActive(false);
        }

        if (player == PlayerAuthority.Player2)
        {
            statGrid.SetActive(false);
            statPrysmeJ1.SetActive(false);
            StatPrysmeJ2.SetActive(true);
        }
    }
    #endregion

}

