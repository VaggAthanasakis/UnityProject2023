using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour {

    public static Dice instance { get; private set; }
    private int diceValue;

    public void Awake() {
        instance = this;
    }

    public int RollDice() {
        return Random.Range(1,21);
    }


} 
