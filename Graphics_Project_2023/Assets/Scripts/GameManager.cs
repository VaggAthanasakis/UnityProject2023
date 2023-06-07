using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
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

    private int enterCombatModeRange = 3;
    public bool isCheckingForCombat = false;
    private int gameRound = 1;

    /* Heroes Prefabs */
    [SerializeField] Heroes fighterPrefab;
    [SerializeField] Heroes magePrefab;
    [SerializeField] Heroes rangerPrefab;
    [SerializeField] Heroes priestPrefab;
    [SerializeField] Heroes musicianPrefab;

    /* Enemies Prefabs */
    [SerializeField] Heroes enemyFighterPrefab;
    [SerializeField] Heroes enemyMagePrefab;
    [SerializeField] Heroes enemyRangerPrefab;
    [SerializeField] Heroes enemyPriestPrefab;
    [SerializeField] Heroes enemyMusicianPrefab;

    /* GameObject Prefabs */
    [SerializeField] GameObject cube;


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
        /*foreach (Heroes hero in aliveCharacters) {
            if (hero.GetIsPlayersTurn()) {
                return hero;
            }
        }
        return null;*/
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
        HeroesAndEnemiesToSpawn(heroesFromCharacterSelectionScene);
        SetAliveCharactersAtTurnSystem();
        //GameObjectsInstantiation();
        
        /* Start The Free Roam Music */
        StartCoroutine(SoundManager.Instance.StopSound(SoundManager.MAIN_MENU_CHAR_SELECTION_MUSIC));
        StartCoroutine(SoundManager.Instance.PlaySound(SoundManager.FREE_ROAM_MUSIC));

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
        //CheckForCombatMode();
        //Debug.Log("Current State: "+currentState);
        switch (currentState) {
            case State.FreeRoam:
                UI_Manager.Instance.SetStateInfo();
                break;
            case State.CombatMode:
                //StartCoroutine(SoundManager.Instance.StopSound(SoundManager.FREE_ROAM_MUSIC));
                //SoundManager.Instance.StopSound(SoundManager.FREE_ROAM_MUSIC);
                //SoundManager.Instance.PlaySound(SoundManager.COMBAT_MODE_MUSIC);
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
        if (aliveEnemies.Count <= 0) { //
            currentState = State.Victory;
            Debug.Log("VICTORY");
        }
        if (aliveHeroes.Count <= 0) {
            currentState = State.GameOver;
            Debug.Log("DEFEAT");
        }
    }

    /* Reset Heroes MoveRange */
    private void ResetCharactersFeatures() {
        foreach (Heroes hero in aliveCharacters) {
            hero.SetRemainingMoveRange(hero.GetMoveRange());
            hero.SetCurrentArmorClass(hero.GetArmorClass());
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

    private void HeroesAndEnemiesToSpawn(List<string> listOfHeroes) {
        int xWorldPos = 0;
        this.aliveHeroes = new List<Heroes>();
        this.aliveEnemies = new List<Heroes>();
        this.aliveCharacters = new List<Heroes>();

        if (currentState == GameManager.State.FreeRoam) {
            /* Create Heroes */
            foreach (string heroString in listOfHeroes) {
                //Debug.Log(heroString);
            }

            foreach (string heroString in listOfHeroes) {
                if (heroString.Equals(Fighter.HERO_CLASS)) {
                    Fighter fighter = (Fighter)Instantiate(fighterPrefab, new Vector3(xWorldPos, 0, 1), Quaternion.identity);
                    this.aliveCharacters.Add(fighter);
                    this.aliveHeroes.Add(fighter);
                }
                else if (heroString.Equals(Ranger.HERO_CLASS)) {
                    Ranger ranger = (Ranger)Instantiate(rangerPrefab, new Vector3(xWorldPos, 0, 1), Quaternion.identity);
                    this.aliveCharacters.Add(ranger);
                    this.aliveHeroes.Add(ranger);
                }
                else if (heroString.Equals(Mage.HERO_CLASS)) {
                    Mage mage = (Mage)Instantiate(magePrefab, new Vector3(xWorldPos, 0, 1), Quaternion.identity);
                    this.aliveCharacters.Add(mage);
                    this.aliveHeroes.Add(mage);

                }
                else if (heroString.Equals(Priest.HERO_CLASS)) {
                    Priest priest = (Priest)Instantiate(priestPrefab, new Vector3(xWorldPos, 0, 1), Quaternion.identity);
                    this.aliveCharacters.Add(priest);
                    this.aliveHeroes.Add(priest);
                }
                else if (heroString.Equals(Musician.HERO_CLASS)) {
                    Musician musician = (Musician)Instantiate(musicianPrefab, new Vector3(xWorldPos, 0, 1), Quaternion.identity);
                    this.aliveCharacters.Add(musician);
                    this.aliveHeroes.Add(musician);
                }
                xWorldPos++;
            } 
        }

        /* Create Enemies */
        //Fighter enemyFighter = (Fighter)Instantiate(enemyFighterPrefab, new Vector3(2,0,9), Quaternion.identity);
         Ranger enemyRanger = (Ranger)Instantiate(enemyRangerPrefab,new Vector3(4,0,9), Quaternion.identity);
         //Priest enemyPriest = (Priest)Instantiate(enemyPriestPrefab, new Vector3(5,0,7), Quaternion.identity);

        // this.aliveCharacters.Add(enemyFighter);
         this.aliveCharacters.Add(enemyRanger);
        // this.aliveCharacters.Add(enemyPriest);
        // this.aliveEnemies.Add(enemyFighter);
         this.aliveEnemies.Add(enemyRanger);
        // this.aliveEnemies.Add(enemyPriest);
        //Musician enemyMusician = (Musician)Instantiate(enemyMusicianPrefab, new Vector3(5,0,9), Quaternion.identity);
        //Musician enemyMusician2 = (Musician)Instantiate(enemyMusicianPrefab, new Vector3(6,0,9), Quaternion.identity);
        /*this.aliveCharacters.Add(enemyMusician);
        this.aliveCharacters.Add(enemyMusician2);
        this.aliveEnemies.Add(enemyMusician);
        this.aliveEnemies.Add(enemyMusician2);*/
    }

    private void SetAliveCharactersAtTurnSystem() {
        TurnSystem.Instance.SetPlayingCharacters(this.aliveCharacters);
    }

    public void SetPlayingCharacters(List<Heroes> playingCharacters) {
        this.aliveCharacters = playingCharacters;
    }

    private void GameObjectsInstantiation() {
        GameObject cubeObject = Instantiate(cube,new Vector3(4,1,4), Quaternion.identity);
        SetNoWalkableAreaAtObjectInstantiation(cubeObject);
    }

    public void SetNoWalkableAreaAtObjectInstantiation(GameObject gameObject) {
        Renderer renderer = gameObject.GetComponent<Renderer>();
        Bounds bounds = renderer.bounds;
        Vector3 size = bounds.size;
        
        int width = Mathf.CeilToInt(size.x/2); 
        int depth = Mathf.CeilToInt(size.z/2); 

        for (int i=-width; i<=width-1; i++) {
            for (int j = -depth; j <= depth-1; j++) {
                Vector3 startingGameObjectPosition = gameObject.transform.position;
                GridPosition StartingGameObjectGridPosition = PathFinding.Instance.GetGridPosition(startingGameObjectPosition + new Vector3(i, 0, j));// +  //tmpGridPosition;

                PathNode objectPathNode = PathFinding.Instance.Grid().GetPathNode(StartingGameObjectGridPosition);
                objectPathNode.SetIsWalkable(false);
            }
        }
  
    }

    /* Now we will check if during the movement, the hero comes close to an enemy
     * if yes, then the game state changes to combatMode */
    /* We calculate the distance between the current position of the hero who is moving
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
                isCheckingForCombat = false;
                StartCoroutine(SoundManager.Instance.StopSound(SoundManager.FREE_ROAM_MUSIC));
                StartCoroutine(SoundManager.Instance.PlaySound(SoundManager.COMBAT_MODE_MUSIC));
                return true;
            }
        }
        return false;
    }

}
