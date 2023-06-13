using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TurnSystem : MonoBehaviour {
    public static TurnSystem Instance { get; private set; }

    public event EventHandler<OnTurnChangedEventArgs> OnTurnChanged;
    public class OnTurnChangedEventArgs : EventArgs {
        public Heroes heroWithTurn;
    }

    /* Event for Round End */
    public event EventHandler<OnRoundEndedEventArgs> OnRoundEnded;
    public class OnRoundEndedEventArgs : EventArgs {
        public int roundNum;
    }

    private int turnNumber = 1;
    private int roundNumber = 1;

    private Heroes heroWithTurn;
    private int numOfCharactersAtRoundStart;
    //private bool isHeroTurn = true;
    public List<Heroes> playingCharacters = new List<Heroes>();
    private List<Heroes> tmpTurnPlay = new List<Heroes>();
    public List<int> turnBasedOnDice = new List<int>();

    //private Heroes heroHasTurm;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    public Heroes GetHeroWithTurn() {
        return this.heroWithTurn;
    }

    public void SetPlayingCharacters(List<Heroes> playingCharacters) {
        this.playingCharacters = playingCharacters;
        this.numOfCharactersAtRoundStart = playingCharacters.Count;
    }

    public void ResetTurnNumber() {
        this.turnNumber = 1;
    }
    public void ResetRoundNumber() {
        this.roundNumber = 1;
    }

    /* This Method Sorts The Characters List Based On The Turn */
    public List<Heroes> CharactersSortByDicePlay() {

        turnBasedOnDice.Sort();
        turnBasedOnDice.Reverse();
        tmpTurnPlay.Clear();
        for (int i = 0; i < turnBasedOnDice.Count; i++) {
            foreach (Heroes hero in playingCharacters) {
                if (hero == null) {
                    continue;
                }
                if (turnBasedOnDice != null && hero.diceValue == turnBasedOnDice[i]) {
                    if (!tmpTurnPlay.Contains(hero)) { // if the hero is already there because of duplicate dice value with some
                        tmpTurnPlay.Add(hero);         // other hero, then do not add it again
                    }
                }
            }    
        }
        this.playingCharacters = tmpTurnPlay;
        for (int j = 0; j< this.playingCharacters.Count; j++) {
            if (this.playingCharacters[j] != null) {
                this.playingCharacters[j].SetIsPlayersTurn(true);
                Debug.Log("Player's turn: " + this.playingCharacters[j].ToString());
                this.heroWithTurn = this.playingCharacters[j];
                //StartCoroutine(FirstTurn(this.playingCharacters[j]));
                FirstTurn(this.playingCharacters[j]);
               /* OnTurnChanged?.Invoke(this, new OnTurnChangedEventArgs { // inform the first player that it is his turn
                    heroWithTurn = this.playingCharacters[j]
                });*/
                break;
            }
        }
        return this.playingCharacters;
    }

    /* Wait for some time after the dice button is pushed */
    public void FirstTurn(Heroes heroTurn) {
        /**/
        UI_Manager.Instance.nextTurnButton.SetActive(!heroWithTurn.GetIsEnemy());
        /**/
        OnTurnChanged?.Invoke(this, new OnTurnChangedEventArgs { // inform the first player that it is his turn
            heroWithTurn = heroTurn
        });
        UI_Manager.Instance.SetTurnInfo(this.heroWithTurn.GetIsEnemy(),true);

    }
    
    public void NextTurn() {
        /* find player with turn previously */
        int indexOfHeroWithTurn = 0;
        foreach (Heroes hadTurnHero in this.playingCharacters) {
            if (hadTurnHero.GetIsPlayersTurn()) {
                Debug.Log("Previous Turn: "+hadTurnHero.ToString());
                hadTurnHero.performedActions = 0;
                break;
            }
            indexOfHeroWithTurn++;
        }
        this.playingCharacters[indexOfHeroWithTurn].SetIsPlayersTurn(false);
        int k1 = 1;
        int i = indexOfHeroWithTurn;
        Heroes lastHeroWithTurn = null ;
        for (i = indexOfHeroWithTurn; i < this.playingCharacters.Count - 1; i++) {
            if (this.playingCharacters[indexOfHeroWithTurn + k1] != null && !this.playingCharacters[indexOfHeroWithTurn + k1].GetIsDead()) {
                this.playingCharacters[indexOfHeroWithTurn + k1].SetIsPlayersTurn(true);
                this.heroWithTurn = this.playingCharacters[indexOfHeroWithTurn + k1];
                lastHeroWithTurn = this.playingCharacters[indexOfHeroWithTurn + k1];
                /**/
                if (heroWithTurn.GetIsEnemy()) {
                    UI_Manager.Instance.nextTurnButton.SetActive(false);
                }
                /**/
                OnTurnChanged?.Invoke(this, new OnTurnChangedEventArgs {
                    heroWithTurn = this.playingCharacters[indexOfHeroWithTurn + k1]
                });
                UI_Manager.Instance.SetTurnInfo(this.heroWithTurn.GetIsEnemy(), true);
                break;
            }
            else {
                k1++;
            }       
        }
        turnNumber++;
        
        if (i >= this.playingCharacters.Count -1) { // have to change round
            /**/if (lastHeroWithTurn != null) {
                lastHeroWithTurn.SetIsPlayersTurn(false);
                lastHeroWithTurn.SelectedHeroVisual();
            }/**/
            UI_Manager.Instance.nextTurnButton.SetActive(false); // hide next turn button
            roundNumber++;
            UI_Manager.Instance.actionButtons.SetActive(false);
            UI_Manager.Instance.diceButton.SetActive(true);
            OnRoundEnded?.Invoke(this, new OnRoundEndedEventArgs {
                roundNum = roundNumber
            });
            UI_Manager.Instance.SetTurnInfo(false, false);
        }
        UI_Manager.Instance.gameRound.text = "ROUND " + TurnSystem.Instance.GetRoundNumber();
        UI_Manager.Instance.gameTurn.text = "TURN " + TurnSystem.Instance.GetTurnNumber();
    }


    public int GetTurnNumber() {
        return this.turnNumber;
    }

    public int GetRoundNumber() {
        return this.roundNumber;
    }

    public bool IsHeroTurn() {
        return this.playingCharacters[this.turnNumber - 1].GetIsPlayersTurn();
        ///return isHeroTurn;
    }

}