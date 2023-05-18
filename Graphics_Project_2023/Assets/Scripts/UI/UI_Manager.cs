using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Manager : MonoBehaviour
{
    private const string MAIN_GAME_SCENE = "MainGameScene";
    //private const string MAIN_MENU_SCENE = "MainMenuScene";

    [SerializeField] GameObject actionButtons;
    [SerializeField] GameObject DiceButton;
    //[SerializeField] GameObject mainMenuButtons;

    private void Awake() {

        this.actionButtons.SetActive(true);
        //this.mainMenuButtons.SetActive(true);
        this.DiceButton.SetActive(true);
    }

 

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

    private IEnumerable Attack_Wait(Heroes hero, Heroes enemyHero) {
        float attackDuration = 5f;
        yield return new WaitForSeconds(attackDuration);
        //hero.SetIsAttacking(false);
        //enemyHero.SetGetsHit(false);
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

    

}
