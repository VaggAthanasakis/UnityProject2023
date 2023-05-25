using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Mage : Heroes {
 
    private int strength;
    private int intelligence; // main attribute
    public const string HERO_CLASS = "Mage";

    /* Awake(), Start(), Update() */
    protected override void Awake() {
        base.Awake();
        this.SetNumOfAttributes(4);
        //targetPosition = this.transform.position;
        SetHeroFeatures();
    }

    // Start is called before the first frame update
    protected override void Start() { 
        base.Start();
        HeroAttributesToString();
        /*gridPosition = PathFinding.Instance.GetGridPosition(this.transform.position);
        PathFinding.Instance.AddUnitAtGridPosition(gridPosition,this.gameObject.GetComponent<Unit>());
        currentPositionIndex = 0;
        */
    }

    // Update is called once per frame
    void Update() {
        if (GameManager.Instance.GetCurrentState() == GameManager.State.FreeRoam || GameManager.Instance.GetCurrentState() == GameManager.State.CombatMode) {
            PerformMove();
        }
        base.AnimationsDurationControll();
    }

    /* Getters*/
    public int GetStrength() {
        return this.strength;
    }
    public int GetIntelligence() {
        return this.intelligence;
    }

    /* Setters */
    public void SetStrength(int str) {
        this.strength = str;
    }

    public void SetIntelligence(int amount) {
        this.intelligence = amount;
    }

    /********* Methods **********/

    protected override void SetHeroFeatures() {
        /* * */
        this.heroClass = HERO_CLASS;

        // setting the attributes
        int sumOfAttributesPoints = ((this.GetNumOfAttributes() - 1) * 4 + 2);
        this.intelligence = sumOfAttributesPoints / 2;
        int remain = sumOfAttributesPoints - this.intelligence;
        this.SetConstitution(remain / 2);
        remain = sumOfAttributesPoints - this.intelligence - this.GetConstitution();
        this.strength = remain/2;
        this.SetDexterity(remain - this.strength);

        // setting the features
        this.SetHealthPoints(10 + this.GetConstitution());
        this.SetArmorClass(10 + this.GetDexterity());
        this.SetAttackRange(3);
        this.SetMoveRange(4);
        this.SetCurrentHealthPoints(this.GetHealthPoints());
        this.SetRemainingMoveRange(this.GetMoveRange());

        /* Initialize Hero's Statistics */
        this.InitializeHeroStatistics();
    }

    /* Calculates the heal amount that the hero will produce */
    public override void HealAmountCalculation() {
        int healAmount = diceValue + this.GetIntelligence(); //MPOREI NA TO ALLAJO
        this.SetIsHealing(true);
        this.SetCurrentHealAmount(healAmount);
    }

    public override void HeroAttributesToString() {
        base.attributesToString = "Strength: " + this.GetStrength() + "\nArmor Class: " + this.GetArmorClass() + "\nMove Range: " + this.GetMoveRange() + "\n";
    }



}
