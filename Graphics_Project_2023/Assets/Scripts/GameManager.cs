using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private Heroes heroWithTurn = null;

    /******/
    private List<Heroes> heroesPrefabs = new List<Heroes>();
    private List<Heroes> enemiesPrefabs = new List<Heroes>();
    public List<Heroes> spawnedCharacters;
    public List<Heroes> aliveCharacters;

    private List<int> turnBasedOnDice = new List<int>();

    /* Heroes Prefabs */
    [SerializeField] Heroes fighterPrefab;
    [SerializeField] Heroes magePrefab;
    [SerializeField] Heroes rangerPrefab;
    [SerializeField] Heroes priestPrefab;

    /* Enemies Prefabs */
    [SerializeField] Heroes enemyFighterPrefab;
    [SerializeField] Heroes enemyMagePrefab;
    [SerializeField] Heroes enemyRangerPrefab;
    [SerializeField] Heroes enemyPriestPrefab;

    /* GameObject Prefabs */
    [SerializeField] GameObject cube;


    public enum State {  
        WaitingToStart,
        CountdownToStart,
        FreeRoam,
        CombatMode,
        GameOver,
    }

    private State currentState;
    private float timeBuffer = 1f;
    private float waitingForGameStart = 2f;

    public State GetCurrentState() {
        return this.currentState;
    }

    public void SetCurrentState(State newState) {
        this.currentState = newState;
    }

    public Heroes GetHeroWithTurn() {
        foreach (Heroes hero in spawnedCharacters) {
            if (hero.GetIsPlayersTurn()) {
                return hero;
            }
        }
        return null;
    }

    private void Awake() {
        Instance = this;
        currentState = State.WaitingToStart;   
    }

    public void Start() {
        FillPrefabLists();
        HeroesAndEnemiesToSpawn(2);
        SetAliveCharactersAtTurnSystem();
        GameObjectsInstantiation();
    }

    private void Update() {
        //Debug.Log("Current State: "+currentState);
        UI_Manager.Instance.SetGameStateText();
        switch (currentState) {
            case State.WaitingToStart:
                timeBuffer -= Time.deltaTime;
                if (timeBuffer < 0f) {
                    currentState = State.CountdownToStart;
                }
                break;
            case State.CountdownToStart:
                waitingForGameStart -= Time.deltaTime;
                if (waitingForGameStart < 0f) {
                    currentState = State.FreeRoam;
                }
                break;
            case State.FreeRoam:
                break;
            case State.GameOver:
                break; 
        }
        //Debug.Log(currentState);  
    }

    private void FillPrefabLists() {
        /* Fill Heroes Prefab List */
        this.heroesPrefabs.Add(fighterPrefab);
        this.heroesPrefabs.Add(magePrefab);
        this.heroesPrefabs.Add(rangerPrefab);
        this.heroesPrefabs.Add(priestPrefab);

        /* Fill Enemies Prefab List */
        this.enemiesPrefabs.Add(enemyFighterPrefab);
        this.enemiesPrefabs.Add(enemyMagePrefab);
        this.enemiesPrefabs.Add(enemyRangerPrefab);
        this.enemiesPrefabs.Add(enemyPriestPrefab);
    }

    private void HeroesAndEnemiesToSpawn(int numberOfHeroes) {
        /* Create Heroes */
        Fighter fighter = (Fighter)Instantiate(fighterPrefab, Vector3.zero, Quaternion.identity);

        /* Create Enemies */
        Fighter enemyFighter =(Fighter)Instantiate(enemyFighterPrefab, new Vector3 (1,0,3), Quaternion.identity);

        this.spawnedCharacters.Add(fighter); 
        this.spawnedCharacters.Add(enemyFighter);
        aliveCharacters = spawnedCharacters;
    }

    private void SetAliveCharactersAtTurnSystem() {
        TurnSystem.Instance.SetPlayingCharacters(this.aliveCharacters);
    }

    private void GameObjectsInstantiation() {
        GameObject cubeObject = Instantiate(cube,new Vector3(4,1,2), Quaternion.identity);
        SetNoWalkableAreaAtObjectInstantiation(cubeObject);
    }

    private void SetNoWalkableAreaAtObjectInstantiation(GameObject gameObject) {
        Renderer renderer = gameObject.GetComponent<Renderer>();
        Bounds bounds = renderer.bounds;
        Vector3 size = bounds.size;
        
        int width = Mathf.CeilToInt(size.x/2); 
        int depth = Mathf.CeilToInt(size.z/2); 

        Debug.Log("Size X: "+width);
        Debug.Log("Size Z: "+depth);

        for (int i=-width; i<=width; i++) {
            for (int j = -depth; j <= depth; j++) {
                //GridPosition tmpGridPosition = PathFinding.Instance.GetGridPosition(new Vector3(i, 0, j));
                Vector3 startingGameObjectPosition = gameObject.transform.position;
                GridPosition StartingGameObjectGridPosition = PathFinding.Instance.GetGridPosition(startingGameObjectPosition + new Vector3(i, 0, j));// +  //tmpGridPosition;

                PathNode objectPathNode = PathFinding.Instance.Grid().GetPathNode(StartingGameObjectGridPosition);
                objectPathNode.SetIsWalkable(false);
            }
        }
  
    }

    /* Represents One Game Round */
    /*private void GameRound() {
        int numberOfTurns = this.aliveCharacters.Count;



    }*/


}
