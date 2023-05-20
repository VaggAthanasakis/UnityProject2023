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
    //private bool isHeroTurn = true;
    private List<Heroes> playingCharacters = new List<Heroes>();
    private List<Heroes> tmpTurnPlay = new List<Heroes>();
    public List<int> turnBasedOnDice = new List<int>();

    //private Heroes heroHasTurm;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //heroHasTurm = null;
    }

    public void SetPlayingCharacters(List<Heroes> playingCharacters) {
        this.playingCharacters = playingCharacters;
    }

    /* This Method Sorts The Characters List Based On The Turn */
    public List<Heroes> CharactersSortByDicePlay() {

        turnBasedOnDice.Sort();
        turnBasedOnDice.Reverse();
        for (int i = 0; i < turnBasedOnDice.Count; i++) {
            foreach (Heroes hero in playingCharacters) {
                if (hero.diceValue == turnBasedOnDice[i])
                    tmpTurnPlay.Add(hero);
            }
            
        }
        this.playingCharacters = tmpTurnPlay;
        this.playingCharacters[0].SetIsPlayersTurn(true);
        Debug.Log("Player's turn: "+this.playingCharacters[0].ToString());
        OnTurnChanged?.Invoke(this, new OnTurnChangedEventArgs {
            heroWithTurn = this.playingCharacters[0]
        });
        return this.playingCharacters;
    }

    public void NextTurn() { // den exei xrhsimopoihthei akoma
        /* Check if the round has ended */
        if (turnNumber > this.playingCharacters.Count) {
            /* fire the event of round end */
            roundNumber++;
            turnNumber = 1;
            Debug.Log("Round "+roundNumber+" Ended");
            OnRoundEnded?.Invoke(this, new OnRoundEndedEventArgs {
                roundNum = roundNumber
            });
            return;
        }
        Debug.Log("Next Turn: " + turnNumber);
        this.playingCharacters[turnNumber-1].SetIsPlayersTurn(false);
        turnNumber++;
        this.playingCharacters[turnNumber - 1].SetIsPlayersTurn(true);
        
        OnTurnChanged?.Invoke(this, new OnTurnChangedEventArgs { 
            heroWithTurn = this.playingCharacters[turnNumber - 1]
        });
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