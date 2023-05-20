using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    //private Heroes heroWithTurn = null;

    /******/
    private List<Heroes> heroesPrefabs = new List<Heroes>();
    private List<Heroes> enemiesPrefabs = new List<Heroes>();
    public List<Heroes> spawnedCharacters;
    public List<Heroes> aliveCharacters;

    private List<string> heroesFromCharacterSelectionScene = new List<string>();

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
        FreeRoam,
        CombatMode,
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
        foreach (Heroes hero in spawnedCharacters) {
            if (hero.GetIsPlayersTurn()) {
                return hero;
            }
        }
        return null;
    }

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    
    }


    public void Start() {
        Debug.Log("START AT GAMEMANAGER "+this.currentState);
        heroesFromCharacterSelectionScene = SceneLoader.selectedCharacters;
        if (GameManager.Instance.GetCurrentState() == GameManager.State.FreeRoam) {
            Debug.Log("INSIDE START WITH FREE ROAM");
            FillPrefabLists();
            HeroesAndEnemiesToSpawn(heroesFromCharacterSelectionScene);
            SetAliveCharactersAtTurnSystem();
            GameObjectsInstantiation();
        }
    }

    private void Update() {
        //Debug.Log("Current State: "+currentState);
        switch (currentState) {
            case State.FreeRoam:
                UI_Manager.Instance.SetStateInfo();
                break;
            case State.CombatMode:
                UI_Manager.Instance.SetStateInfo();
                break;
            case State.GameOver:
                UI_Manager.Instance.SetStateInfo();
                break; 
        }
        
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

    private void HeroesAndEnemiesToSpawn(List<string> listOfHeroes) {
        int i = 0;

        if (GameManager.Instance.GetCurrentState() == GameManager.State.FreeRoam) {
            /* Create Heroes */
            foreach (string heroString in listOfHeroes) {
                if (heroString.Equals(Fighter.HERO_CLASS)) {
                    Fighter fighter = (Fighter)Instantiate(fighterPrefab,new Vector3(i,0,1), Quaternion.identity);
                    this.spawnedCharacters.Add(fighter);
                }
                else if (heroString.Equals(Ranger.HERO_CLASS)) {
                    Ranger ranger = (Ranger)Instantiate(rangerPrefab, new Vector3(i, 0, 1), Quaternion.identity);
                    this.spawnedCharacters.Add(ranger);
                }
                else if (heroString.Equals(Mage.HERO_CLASS)) {
                    Mage mage = (Mage)Instantiate(magePrefab, new Vector3(i, 0, 1), Quaternion.identity);
                    this.spawnedCharacters.Add(mage);
                }
                else if (heroString.Equals(Priest.HERO_CLASS)) {
                    Priest priest = (Priest)Instantiate(priestPrefab, new Vector3(i, 0, 1), Quaternion.identity);
                    this.spawnedCharacters.Add(priest);
                }

                i++;
            }

            aliveCharacters = spawnedCharacters;
        }
    }

    private void SetAliveCharactersAtTurnSystem() {
        TurnSystem.Instance.SetPlayingCharacters(this.aliveCharacters);
    }

    private void GameObjectsInstantiation() {
        GameObject cubeObject = Instantiate(cube,new Vector3(4,1,4), Quaternion.identity);
        SetNoWalkableAreaAtObjectInstantiation(cubeObject);
    }

    private void SetNoWalkableAreaAtObjectInstantiation(GameObject gameObject) {
        Renderer renderer = gameObject.GetComponent<Renderer>();
        Bounds bounds = renderer.bounds;
        Vector3 size = bounds.size;
        
        int width = Mathf.CeilToInt(size.x/2); 
        int depth = Mathf.CeilToInt(size.z/2); 

        /*Debug.Log("Size X: "+width);
        Debug.Log("Size Z: "+depth);*/

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
