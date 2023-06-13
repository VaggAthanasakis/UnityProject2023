using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Summoner : Heroes {

    private int charisma;
    public const string HERO_CLASS = "Summoner";

    /* Awake(), Start(), Update() */
    protected override void Awake() {
        base.Awake();
        this.SetNumOfAttributes(3);
        SetHeroFeatures();
        AddAction("CallForHelp");
    }

    // Start is called before the first frame update
    protected override void Start() {
        base.Start();
        HeroAttributesToString();
        SetWalkableNodeAtHeroPosition(false);
    }

    // Update is called once per frame
    private void Update() {
        if (GameManager.Instance.GetCurrentState() == GameManager.State.FreeRoam || GameManager.Instance.GetCurrentState() == GameManager.State.CombatMode) {
            PerformMove();
        }
        StartCoroutine(base.AnimationsDurationControll());
    }
    /* Getters */
    public int GetCharisma() {
        return this.charisma;
    }

    /* Setters */
    public void SetCharisma(int cha) {
        this.charisma = cha;
    }

    protected override void SetHeroFeatures() {
        /* * */
        this.heroClass = HERO_CLASS;

        // setting the attributes
        int sumOfAttributesPoints = ((this.GetNumOfAttributes() - 1) * 4 + 2);
        this.charisma = sumOfAttributesPoints / 2;
        int remain = sumOfAttributesPoints - charisma;
        this.SetDexterity(remain / 2 - 2);
        this.SetConstitution(sumOfAttributesPoints - charisma - this.GetDexterity());

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

    /* function that is called when the hero levels up */
    protected override void LevelUp() {
        base.LevelUp(); // mporei na mhn xreiazetai
        /* if we reach here, the hero has level > 1 AND experience points > 2 */

        /* Case Upgrade To Level 3 */
        /* We level up if the hero has 4 experience points */
        if (this.GetLevel() == 2 && this.GetExperiencePoints() >= 4) {
            /// code for level up
            this.SetCharisma(this.GetCharisma() + 3);
            this.SetLevel(this.GetLevel() + 1);
            Debug.Log("Level Up");
            UI_Manager.Instance.SetGameInfo("Level Up!\nNew Level = " + this.GetLevel());
            OnHeroLevelChanged?.Invoke(this, EventArgs.Empty);
            SoundManager.Instance.PlaySoundWithoutFade(SoundManager.LEVEL_UP);
        }
        /* Case Upgrade To Level 4 */
        else if (this.GetLevel() == 3 && this.GetExperiencePoints() >= 6) {
            // code for level up
            /* At this level the character is allowed to perform 2 main actions at the same turn */
            this.numOfAllowedActions++;
            this.SetLevel(this.GetLevel() + 1);
            Debug.Log("Level Up");
            UI_Manager.Instance.SetGameInfo("Level Up!\nNew Level = " + this.GetLevel());
            OnHeroLevelChanged?.Invoke(this, EventArgs.Empty);
            SoundManager.Instance.PlaySoundWithoutFade(SoundManager.LEVEL_UP);
        }
        else {
            Debug.Log("Error At Level Up. XP = " + this.GetExperiencePoints() + " level = " + this.GetLevel());

        }

    }

    public override void HeroAttributesToString() {
        base.attributesToString = "Charisma: " + this.GetCharisma() + "\nArmor Class: " + this.GetArmorClass() +"\nHealth: "+this.GetCurrentHealthPoints() +"\nMove Range: " + this.GetMoveRange() + "\nAction Range: " + this.GetAttackRange() + "\nDice Value: " + this.diceValue;
    }

    public override void HelpPowerAmountCalculation() {
        int power = diceValue + this.charisma - 3;
        this.SetIsAttacking(true);
        this.SetCurrentAttackAmount(power);
    }


}
