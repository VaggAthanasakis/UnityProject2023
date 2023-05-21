using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Fighter : Heroes {
  
    // main attribute
    private int strength;
    public const string HERO_CLASS = "Fighter";

    /*   */

    /* Awake(), Start(), Update() */
    protected override void Awake() {
        base.Awake();
        this.SetNumOfAttributes(3);
        //targetPosition = this.transform.position;
        SetHeroFeatures();
    }

    // Start is called before the first frame update
    protected override void Start() {
        base.Start();

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
    


    public  void Interact() {
        //if (this.GetIsSelected() && this.GetIsPlayersTurn()) {
        if (this.GetIsPlayersTurn()) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            /* Check if we want to interact with an other hero */
            /* if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue)) {
                 Heroes otherHero = hit.collider.GetComponent<Heroes>();*/

            Heroes otherHero = null;
            while (otherHero != null && otherHero.GetIsEnemy() != this.GetIsEnemy() && otherHero != this) {
                otherHero = MouseClick.instance.GetSelectedHero();
            }
            Debug.Log(otherHero.ToString());
                //otherHero.statistics();
            /* Attack */
                if (otherHero != null && otherHero != this && otherHero.GetIsEnemy() != this.GetIsEnemy()) {
                    this.AttackAmountCalculation();
                   // Debug.Log("Crr Attack amount: "+this.GetCurrentAttackAmount());
                    //Debug.Log("Crr Attack amount: " + this.GetCurrentAttackAmount());

                    if (otherHero.GetArmorClass() < this.GetCurrentAttackAmount()) {
                        otherHero.TakeDamage(this.GetCurrentAttackAmount(),this);
                        //if (otherHero.GetIsDead() || otherHero.GetCurrentHealthPoints() <= 0) {
                        if (otherHero.GetIsDead()) {
                            Debug.Log("Enemy killed!");
                            this.SetNumOfKills(this.GetNumOfKills() + 1);
                            
                        }
                    }
                    
                    
                }
                

            }
        }
      
    }


//}
