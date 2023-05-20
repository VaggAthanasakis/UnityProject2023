using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranger : Heroes {

    private int strength;
    public const string HERO_CLASS = "Ranger";

    /* Awake(), Start(), Update() */
    protected override void Awake() {
        base.Awake();
        this.SetNumOfAttributes(3);
        //targetPosition = this.transform.position;
        SetHeroFeatures();
    }

    // Update is called once per frame
    private void Update() {
        if (GameManager.Instance.GetCurrentState() == GameManager.State.FreeRoam || GameManager.Instance.GetCurrentState() == GameManager.State.CombatMode) {
            PerformMove();
        }
        base.AnimationsDurationControll();

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
        this.SetDexterity(sumOfAttributesPoints / 2);
        int remain = sumOfAttributesPoints - this.GetDexterity();
        this.strength = remain / 2;
        this.SetConstitution(sumOfAttributesPoints - strength - this.GetDexterity());

        // setting the features
        this.SetHealthPoints(10 + this.GetConstitution());
        this.SetArmorClass(10 + this.GetDexterity());
        this.SetAttackRange(6);
        this.SetMoveRange(7);
        this.SetCurrentHealthPoints(this.GetHealthPoints());
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

    protected override void LevelUp() {
        base.LevelUp(); // mporei na mhn xreiazetai
        /* if we reach here, the hero has level > 1 AND experience points > 2 */

        /* Case Upgrade To Level 3 */
        /* We level up if the hero has 4 experience points */
        if (this.GetLevel() == 2 && this.GetExperiencePoints() == 4) {
            /// code for level up
            this.SetLevel(this.GetLevel()+1);
        }
        else if (this.GetLevel() == 3 && this.GetExperiencePoints() == 6) {
            // code for level up
            this.SetLevel(this.GetLevel() + 1);
        }

    }





}
