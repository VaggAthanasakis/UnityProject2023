using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager Instance { get; private set; }

    private const string MAIN_GAME_SCENE = "MainGameScene";
    private string freeRoam = "Free Roam";
    private string combatMode = "Combat Mode";
    private string gameOver = "Game Over";

    [SerializeField] GameObject actionButtons;
    [SerializeField] GameObject DiceButton;
    [SerializeField] TextMeshProUGUI gameState;

    private void Awake() {
        Instance = this;

        // this.gameState.text = freeRoam;
        this.actionButtons.SetActive(true);
        this.DiceButton.SetActive(true);
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
    }

    public void Buttom_Heal() {
        Debug.Log("Heal Button Pushed!");
        // IF HERO CANNOT FURTHER MOVE
        TurnSystem.Instance.NextTurn(); // na mpei elegxos an exei kai allo move
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
    public void SetGameStateText() {
        if (GameManager.Instance.GetCurrentState() == GameManager.State.FreeRoam) {
            this.gameState.text = freeRoam;
        }
        else if (GameManager.Instance.GetCurrentState() == GameManager.State.CombatMode) {
            this.gameState.text = combatMode;
        }
        else if (GameManager.Instance.GetCurrentState() == GameManager.State.GameOver) {
            this.gameState.text = gameOver;
        }
    }



}
