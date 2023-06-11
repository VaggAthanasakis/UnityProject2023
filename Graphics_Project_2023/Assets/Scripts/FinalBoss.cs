using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FinalBoss : Heroes {

    private int strength;
    private int intelligence; // main attribute
    public const string HERO_CLASS = "Final Boss";

    /* Awake(), Start(), Update() */
    protected override void Awake() {
        base.Awake();
        this.SetNumOfAttributes(4);
        //targetPosition = this.transform.position;
        SetHeroFeatures();
        AddAction("Attack");
        //AddAction("CastSpell");

    }

    // Start is called before the first frame update
    protected override void Start() {
        base.Start();
        HeroAttributesToString();
        this.SetIsEnemy(true);
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
        this.strength = sumOfAttributesPoints / 2 ;
        int remain = sumOfAttributesPoints - this.strength;
        this.SetConstitution(remain / 2);
        remain = sumOfAttributesPoints - this.strength - this.GetConstitution();
        this.intelligence = remain / 2;
        this.SetDexterity(remain - this.intelligence - 3);
        this.strength += 1;

        // setting the features
        this.SetHealthPoints(10 + this.GetConstitution());
        this.SetArmorClass(10 + this.GetDexterity());
        this.SetAttackRange(6);
        this.SetMoveRange(5);
        this.SetCurrentHealthPoints(this.GetHealthPoints());
        this.SetCurrentArmorClass(this.GetArmorClass());
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
            this.SetStrength(this.GetStrength()+2);
            this.SetIntelligence(this.GetIntelligence() + 2);
            this.SetAttackRange(this.GetAttackRange() + 2);
            this.SetLevel(this.GetLevel() + 1);
            Debug.Log("Level Up");
            UI_Manager.Instance.SetGameInfo("Level Up!\nNew Level = " + this.GetLevel());
            OnHeroLevelChanged?.Invoke(this, EventArgs.Empty);
            SoundManager.Instance.PlaySoundWithoutFade(SoundManager.LEVEL_UP);
        }
        /* Case Upgrade To Level 4 */
        else if (this.GetLevel() == 3 && this.GetExperiencePoints() == 6) {
            // code for level up
            /* At this level the character is allowed to perform 2 main actions at the same turn */
            this.numOfAllowedActions++;
            this.SetStrength(this.GetStrength() + 2);
            this.SetIntelligence(this.GetIntelligence() + 2);
            this.SetLevel(this.GetLevel() + 1);
            Debug.Log("Level Up");
            UI_Manager.Instance.SetGameInfo("Level Up!\nNew Level = " + this.GetLevel());
            OnHeroLevelChanged?.Invoke(this, EventArgs.Empty);
            SoundManager.Instance.PlaySoundWithoutFade(SoundManager.LEVEL_UP);
        }

    }

    public override void HeroAttributesToString() {
        base.attributesToString = "Intelligence: " + this.GetIntelligence() + "\nArmor Class: " + this.GetArmorClass() + "\nHealth: " + this.GetCurrentHealthPoints() + "\nMove Range: " + this.GetMoveRange() + "\nAction Range: " + this.GetAttackRange() + "\nDice Value: " + this.diceValue;
    }

    /* Calculates the damage amount of the attack. damageAmount = dice + main attribute */
    public override void AttackAmountCalculation() {
        int damageAmount = diceValue + this.strength - 3;
        this.SetIsAttacking(true);
        this.SetCurrentAttackAmount(damageAmount);
    }

}
