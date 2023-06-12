using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Musician : Heroes {

    private int intelligence;
    public const string HERO_CLASS = "Musician";

    /* Awake(), Start(), Update() */
    protected override void Awake() {
        base.Awake();
        this.SetNumOfAttributes(3);
        //targetPosition = this.transform.position;
        SetHeroFeatures();
        AddAction("PlayMusic");
    }

    // Start is called before the first frame update
    protected override void Start() {
        base.Start();
        HeroAttributesToString();
    }

    // Update is called once per frame
    private void Update() {
        if (GameManager.Instance.GetCurrentState() == GameManager.State.FreeRoam || GameManager.Instance.GetCurrentState() == GameManager.State.CombatMode) {
            PerformMove();
        }
        StartCoroutine(base.AnimationsDurationControll());
    }

    /* Getters */
    public int GetIntelligence() {
        return this.intelligence;
    }

    /* Setters */
    public void SetCharisma(int intell) {
        this.intelligence = intell;
    }

    /********* Methods **********/
    protected override void SetHeroFeatures() {
        /* * */
        this.heroClass = HERO_CLASS;

        // setting the attributes
        int sumOfAttributesPoints = ((this.GetNumOfAttributes() - 1) * 4 + 2);
        this.intelligence = sumOfAttributesPoints / 2;
        int remain = sumOfAttributesPoints - intelligence;
        this.SetDexterity(remain / 2);
        this.SetConstitution(sumOfAttributesPoints - intelligence - this.GetDexterity());

        // setting the features
        this.SetHealthPoints(10 + this.GetConstitution());
        this.SetArmorClass(10 + this.GetDexterity());
        this.SetAttackRange(2); // dont mind
        this.SetMoveRange(4);
        this.SetCurrentHealthPoints(this.GetHealthPoints());
        this.SetCurrentArmorClass(this.GetArmorClass());
        this.SetRemainingMoveRange(this.GetMoveRange());

        /* Initialize Hero's Statistics */
        this.InitializeHeroStatistics();

    }

    /* function that is called when the hero levels up */
    protected override void LevelUp() {
        base.LevelUp(); 
        /* if we reach here, the hero has level > 1 AND experience points > 2 */

        /* Case Upgrade To Level 3 */
        /* We level up if the hero has 4 experience points */
        if (this.GetLevel() == 2 && this.GetExperiencePoints() == 4) {
            /// code for level up
            this.intelligence += 5;
            this.SetArmorClass(this.GetArmorClass() + 2);
            this.SetCurrentArmorClass(this.GetArmorClass() + 2);
            this.SetLevel(this.GetLevel() + 1);
            SoundManager.Instance.PlaySoundWithoutFade(SoundManager.LEVEL_UP);
        }
        /* Case Upgrade To Level 4 */
        else if (this.GetLevel() == 3 && this.GetExperiencePoints() >= 6) {
            // code for level up
            /* At this level the character is allowed to perform 2 main actions at the same turn */
            this.numOfAllowedActions++;
            this.SetLevel(this.GetLevel() + 1);
            SoundManager.Instance.PlaySoundWithoutFade(SoundManager.LEVEL_UP);
        }
    }

    public override void HeroAttributesToString() {
        base.attributesToString = "Intelligence: " + this.GetIntelligence() + "\nArmor Class: " + this.GetArmorClass() + "\nMove Range: " + this.GetMoveRange() + "\nAction Range: " + this.GetAttackRange() + "\nDice Value: " + this.diceValue;
    }

    /* Calculate the volume of the music */
    public override void SoundVolumeCalculation() {
        int decibel = diceValue + this.intelligence - 2;
        this.SetIsPlayingMusic(true);
        this.SetCurrentAttackAmount(decibel);
    }


}
