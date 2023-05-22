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

    public void SetPlayingCharacters(List<Heroes> playingCharacters) {
        this.playingCharacters = playingCharacters;
        this.numOfCharactersAtRoundStart = playingCharacters.Count;
    }

    public void ResetTurnNumber() {
        this.turnNumber = 1;
    }

    /* This Method Sorts The Characters List Based On The Turn */
    public List<Heroes> CharactersSortByDicePlay() {

        turnBasedOnDice.Sort();
        turnBasedOnDice.Reverse();
        tmpTurnPlay.Clear();
        Debug.Log("TurnBasedOnDice.Count "+turnBasedOnDice.Count);
        for (int i = 0; i < turnBasedOnDice.Count; i++) {
            foreach (Heroes hero in playingCharacters) {
                if (hero.diceValue == turnBasedOnDice[i])
                    tmpTurnPlay.Add(hero);
            }
            
        }
        Debug.Log("PlayingCharacters.Count " + playingCharacters.Count);
        this.playingCharacters = tmpTurnPlay;
        Debug.Log("PlayingCharacters.Count " + playingCharacters.Count);
        

        for (int j = 0; j< this.playingCharacters.Count; j++) {
            if (this.playingCharacters[j] != null) {
                this.playingCharacters[j].SetIsPlayersTurn(true);
                Debug.Log("Player's turn: " + this.playingCharacters[j].ToString());
                OnTurnChanged?.Invoke(this, new OnTurnChangedEventArgs { // inform the first player that it is his turn
                    heroWithTurn = this.playingCharacters[j]
                });
                break;
            }
        }
        return this.playingCharacters;
    }

    public void NextTurn() {
        /* find player with turn previously */
        int indexOfHeroWithTurn = 0;
        foreach (Heroes hadTurnHero in this.playingCharacters) {
            if (hadTurnHero.GetIsPlayersTurn()) {
                Debug.Log("Previous Turn: "+hadTurnHero.ToString());
                break;
            }
            indexOfHeroWithTurn++;
        }
        this.playingCharacters[indexOfHeroWithTurn].SetIsPlayersTurn(false);
        int k1 = 1;
        int i = indexOfHeroWithTurn;
        for (i = indexOfHeroWithTurn; i < this.playingCharacters.Count - 1; i++) {
            if (this.playingCharacters[indexOfHeroWithTurn + k1] != null && !this.playingCharacters[indexOfHeroWithTurn + k1].GetIsDead()) {
                this.playingCharacters[indexOfHeroWithTurn + k1].SetIsPlayersTurn(true);
                Debug.Log("Current Turn " + this.playingCharacters[indexOfHeroWithTurn + k1] + " with k1= " + k1);
                OnTurnChanged?.Invoke(this, new OnTurnChangedEventArgs {
                    heroWithTurn = this.playingCharacters[indexOfHeroWithTurn + k1]
                });
                break;
            }
            else {
                k1++;
            }
        }
        Debug.Log("i = "+i);
        Debug.Log("Count+1 "+ this.playingCharacters.Count);
        if (i >= this.playingCharacters.Count -1) { // have to change round
            OnRoundEnded?.Invoke(this, new OnRoundEndedEventArgs {
                roundNum = roundNumber
            });
            
        }
    }


    /*
    public void NextTurn() { // den exei xrhsimopoihthei akoma
         Check if the round has ended 
        Debug.Log("Turn Number "+turnNumber);
        Debug.Log("PlayingCharacter.Count "+ GameManager.Instance.aliveCharacters.Count);
        Debug.Log("NumOfChara... "+numOfCharactersAtRoundStart);

        //if (turnNumber >= numOfCharactersAtRoundStart || turnNumber == this.playingCharacters.Count) {
        if (turnNumber >= numOfCharactersAtRoundStart) {
             fire the event of round end 
            Debug.Log("Round " + roundNumber + " Ended");
            roundNumber++;
            turnNumber = 1;

            OnRoundEnded?.Invoke(this, new OnRoundEndedEventArgs {
                roundNum = roundNumber
            });

        }
        else {
            

            
            this.playingCharacters[turnNumber - 1].SetIsPlayersTurn(false); // previous character loses turn
             Have to check if the next character has been killed previously in the round 
            int k = 0;
            for (int i = turnNumber + 1; i < this.playingCharacters.Count; i++) {
                if (turnNumber + k >= this.numOfCharactersAtRoundStart) {
                    // All Players Have Play
                    Debug.Log("All Players Have Play");
                    OnRoundEnded?.Invoke(this, new OnRoundEndedEventArgs {
                        roundNum = roundNumber
                    });
                    return;
                }
                else if (this.playingCharacters[turnNumber + k] != null && !this.playingCharacters[turnNumber + k].GetIsDead()) {
                    this.playingCharacters[turnNumber + k].SetIsPlayersTurn(true);
                }
                else {
                    k++;
                }
            }


            int test1 = turnNumber - 1 + k;
            Debug.Log("[turnNUmber -1 +k] " + test1);
            Debug.Log("playingCharacters.Count " + playingCharacters.Count);
            OnTurnChanged?.Invoke(this, new OnTurnChangedEventArgs {
                heroWithTurn = this.playingCharacters[turnNumber + k]
            });
            turnNumber++;
            Debug.Log("Next Turn: " + turnNumber);
        }
    }
        */
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