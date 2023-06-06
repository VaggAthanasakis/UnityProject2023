using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Fighter : Heroes {
  
    // main attribute
    private int strength;
    public const string HERO_CLASS = "Fighter";
    /* Awake(), Start(), Update() */
    protected override void Awake() {
        base.Awake();
        this.SetNumOfAttributes(3);
        //targetPosition = this.transform.position;
        SetHeroFeatures();
        AddAction("Attack");
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
        //Interact();
    }

    /* Getters */
    public int GetStrength() {
        return this.strength;
    }

    /* Setters */
    public void SetStrength(int str) {
        this.strength = str;
    }

    /********* Methods **********/
    protected override void SetHeroFeatures() {
        /* * */
        this.heroClass = HERO_CLASS;

        // setting the attributes
        int sumOfAttributesPoints = ((this.GetNumOfAttributes() - 1) * 4 + 2);  
        this.strength = sumOfAttributesPoints / 2;
        int remain = sumOfAttributesPoints - strength;
        this.SetDexterity(remain / 2);
        this.SetConstitution(sumOfAttributesPoints - strength - this.GetDexterity());
       
        // setting the features
        this.SetHealthPoints(10 + this.GetConstitution());
        this.SetArmorClass(10 + this.GetDexterity());
        this.SetAttackRange(2);
        this.SetMoveRange(4);
        this.SetCurrentHealthPoints(this.GetHealthPoints());
        this.SetCurrentArmorClass(this.GetArmorClass());
        this.SetRemainingMoveRange(this.GetMoveRange());

        /* Initialize Hero's Statistics */
        this.InitializeHeroStatistics();
      
    }
    /* Calculates the damage amount of the attack. damageAmount = dice + main attribute */
    public override void AttackAmountCalculation() {
            int damageAmount = diceValue + this.GetStrength();
            this.SetIsAttacking(true);
            this.SetCurrentAttackAmount(damageAmount);   
    }

    /*************/
    public override void HeroAttributesToString() {
        base.attributesToString = "Strength: "+this.GetStrength()+"\nArmor Class: "+this.GetArmorClass()+"\nMove Range: "+this.GetMoveRange()+"\n";
    }

    /* function that is called when the hero levels up */
    protected override void LevelUp() {
        base.LevelUp(); // mporei na mhn xreiazetai
        /* if we reach here, the hero has level > 1 AND experience points > 2 */

        /* Case Upgrade To Level 3 */
        /* We level up if the hero has 4 experience points */
        if (this.GetLevel() == 2 && this.GetExperiencePoints() == 4) {
            /// code for level up
            this.SetStrength(this.GetStrength()+3);
            this.SetArmorClass(this.GetArmorClass()+2);
            this.SetCurrentArmorClass(this.GetCurrentArmorClass()+2);
            this.SetLevel(this.GetLevel() + 1);
            Debug.Log("Level Up");
            UI_Manager.Instance.SetGameInfo("Level Up!\nNew Level = " + this.GetLevel());
            OnHeroLevelChanged?.Invoke(this, EventArgs.Empty);
        }
        /* Case Upgrade To Level 4 */
        else if (this.GetLevel() == 3 && this.GetExperiencePoints() == 6) {
            // code for level up
            /* At this level the character is allowed to perform 2 main actions at the same turn */
            this.numOfAllowedActions++;
            this.SetLevel(this.GetLevel() + 1);
            Debug.Log("Level Up");
            UI_Manager.Instance.SetGameInfo("Level Up!\nNew Level = " + this.GetLevel());
            OnHeroLevelChanged?.Invoke(this, EventArgs.Empty);
        }
    }

}


