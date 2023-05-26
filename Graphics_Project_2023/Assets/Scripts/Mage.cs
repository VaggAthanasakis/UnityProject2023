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

    /* function that is called when the hero levels up */
    protected override void LevelUp() {
        base.LevelUp(); // mporei na mhn xreiazetai
        /* if we reach here, the hero has level > 1 AND experience points > 2 */

        /* Case Upgrade To Level 3 */
        /* We level up if the hero has 4 experience points */
        if (this.GetLevel() == 2 && this.GetExperiencePoints() == 4) {
            /// code for level up
            this.SetLevel(this.GetLevel() + 1);
        }
        /* Case Upgrade To Level 4 */
        else if (this.GetLevel() == 3 && this.GetExperiencePoints() == 6) {
            // code for level up
            /* At this level the character is allowed to perform 2 main actions at the same turn */
            this.numOfAllowedActions++;
            this.SetLevel(this.GetLevel() + 1);
        }

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

    /* Calculates the damage amount of the attack. damageAmount = dice + main attribute */
    public override void AttackAmountCalculation() {
        int damageAmount = diceValue + this.intelligence - 3;
        this.SetIsCastSpelling(true);
        this.SetCurrentAttackAmount(damageAmount);
    }


}
