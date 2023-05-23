using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UI_Manager : MonoBehaviour
{
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

    [SerializeField] GameObject actionButtons;
    [SerializeField] GameObject DiceButton;

    /* Header Texts */
    [SerializeField] TextMeshProUGUI gameState;
    [SerializeField] TextMeshProUGUI stateInfo;

    /* Body Selected Hero Texts And Panel */
    [SerializeField] private GameObject selectedHeroPanel;
    [SerializeField] private TextMeshProUGUI SelectedHeroInfoTitle;
    [SerializeField] private TextMeshProUGUI HeroInfo;
    private Heroes selectedHero = null;

    /* Body Selected Object Texts And Panel */
    [SerializeField] private GameObject selectedObjectPanel;
    [SerializeField] private TextMeshProUGUI SelectedObjectInfoTitle;
    [SerializeField] private TextMeshProUGUI selectedObjectInfo;
    private Heroes selectedObject = null;

    /* Body Game Round And Turn Texts */
    [SerializeField] private GameObject roundInfoPanel;
    [SerializeField] private TextMeshProUGUI gameRound;
    [SerializeField] private TextMeshProUGUI gameTurn;



    private void Awake() {
        Instance = this;

        // this.gameState.text = freeRoam;
        this.actionButtons.SetActive(true);
        this.DiceButton.SetActive(true);
        this.selectedHeroPanel.SetActive(false);
        this.selectedObjectPanel.SetActive(false);
        this.roundInfoPanel.SetActive(false); // PREPEI NA TO KANO NA ANAVEI OTAN COMBAT
        gameRound.text = "ROUND 1";
        gameTurn.text = "TURN 1";

    }

    private void Start() {
        GameManager.Instance.SetCurrentState(GameManager.State.FreeRoam);
        MouseClick.instance.OnHeroSelectAction += MouseClick_OnHeroSelectAction;
        TurnSystem.Instance.OnRoundEnded += TurnSystem_OnRoundEnded;
    }

    /* event that arrives when game round changes */
    private void TurnSystem_OnRoundEnded(object sender, TurnSystem.OnRoundEndedEventArgs e) {
        this.gameRound.text = e.roundNum.ToString();
    }

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
            // IF HERO CANNOT FURTHER MOVE
            TurnSystem.Instance.NextTurn(); // na mpei elegxos an exei kai allo move
            gameRound.text = "ROUND " + TurnSystem.Instance.GetRoundNumber();
            gameTurn.text = "TURN " + TurnSystem.Instance.GetTurnNumber();
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
            // IF HERO CANNOT FURTHER MOVE
            TurnSystem.Instance.NextTurn(); // na mpei elegxos an exei kai allo move
            gameRound.text = "ROUND " + TurnSystem.Instance.GetRoundNumber();
            gameTurn.text = "TURN " + TurnSystem.Instance.GetTurnNumber();
        }
    }

    public void DicePlay() {
        this.roundInfoPanel.SetActive(true);

        TurnSystem.Instance.turnBasedOnDice.Clear();
        
        //TurnSystem.Instance.SetPlayingCharacters(GameManager.Instance.aliveCharacters);// some heroes may died in the previous round
       
        foreach (Heroes character in GameManager.Instance.aliveCharacters) {
            int diceValue = Dice.instance.RollDice();
            Debug.Log("Class: " + character.ToString() + " Is Enemy: " + character.GetIsEnemy() + " Dice Value = " + diceValue);
            character.diceValue = diceValue;
            TurnSystem.Instance.turnBasedOnDice.Add(diceValue);
        }
        TurnSystem.Instance.CharactersSortByDicePlay();

    }

    /***********************************************************************/
    public void SetStateInfo() {
        if (GameManager.Instance.GetCurrentState() == GameManager.State.FreeRoam) {
            this.gameState.text = freeRoam;
            this.stateInfo.text = freeRoamInfo;
        }
        else if (GameManager.Instance.GetCurrentState() == GameManager.State.CombatMode) {
            this.gameState.text = combatMode;
            this.stateInfo.text = combatModeInfo;
        }
        else if (GameManager.Instance.GetCurrentState() == GameManager.State.GameOver) {
            this.gameState.text = gameOver;
            this.stateInfo.text = defeatInfo;
        }
        else if (GameManager.Instance.GetCurrentState() == GameManager.State.Victory) {
            this.gameState.text = victory;
            this.stateInfo.text = victoryInfo;
        }
 
    }



}
