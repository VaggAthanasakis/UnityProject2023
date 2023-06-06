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
    [SerializeField] private GameObject isHeroSelected_Enemy;

    private bool isWalking = false;
    private bool isSelected = false;
    private bool isDead = false;
    private bool isAttacking = false;
    private bool isHealing = false;
    private bool getsHit = false;
    private bool isBegging = false;
    private bool isCastSpelling = false;
    private bool isPlayingMusic = false;
    private int currentHealthPoints;
    private int currentArmorClass;
    private int remainingMoveRange;

    protected Vector3 targetPosition;
    protected GridPosition gridPosition;
    public int currentPositionIndex;
    protected int currentAttackAmount;
    protected int currentNegotiateValue;
    protected int currentHealAmount;
    protected List<Vector3> positionList = new List<Vector3>();

    /* Useful Variables  */
    private int level;
    private int experiencePoints;
    private int numOfKills;
    private int numOfHeals;
    private int numOfBegs;
    private bool isPointedByMouse;
    public string heroClass;
    public int diceValue;
    public int performedActions = 0;
    public int numOfAllowedActions = 1;

    public string attributesToString; 

    /* Useful Variables For Animations */
    protected int getHitAnimationsCounter = 0;
    protected int attackingAnimationsCounter = 0;
    protected int healingAnimationCounter = 0;
    protected int beggingAnimationCounter = 0;
    protected int castSpellingAnimationCounter = 0;
    protected int musicPlayingAnimationCounter =0;

    // List with actions
    private List<string> actions = new List<string>();

  
    /**********************/
    protected virtual void Awake() {
        targetPosition = this.transform.position;
        AddAction("Dash");
        //SetWalkableNodeAtHeroPosition(false);
    }

    protected virtual void Start() {
        MouseClick.instance.OnHeroSelectAction += Instance_OnHeroSelectAction;
        MouseClick.instance.OnHeroPointingAction += MouseClick_OnHeroPointigAction;
        TurnSystem.Instance.OnTurnChanged += Instance_OnTurnChanged;

        gridPosition = PathFinding.Instance.GetGridPosition(this.transform.position);
        currentPositionIndex = 0;
        SetWalkableNodeAtHeroPosition(false);
        //TakeDamage(10, null);
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

        if (this.GetIsDead() || this == null) { return; }

        if ((Heroes)e.selectedHero == this && this.GetIsEnemy()) {
            this.SetIsSelected(true);
            SelectedHeroVisual();
            Debug.Log("Selected " + this.ToString());
        }
        else if ((Heroes)e.selectedHero == this && !this.GetIsEnemy() && GameManager.Instance.GetCurrentState() == GameManager.State.FreeRoam) {
            this.SetIsSelected(true);
            SelectedHeroVisual();
            Debug.Log("Selected " + this.ToString());
        }
        else if ((Heroes)e.selectedHero == this && !this.GetIsEnemy() && GameManager.Instance.GetCurrentState() == GameManager.State.CombatMode) {
            //if (isPlayersTurn) {
                this.SetIsSelected(true);
                SelectedHeroVisual();
                Debug.Log("Selected " + this.ToString());
            //}
            /*else {
                this.SetIsSelected(false);
                SelectedHeroVisual();
            }*/
        }

        else if ((Heroes)e.selectedHero != this && this.GetIsSelected() && e.selectedHero.GetIsEnemy() != this.GetIsEnemy()) {
            //this.SetIsSelected(false);
            SelectedHeroVisual();
        }
        else {
            //Debug.Log("Eror Here (OnHeroSelection) for Player: " + this.ToString());
            this.SetIsSelected(false);
            SelectedHeroVisual();
        }

    }

    /**************************************************************/
    private void MouseClick_OnHeroPointigAction(object sender, MouseClick.OnHeroPointingActionEventArgs e) {
        //Debug.Log("Pointig On A Mage!");
        if (e.pointedHero != null && e.pointedHero == this) {
            this.isPointedByMouse = true;

        }
        else {
            this.isPointedByMouse = false;
        }
    }

    /**************************************************************/
    private void Instance_OnTurnChanged(object sender, TurnSystem.OnTurnChangedEventArgs e) {

        if (e.heroWithTurn == this) {
            this.SetIsPlayersTurn(true);
            this.SetIsSelected(true);
            SelectedHeroVisual();
            Debug.Log("This Players Turn: " + this.ToString());
            if (this.isEnemy) {
                EnemyAIAction();
            }
            else {
                MouseClick.instance.SetSelectedHero(this);
            }
        }
        else {
            if (this.GetIsDead() || this == null) { return; }

            //Debug.Log("Eror Here (OnTurnChanged) for Player: "+this.ToString());
            this.SetIsPlayersTurn(false);
            this.SetIsSelected(false);
            SelectedHeroVisual();
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
    public int GetCurrentArmorClass() {
        return this.currentArmorClass;
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
    public int GetCurrentHealAmount() {
        return this.currentHealAmount;
    }
    public bool IsPointedByMouse() {
        return this.isPointedByMouse;
    }
    public bool GetIsBegging() {
        return this.isBegging;
    }
    public int GetCurrentNegotiateValue() { 
        return this.currentNegotiateValue;
    }
    public int GetNumOfBegs() {
        return this.numOfBegs;
    }
    public bool GetIsCastSpelling() {
        return this.isCastSpelling;
    }
    public List<string> GetActionsList() {
        return this.actions;
    }
    public bool GetIsPlayingMusic() { 
        return this.isPlayingMusic;
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
    public void SetCurrentArmorClass(int curr) {
        this.currentArmorClass = curr;
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
    public void SetCurrentHealAmount(int value) {
        this.currentHealAmount = value;
    }
    public void SetIsPointedByMouse(bool b) {
        this.isPointedByMouse = b;
    }
    public void SetIsBegging(bool b) {
        this.isBegging = b; 
    }
    public void SetCurrentNegotiateValue(int value) {
        this.currentNegotiateValue = value;
    }
    public void SetNumOfBegs(int value) {
        this.numOfBegs = value;
    }
    public void SetIsCastSpelling(bool b) {
        this.isCastSpelling = b;
    }
    public void SetIsPlayingMusic(bool b) {
        this.isPlayingMusic = b;
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

    /* Event that is fired when the hero is moving in combat mode to inform the UI step bar */
    public event EventHandler<OnRemainingMoveRangeChangedEventArgs> OnRemainingMoveRangeChanged;
    public class OnRemainingMoveRangeChangedEventArgs : EventArgs {
        public float remainingSteps;
    }

    /* Firing this event when the hero level up */
    public EventHandler OnHeroLevelChanged;

    /* Methods */
    public void TakeDamage(int damageAmount, Heroes otherHero) {
        int newHealth = this.GetCurrentHealthPoints() - damageAmount;
        if (newHealth < 0) {
            newHealth = 0;
        }
        this.SetCurrentHealthPoints(newHealth);
        OnHealthChanged?.Invoke(this, new OnHealthChangedEventArgs {
            healthNormalized = (float)(this.GetCurrentHealthPoints()) / this.GetHealthPoints()
        });
       
        if (newHealth <= 0) {
            this.killHero();
            this.SetIsDead(true);
        }
        else {
            this.SetGetsHit(true);
            this.SetIsDead(false);
        }     
    }

    /* This method is called when hero's currentHealthPoints <= 0 */
    public void killHero() {
        float destroyObjectDelay = 5f;
        /* Have to make the node in which the character died Walkable */
        SetWalkableNodeAtHeroPosition(true);
        this.SetIsDead(true);
        this.SetCurrentHealthPoints(0);
        GameManager.Instance.aliveCharacters.Remove(this); // remove the character for the game

        if (isEnemy) {
            GameManager.Instance.aliveEnemies.Remove(this);
        }
        else {
            GameManager.Instance.aliveHeroes.Remove(this);
        }
        Destroy(this.gameObject, destroyObjectDelay);
    }

    /* This method is called when an oneother hero heals this hero */
    public void GetHeal(int healAmount, Heroes otherHero) {
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
        UI_Manager.Instance.SetGameInfo("Successful Heal");
        Debug.Log("Heal");
    }


    public void Move(Vector3 target) {
        if (this.GetIsSelected()) {
            this.targetPosition = target;
        }
    }
    /* This method is called when the hero wants to move */
    protected void PerformMove() {
        float stoppingDistance = 0.05f;
        float rotateSpeed = 15f;
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
            SetWalkableNodeAtHeroPosition(true);

            if (GameManager.Instance.CheckForCombatMode(targetPosition) && !this.isEnemy) {
                this.positionList.Clear();
                this.SetIsWalking(false);
                currentPositionIndex = 0;
                SetWalkableNodeAtHeroPosition(false);
                return;
            }
            
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
                    positionList.Clear();
                    currentPositionIndex = 0;
                    this.SetIsWalking(false);
                    SetWalkableNodeAtHeroPosition(false);
                }
            }
        }
       
    }

    /* Set the node that the hero is currently at as Walkable or NoWalkable */
    private void SetWalkableNodeAtHeroPosition(bool isWalkable) {
        GridPosition heroGridPos = PathFinding.Instance.GetGridPosition(this.transform.position);
        PathNode heroNode = PathFinding.Instance.Grid().GetPathNode(heroGridPos);
        heroNode.SetIsWalkable(isWalkable);
    }

    /* Set the path that the hero must follow in order to move */
    public void SetPositionsList(List<Vector3> pathGridPositions) {
        /* When we are at combat mode, we check if the position we want to move to can be reached with the hero's moveRange amount
         * If we are at free roam, we dont mind about the moveRange and the hero can move freely */
        if (pathGridPositions.Count - 1 > this.remainingMoveRange && GameManager.Instance.GetCurrentState() == GameManager.State.CombatMode) {
            Debug.Log(pathGridPositions.Count - 1 + " > " + this.remainingMoveRange);
            Debug.Log("Hero cannot move that far!");
            UI_Manager.Instance.SetGameInfo("Hero Cannot Move That Far!");
            return;
        }
        if (GameManager.Instance.GetCurrentState() == GameManager.State.CombatMode) {
            this.remainingMoveRange -= pathGridPositions.Count - 1; // decrease the current move range 
            /* If there are no remaining steps, next turn */
            /*if (this.remain) { 
            
            }*/
            float remainingRange = (float)(this.remainingMoveRange) / this.moveRange;
            /* Fire the event to Inform the UI Step Bar */
            OnRemainingMoveRangeChanged?.Invoke(this, new OnRemainingMoveRangeChangedEventArgs {
                remainingSteps = remainingRange
            });
        }
        foreach (Vector3 pathPosition in pathGridPositions)
            positionList.Add(pathPosition);
       
    }

    /* Initialize hero's statistic values */
    protected void InitializeHeroStatistics() {
        this.SetNumOfHeals(0);
        this.SetNumOfKills(0);
        this.SetLevel(1);
        this.experiencePoints = 0;
    }

    /* Get hero's statistscs to string */
    public String HeroStatisticsToString() {
        return "Hero Class: " + this.heroClass + "\nHero Level: " + this.GetLevel() + "\nHero Kills: " + this.GetNumOfKills() + "\nHero Heals: " + this.GetNumOfHeals();
    }

    /* Get Heroes Attributes To String */
    public virtual void HeroAttributesToString() { }

    /* Activate the selected visual when is hero's turn or when selected */
    public void SelectedHeroVisual() {
        if (this.isDead || this == null) { return; }
        if (!this.isEnemy) {
            isHeroSelected.SetActive(this.isSelected || this.isPlayersTurn);
            isHeroSelected_Enemy.SetActive(false);
        }
        else if (this.isEnemy) {
            isHeroSelected_Enemy.SetActive(this.isSelected || this.isPlayersTurn);
            isHeroSelected.SetActive(false);
        }
    }

    /* Method That Points The Character To The Interacted Hero */
    private void PointAtTheInteractedHero(Heroes interactedHero) {
        Vector3 direction = interactedHero.transform.position - this.transform.position;
        if (direction != Vector3.zero) {
            this.transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    /* Method That Points The Character To The Interacted Object */
    public void PointAtTheInteractedObject(GameObject InteractedObject) {
        Vector3 direction = InteractedObject.transform.position - this.transform.position;
        if (direction != Vector3.zero) {
            this.transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    /* Method that set the hero's attributes */
    protected virtual void SetHeroFeatures() { }

    /* This method will be implemented by each hero in order to have custom implementations */
    protected virtual void LevelUp() { }

    /* At the first level up, we increase the health, the dexterity and the constitution of the hero */
    /* It happens if the hero has at least 2 experience points */
    public void FirstLevelUp() {
        if (this.GetLevel() == 1 && this.GetExperiencePoints() == 2) {
            this.currentHealthPoints += 5;
            this.healthPoints += 5;
            this.dexterity += 2;
            this.constitution += 2;
            this.level++;
            Debug.Log("Level Up");
            UI_Manager.Instance.SetGameInfo("Level Up!\nNew Level = "+this.level);
            this.OnHeroLevelChanged?.Invoke(this, EventArgs.Empty);
        }
        else {
            LevelUp();
            //this.OnHeroLevelChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /* This method controlls the animations of the characters in order to be performed 
     * a spesific amount of times by changing the state of each hero back to idle after an action */
    protected void AnimationsDurationControll() {
        int animationHitDuration = 2;
        int animationsAttackingDuration = 100;
        int animationHealingDuration = 2;
        int animationBeggingDuration = 2;
        int animationCastSpellingDuration = 3;
        int animationMusicPlayingDuration = 3;

        /* if the hero gets hit */
        if (this.GetGetsHit()) {
            this.getHitAnimationsCounter++;
            if (getHitAnimationsCounter >= animationHitDuration) {
                this.SetGetsHit(false);
                this.getHitAnimationsCounter = 0;
            }
        }
        /* If the hero is attacking */
        if (this.GetIsAttacking()) {
            this.attackingAnimationsCounter++;
            if (attackingAnimationsCounter >= animationsAttackingDuration) {
                this.SetIsAttacking(false);
                this.attackingAnimationsCounter = 0;
            }
        }
        /* If the hero is healing */
        if (this.GetIsHealing()) {
            this.healingAnimationCounter++;
            if (healingAnimationCounter >= animationHealingDuration) {
                this.SetIsHealing(false);
                this.healingAnimationCounter = 0;
            }
        }
        /* If the hero is begging */
        if (this.GetIsBegging()) {
            this.beggingAnimationCounter++;
            if (beggingAnimationCounter >= animationBeggingDuration) {
                this.SetIsBegging(false);
                this.beggingAnimationCounter = 0;
            }
        }
        /* if the hero cast a spell */
        if (this.isCastSpelling) {
            this.castSpellingAnimationCounter++;
            if (castSpellingAnimationCounter >= animationCastSpellingDuration) {
                this.SetIsCastSpelling(false);
                this.castSpellingAnimationCounter = 0;
            }

        }
        /* if the hero is musician */
        if (this.isPlayingMusic) {
            this.musicPlayingAnimationCounter++;
            if (musicPlayingAnimationCounter >= animationMusicPlayingDuration) {
                this.SetIsPlayingMusic(false);
                this.musicPlayingAnimationCounter = 0;
            }
        }

    }

    /*****  HERO ACTIONS ****/

    /********** ATTACK ACTION ******/

    /*** Add action to hero ***/
    public void AddAction(string action) {
        this.actions.Add(action);
    }

    /* Calculate the attack damage amount of the attack action */
    public virtual void AttackAmountCalculation() { }

    /* Permorms the attack action */
    public virtual void PerformAttack(Heroes heroToAttack) {
        /* If we are not in combat mode, return */
        if (GameManager.Instance.GetCurrentState() != GameManager.State.CombatMode) return;

        if (heroToAttack.GetIsEnemy() != this.GetIsEnemy()) {
            this.performedActions++;                                // increase the number of permoemed actionsof the hero
            if (this.performedActions > this.numOfAllowedActions) { // hero can permorm numOfAllowedActions action at every turn
                this.SetIsAttacking(false);
                Debug.Log("Max Allowed Actions Performed. Next Turn!");
                UI_Manager.Instance.SetGameInfo("Max Allowed Actions Performed. Next Turn!");
                return;
            }
            if (diceValue == 1) {                                  // if dicevalue == 1 -> lose turn
                this.SetIsAttacking(false);
                Debug.Log("Dice Value = 1. Lost Turn!");
                UI_Manager.Instance.SetGameInfo("Dice Value = 1. Lost Turn!");
                return;
            }
            /* if the other hero is out of range */
            int distance_normalized = PathFinding.Instance.CalculateSimpleDistance(this.transform.position,heroToAttack.transform.position);
            Debug.Log("DISTANCE "+distance_normalized);
            if (distance_normalized > this.attackRange) {
                Debug.Log("Other Hero Out Of Range");
                UI_Manager.Instance.SetGameInfo("Other Hero Out Of Range!");
                this.SetIsAttacking(false);
                return;
            }

            AttackAmountCalculation(); // calculate the attack amount
            PointAtTheInteractedHero(heroToAttack);
            if (heroToAttack.GetCurrentArmorClass() < this.GetCurrentAttackAmount()) {
                Debug.Log("Successfull Attack");
                UI_Manager.Instance.SetGameInfo("Successfull Attack!");
                heroToAttack.TakeDamage(this.GetCurrentAttackAmount(), this);
                this.IncreaseExperiencePoints();
                if (heroToAttack.GetIsDead()) {
                    // NA KANO WALKABLE TO NODE PANO STO OPOIO PETHANE
                    Debug.Log("Enemy killed!");
                    this.SetNumOfKills(this.GetNumOfKills() + 1);
                    this.IncreaseExperiencePoints();
                }
                FirstLevelUp(); // check if the hero can level up
            }
            else {
                Debug.Log("Unsuccessful Due To Armor > DamageAmount");
                UI_Manager.Instance.SetGameInfo("Unsuccessfull Attack Due To Hero Armor");
            }
        }
    }

    /****** HEAL ACTION *******/
    /* Calculate the heal amount of the heal action */
    public virtual void HealAmountCalculation() { }

    /* This Function is called when this hero wants to heal another hero */
    public void PerformHeal(Heroes heroToHeal) {
        if (GameManager.Instance.GetCurrentState() != GameManager.State.CombatMode) return;
        if (heroToHeal.GetIsEnemy() == this.GetIsEnemy()) {          // we want to have the same GetIsEnemy()
            this.performedActions++;                                 // increase the number of permoemed actionsof the hero
            if (this.performedActions > this.numOfAllowedActions) {  // hero can permorm numOfAllowedActions action at every turn
                this.SetIsHealing(false);
                Debug.Log("Max Allowed Actions Performed. Next Turn!");
                UI_Manager.Instance.SetGameInfo("Max Allowed Actions Performed. Next Turn!");
                return;
            }
            if (diceValue == 1) {                                  // if dicevalue == 1 -> lose turn
                this.SetIsHealing(false);
                Debug.Log("Dice Value = 1. Lost Turn!");
                UI_Manager.Instance.SetGameInfo("Dice Value = 1. Lost Turn!");
                return;
            }
            /* if the other hero is out of range */
            int distance = PathFinding.Instance.CalculateSimpleDistance(this.transform.position, heroToHeal.transform.position);
            int distance_normalized = distance; // because distance is 10x in comparison to attack range
            if (distance_normalized > this.attackRange) {
                Debug.Log("Other Hero Out Of Range");
                UI_Manager.Instance.SetGameInfo("Other Hero Out Of Range!");
                this.SetIsAttacking(false);
                return;
            }

            HealAmountCalculation();
            PointAtTheInteractedHero(heroToHeal);
            heroToHeal.GetHeal(this.GetCurrentHealAmount(), this);
            /* if hero has max health, do not get xp points */
            if (heroToHeal.GetCurrentHealthPoints() == heroToHeal.healthPoints) {
               // WaitTimeBuffer(300);
                return;
            }
            this.IncreaseExperiencePoints();
            FirstLevelUp();
        }
    }

    /********** OBJECT INTERACT ACTION **********/
    /* This function is called when the character interacts with interactable objects */
    public bool ObjectInteract() {
        InteractableObject objectToInteract = MouseClick.instance.GetSelectedInteractableObject();
        if (objectToInteract != null) {
            // write code to happen after the open button pushed
            Debug.Log("Pushed to interact!");
           
            /* Check What Interactable Object We Have */
            if (objectToInteract.GetObjectType() == InteractableObject.Type.Chest) {
                return objectToInteract.ChestOpen(this);
            }
            else if (objectToInteract.GetObjectType() == InteractableObject.Type.Door) {
                return false;
            }
            return false;
        }
        return false;
    }

    /********** CAST SPELL ************/
    /* This function is called when the hero (Mage or Priest) want to cast a spell
     * The Spell will randomly either heal all the heroes or damage all the enemies */
    public void CastSpell() {
        if (GameManager.Instance.GetCurrentState() != GameManager.State.CombatMode) return;
        if (this.heroClass != Priest.HERO_CLASS && this.heroClass != Mage.HERO_CLASS) { return; }
        this.performedActions++;                                     // increase the number of permoemed actionsof the hero
        if (this.performedActions > this.numOfAllowedActions) {      // hero can permorm numOfAllowedActions action at every turn
            Debug.Log("Max Allowed Actions Performed. Next Turn!");
            UI_Manager.Instance.SetGameInfo("Max Allowed Actions Performed. Next Turn!");
            return;
        }
        if (diceValue == 1) {                                  // if dicevalue == 1 -> lose turn
            Debug.Log("Dice Value = 1. Lost Turn!");
            UI_Manager.Instance.SetGameInfo("Dice Value = 1. Lost Turn!");
            return;
        }        
        int randNumber = UnityEngine.Random.Range(1,11);
        /* Heal All Heroes */
        /* Because we will permorm heal for every hero, numOfAllowedActions will be increased by PerformHeal each time -> will perform only one (none because of the above increasement) */
        /* so temporarly, we increase the numOfAllowedActions and decrease it again later */
        int initial_numOfAllowedActions = this.numOfAllowedActions;
        if (randNumber <= 3) {
            if (!this.GetIsEnemy()) {
                this.numOfAllowedActions = GameManager.Instance.aliveHeroes.Count + 1;
                foreach (Heroes hero in GameManager.Instance.aliveHeroes) {
                    this.PerformHeal(hero);
                }
            }
            else if (this.GetIsEnemy()) {
                this.numOfAllowedActions = GameManager.Instance.aliveEnemies.Count + 1;
                foreach (Heroes hero in GameManager.Instance.aliveEnemies) {
                    this.PerformHeal(hero);
                }
            }
        }
        /* Attack all enemies */
        else if (randNumber > 3) {
            if (!this.GetIsEnemy()) {
                this.numOfAllowedActions = GameManager.Instance.aliveEnemies.Count + 1;
                foreach (Heroes hero in GameManager.Instance.aliveEnemies) {
                    this.PerformAttack(hero);
                }
            }
            else if (this.GetIsEnemy()) {
                this.numOfAllowedActions = GameManager.Instance.aliveHeroes.Count + 1;
                foreach (Heroes hero in GameManager.Instance.aliveHeroes) {
                    this.PerformAttack(hero);
                }
            }
        }
        this.numOfAllowedActions = initial_numOfAllowedActions;   
    }

    /******** ACTION BEG ********/
    public virtual void NegotiateAmount() { }

    /* This function is called when the hero want to beg an enemy to come by his side */
    public void Beg(Heroes enemyHero) {
        int begOffset = 4;
        /* If the game is not in combat mode, then return */
        /* If this hero is not a priest, then return since only he can use this action  */
        if (GameManager.Instance.GetCurrentState() != GameManager.State.CombatMode) { return; }
        if (this.heroClass != Priest.HERO_CLASS || this.GetIsEnemy() == enemyHero.GetIsEnemy()) { return; }
        this.performedActions++; // increase the number of permoemed actionsof the hero
        if (this.performedActions > this.numOfAllowedActions) { // hero can permorm numOfAllowedActions action at every turn
            this.SetIsBegging(false);
            Debug.Log("Max Allowed Actions Performed. Next Turn!");
            UI_Manager.Instance.SetGameInfo("Max Allowed Actions Performed. Next Turn!");
            return;
        }
        if (diceValue == 1) {                                  // if dicevalue == 1 -> lose turn
            this.SetIsBegging(false);
            Debug.Log("Dice Value = 1. Lost Turn!");
            UI_Manager.Instance.SetGameInfo("Dice Value = 1. Lost Turn!");
            return;
        }
        /* if the other hero is out of range */
        int distance_normalized = PathFinding.Instance.CalculateSimpleDistance(this.transform.position, enemyHero.transform.position);
        if (distance_normalized > this.attackRange) {
            Debug.Log("Other Hero Out Of Range");
            UI_Manager.Instance.SetGameInfo("Other Hero Out Of Range");
            this.SetIsAttacking(false);

            return;
        }

        NegotiateAmount();
        PointAtTheInteractedHero(enemyHero);
        if (enemyHero.GetCurrentArmorClass() < this.GetCurrentNegotiateValue() + begOffset) {
            Debug.Log("Successful Beg");
            UI_Manager.Instance.SetGameInfo("Successfull Beg!");

            if (enemyHero.GetIsEnemy()) {
                enemyHero.SetIsEnemy(!enemyHero.GetIsEnemy());
                enemyHero.SelectedHeroVisual();
                GameManager.Instance.aliveEnemies.Remove(enemyHero);
                GameManager.Instance.aliveHeroes.Add(enemyHero);
            }
            else {
                enemyHero.SetIsEnemy(!enemyHero.GetIsEnemy());
                enemyHero.SelectedHeroVisual();
                GameManager.Instance.aliveHeroes.Remove(enemyHero);
                GameManager.Instance.aliveEnemies.Add(enemyHero);
            }
            this.SetNumOfBegs(this.GetNumOfBegs() + 1);
            this.IncreaseExperiencePoints();
            FirstLevelUp();
        }
        else {
            this.isBegging = false;
            Debug.Log("Unsuccessful Beg");
            UI_Manager.Instance.SetGameInfo("Unsuccessfull Beg!");
        }
    }

    /*********** DASH ACTION ***************/
    /* This function allows the hero to move twice the distance he normally could in a round */
    public void Dash() {
        if (GameManager.Instance.GetCurrentState() != GameManager.State.CombatMode) return;
        this.performedActions++; // increase the number of permoemed actions of the hero
        if (this.performedActions > this.numOfAllowedActions) { // hero can permorm numOfAllowedActions action at every turn
            Debug.Log("Max Allowed Actions Performed. Next Turn!");
            UI_Manager.Instance.SetGameInfo("Max Allowed Actions Performed. Next Turn!");
            return;
        }
        if (diceValue == 1) {                                  // if dicevalue == 1 -> lose turn
            Debug.Log("Unsuccessfull Dash!");
            UI_Manager.Instance.SetGameInfo("Unsuccessfull Dash!");
            return;
        }
        this.remainingMoveRange = 2 * this.moveRange;
        OnRemainingMoveRangeChanged?.Invoke(this, new OnRemainingMoveRangeChangedEventArgs {
            remainingSteps = this.remainingMoveRange
        });
    }

    /********************* PLAY MUSIC ***********************/
    /* Musician playes music in order to try his allies not to be able to take damage at all at this round */
    /* This is possible by increazing the armor class of the ally */
    /* If he achieves to play loud music, the action is successful */

    public virtual void SoundVolumeCalculation() { } // Calculate the volume of the music
    public void PlayMusic() {
        if (GameManager.Instance.GetCurrentState() != GameManager.State.CombatMode) return;
        this.performedActions++; // increase the number of permoemed actions of the hero
        if (this.performedActions > this.numOfAllowedActions) { // hero can permorm numOfAllowedActions action at every turn
            Debug.Log("Max Allowed Actions Performed. Next Turn!");
            UI_Manager.Instance.SetGameInfo("Max Allowed Actions Performed. Next Turn!");
            return;
        }
        if (diceValue == 1) {                                  // if dicevalue == 1 -> lose turn
            Debug.Log("Unsuccessfull Dash!");
            UI_Manager.Instance.SetGameInfo("Unsuccessfull Dash!");
            return;
        }

        SoundVolumeCalculation(); // store the volume at the attack amount in order not to declare too many variables

        int actionSuccessfulOffset = 20;
        if (this.GetCurrentAttackAmount() > actionSuccessfulOffset) {
            // then we can perform the action
            if (!this.GetIsEnemy()) { // if we have a hero
                List<Heroes> heroesList = GameManager.Instance.aliveHeroes;
                foreach (Heroes hero in heroesList) {
                    hero.currentArmorClass = 2*hero.armorClass; 
                }
            }
            else {                   // else if we have an enemy
                List<Heroes> enemiesList = GameManager.Instance.aliveEnemies;
                foreach (Heroes hero in enemiesList) {
                    hero.currentArmorClass = 2 * hero.armorClass;
                }
            }
            UI_Manager.Instance.SetGameInfo("Successful Allies Protection!");
            this.IncreaseExperiencePoints();
            FirstLevelUp();
        }
        // Unsuccessful Action
        else {
            Debug.Log("Unsuccessful Action. Music Volume Too Low!");
            UI_Manager.Instance.SetGameInfo("Unsuccessful Action. Music Volume Too Low!");
        }
    }



    /*********** Enemy AI Behaviour ************/
    public void EnemyAIAction() {
        Debug.Log("Inside AI");
        /* Check if the enemy has permormed all the allowed action on the round */
        if (this.performedActions >= this.numOfAllowedActions) {
            return;
        }
        
        /* If this hero is an enemy and has turn and we are at combat mode */
        if (this.isEnemy && this.isPlayersTurn && GameManager.Instance.GetCurrentState() == GameManager.State.CombatMode) {
            Heroes closerEnemy = null, closerHero = null;
            int closerEnemyDistance = int.MaxValue, closerHeroDistance = int.MaxValue;
            /* Calculate distance to every hero on the board close to this.hero */
            Dictionary<Heroes, int> hashMap_distanceToEnemies = new Dictionary<Heroes, int>();
            Dictionary<Heroes, int> hashMap_distanceToHeroes = new Dictionary<Heroes, int>();
            
            /* Calculate the distance to every enemy */
            List<Heroes> aliveEnemies = GameManager.Instance.aliveEnemies;
            foreach (Heroes enemy in aliveEnemies) {
                int distance;
                distance = PathFinding.Instance.CalculateSimpleDistance(this.transform.position,enemy.transform.position);
                //Debug.Log("DistanceToEnemy"+ distance/10);
                hashMap_distanceToEnemies.Add(enemy,distance);
            }

            /* Find the enemy that is closer to this enemy */
            foreach (KeyValuePair<Heroes, int> pair in hashMap_distanceToEnemies) {
                if (pair.Value < closerEnemyDistance) {
                    closerEnemy = pair.Key;
                    closerEnemyDistance = pair.Value;
                }
            }
            /* Calculate the distance to every hero */
            List<Heroes> aliveHeroes = GameManager.Instance.aliveHeroes;
            foreach (Heroes hero in aliveHeroes) {
                int distance;
                distance = PathFinding.Instance.CalculateSimpleDistance(this.transform.position, hero.transform.position);
                //Debug.Log("DistanceToHero" + distance/10);
                hashMap_distanceToHeroes.Add(hero, distance);
            }

            /* Find the enemy that is closer to this enemy */
            foreach (KeyValuePair<Heroes, int> pair in hashMap_distanceToHeroes) {
                if (pair.Value < closerHeroDistance) {
                    closerHero = pair.Key;
                    closerHeroDistance = pair.Value;
                }
            }
            //Debug.Log("Move Enemy "+this+" to hero "+closerHero);
            // Now we know the closer hero and the closer enemy

            /* if we are at range of actions -> permorm action, else move towards target */
            // First check if the this enemy is a FIGHTER or a RANGER and if the distance is < attack range
            if ((this.heroClass.Equals("Fighter") || this.heroClass.Equals("Ranger"))) { 
                if (closerHeroDistance <= this.attackRange) {
                    Debug.Log("Enemy AI Attacking!");
                    // now attack the hero who is closer
                    this.PerformAttack(closerHero);
                    this.EnemyAIAction();
                    StartCoroutine(TurnSystem.Instance.NextTurn());   // na mpei elegxos an exei kai allo move
                }
                // else if we are not near an enemy, move towards him and try attack him again
                else if (closerHeroDistance < 2 * this.attackRange) {
                    Debug.Log("Old Distance " + closerHeroDistance);
                    closerHeroDistance = MoveEnemyAI(closerHero);
                    //Debug.Log("Move Enemy " + this + " to hero " + closerHero);
                    // move closer to the enemy and try attack him again
                    if (closerHeroDistance <= this.attackRange) {
                        this.PerformAttack(closerHero);
                        this.EnemyAIAction();
                        StartCoroutine(TurnSystem.Instance.NextTurn()); // na mpei elegxos an exei kai allo move
                    }
                    else {  // else, we are far far away, move nowards the enemy, then dash and then move again
                        MoveEnemyAI(closerHero);
                        this.Dash();
                        MoveEnemyAI(closerHero);
                        this.EnemyAIAction();
                        Debug.Log(this + " Dash");
                        StartCoroutine(TurnSystem.Instance.NextTurn());
                    }
                }
                // else dash
                else {
                    MoveEnemyAI(closerHero);
                    this.Dash();
                    MoveEnemyAI(closerHero);
                    this.EnemyAIAction();
                    Debug.Log(this + " Dash");
                    StartCoroutine(TurnSystem.Instance.NextTurn());
                }

            }
            /********************************************** If this hero is a MAGE ************************************/
            else if (this.heroClass.Equals("Mage")) {
                // if an ally, is close and needs healing, -> heal
                if (closerEnemyDistance <= this.attackRange && closerEnemy.GetCurrentHealAmount() < closerEnemy.GetHealthPoints()) {
                    this.PerformHeal(closerEnemy);
                    Debug.Log(this + " Heal");
                    this.EnemyAIAction();
                    StartCoroutine(TurnSystem.Instance.NextTurn()); // na mpei elegxos an exei kai allo move
                }
                else {
                    // else either cast a spell or do nothing 
                    int k = UnityEngine.Random.Range(1, 11);
                    if (k < 6) {
                        MoveEnemyAI(closerHero); // move towards the enemya and cast spell
                        this.CastSpell();
                        Debug.Log(this + " CastSpell");
                        this.EnemyAIAction();
                        StartCoroutine(TurnSystem.Instance.NextTurn());
                    }
                    else {
                        MoveEnemyAI(closerHero);
                        this.Dash();
                        MoveEnemyAI(closerHero);
                        Debug.Log(this + " Dash");
                        this.EnemyAIAction();
                        StartCoroutine(TurnSystem.Instance.NextTurn());
                    }
                }
            }
            /******************************************* If this hero is a PRIEST ***********************************/
            else if (this.heroClass.Equals("Priest")) {
                int k = UnityEngine.Random.Range(1, 11);
                // if there is an hero near, try beg him
                if (closerHeroDistance <= this.attackRange) {
                    this.Beg(closerHero);
                    Debug.Log(this + " Beg");
                    this.EnemyAIAction();
                    StartCoroutine(TurnSystem.Instance.NextTurn());
                }
                // else, heal 
                else if (closerEnemyDistance <= this.attackRange && closerEnemy.GetCurrentHealthPoints() < closerEnemy.GetHealthPoints()) {
                    this.PerformHeal(closerEnemy);
                    Debug.Log(this + " Heal");
                    this.EnemyAIAction();
                    StartCoroutine(TurnSystem.Instance.NextTurn());
                }
                else if (closerHeroDistance > this.attackRange) {
                    Debug.Log("OLD " + closerHeroDistance);
                    closerHeroDistance = MoveEnemyAI(closerHero);
                    Debug.Log("NEW " + closerHeroDistance);
                    if (closerHeroDistance <= this.attackRange) {
                        this.Beg(closerHero);
                        Debug.Log(this + " Beg");
                        this.EnemyAIAction();
                        StartCoroutine(TurnSystem.Instance.NextTurn());
                    }
                    else { // else dash
                        MoveEnemyAI(closerHero);
                        this.Dash();
                        this.EnemyAIAction();
                        Debug.Log(this + " Dash");
                        StartCoroutine(TurnSystem.Instance.NextTurn());
                    }
                }
                // else spell cast or do nothing (Dash)
                else if (k < 6) {
                    this.CastSpell();
                    Debug.Log(this + " Cast spell");
                    this.EnemyAIAction();
                    StartCoroutine(TurnSystem.Instance.NextTurn());
                }
            }
            /******************************************* If this hero is a MUSICIAN ***********************************/
            else if (this.heroClass.Equals("Musician")) {
                int offset = 8;
                /* If the musician is far away from the closer ally, approach him */
                if (closerEnemyDistance < offset) {
                    this.PlayMusic();          // play music
                    Debug.Log(this + " Play Music");
                    this.EnemyAIAction();
                    StartCoroutine(TurnSystem.Instance.NextTurn());
                }
                else {
                    MoveEnemyAI(closerEnemy);  // move him to the closer ally for protection
                    this.Dash();               // or dash
                    Debug.Log(this + " Dash");
                    this.EnemyAIAction();
                    StartCoroutine(TurnSystem.Instance.NextTurn());
                }

            }
        }
        /* else return */
        else {
            Debug.Log("Either not enemy or not hero's turn or not combat mode!");
        }
    }

    /* function that is called to move the enemy AI characters */
    /* Returns the new distance to the heroToFollow */
    private int MoveEnemyAI(Heroes heroToFollow) {
        /* at first, we find the heroToFollow pathnode that is in front of him */
        Vector3 heroPos = heroToFollow.transform.position;
        
        Vector3 tmp = new Vector3(heroPos.x,heroPos.y,heroPos.z + 1);
        GridPosition heroFront =  PathFinding.Instance.GetGridPosition(tmp);
        Debug.Log("HeroFront "+heroFront);

        // check if the hero front is inside the grid

        PathFinding.Instance.FindPathForEnemyAI(heroFront);
        int i = 1;
        while (!PathFinding.Instance.FindPathForEnemyAI(heroFront) && i<=3) {
            Vector3 tmp1 = new Vector3(heroPos.x +i , heroPos.y, heroPos.z + i);
            tmp = tmp1;
            heroFront = PathFinding.Instance.GetGridPosition(tmp);
            i++;
        }

        int newDistance = PathFinding.Instance.CalculateSimpleDistance(this.transform.position, heroToFollow.transform.position);
        //Debug.Log("NewDistance "+newDistance);
        return newDistance;
    }

 

}
