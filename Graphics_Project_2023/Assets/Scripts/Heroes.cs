using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Heroes : MonoBehaviour {

    private int numOfAttributes;
    private int healthPoints;
    private int armorClass;
    private int attackRange;
    private int moveRange;
    private int dexterity;
    private int constitution;

    /**********************/
    [SerializeField] private bool isEnemy;
    [SerializeField] private bool isPlayersTurn;
    [SerializeField] private GameObject isHeroSelected;
    private bool isWalking = false;
    private bool isSelected = false;
    private bool isDead = false;
    private bool isAttacking = false;
    private bool isHealing = false;
    private bool getsHit = false;
    private int currentHealthPoints;
    private int remainingMoveRange;

    protected Vector3 targetPosition;
    protected GridPosition gridPosition;
    public int currentPositionIndex;
    protected int currentAttackAmount;
    protected List<Vector3> positionList = new List<Vector3>();

    /* Useful Variables  */
    private int level;
    private int experiencePoints;
    private int numOfKills;
    private int numOfHeals;
    public string heroClass;
    public int diceValue;

    /* Useful Variables For Animations */
    protected int getHitAnimationsCounter = 0;
    protected int attackingAnimationsCounter = 0;

    /**********************/
    protected virtual void Awake() {
        targetPosition = this.transform.position;
        
    }

    protected virtual void Start() {
        MouseClick.instance.OnHeroSelectAction += Instance_OnHeroSelectAction;
        MouseClick.instance.OnHeroPointingAction += MouseClick_OnHeroPointigAction;
        TurnSystem.Instance.OnTurnChanged += Instance_OnTurnChanged;

        gridPosition = PathFinding.Instance.GetGridPosition(this.transform.position);
        currentPositionIndex = 0;
        //PathFinding.Instance.AddUnitAtGridPosition(gridPosition, this.gameObject.GetComponent<Unit>());
    }


    /****************************** Incoming Events ***********************************/

    /* This event arrives from MouseClick when the user clicks on any hero
    // Inside it, we check if the hero selected is this hero 
    // If yes, we activate the selected visual 
    // if not, but 1) This hero was previously selected
                   2) This hero is !GetIsEnemy() of the selected hero
    // then we leave the selected visual activated
    // else we diactivate it */
    private void Instance_OnHeroSelectAction(object sender, MouseClick.OnHeroSelectActionEventArgs e) {
        if ((Heroes)e.selectedHero == this) {
            this.SetIsSelected(true);
            SelectedHeroVisual();
            Debug.Log("Selected " + this.ToString());
        }
        else if ((Heroes)e.selectedHero != this && this.GetIsSelected() && e.selectedHero.GetIsEnemy() != this.GetIsEnemy()) {
            //this.SetIsSelected(false);
            SelectedHeroVisual();
        }
        else {
            this.SetIsSelected(false);
            SelectedHeroVisual();
        }

    }

    /**************************************************************/
    private void MouseClick_OnHeroPointigAction(object sender, MouseClick.OnHeroPointingActionEventArgs e) {
        //Debug.Log("Pointig On A Mage!");
        if (e.pointedHero != null && e.pointedHero == this) {
            //Debug.Log(e.pointedHero.ToString());
        }
    }

    /**************************************************************/
    private void Instance_OnTurnChanged(object sender, TurnSystem.OnTurnChangedEventArgs e) {

        if (e.heroWithTurn == this) {
            this.SetIsPlayersTurn(true);
            Debug.Log("This Players Turn: "+this.ToString());
        }
        else {
            this.SetIsPlayersTurn(false);

        }
        /* Set the opposite turn for hero and enemy */
        /*if (!this.GetIsEnemy()) {
            this.SetIsPlayersTurn(TurnSystem.Instance.IsHeroTurn());
        }
        else if (this.GetIsEnemy()) {
            this.SetIsPlayersTurn(!TurnSystem.Instance.IsHeroTurn());
        }
        else
            Debug.Log("Neither Hero Or Enemy!");*/
    }
    /**************************************************************/


  

    /* Generate Getters */
    public int GetNumOfAttributes() {
        return this.numOfAttributes; 
    }
    public int GetHealthPoints() {
        return this.healthPoints;
    }
    public int GetArmorClass() {
        return this.armorClass;
    }
    public int GetAttackRange() {
        return this.attackRange;
    }
    public int GetMoveRange() {
        return this.moveRange;
    }
    public int GetDexterity() {
        return this.dexterity;
    }
    public int GetConstitution() {
        return this.constitution;
    }
    public bool GetIsWalking() {
        return this.isWalking;
    }
    public bool GetIsSelected() {
        return this.isSelected;
    }
    public bool GetIsDead() {
        return this.isDead;
    }
    public int GetCurrentHealthPoints() {
        return this.currentHealthPoints;
    }
    public bool GetIsEnemy() {
        return this.isEnemy;
    }
    public bool GetIsHealing() {
        return this.isHealing;
    }
    public bool GetIsAttacking() {
        return this.isAttacking;
    }
    public bool GetGetsHit() {
        return this.getsHit;
    }
    public int GetCurrentAttackAmount() {
        return this.currentAttackAmount;
    }
    public bool GetIsPlayersTurn() {
        return this.isPlayersTurn;
    }
    public int GetNumOfKills() {
        return this.numOfKills;
    }
    public int GetNumOfHeals() {
        return this.numOfHeals;
    }
    public int GetLevel() {
        return this.level;
    }
    public List<Vector3> GetPositionList() {
        return this.positionList;
    }
    public int GetExperiencePoints() {
        return this.experiencePoints;
    }
    public int GetRemainingMoveRange() {
        return this.remainingMoveRange;
    }

    /* Generate Setters */
    public void SetNumOfAttributes(int numOfAttributes) {
        this.numOfAttributes = numOfAttributes;
    }
    public void SetHealthPoints(int points) {
        this.healthPoints = points;
    }
    public void SetArmorClass(int armorClass) {
        this.armorClass = armorClass;
    }
    public void SetAttackRange(int range) {
        this.attackRange = range;
    }
    public void SetMoveRange(int range) {
        this.moveRange = range;
    }
    public void SetDexterity(int dexterity) {
        this.dexterity = dexterity;
    }
    public void SetConstitution(int constitution) {
        this.constitution = constitution;
    }
    public void SetIsWalking(bool cond) {
        this.isWalking = cond;
    }
    public void SetIsSelected(bool cond) {
        this.isSelected = cond;
    }
    public void SetIsDead(bool cond) {
        this.isDead = cond;
    }
    public void SetCurrentHealthPoints(int curr) {
        this.currentHealthPoints = curr;
    }
    public void SetIsEnemy(bool b) {
        this.isEnemy = b;
    }
    public void SetIsHealing(bool b) {
        this.isHealing = b;
    }
    public void SetIsAttacking(bool b) {
        this.isAttacking = b;
    }
    public void SetGetsHit(bool b) {
        this.getsHit = b;
    }
    public void SetCurrentAttackAmount(int amount) {
        this.currentAttackAmount = amount;
    }
    public void SetIsPlayersTurn(bool b) {
        this.isPlayersTurn = b;
    }
    public void SetNumOfKills(int num) {
        this.numOfKills = num;
    }
    public void SetNumOfHeals(int num) {
        this.numOfHeals = num;
    }
    public void SetLevel(int num) {
        this.level = num;
    }

    public void SetExperiencePoints(int value) {
        this.experiencePoints = value;
    }
    public void SetRemainingMoveRange(int value) {
        this.remainingMoveRange = value;
    }

    /* Increase Experience Points By One */
    public void IncreaseExperiencePoints() {
        this.experiencePoints++;
    }

    /******** Hero Firing Events *********/

    /* Event that is fired when hero's health changes and update the health bar */
    public event EventHandler<OnHealthChangedEventArgs> OnHealthChanged;
    public class OnHealthChangedEventArgs : EventArgs {
        public float healthNormalized;
    }

    /* Firing this event when the hero level up */
    public EventHandler OnHeroLevelChanged;
   /* public class OnHeroLevelChangedEventArgs : EventArgs {
        public Heroes heroWithLevelUp;
    }*/

    /* Methods */
    public void TakeDamage(int damageAmount, Heroes otherHero) {
        int newHealth = this.GetCurrentHealthPoints() - damageAmount;
        if (newHealth < 0) {
            newHealth = 0;
        }
        this.SetCurrentHealthPoints(newHealth);
        //Debug.Log("Health Normalized: "+((float)this.GetCurrentHealthPoints() / this.GetHealthPoints()));
        OnHealthChanged?.Invoke(this, new OnHealthChangedEventArgs {
            healthNormalized = (float)(this.GetCurrentHealthPoints()) / this.GetHealthPoints()
        });
       
        if (newHealth <= 0) {
            this.killHero();
            Debug.Log("Kill");
            this.SetIsDead(true);
        }
        else {
            this.SetGetsHit(true);
            this.SetIsDead(false);
        }
        
        //otherHero.SetIdle();
      
    }

    public void killHero() {
        /* Have to make the node in which the character died Walkable */
        Vector3 killPosition = this.transform.position;
        GridPosition killPosionAtGrid = PathFinding.Instance.GetGridPosition(killPosition);
        PathNode killNode = PathFinding.Instance.Grid().GetPathNode(killPosionAtGrid);
        killNode.SetIsWalkable(true);

        this.SetIsDead(true);
        this.SetCurrentHealthPoints(0);
        Destroy(this.gameObject, 5f);
    }

    public void GetHeal(int healAmount) {
        int newHealth = this.GetCurrentHealthPoints() + healAmount;
        if (newHealth > this.GetHealthPoints()) {
            this.SetCurrentHealthPoints(this.GetHealthPoints());
        }
        else {
            this.SetCurrentHealthPoints(newHealth);
        }
        OnHealthChanged?.Invoke(this, new OnHealthChangedEventArgs {
            healthNormalized = (float)(this.GetCurrentHealthPoints()) / this.GetHealthPoints()
        });
        Debug.Log("Heal");
    }


    public void Move(Vector3 target) {
        if (this.GetIsSelected()) {
            this.targetPosition = target;
            //TakeDamage(5);
        }
    }
    protected void PerformMove() {
        float stoppingDistance = 0.05f;
        float rotateSpeed = 10f;
        Vector3 moveDirection;

        /* Cannot Perform Move When Is Dead */
        if (this.isDead) { return; }

        /* If we are at free roam state => we can move
         * If we are at combat mode and it is our turn => we can move 
           else, we cannot */
        if ((isPlayersTurn && GameManager.Instance.GetCurrentState() == GameManager.State.CombatMode) || GameManager.Instance.GetCurrentState() == GameManager.State.FreeRoam) {
            if (positionList.Count <= 0) {
                this.SetIsWalking(false);
                return;
            }
            targetPosition = positionList[currentPositionIndex];
            moveDirection = (targetPosition - transform.position).normalized;
            transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);

            if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance) {
                float moveSpeed = 4f;
                transform.position += moveDirection * moveSpeed * Time.deltaTime;
                this.SetIsWalking(true);              
            }
            else {
                this.SetIsWalking(true);
                currentPositionIndex++;
                if (currentPositionIndex >= positionList.Count) {
                    //ÅÍÄ
                    // at targetPoint
                    //Debug.Log("CurrentPos:" + currentPositionIndex);
                    positionList.Clear();
                    currentPositionIndex = 0;
                }
            }

        }
    }
    /* Set the path that the hero must follow in order to move */
    public void SetPositionsList(List<Vector3> pathGridPositions) {
        /* When we are at combat mode, we check if the position we want to move to can be reached with the hero's moveRange amount
         * If we are at free roam, we dont mind about the moveRange and the hero can move freely */
        if (pathGridPositions.Count - 1 > this.remainingMoveRange && GameManager.Instance.GetCurrentState() == GameManager.State.CombatMode) {
            Debug.Log(pathGridPositions.Count - 1 + " > " + this.GetMoveRange());
            Debug.Log("Hero cannot move that far!");
            return;
        }
        if (GameManager.Instance.GetCurrentState() == GameManager.State.CombatMode) {
            this.remainingMoveRange -= pathGridPositions.Count - 1;
        }
        foreach (Vector3 pathPosition in pathGridPositions)
            positionList.Add(pathPosition);
        
    }

    public void SetIdle() {
        isWalking = false;
        //isSelected = false;
        isDead = false;
        isAttacking = false;
        isHealing = false;
        getsHit = false;
    }

    protected void InitializeHeroStatistics() {
        this.SetNumOfHeals(0);
        this.SetNumOfKills(0);
        this.SetLevel(1);
        this.experiencePoints = 0;
    }

    public String HeroStatisticsToString() {
        return "Hero Class: " + this.heroClass + "\nHero Level: " + this.GetLevel() + "\nHero Kills: " + this.GetNumOfKills() + "\nHero Heals: " + this.GetNumOfHeals();
    }

    public void SelectedHeroVisual() {
        isHeroSelected.SetActive(this.isSelected);
    }

    /* Method That Points The Character To The Interacted Hero */
    private void PointAtTheInteractedHero(Heroes interactedHero) {
        Vector3 direction = interactedHero.transform.position - this.transform.position;
        if (direction != Vector3.zero) {
            this.transform.rotation = Quaternion.LookRotation(direction);
        }

    }

    //public virtual void Interact() { }
    public virtual void AttackAmountCalculation() { }

    public virtual void PerformAttack(Heroes heroToAttack) {
        if (heroToAttack.GetIsEnemy() != this.GetIsEnemy()) {
            if (diceValue == 1) {
                this.SetIsAttacking(false);
                Debug.Log("Unsuccessfull Attack!");
                return;
            }
            AttackAmountCalculation();
            PointAtTheInteractedHero(heroToAttack);
            if (heroToAttack.GetArmorClass() < this.GetCurrentAttackAmount()) {
                heroToAttack.TakeDamage(this.GetCurrentAttackAmount(), this);
                this.IncreaseExperiencePoints();
                if (heroToAttack.GetIsDead()) {
                    // NA KANO WALKABLE TO NODE PANO STO OPOIO PETHANE
                    Debug.Log("Enemy killed!");
                    this.SetNumOfKills(this.GetNumOfKills() + 1);
                    this.IncreaseExperiencePoints();
                }
                FirstLevelUp();
            }
        }

    }
    protected virtual void SetHeroFeatures() { }

    /* This method will be implemented by each hero in order to have custom implementations */
    protected virtual void LevelUp() { }

    /* At the first level up, we increase the health, the dexterity and the constitution of the hero */
    /* It happens if the hero has at least 2 experience points */
    public void FirstLevelUp() {
        if (this.GetLevel() == 1 && this.GetExperiencePoints() <= 2) {
            this.currentHealthPoints += 5;
            this.healthPoints += 5;
            this.dexterity += 2;
            this.constitution += 2;
            this.level++;
            Debug.Log("Level Up");
            this.OnHeroLevelChanged?.Invoke(this, EventArgs.Empty);
        }
        else {
            LevelUp();
            this.OnHeroLevelChanged?.Invoke(this, EventArgs.Empty);
        }
    }


    protected void AnimationsDurationControll() {
        int animationHitDuration = 2;
        int animationsAttackingDuration = 100;

        if (this.GetGetsHit()) {
            this.getHitAnimationsCounter++;
            if (getHitAnimationsCounter >= animationHitDuration) {
                this.SetGetsHit(false);
                this.getHitAnimationsCounter = 0;
            }
        }

        if (this.GetIsAttacking()) {
            this.attackingAnimationsCounter++;
            if (attackingAnimationsCounter >= animationsAttackingDuration) {
                this.SetIsAttacking(false);
                this.attackingAnimationsCounter = 0;
            }
        }
    }



}
