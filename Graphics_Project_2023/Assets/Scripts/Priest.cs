using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Priest : Heroes {

    private int charisma;
    public const string HERO_CLASS = "Priest";
 
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
    private void Update() {
        PerformMove();
        
    }
    /* Getters */
    public int GetCharisma() {
        return this.charisma;
    }

    /* Setters */
    public void SetCharisma(int cha) {
        this.charisma = cha;
    }

    /********* Methods **********/
    protected override void SetHeroFeatures() {
        /* * */
        this.heroClass = HERO_CLASS;

        // setting the attributes
        int sumOfAttributesPoints = ((this.GetNumOfAttributes() - 1) * 4 + 2);
        this.charisma = sumOfAttributesPoints / 2;
        int remain = sumOfAttributesPoints - charisma;
        this.SetDexterity(remain / 2);
        this.SetConstitution(sumOfAttributesPoints - charisma - this.GetDexterity());

        // setting the features
        this.SetHealthPoints(10 + this.GetConstitution());
        this.SetArmorClass(10 + this.GetDexterity());
        this.SetAttackRange(2);
        this.SetMoveRange(4);
        this.SetCurrentHealthPoints(this.GetHealthPoints());

        /* Initialize Hero's Statistics */
        this.InitializeHeroStatistics();

    }
    
    

 
  


}
