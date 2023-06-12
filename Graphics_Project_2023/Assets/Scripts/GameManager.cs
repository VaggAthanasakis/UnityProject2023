using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    public static bool isGamePaused = false;
   
    /******/
    private List<Heroes> heroesPrefabs = new List<Heroes>();
    private List<Heroes> enemiesPrefabs = new List<Heroes>();

    public List<Heroes> aliveCharacters;  // list with both heroes and enemies
    public List<Heroes> aliveHeroes;  // list with heroes
    public List<Heroes> aliveEnemies; // list with the alive enemies

    private List<string> heroesFromCharacterSelectionScene = new List<string>();

    private List<int> turnBasedOnDice = new List<int>();

    private int enterCombatModeRange = 8;
    public bool isCheckingForCombat = false;
    private int gameRound = 1;

    /* Heroes Prefabs */
    [SerializeField] Heroes fighterPrefab;
    [SerializeField] Heroes magePrefab;
    [SerializeField] Heroes rangerPrefab;
    [SerializeField] Heroes priestPrefab;
    [SerializeField] Heroes musicianPrefab;
    [SerializeField] Heroes summonerPrefab;

    /* Enemies Prefabs */
    [SerializeField] Heroes enemyFighterPrefab;
    [SerializeField] Heroes enemyMagePrefab;
    [SerializeField] Heroes enemyRangerPrefab;
    [SerializeField] Heroes enemyPriestPrefab;
    [SerializeField] Heroes enemyMusicianPrefab;
    [SerializeField] Heroes enemySummonerPrefab;
    [SerializeField] Heroes enemyBossPrefab;

    /* GameObject Prefabs */
    [SerializeField] GameObject chestPrefab;
    [SerializeField] GameObject treePrefab;
    [SerializeField] GameObject rockPrefab;

    // variable to keep on track at which stage of the game we are
    private int atEnemyGroup = 1;


    public enum State {  
        FreeRoam,
        CombatMode,
        Victory,
        GameOver,
    }

    private State currentState;

    public State GetCurrentState() {
        return this.currentState;
    }

    public void SetCurrentState(State newState) {
        this.currentState = newState;
    }

    public Heroes GetHeroWithTurn() {
        return TurnSystem.Instance.GetHeroWithTurn();
    }

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    public void Start() {
        TurnSystem.Instance.OnRoundEnded += TurnSystem_OnRoundEnded;
        heroesFromCharacterSelectionScene = SceneLoader.selectedCharacters;
        FillPrefabLists();
        HeroesToSpawn(heroesFromCharacterSelectionScene);
        // Spawn the first Group Of Enemies
        SpawnNextGroupOfEnemies();
        SetAliveCharactersAtTurnSystem();
        
        /* Start The Free Roam Music */
        //StartCoroutine(SoundManager.Instance.StopSound(SoundManager.MAIN_MENU_CHAR_SELECTION_MUSIC));
        //StartCoroutine(SoundManager.Instance.PlaySound(SoundManager.FREE_ROAM_MUSIC));
        // Spawn the objects
        RandomGameObjectsInstantiation();

    }

    /* This event arrives when the round ends */
    private void TurnSystem_OnRoundEnded(object sender, TurnSystem.OnRoundEndedEventArgs e) {
        Debug.Log("ROUND ENDED!");
        TurnSystem.Instance.ResetTurnNumber();
        ResetCharactersFeatures();
        //SetAliveCharactersAtTurnSystem();
        gameRound++;
    }

    private void Update() {
        switch (currentState) {
            case State.FreeRoam:
                UI_Manager.Instance.SetStateInfo();
                break;
            case State.CombatMode:
                UI_Manager.Instance.SetStateInfo();
                CheckIfGameEnded();
                break;
            case State.Victory:
                UI_Manager.Instance.SetStateInfo();
                break;
            case State.GameOver:
                UI_Manager.Instance.SetStateInfo();
                break; 
        }  
    }

    /* Chech if the game has ended, check if all heroes are dead or all enemies are dead */
    private void CheckIfGameEnded() {
        if (aliveEnemies.Count <= 0 && currentState != State.Victory ) { //
            UI_Manager.Instance.SetTurnInfo(false, false);  // deactivate the turn panel
            if (atEnemyGroup <= 3) { // if we have defeat the final group of enemies then win
                /* Spawn next Group here */
                TurnSystem.Instance.ResetRoundNumber();
                TurnSystem.Instance.ResetTurnNumber();
                currentState = GameManager.State.FreeRoam;
                SoundManager.Instance.StopSoundWithoutFade(SoundManager.COMBAT_MODE_MUSIC);
                SoundManager.Instance.PlaySoundWithoutFade(SoundManager.FREE_ROAM_MUSIC);
                SpawnNextGroupOfEnemies();
                SetAliveCharactersAtTurnSystem();
            }
            else  {
                // else we have defeat all the groups of enemies -> victory
                currentState = State.Victory;
                SoundManager.Instance.StopSoundWithoutFade(SoundManager.COMBAT_MODE_MUSIC);
                SoundManager.Instance.PlaySoundWithoutFade(SoundManager.VICTORY_MUSIC);
                SoundManager.Instance.PlaySoundWithoutFade(SoundManager.MAIN_MENU_CHAR_SELECTION_MUSIC);
                Debug.Log("VICTORY");
            }
        }
        if (aliveHeroes.Count <= 0 && currentState != State.GameOver) {
            currentState = State.GameOver;
            SoundManager.Instance.StopSoundWithoutFade(SoundManager.COMBAT_MODE_MUSIC);
            SoundManager.Instance.PlaySoundWithoutFade(SoundManager.DEFEAT_MUSIC);
            SoundManager.Instance.PlaySoundWithoutFade(SoundManager.MAIN_MENU_CHAR_SELECTION_MUSIC);
            Debug.Log("DEFEAT");
        }
    }

    /* Reset Heroes MoveRange */
    private void ResetCharactersFeatures() {
        foreach (Heroes hero in aliveCharacters) {
            hero.SetRemainingMoveRange(hero.GetMoveRange());
            hero.SetCurrentArmorClass(hero.GetArmorClass());
            hero.performedActions = 0;
            hero.SetPositionsList(new List<Vector3>());
            hero.SetIsWalking(false);
            hero.SetIsPlayersTurn(false);
        }
    }

    private void FillPrefabLists() {
        /* Fill Heroes Prefab List */
        this.heroesPrefabs.Add(fighterPrefab);
        this.heroesPrefabs.Add(magePrefab);
        this.heroesPrefabs.Add(rangerPrefab);
        this.heroesPrefabs.Add(priestPrefab);
        this.heroesPrefabs.Add(musicianPrefab);

        /* Fill Enemies Prefab List */
        this.enemiesPrefabs.Add(enemyFighterPrefab);
        this.enemiesPrefabs.Add(enemyMagePrefab);
        this.enemiesPrefabs.Add(enemyRangerPrefab);
        this.enemiesPrefabs.Add(enemyPriestPrefab);
        this.enemiesPrefabs.Add(enemyMusicianPrefab);
    }

    private void HeroesToSpawn(List<string> listOfHeroes) {
        int xWorldPos = 0;
        this.aliveHeroes = new List<Heroes>();
        this.aliveEnemies = new List<Heroes>();
        this.aliveCharacters = new List<Heroes>();

        if (currentState == GameManager.State.FreeRoam) {
            /* Create Heroes */
            foreach (string heroString in listOfHeroes) {
                //Debug.Log(heroString);
            }

            GridPosition startGridPosition = new GridPosition(9,7);  // checked this via the debag objects
            foreach (string heroString in listOfHeroes) {
                if (heroString.Equals(Fighter.HERO_CLASS)) {
                    Vector3 posToSpawn = PathFinding.Instance.Grid().GetWorldPosition(new GridPosition(startGridPosition.x + xWorldPos, startGridPosition.z));
                    Fighter fighter = (Fighter)Instantiate(fighterPrefab, posToSpawn, Quaternion.identity);
                    this.aliveCharacters.Add(fighter);
                    this.aliveHeroes.Add(fighter);
                }
                else if (heroString.Equals(Ranger.HERO_CLASS)) {
                    Vector3 posToSpawn = PathFinding.Instance.Grid().GetWorldPosition(new GridPosition(startGridPosition.x + xWorldPos, startGridPosition.z));
                    Ranger ranger = (Ranger)Instantiate(rangerPrefab, posToSpawn, Quaternion.identity);
                    this.aliveCharacters.Add(ranger);
                    this.aliveHeroes.Add(ranger);
                }
                else if (heroString.Equals(Mage.HERO_CLASS)) {
                    Vector3 posToSpawn = PathFinding.Instance.Grid().GetWorldPosition(new GridPosition(startGridPosition.x + xWorldPos, startGridPosition.z));
                    Mage mage = (Mage)Instantiate(magePrefab, posToSpawn, Quaternion.identity);
                    this.aliveCharacters.Add(mage);
                    this.aliveHeroes.Add(mage);

                }
                else if (heroString.Equals(Priest.HERO_CLASS)) {
                    Vector3 posToSpawn = PathFinding.Instance.Grid().GetWorldPosition(new GridPosition(startGridPosition.x + xWorldPos, startGridPosition.z));
                    Priest priest = (Priest)Instantiate(priestPrefab, posToSpawn, Quaternion.identity);
                    this.aliveCharacters.Add(priest);
                    this.aliveHeroes.Add(priest);
                }
                else if (heroString.Equals(Musician.HERO_CLASS)) {
                    Vector3 posToSpawn = PathFinding.Instance.Grid().GetWorldPosition(new GridPosition(startGridPosition.x + xWorldPos, startGridPosition.z));
                    Musician musician = (Musician)Instantiate(musicianPrefab, posToSpawn, Quaternion.identity);
                    this.aliveCharacters.Add(musician);
                    this.aliveHeroes.Add(musician);
                }
                else if (heroString.Equals(Summoner.HERO_CLASS)) {
                    Vector3 posToSpawn = PathFinding.Instance.Grid().GetWorldPosition(new GridPosition(startGridPosition.x + xWorldPos, startGridPosition.z));
                    Summoner summoner = (Summoner)Instantiate(summonerPrefab, posToSpawn, Quaternion.identity);
                    this.aliveCharacters.Add(summoner);
                    this.aliveHeroes.Add(summoner);
                }
                xWorldPos++;
            } 
        }
    }

    /* This method creates the group of enemies
     * There are three group of enemies, with the 3rd one being the boss
     * We Spawn Enemies due to the curse of the battle */
    private void SpawnNextGroupOfEnemies() {
        int enemiesToSpawn = 0;
        GridPosition positionOfFirstEnemy = new GridPosition(0,0);
        int numOfAliveHeroes = this.aliveHeroes.Count;
        // If we are the first group of enemies
        if (atEnemyGroup == 1) {
            Debug.Log("Heroes Alive: " + numOfAliveHeroes);
            enemiesToSpawn = numOfAliveHeroes - 1;
            if (enemiesToSpawn <= 0) { enemiesToSpawn = 1; }
            Debug.Log("enemies To spawn: " + enemiesToSpawn);
            positionOfFirstEnemy = new GridPosition(76, 14);   // checked this via debug objects
            Debug.Log("At First Group");
        }
        else if (atEnemyGroup == 2) {
            enemiesToSpawn = numOfAliveHeroes;
            if (enemiesToSpawn <= 0) { enemiesToSpawn = 1; }
            positionOfFirstEnemy = new GridPosition(65, 65); // adding at z axis for the next
            Debug.Log("At Second Group");
        }
        else if (atEnemyGroup == 3) {
            // here we will spawn the final boss of the enemies
            enemiesToSpawn = numOfAliveHeroes;
            if (enemiesToSpawn <= 0) { enemiesToSpawn = 1; }
            positionOfFirstEnemy = new GridPosition(23, 61);
            Debug.Log("At 3rd Group");
        }
        else {
            Debug.Log("Spawned all Enemies");
            return;
        }
        //atEnemyGroup++; // increase the counter

        /* Now Randomly Spawn The Enemies */

        /* if we are at the 3rd group, we also need to spawn the enemy boss */
        if (atEnemyGroup == 3) {
            Debug.Log("Spawn Boss");
            positionOfFirstEnemy = new GridPosition(positionOfFirstEnemy.x, positionOfFirstEnemy.z);
            Vector3 worldPos = PathFinding.Instance.Grid().GetWorldPosition(positionOfFirstEnemy);
            InstantiateHeroOnPosition(FinalBoss.HERO_CLASS, worldPos, true); // true because it is an enemy
        }
        for (int i=1; i<= enemiesToSpawn; i++) { 
            int randNumber = Random.Range(1, 7); // since we have 6 prefabs of enemies
            if (randNumber == 1) {               // spawn fighter
                positionOfFirstEnemy = new GridPosition(positionOfFirstEnemy.x, positionOfFirstEnemy.z + i);
                Vector3 worldPos = PathFinding.Instance.Grid().GetWorldPosition(positionOfFirstEnemy);
                InstantiateHeroOnPosition(Fighter.HERO_CLASS, worldPos, true); // true because it is an enemy
            }
            else if (randNumber == 2) {         // spawn ranger
                positionOfFirstEnemy = new GridPosition(positionOfFirstEnemy.x, positionOfFirstEnemy.z + i);
                Vector3 worldPos = PathFinding.Instance.Grid().GetWorldPosition(positionOfFirstEnemy);
                InstantiateHeroOnPosition(Ranger.HERO_CLASS, worldPos, true); // true because it is an enemy
            }
            else if (randNumber == 3) {         // spawn mage
                positionOfFirstEnemy = new GridPosition(positionOfFirstEnemy.x, positionOfFirstEnemy.z + i);
                Vector3 worldPos = PathFinding.Instance.Grid().GetWorldPosition(positionOfFirstEnemy);
                InstantiateHeroOnPosition(Mage.HERO_CLASS, worldPos, true); // true because it is an enemy
            }
            else if (randNumber == 4) {         // spawn priest
                positionOfFirstEnemy = new GridPosition(positionOfFirstEnemy.x, positionOfFirstEnemy.z + i);
                Vector3 worldPos = PathFinding.Instance.Grid().GetWorldPosition(positionOfFirstEnemy);
                InstantiateHeroOnPosition(Priest.HERO_CLASS, worldPos, true); // true because it is an enemy
            }
            else if (randNumber == 5) {         // spawn musician
                positionOfFirstEnemy = new GridPosition(positionOfFirstEnemy.x, positionOfFirstEnemy.z + i);
                Vector3 worldPos = PathFinding.Instance.Grid().GetWorldPosition(positionOfFirstEnemy);
                InstantiateHeroOnPosition(Musician.HERO_CLASS, worldPos, true); // true because it is an enemy
            }
            else if (randNumber == 6) {         // spawn summoner
                positionOfFirstEnemy = new GridPosition(positionOfFirstEnemy.x, positionOfFirstEnemy.z + i);
                Vector3 worldPos = PathFinding.Instance.Grid().GetWorldPosition(positionOfFirstEnemy);
                InstantiateHeroOnPosition(Summoner.HERO_CLASS, worldPos, true); // true because it is an enemy
            }
        }

        atEnemyGroup++; // increase the counter


    }

    private void SetAliveCharactersAtTurnSystem() {
        TurnSystem.Instance.SetPlayingCharacters(this.aliveCharacters);
        Debug.Log("========================");
        foreach (Heroes hero in this.aliveCharacters) {
            Debug.Log(hero);
        }
        Debug.Log("========================");
    }

    public void SetPlayingCharacters(List<Heroes> playingCharacters) {
        this.aliveCharacters = playingCharacters;
    }

    /* This method creates a randmom amount of gameObjects to the board at random places */
    private void RandomGameObjectsInstantiation() {
        int numOfChests = Random.Range(5, 10); // number of chest to instantiate
        int numOfRocks = Random.Range(10, 15); // number of rocks to instantiate

        // instantiate all the chests
        while (numOfChests != 0) {
            int randx = Random.Range(1, 101);
            int randz = Random.Range(1, 81);
            if ((randx >= 55 && randx <= 70) && (randz >= 3 && randz <= 22)) {
                Vector3 posToSpawn = PathFinding.Instance.Grid().GetWorldPosition(new GridPosition(randx,randz));
                GameObject chest = Instantiate(chestPrefab, posToSpawn, Quaternion.identity);
                PathNode node = PathFinding.Instance.Grid().GetPathNode(new GridPosition(randx,randz));
               
                /* Check if there is already an object there */
                if (node != null) { node.SetIsWalkable(false); }

                numOfChests--;
            }
            else if ((randx >= 64 && randx <= 94) && (randz >= 30 && randz <= 42)) {
                Vector3 posToSpawn = PathFinding.Instance.Grid().GetWorldPosition(new GridPosition(randx, randz));
                GameObject chest = Instantiate(chestPrefab, posToSpawn, Quaternion.identity);
                PathNode node = PathFinding.Instance.Grid().GetPathNode(new GridPosition(randx, randz));

                /* Check if there is already an object there */
                if (node != null) { node.SetIsWalkable(false); }
                numOfChests--;
            }
            else if ((randx >= 22 && randx <= 66) && (randz >= 43 && randz <= 60)) {
                Vector3 posToSpawn = PathFinding.Instance.Grid().GetWorldPosition(new GridPosition(randx, randz));
                GameObject chest = Instantiate(chestPrefab, posToSpawn, Quaternion.identity);
                PathNode node = PathFinding.Instance.Grid().GetPathNode(new GridPosition(randx, randz));

                /* Check if there is already an object there */
                if (node != null) { node.SetIsWalkable(false); }
                numOfChests--;
            }
        }
        // intantiate the rocks
        while (numOfRocks != 0) {
            int randx = Random.Range(1, 101);
            int randz = Random.Range(1, 81);
            if ((randx >= 55 && randx <= 70) && (randz >= 3 && randz <= 22)) {
                Vector3 posToSpawn = PathFinding.Instance.Grid().GetWorldPosition(new GridPosition(randx, randz));
                GameObject rock = Instantiate(rockPrefab, posToSpawn, Quaternion.identity);
                PathNode node = PathFinding.Instance.Grid().GetPathNode(new GridPosition(randx, randz));

                /* Check if there is already an object there */
                if (node != null) { node.SetIsWalkable(false); }
                numOfRocks--;
            }
            else if ((randx >= 64 && randx <= 94) && (randz >= 30 && randz <= 42)) {
                Vector3 posToSpawn = PathFinding.Instance.Grid().GetWorldPosition(new GridPosition(randx, randz));
                GameObject rock = Instantiate(rockPrefab, posToSpawn, Quaternion.identity);
                PathNode node = PathFinding.Instance.Grid().GetPathNode(new GridPosition(randx, randz));

                /* Check if there is already an object there */
                if (node != null) { node.SetIsWalkable(false); }
                numOfRocks--;
            }
            else if ((randx >= 22 && randx <= 66) && (randz >= 43 && randz <= 75)) {
                Vector3 posToSpawn = PathFinding.Instance.Grid().GetWorldPosition(new GridPosition(randx, randz));
                GameObject rock = Instantiate(rockPrefab, posToSpawn, Quaternion.identity);
                PathNode node = PathFinding.Instance.Grid().GetPathNode(new GridPosition(randx, randz));

                /* Check if there is already an object there */
                if (node != null) { node.SetIsWalkable(false); }
                numOfRocks--;
            }
        }
    }

    /* Now we will check if during the movement, the hero comes close to an enemy
     * if yes, then the game state changes to combatMode 
     * We calculate the distance between the current position of the hero who is moving
     * with the position of all the enemies. If that distance is less than the combat mode
     * range, with at least an enemy, then we switch to combat mode */
    public bool CheckForCombatMode(Vector3 targetPosition) {
        int distance;
        /* if we are already, return */
        if (currentState == State.CombatMode) return false;

        /* If there is someone walking check if he is at combat range with an enemy*/
        foreach (Heroes enemy in aliveEnemies) {
            distance = PathFinding.Instance.CalculateDistanceByGrid(targetPosition,enemy);
            /* If they are, enter combat mode */
            if (distance <= enterCombatModeRange) {
                isCheckingForCombat = false;
                currentState = GameManager.State.CombatMode;
                ResetCharactersFeatures();
                UI_Manager.Instance.diceButton.SetActive(true);
                isCheckingForCombat = false;
                StartCoroutine(SoundManager.Instance.StopSound(SoundManager.FREE_ROAM_MUSIC));
                StartCoroutine(SoundManager.Instance.PlaySound(SoundManager.COMBAT_MODE_MUSIC));
                return true;
            }
        }
        return false;
    }

    /* This method takes the hero and the position and spawns it at the world
     * Also, it puts the hero/enemy at the list of alive heroes/enemies and to the alive Characters List
     * and makes the grid hero's node no walkable */
    public void InstantiateHeroOnPosition(string heroClass, Vector3 position, bool isEnemy) {
        switch (heroClass) {
            case Fighter.HERO_CLASS :
                if (!isEnemy) {
                    Fighter fighter = (Fighter)Instantiate(fighterPrefab, position, Quaternion.identity);
                    aliveCharacters.Add(fighter);
                    aliveHeroes.Add(fighter);
                    fighter.SetWalkableNodeAtHeroPosition(false);
                }
                else {
                    Fighter fighter = (Fighter)Instantiate(enemyFighterPrefab, position, Quaternion.identity);
                    aliveCharacters.Add(fighter);
                    aliveEnemies.Add(fighter);
                    fighter.SetWalkableNodeAtHeroPosition(false);
                }
                break;
            case Ranger.HERO_CLASS:
                if (!isEnemy) {
                    Ranger ranger = (Ranger)Instantiate(rangerPrefab, position, Quaternion.identity);
                    aliveCharacters.Add(ranger);
                    aliveHeroes.Add(ranger);
                    ranger.SetWalkableNodeAtHeroPosition(false);
                }
                else {
                    Ranger ranger = (Ranger)Instantiate(enemyRangerPrefab, position, Quaternion.identity);
                    aliveCharacters.Add(ranger);
                    aliveEnemies.Add(ranger);
                    ranger.SetWalkableNodeAtHeroPosition(false);
                }
                break;
            case Mage.HERO_CLASS:
                if (!isEnemy) {
                    Mage mage = (Mage)Instantiate(magePrefab, position, Quaternion.identity);
                    aliveCharacters.Add(mage);
                    aliveHeroes.Add(mage);
                    mage.SetWalkableNodeAtHeroPosition(false);
                }
                else {
                    Mage mage = (Mage)Instantiate(enemyMagePrefab, position, Quaternion.identity);
                    aliveCharacters.Add(mage);
                    aliveEnemies.Add(mage);
                    mage.SetWalkableNodeAtHeroPosition(false);
                }
                break;
            case Priest.HERO_CLASS:
                if (!isEnemy) {
                    Priest priest = (Priest)Instantiate(priestPrefab, position, Quaternion.identity);
                    aliveCharacters.Add(priest);
                    aliveHeroes.Add(priest);
                    priest.SetWalkableNodeAtHeroPosition(false);
                }
                else {
                    Priest priest = (Priest)Instantiate(enemyPriestPrefab, position, Quaternion.identity);
                    aliveCharacters.Add(priest);
                    aliveEnemies.Add(priest);
                    priest.SetWalkableNodeAtHeroPosition(false);
                }
                break;
            case Musician.HERO_CLASS:
                if (!isEnemy) {
                    Musician musician = (Musician)Instantiate(musicianPrefab, position, Quaternion.identity);
                    aliveCharacters.Add(musician);
                    aliveHeroes.Add(musician);
                    musician.SetWalkableNodeAtHeroPosition(false);
                }
                else {
                    Musician musician = (Musician)Instantiate(enemyMusicianPrefab, position, Quaternion.identity);
                    aliveCharacters.Add(musician);
                    aliveEnemies.Add(musician);
                    musician.SetWalkableNodeAtHeroPosition(false);
                }
                break;
            case Summoner.HERO_CLASS:
                if (!isEnemy) {
                    Summoner summoner = (Summoner)Instantiate(summonerPrefab, position, Quaternion.identity);
                    aliveCharacters.Add(summoner);
                    aliveHeroes.Add(summoner);
                    summoner.SetWalkableNodeAtHeroPosition(false);
                }
                else {
                    Summoner summoner = (Summoner)Instantiate(enemySummonerPrefab, position, Quaternion.identity);
                    aliveCharacters.Add(summoner);
                    aliveEnemies.Add(summoner);
                    summoner.SetWalkableNodeAtHeroPosition(false);
                }
                break;
            case FinalBoss.HERO_CLASS:
                FinalBoss boss = (FinalBoss)Instantiate(enemyBossPrefab, position, Quaternion.identity);
                aliveCharacters.Add(boss);
                aliveEnemies.Add(boss);
                boss.SetWalkableNodeAtHeroPosition(false);
                break;
        }
    
    }

}
