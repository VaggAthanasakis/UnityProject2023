using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.Generic;

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
    private bool isBegging = false;
    private bool isCastSpelling = false;
    private int currentHealthPoints;
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

    // List with actions
    private List<string> actions = new List<string>();
  
    /**********************/
    protected virtual void Awake() {
        targetPosition = this.transform.position;
        AddAction("Dash");
    }

    protected virtual void Start() {
        MouseClick.instance.OnHeroSelectAction += Instance_OnHeroSelectAction;
        MouseClick.instance.OnHeroPointingAction += MouseClick_OnHeroPointigAction;
        TurnSystem.Instance.OnTurnChanged += Instance_OnTurnChanged;

        gridPosition = PathFinding.Instance.GetGridPosition(this.transform.position);
        currentPositionIndex = 0;
        setNoWalkableNodeAtHeroPosition();
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
     
    }
    /* This method is called when hero's currentHelathPoints <=0 */
    public void killHero() {
        float destroyObjectDelay = 5f;
        /* Have to make the node in which the character died Walkable */
        Vector3 killPosition = this.transform.position;
        GridPosition killPosionAtGrid = PathFinding.Instance.GetGridPosition(killPosition);
        PathNode killNode = PathFinding.Instance.Grid().GetPathNode(killPosionAtGrid);
        killNode.SetIsWalkable(true);

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

            if (GameManager.Instance.CheckForCombatMode(targetPosition) && !this.isEnemy) {
                this.positionList.Clear();
                this.SetIsWalking(false);
                currentPositionIndex = 0;
                setNoWalkableNodeAtHeroPosition();
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
                    setNoWalkableNodeAtHeroPosition();
                }
            }
        }
       

    }

    /* Set the node that the hero is currently at as NoWalkable */
    private void setNoWalkableNodeAtHeroPosition() {
        GridPosition heroGridPos = PathFinding.Instance.GetGridPosition(this.transform.position);
        PathNode heroNode = PathFinding.Instance.Grid().GetPathNode(heroGridPos);
        heroNode.SetIsWalkable(false);
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

    public void SetIdle() {
        isWalking = false;
        isDead = false;
        isAttacking = false;
        isHealing = false;
        getsHit = false;
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
        isHeroSelected.SetActive(this.isSelected || this.isPlayersTurn);
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
        if (this.GetLevel() == 1 && this.GetExperiencePoints() <= 2) {
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
            this.OnHeroLevelChanged?.Invoke(this, EventArgs.Empty);
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
            this.performedActions++; // increase the number of permoemed actionsof the hero
            if (diceValue == 1 || this.performedActions > this.numOfAllowedActions) { // hero can permorm numOfAllowedActions action at every turn
                this.SetIsAttacking(false);
                Debug.Log("Unsuccessfull Attack!");
                UI_Manager.Instance.SetGameInfo("Unsuccessfull Attack!");
                return;
            }
            /* if the other hero is out of range */
            int distance = PathFinding.Instance.CalculateSimpleDistance(this.transform.position,heroToAttack.transform.position);
            int distance_normalized = distance / 10; // because distance is 10x in comparison to attack range
            Debug.Log("DISTANCE "+distance_normalized);
            if (distance_normalized > this.attackRange) {
                Debug.Log("Other Hero Out Of Range");
                UI_Manager.Instance.SetGameInfo("Other Hero Out Of Range!");
                this.SetIsAttacking(false);
                return;
            }

            AttackAmountCalculation(); // calculate the attack amount
            PointAtTheInteractedHero(heroToAttack);
            if (heroToAttack.GetArmorClass() < this.GetCurrentAttackAmount()) {
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
        if (heroToHeal.GetIsEnemy() == this.GetIsEnemy()) { // we want to have the same GetIsEnemy()
            this.performedActions++; // increase the number of permoemed actionsof the hero
            if (diceValue == 1 || this.performedActions > this.numOfAllowedActions) { // hero can permorm numOfAllowedActions action at every turn
                this.SetIsHealing(false);
                Debug.Log("Unsuccessfull Heal!");
                UI_Manager.Instance.SetGameInfo("Unsuccessfull Heal!");
                return;
            }
            /* if the other hero is out of range */
            int distance = PathFinding.Instance.CalculateSimpleDistance(this.transform.position, heroToHeal.transform.position);
            int distance_normalized = distance / 10; // because distance is 10x in comparison to attack range
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
        this.performedActions++; // increase the number of permoemed actionsof the hero
        if (diceValue == 1 || this.performedActions > this.numOfAllowedActions) { // hero can permorm numOfAllowedActions action at every turn
            //this.SetIsBegging(false);
            Debug.Log("Unsuccessfull Spell Cast!");
            UI_Manager.Instance.SetGameInfo("Unsuccessfull Spell Cast!");

            return;
        }
        int randNumber = UnityEngine.Random.Range(1,11);
        /* Heal All Heroes */
        if (randNumber <= 5) {
            if (!this.GetIsEnemy()) {
                foreach (Heroes hero in GameManager.Instance.aliveHeroes) {
                    this.PerformHeal(hero);
                }
            }
            else if (this.GetIsEnemy()) {
                foreach (Heroes hero in GameManager.Instance.aliveEnemies) {
                    this.PerformHeal(hero);
                }
            }
        }
        /* Attack all enemies */
        else if (randNumber > 5) {
            if (!this.GetIsEnemy()) {
                foreach (Heroes hero in GameManager.Instance.aliveEnemies) {
                    this.PerformAttack(hero);
                }
            }
            else if (this.GetIsEnemy()) {
                foreach (Heroes hero in GameManager.Instance.aliveHeroes) {
                    this.PerformAttack(hero);
                }
            }
        }
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
        if (diceValue == 1 || this.performedActions > this.numOfAllowedActions) { // hero can permorm numOfAllowedActions action at every turn
            this.SetIsBegging(false);
            Debug.Log("Unsuccessfull Beg!");
            UI_Manager.Instance.SetGameInfo("Unsuccessfull Beg!");

            return;
        }

        /* if the other hero is out of range */
        int distance = PathFinding.Instance.CalculateSimpleDistance(this.transform.position, enemyHero.transform.position);
        int distance_normalized = distance / 10; // because distance is 10x in comparison to attack range
        if (distance_normalized > this.attackRange) {
            Debug.Log("Other Hero Out Of Range");
            UI_Manager.Instance.SetGameInfo("Other Hero Out Of Range");
            this.SetIsAttacking(false);
            return;
        }

        NegotiateAmount();
        PointAtTheInteractedHero(enemyHero);
        if (enemyHero.GetArmorClass() < this.GetCurrentNegotiateValue() + begOffset) {
            Debug.Log("Successful Beg");
            UI_Manager.Instance.SetGameInfo("Successfull Beg!");

            if (enemyHero.GetIsEnemy()) {
                enemyHero.SetIsEnemy(!enemyHero.GetIsEnemy());
                GameManager.Instance.aliveEnemies.Remove(enemyHero);
                GameManager.Instance.aliveHeroes.Add(enemyHero);
            }
            else {
                enemyHero.SetIsEnemy(!enemyHero.GetIsEnemy());
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
        if (diceValue == 1 || this.performedActions > this.numOfAllowedActions) { // hero can permorm numOfAllowedActions action at every turn
            //this.SetIsBegging(false);
            Debug.Log("Unsuccessfull Spell Cast!");
            UI_Manager.Instance.SetGameInfo("Unsuccessfull Spell Cast!");
            return;
        }
        this.remainingMoveRange = 2 * this.moveRange;
        OnRemainingMoveRangeChanged?.Invoke(this, new OnRemainingMoveRangeChangedEventArgs {
            remainingSteps = this.remainingMoveRange
        });
    }



    /*********** Enemy AI Behaviour ************/
    public void EnemyAIAction() {
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
                Debug.Log("DistanceToEnemy"+ distance/10);
                hashMap_distanceToEnemies.Add(enemy,distance/10);
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
                Debug.Log("DistanceToHero" + distance/10);
                hashMap_distanceToHeroes.Add(hero, distance/10);
            }

            /* Find the enemy that is closer to this enemy */
            foreach (KeyValuePair<Heroes, int> pair in hashMap_distanceToHeroes) {
                if (pair.Value < closerHeroDistance) {
                    closerHero = pair.Key;
                    closerHeroDistance = pair.Value;
                }
            }

            // Now we know the closer hero and the closer enemy

            /* if we are at range of actions -> permorm action, else move towards target */
            // First check if the this enemy is a fighter or a ranger and if the distance is < attack range
            if ((this.heroClass.Equals("Fighter") || this.heroClass.Equals("Ranger")) && closerHeroDistance <= this.attackRange) {
                Debug.Log("Enemy AI Attacking!");
                // now attack the hero who is closer
                this.PerformAttack(closerHero);
                //Debug.Log(this+"");
                // move away from enemy..
                /* next turn... */
                TurnSystem.Instance.NextTurn(); // na mpei elegxos an exei kai allo move
                UI_Manager.Instance.gameRound.text = "ROUND " + TurnSystem.Instance.GetRoundNumber();
                UI_Manager.Instance.gameTurn.text = "TURN " + TurnSystem.Instance.GetTurnNumber();
            }
            /* If this hero is a mage */
            else if (this.heroClass.Equals("Mage")) {
                // if an ally, is close and needs healing, -> heal
                if (closerEnemyDistance <= this.attackRange && closerEnemy.GetCurrentHealAmount() < closerEnemy.GetHealthPoints()) {
                    this.PerformHeal(closerEnemy);
                    Debug.Log(this + " Heal");
                    // move away from enemy..
                    /* next turn... */
                    TurnSystem.Instance.NextTurn(); // na mpei elegxos an exei kai allo move
                    UI_Manager.Instance.gameRound.text = "ROUND " + TurnSystem.Instance.GetRoundNumber();
                    UI_Manager.Instance.gameTurn.text = "TURN " + TurnSystem.Instance.GetTurnNumber();
                }
                else {
                    // else either cast a spell or do nothing (Dash)
                    int k = UnityEngine.Random.Range(1, 11);
                    if (k < 6) {
                        // cast spell
                        this.CastSpell();
                        Debug.Log(this + " CastSpell");
                        // move away from enemy..
                        /* next turn... */
                        TurnSystem.Instance.NextTurn(); // na mpei elegxos an exei kai allo move
                        UI_Manager.Instance.gameRound.text = "ROUND " + TurnSystem.Instance.GetRoundNumber();
                        UI_Manager.Instance.gameTurn.text = "TURN " + TurnSystem.Instance.GetTurnNumber();
                    }
                }
            }
            /* If this hero is a priest */
            else if (this.heroClass.Equals("Priest")) {
                int k = UnityEngine.Random.Range(1, 11);
                // if there is an hero near, try beg him
                if (closerHeroDistance <= this.attackRange) {
                    this.Beg(closerHero);
                    Debug.Log(this + " Beg");
                    // move away from enemy..
                    /* next turn... */
                    TurnSystem.Instance.NextTurn(); // na mpei elegxos an exei kai allo move
                    UI_Manager.Instance.gameRound.text = "ROUND " + TurnSystem.Instance.GetRoundNumber();
                    UI_Manager.Instance.gameTurn.text = "TURN " + TurnSystem.Instance.GetTurnNumber();
                }
                // else, heal 
                else if (closerEnemyDistance <= this.attackRange && closerEnemy.GetCurrentHealthPoints() < closerEnemy.GetHealthPoints()) {
                    this.PerformHeal(closerEnemy);
                    Debug.Log(this + " Heal");
                    // move away from enemy..
                    /* next turn... */
                    TurnSystem.Instance.NextTurn(); // na mpei elegxos an exei kai allo move
                    UI_Manager.Instance.gameRound.text = "ROUND " + TurnSystem.Instance.GetRoundNumber();
                    UI_Manager.Instance.gameTurn.text = "TURN " + TurnSystem.Instance.GetTurnNumber();
                }
                // else spell cast or do nothing (Dash)
                else if (k < 6) {
                    this.CastSpell();
                    Debug.Log(this + " Cast spell");
                    // move away from enemy..
                    /* next turn... */
                    TurnSystem.Instance.NextTurn(); // na mpei elegxos an exei kai allo move
                    UI_Manager.Instance.gameRound.text = "ROUND " + TurnSystem.Instance.GetRoundNumber();
                    UI_Manager.Instance.gameTurn.text = "TURN " + TurnSystem.Instance.GetTurnNumber();
                }
            
            }
            /* if hero is out of range -> try to approach target */
            else {
                // Perform Dash Action and try to approach enemy or hero
                this.Dash();
                Debug.Log(this + " Dash");
                // move away from enemy..
                /* next turn... */
                TurnSystem.Instance.NextTurn(); // na mpei elegxos an exei kai allo move
                UI_Manager.Instance.gameRound.text = "ROUND " + TurnSystem.Instance.GetRoundNumber();
                UI_Manager.Instance.gameTurn.text = "TURN " + TurnSystem.Instance.GetTurnNumber();
            }
        }
        /* else return */
        else {
            Debug.Log("Either not enemy or not hero's turn or not combat mode!");
        }

    
    }

    /* function that is called to move the enemy AI characters */
    private void MoveEnemyAI(Heroes heroToFollow) {
        /* at first, we find the heroToFollow pathnode that is in front of him */
        Vector3 heroPos = heroToFollow.transform.forward;
        Vector3 tmp = new Vector3(heroPos.x,heroPos.y,heroPos.z + 1);

        // convert it to gridPosition;
        GridPosition heroFront =  PathFinding.Instance.GetGridPosition(tmp);
        GridPosition thisHeroPos = PathFinding.Instance.GetGridPosition(this.transform.position);
        
        //PathFinding.Instance.Grid().FindPath();
    }


}
