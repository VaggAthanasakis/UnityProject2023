using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UI_Manager : MonoBehaviour {
    public static UI_Manager Instance { get; private set; }
 
    /* Text for game state */
    private string freeRoam = "Free Roam";
    private string combatMode = "Combat Mode";
    private string victory = "Victory";
    private string gameOver = "Game Over";

    /* Text for state info */
    private string freeRoamInfo = "At This State You Can Move Around Freely Discovering The Environment";
    private string combatModeInfo = "Try To Kill All The Enemies";
    private string victoryInfo = "You Killed All The Enemies!";
    private string defeatInfo = "Enemies Killed All Your Heroes..";

    /* Actions/Turn System Panel */
    [SerializeField] GameObject footerBarInfo;
    [SerializeField] GameObject actionButtons;
    [SerializeField] GameObject gameTurnButtons;
    [SerializeField] GameObject nextTurnButton;
    [SerializeField] public GameObject diceButton;

    /* Action Buttons */
    [SerializeField] GameObject attackButton;
    [SerializeField] GameObject healButton;
    [SerializeField] GameObject begEnemyButton;
    [SerializeField] GameObject castSpellButton;
    [SerializeField] GameObject dashButton;
    [SerializeField] GameObject playMusicButton;

    /* Game Info Panel */
    [SerializeField] public GameObject gameInfo;
    [SerializeField] public TextMeshProUGUI gameInfoText;
    private int gameInfoVisualTimer = 100;

    /* Header Texts */
    [SerializeField] TextMeshProUGUI gameState;
    [SerializeField] TextMeshProUGUI stateInfo;

    /* Body Selected Hero Texts And Panel */
    [SerializeField] private GameObject selectedHeroPanel;
    [SerializeField] private TextMeshProUGUI SelectedHeroInfoTitle;
    [SerializeField] private TextMeshProUGUI HeroInfo;
    private Heroes selectedHero = null;

    /* Body Selected Object Texts And Panel */
    [SerializeField] public GameObject selectedObjectPanel;
    [SerializeField] private TextMeshProUGUI SelectedObjectInfoTitle;
    [SerializeField] private TextMeshProUGUI selectedObjectInfo;
    private Heroes selectedObject = null;

    /* Body Game Round And Turn Texts */
    [SerializeField] private GameObject roundInfoPanel;
    [SerializeField] public TextMeshProUGUI gameRound;
    [SerializeField] public TextMeshProUGUI gameTurn;

    /* Pause Menu UI */
    [SerializeField] private GameObject PauseMenuPanel;


    private void Awake() {
        Instance = this;
        this.gameInfo.SetActive(false);
        // this.gameState.text = freeRoam;
        this.actionButtons.SetActive(false);
        this.diceButton.SetActive(true);
        this.nextTurnButton.SetActive(false);
        this.selectedHeroPanel.SetActive(false);
        this.selectedObjectPanel.SetActive(false);
        this.roundInfoPanel.SetActive(false); 
        gameRound.text = "ROUND 1";
        gameTurn.text = "TURN 1";
        //DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        GameManager.Instance.SetCurrentState(GameManager.State.FreeRoam);
        MouseClick.instance.OnHeroSelectAction += MouseClick_OnHeroSelectAction;
        MouseClick.instance.OnInteractableObjectSelection += MouseClick_OnInteractableObjectSelection;
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

 
    private void Update() {
        /* If the gameInfo panel is active for some frames, then diactivate it */
        if (this.gameInfo.activeSelf == true && this.gameInfoVisualTimer <= 0) {
            this.gameInfo.SetActive(false);
            this.gameInfoVisualTimer = 100;
        }
        else if(this.gameInfo.activeSelf == true && this.gameInfoVisualTimer > 0) {
            this.gameInfoVisualTimer -= 1;
        }

        PauseMenuScript(); // handle pause menu
    }

    /* When a hero is selected, we display his info */
    private void MouseClick_OnHeroSelectAction(object sender, MouseClick.OnHeroSelectActionEventArgs e) {
        //this.selectedHero = e.selectedHero;
        if (e.selectedHero != null && !e.selectedHero.GetIsEnemy()) {
            this.selectedHero = e.selectedHero;
            this.SelectedHeroInfoTitle.text = "Selected Hero Info";
            this.HeroInfo.text = selectedHero.HeroStatisticsToString();
            this.selectedHeroPanel.SetActive(true);
        }
        else if (e.selectedHero != null && e.selectedHero.GetIsEnemy() && this.selectedHero != null) { 
            // if we selected an enemy, we keep the panel as it is
        }
        else
            this.selectedHeroPanel.SetActive(false);

    }

    /* When an interactable game object is selected, we display his info */
    private void MouseClick_OnInteractableObjectSelection(object sender, MouseClick.OnInteractableObjectSelectionEventArgs e) {
        if (e.selectedInteractableObject != null && e.selectedInteractableObject.GetObjectType() == InteractableObject.Type.Chest) {
            this.SelectedObjectInfoTitle.text = "Mystery Box";
            this.selectedObjectInfo.text = e.selectedInteractableObject.InteractableObjectToString();
            this.selectedObjectPanel.SetActive(true);
        }
    }

    /***********************************************************************/
    /* Code for buttons */
    public void Button_Attack() {
        Debug.Log("Attack pushed");
        Heroes heroWithTurn = GameManager.Instance.GetHeroWithTurn();
        Heroes attackedHero;
        if (heroWithTurn.GetIsEnemy()) {
            attackedHero = MouseClick.instance.GetSelectedHero();
        }
        else {
            attackedHero = MouseClick.instance.GetSelectedEnemy();
        }
        if (attackedHero != null && attackedHero != heroWithTurn) {
            heroWithTurn.PerformAttack(attackedHero);
            if (heroWithTurn.GetRemainingMoveRange() <= 0 && heroWithTurn.performedActions >= heroWithTurn.numOfAllowedActions) {
                StartCoroutine(TurnSystem.Instance.NextTurn()); // na mpei elegxos an exei kai allo move
            }
        }
        else if(attackedHero == null) {
            Debug.Log("ATTACKED HERO NULL");           
        }

        /* AUTA THA MPOUN OTAN KANO TO AI

        Heroes enemyHero = MouseClick.instance.GetSelectedEnemy();
        
        if (enemyHero != null && enemyHero != heroWithTurn) {
            Debug.Log("Can Perform Attack!");
            heroWithTurn.PerformAttack(enemyHero);
            // IF HERO CANNOT FURTHER MOVE
            TurnSystem.Instance.NextTurn(); // na mpei elegxos an exei kai allo move
            gameRound.text = "ROUND " + TurnSystem.Instance.GetRoundNumber();
            gameTurn.text = "TURN " + TurnSystem.Instance.GetTurnNumber();
        }
        */
    }

    public void Buttom_Heal() {
        Debug.Log("Heal Button Pushed!");
        Heroes heroWithTurn = GameManager.Instance.GetHeroWithTurn();
        if (heroWithTurn.heroClass != Mage.HERO_CLASS && heroWithTurn.heroClass != Priest.HERO_CLASS) { return; }

        Heroes heroToHeal = MouseClick.instance.GetSelectedHero();
        /* If we have selected an other hero to heal */
        if (heroToHeal != null && heroToHeal != heroWithTurn && heroWithTurn.GetIsEnemy() == heroToHeal.GetIsEnemy()) {
            heroWithTurn.PerformHeal(heroToHeal);
            /* if the hero cannot move further and has complete the number of allowed actions per round, then next turn */
            if (heroWithTurn.GetRemainingMoveRange() <= 0 && heroWithTurn.performedActions >= heroWithTurn.numOfAllowedActions) {
                StartCoroutine(TurnSystem.Instance.NextTurn()); // na mpei elegxos an exei kai allo move
            }
        }
    }

    /* Button for playing the dice for all the characters */
    public void Button_DicePlay() {
        this.roundInfoPanel.SetActive(true);
        TurnSystem.Instance.turnBasedOnDice.Clear();
        TurnSystem.Instance.SetPlayingCharacters(GameManager.Instance.aliveCharacters);// some heroes may died in the previous round 
        foreach (Heroes character in GameManager.Instance.aliveCharacters) {
            int diceValue = Dice.instance.RollDice();
            //Debug.Log("Class: " + character.ToString() + " Is Enemy: " + character.GetIsEnemy() + " Dice Value = " + diceValue);
            character.diceValue = diceValue;
            TurnSystem.Instance.turnBasedOnDice.Add(diceValue);
        }
        TurnSystem.Instance.CharactersSortByDicePlay();
        this.diceButton.SetActive(false);
    }

    /* If the player has remaining move range but do not want to use it, should press this button */
    public void Button_NextTurn() {
        Heroes heroWithTurn = GameManager.Instance.GetHeroWithTurn();
        StartCoroutine(TurnSystem.Instance.NextTurn()); 
    }

    /* button for enemy beg */
    public void Button_BegEnemy() {
        Debug.Log("Buttom Beg Enemy Pushed");
        Heroes heroWithTurn = GameManager.Instance.GetHeroWithTurn();
        Heroes enemyToBeg = MouseClick.instance.GetSelectedEnemy();

        if (heroWithTurn.heroClass != Priest.HERO_CLASS || heroWithTurn.GetIsEnemy() == enemyToBeg.GetIsEnemy()) { return; }
        heroWithTurn.Beg(enemyToBeg);
        /* if the hero cannot move further and has complete the number of allowed actions per round, then next turn */
        if (heroWithTurn.GetRemainingMoveRange() <= 0 && heroWithTurn.performedActions >= heroWithTurn.numOfAllowedActions) {
            StartCoroutine(TurnSystem.Instance.NextTurn()); 
        }
    }

    /* Button for cast spelling */
    public void Button_CastSpell() {
        Debug.Log("Buttom Cast Spell Pushed");
        Heroes heroWithTurn = GameManager.Instance.GetHeroWithTurn();

        if (heroWithTurn.heroClass != Priest.HERO_CLASS && heroWithTurn.heroClass != Mage.HERO_CLASS) { return; }
        heroWithTurn.CastSpell();
        /* if the hero cannot move further and has complete the number of allowed actions per round, then next turn */
        if (heroWithTurn.GetRemainingMoveRange() <= 0 && heroWithTurn.performedActions >= heroWithTurn.numOfAllowedActions) {
            StartCoroutine(TurnSystem.Instance.NextTurn()); // na mpei elegxos an exei kai allo move
        }
    }

    /* Button for Dash action */
    public void Button_Dash() {
        Debug.Log("Button Dash Pushed");
        Heroes heroWithTurn = GameManager.Instance.GetHeroWithTurn();
        heroWithTurn.Dash();
        /* if the hero cannot move further and has complete the number of allowed actions per round, then next turn */
        if (heroWithTurn.GetRemainingMoveRange() <= 0 && heroWithTurn.performedActions >= heroWithTurn.numOfAllowedActions) {
            StartCoroutine(TurnSystem.Instance.NextTurn()); 
        }
    }

    /* Buttom for Playing Music */
    public void Button_PlayMusic() {
        Debug.Log("Buttom Play Music Pushed");
        Heroes heroWithTurn = GameManager.Instance.GetHeroWithTurn();

        if (heroWithTurn.heroClass != Musician.HERO_CLASS) { return; }
        heroWithTurn.PlayMusic();
        /* if the hero cannot move further and has complete the number of allowed actions per round, then next turn */
        if (heroWithTurn.GetRemainingMoveRange() <= 0 && heroWithTurn.performedActions >= heroWithTurn.numOfAllowedActions) {
            StartCoroutine(TurnSystem.Instance.NextTurn()); 
        }
    }

    /***********************************************************************/
    public void SetStateInfo() {
        if (GameManager.Instance.GetCurrentState() == GameManager.State.FreeRoam) {
            this.gameState.text = freeRoam;
            this.stateInfo.text = freeRoamInfo;
            SetActionButtonsPanelActive(false);
        }
        else if (GameManager.Instance.GetCurrentState() == GameManager.State.CombatMode) {
            this.gameState.text = combatMode;
            this.stateInfo.text = combatModeInfo;
            //SetActionButtonsPanelActive(true);
            this.footerBarInfo.SetActive(true);
            this.gameTurnButtons.SetActive(true);
        }
        else if (GameManager.Instance.GetCurrentState() == GameManager.State.GameOver) {
            this.gameState.text = gameOver;
            this.stateInfo.text = defeatInfo;
            SetActionButtonsPanelActive(false);
        }
        else if (GameManager.Instance.GetCurrentState() == GameManager.State.Victory) {
            this.gameState.text = victory;
            this.stateInfo.text = victoryInfo;
            SetActionButtonsPanelActive(false);
        }
    }

    /* This method sets the panel with the turn and action buttons  active/non-active  */
    private void SetActionButtonsPanelActive(bool b) {
            this.actionButtons.SetActive(b);
            this.footerBarInfo.SetActive(b);
            this.gameTurnButtons.SetActive(b);
            this.nextTurnButton.SetActive(b);

    }
    /* Activate the proper action buttons based on the current hero who has turn */
    private void TurnSystem_OnTurnChanged(object sender, TurnSystem.OnTurnChangedEventArgs e) {
        if (!e.heroWithTurn.GetIsEnemy()) {
            SetActionButtonsPanelActive(true);
            if (e.heroWithTurn.GetActionsList().Contains("Attack")) {
                this.attackButton.SetActive(true);
            }
            else
                this.attackButton.SetActive(false);
            if (e.heroWithTurn.GetActionsList().Contains("Heal")) {
                this.healButton.SetActive(true);
            }
            else
                this.healButton.SetActive(false);
            if (e.heroWithTurn.GetActionsList().Contains("BegEnemy")) {
                this.begEnemyButton.SetActive(true);
            }
            else
                this.begEnemyButton.SetActive(false);
            if (e.heroWithTurn.GetActionsList().Contains("CastSpell")) {
                this.castSpellButton.SetActive(true);
            }
            else
                this.castSpellButton.SetActive(false);
            if (e.heroWithTurn.GetActionsList().Contains("Dash")) {
                this.dashButton.SetActive(true);
            }
            else
                this.dashButton.SetActive(false);
            if (e.heroWithTurn.GetActionsList().Contains("PlayMusic")) {
                this.playMusicButton.SetActive(true);
            }
            else
                this.playMusicButton.SetActive(false);


        }
        else {
            SetActionButtonsPanelActive(false);
        }
    }

    /* Set the info for the gameInfo panel */
    public void SetGameInfo(string info) {
        this.gameInfo.SetActive(true);
        this.gameInfoText.text = info;
    }


    /* Pause Menu Script */
    private void PauseMenuScript() {
        /* We pause the game if we presse Escape */
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (GameManager.isGamePaused) { // if the game is paused and we press escape again -> resume
                ResumeGame();
            }
            else {                         // else pause the game
                PauseGame();
            
            }
        }
    
    }


    /* Resume Game method */
    public void ResumeGame() {
        this.PauseMenuPanel.SetActive(false); // Deactivate the pause menu when we pause the game
        Time.timeScale = 1f;                  // Restore the time rate back to normal
        GameManager.isGamePaused = false;     // Inform the GameManager that the game is not paused
    }


    /* Pause Game Method */
    private void PauseGame() {
        this.PauseMenuPanel.SetActive(true); // activate the pause menu when we pause the game
        Time.timeScale = 0f;                 // completely freeze time
        GameManager.isGamePaused = true;     // Inform the GameManager that the game is paused
    }

    /* Function that is called when we press Menu */
    public void LoadMenu_Button() {
        Debug.Log("Loading Menu...");
        Time.timeScale = 1f;                 // Restore the time rate back to normal
        /* Clear the characters list. not necessary since object get destroyed */
        GameManager.Instance.aliveCharacters = null;
        GameManager.Instance.aliveEnemies = null;
        GameManager.Instance.aliveCharacters = null;
        if (GameManager.Instance.GetCurrentState() == GameManager.State.FreeRoam) {
            Debug.Log("Here1");
            SoundManager.Instance.StopSoundWithoutFade(SoundManager.FREE_ROAM_MUSIC);
            SoundManager.Instance.PlaySoundWithoutFade(SoundManager.MAIN_MENU_CHAR_SELECTION_MUSIC);
            /*StartCoroutine(SoundManager.Instance.StopSound(SoundManager.FREE_ROAM_MUSIC));
            StartCoroutine(SoundManager.Instance.PlaySound(SoundManager.MAIN_MENU_CHAR_SELECTION_MUSIC));*/
        }
        else if (GameManager.Instance.GetCurrentState() == GameManager.State.CombatMode) {
            Debug.Log("Here2");
            SoundManager.Instance.StopSoundWithoutFade(SoundManager.COMBAT_MODE_MUSIC);
            SoundManager.Instance.PlaySoundWithoutFade(SoundManager.MAIN_MENU_CHAR_SELECTION_MUSIC);
            /*StartCoroutine(SoundManager.Instance.StopSound(SoundManager.COMBAT_MODE_MUSIC));
            StartCoroutine(SoundManager.Instance.PlaySound(SoundManager.MAIN_MENU_CHAR_SELECTION_MUSIC));*/
        }
        SceneLoader.LoadScene(SceneLoader.Scene.MainMenuScene);

    }

    /* Function that is called when we press Quit  */
    public void QuitGame_Button() {
        Debug.Log("Quiting Game");
        Application.Quit();
    }


}
