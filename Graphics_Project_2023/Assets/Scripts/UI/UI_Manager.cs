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
    private string gameOver = "Game Over";

    /* Text for state info */
    private string freeRoamInfo = "At This State You Can Move Around Freely Discovering The Environment";
    private string combatModeInfo = "Try To Kill All The Enemies";
 

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
        this.roundInfoPanel.SetActive(true); // PREPEI NA TO KANO NA ANAVEI OTAN COMBAT
    }

    private void Start() {
        MouseClick.instance.OnHeroSelectAction += MouseClick_OnHeroSelectAction;
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
        Debug.Log("Attack Button Pushed!");
        Heroes enemyHero = MouseClick.instance.GetSelectedHero();
        Heroes hero = GameManager.Instance.GetHeroWithTurn();
        if (enemyHero.GetIsEnemy()) {
            Debug.Log("Can Perform Attack! Select Enemy!");
            hero.PerformAttack(enemyHero);  
        }
        // IF HERO CANNOT FURTHER MOVE
        TurnSystem.Instance.NextTurn(); // na mpei elegxos an exei kai allo move
        gameRound.text = "ROUND " + TurnSystem.Instance.GetRoundNumber();
        gameTurn.text = "TURN " + TurnSystem.Instance.GetTurnNumber();
    }

    public void Buttom_Heal() {
        Debug.Log("Heal Button Pushed!");
        // IF HERO CANNOT FURTHER MOVE
        TurnSystem.Instance.NextTurn(); // na mpei elegxos an exei kai allo move
        gameRound.text = "ROUND " + TurnSystem.Instance.GetRoundNumber();
        gameTurn.text = "TURN " + TurnSystem.Instance.GetTurnNumber();
    }

    public void DicePlay() {
        //Debug.Log("Dice Button Pushed!");
        TurnSystem.Instance.turnBasedOnDice.Clear();
        TurnSystem.Instance.SetPlayingCharacters(GameManager.Instance.aliveCharacters);// some heroes may died in the previous round

        foreach (Heroes character in GameManager.Instance.aliveCharacters) {
            //Debug.Log("Class: " + character.ToString());
            int diceValue = Dice.instance.RollDice();
            character.diceValue = diceValue;
            TurnSystem.Instance.turnBasedOnDice.Add(diceValue);
            //Debug.Log("Dice Value: " + diceValue);
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
        }
    }



}
