using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PathFinding : MonoBehaviour {
    public static PathFinding Instance { get; private set; }

    [SerializeField] private Transform _pathfindingDebugObjectPrefab;
    [SerializeField] private LayerMask heroesLayerMask;
    [SerializeField] public LayerMask gameObjectsLayerMask;
    [SerializeField] public LayerMask floorLayerMask;

    private GridPathSystem gridPathSystem;
    private Vector3 selectedHeroStartingPosition;
    public GridPosition startGridPosition;
    private Heroes selectedHero;
    private Heroes heroWithTurn;

    GridPosition prevMousePosition;
    //public GridPosition endEnemyGridPos;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    
        //Grid Creation
        //100 x 100 Grid , 1 unit tile size
        gridPathSystem = new GridPathSystem(10, 10, 1f);
        //Create Debug Objects on each grid tile
        gridPathSystem.CreateDebugObjects(_pathfindingDebugObjectPrefab, this.transform);
    }

    private void Start() {
        MouseClick.instance.OnHeroSelectAction += Instance_OnHeroSelectAction;
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        //startGridPosition = PathFinding.Instance.GetGridPosition(hero.transform.position);
        //Debug.Log("Starting Hero Position: " + startGridPosition.ToString());
        //startGridPosition = new GridPosition(0, 0);
        //startGridPosition = PathFinding.Instance.GetGridPosition(hero.transform.position);
    }

    private void TurnSystem_OnTurnChanged(object sender, TurnSystem.OnTurnChangedEventArgs e) {
        this.heroWithTurn = e.heroWithTurn;
        startGridPosition = PathFinding.Instance.GetGridPosition(heroWithTurn.transform.position);
    }

    /* Get The Selected Player */
    private void Instance_OnHeroSelectAction(object sender, MouseClick.OnHeroSelectActionEventArgs e) {
        // PREPEI NA TO FTIAXO NA PAIRNEI MONO HERO 
        this.selectedHero = e.selectedHero;
        startGridPosition = PathFinding.Instance.GetGridPosition(selectedHero.transform.position);
    }

    //Returns the GridSystem that created
    public GridPathSystem Grid() {
        return gridPathSystem;
    }

    //Returns the grid position when given a world Vector3 position
    public GridPosition GetGridPosition(Vector3 worldPosition) {
        return gridPathSystem.GetGridPosition(worldPosition);
    }

    List<GridPosition> gridPathPositionList;
    private void Update() {
        MoveHero();
    }

    public void MoveHero() {
        if ((heroWithTurn == null && GameManager.Instance.GetCurrentState() == GameManager.State.CombatMode) || selectedHero == null && GameManager.Instance.GetCurrentState() == GameManager.State.FreeRoam) {
            return;
        }
        if (heroWithTurn != null && heroWithTurn.GetIsEnemy()) {
            return;
        }
        if (selectedHero != null && selectedHero.GetIsEnemy()) {
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        /* if we have press the mouse button and we do not point to a hero */
        if (Input.GetMouseButtonDown(0) && !Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, heroesLayerMask) && !Physics.Raycast(ray, out RaycastHit raycastHit2, float.MaxValue,gameObjectsLayerMask)) {

            /* Check if we point at a UI gameObject. If yes then do not move */
            GameObject selected = EventSystem.current.currentSelectedGameObject;
            if (selected != null) {
                Collider selectedCollider = selected.GetComponent<Collider>();
                if (selectedCollider == null) {
                    return;
                }
            }
            /**************************************/
            gridPathPositionList = null;

            GridPosition mouseGridPosition = PathFinding.Instance.GetGridPosition(MouseClick.GetPosition());
            
            this.prevMousePosition = mouseGridPosition;
            
            if (selectedHero != null && GameManager.Instance.GetCurrentState() == GameManager.State.FreeRoam) {
                if (selectedHero.currentPositionIndex == 0) {
                    startGridPosition = PathFinding.Instance.GetGridPosition(selectedHero.transform.position);       
                }
            } 

            if (heroWithTurn != null && GameManager.Instance.GetCurrentState() == GameManager.State.CombatMode) {
                if (heroWithTurn.currentPositionIndex == 0) {
                    startGridPosition = PathFinding.Instance.GetGridPosition(heroWithTurn.transform.position);
                }
            }

            /*************************************************/
            if (GameManager.Instance.GetCurrentState() == GameManager.State.FreeRoam && !this.selectedHero.GetIsEnemy()) {
                /* Find the path that the player must follow */
                gridPathPositionList = gridPathSystem.FindPath(startGridPosition, mouseGridPosition, false);
            }
            else if (GameManager.Instance.GetCurrentState() == GameManager.State.CombatMode && !this.heroWithTurn.GetIsEnemy()) {
                gridPathPositionList = gridPathSystem.FindPath(startGridPosition, mouseGridPosition, false);
            }
            else {
                gridPathPositionList = null;
                //return null;
            }

            if (gridPathPositionList != null) {
                GridPosition end = gridPathPositionList[gridPathPositionList.Count-1];
                GridPosition start = gridPathPositionList[0];
                PathNode startNode = gridPathSystem.GetPathNode(start);
                PathNode endNode = gridPathSystem.GetPathNode(end);

                //Debug.Log("Start Node: "+ startNode.IsWalkable()+" End Node: "+ endNode.IsWalkable());
                
            }
            /* Check if the path is not null */
            if (gridPathPositionList == null) {
                Debug.Log("Cannot Find Path!");
                return;
            }
           
            //temporary unit position list
            List<Vector3> heroPositionsList = new List<Vector3>();

            for (int i = 0; i < gridPathPositionList.Count - 1; i++) {
                //fill the list
                heroPositionsList.Add(gridPathSystem.GetWorldPosition(gridPathPositionList[i]));

                if (i == gridPathPositionList.Count - 2)
                    heroPositionsList.Add(gridPathSystem.GetWorldPosition(gridPathPositionList[i + 1]));

                Debug.DrawLine(
                    gridPathSystem.GetWorldPosition(gridPathPositionList[i]),
                    gridPathSystem.GetWorldPosition(gridPathPositionList[i + 1]),
                    Color.red,
                    10f
                );
            }
            /* if the hero is selected, then move it from the path calculated above */
            if (selectedHero.GetIsSelected() && GameManager.Instance.GetCurrentState() == GameManager.State.FreeRoam) {
                selectedHero.SetPositionsList(heroPositionsList);
            } 
            else if (heroWithTurn != null && heroWithTurn.GetIsPlayersTurn() && GameManager.Instance.GetCurrentState() == GameManager.State.CombatMode) {
                heroWithTurn.SetPositionsList(heroPositionsList);
            }

        }
    }

    public bool FindPathForEnemyAI(GridPosition endEnemyGridPos) {

        if ((heroWithTurn == null && GameManager.Instance.GetCurrentState() == GameManager.State.CombatMode) || !heroWithTurn.GetIsEnemy()) {
            return false;
        }
        gridPathPositionList = null;

        if (heroWithTurn != null && GameManager.Instance.GetCurrentState() == GameManager.State.CombatMode) {
            //if (heroWithTurn.currentPositionIndex == 0) {
                startGridPosition = PathFinding.Instance.GetGridPosition(heroWithTurn.transform.position);
           //}
        }

        if (GameManager.Instance.GetCurrentState() == GameManager.State.CombatMode && this.heroWithTurn.GetIsEnemy()) {
            gridPathPositionList = gridPathSystem.FindPath(startGridPosition, endEnemyGridPos, false);
            Debug.Log("============================================================================");
        }

        if (gridPathPositionList == null || gridPathPositionList.Count == 0) return false;

        //temporary unit position list
        List<Vector3> heroPositionsList = new List<Vector3>();

        for (int i = 0; i < gridPathPositionList.Count - 1; i++) {
            //fill the list
            heroPositionsList.Add(gridPathSystem.GetWorldPosition(gridPathPositionList[i]));

            if (i == gridPathPositionList.Count - 2)
                heroPositionsList.Add(gridPathSystem.GetWorldPosition(gridPathPositionList[i + 1]));

            Debug.DrawLine(
                gridPathSystem.GetWorldPosition(gridPathPositionList[i]),
                gridPathSystem.GetWorldPosition(gridPathPositionList[i + 1]),
                Color.red,
                10f
            );
        }
        /* if the hero is selected, then move it from the path calculated above */
        if (selectedHero.GetIsSelected() && GameManager.Instance.GetCurrentState() == GameManager.State.FreeRoam) {
            selectedHero.SetPositionsList(heroPositionsList);
        }
        else if (heroWithTurn != null && heroWithTurn.GetIsPlayersTurn() && GameManager.Instance.GetCurrentState() == GameManager.State.CombatMode) {
            heroWithTurn.SetPositionsList(heroPositionsList);
        }
        return true;
    }
    


    public int CalculateDistanceByGrid(Vector3 targetPosition,Heroes enemyHero) {
        List<GridPosition> positionList = null;

        GridPosition targetGridPosition = PathFinding.Instance.GetGridPosition(targetPosition);
        GridPosition startGridPosition = PathFinding.Instance.GetGridPosition(enemyHero.transform.position);

        /* Find the path that the player must follow */
        positionList = gridPathSystem.FindPath(startGridPosition, targetGridPosition, true);
        this.prevMousePosition = new GridPosition();
        if (positionList == null) {
            Debug.Log("NULLLLLLLLLLLLLLLLLLLLLLLLL");
            return -1;
        }

        if (gridPathPositionList != null) {
            GridPosition end = positionList[positionList.Count - 1];
            GridPosition start = positionList[0];
            PathNode startNode = gridPathSystem.GetPathNode(start);
            PathNode endNode = gridPathSystem.GetPathNode(end);

        }

        //temporary unit position list
        List<Vector3> heroPositionsList = new List<Vector3>();

        for (int i = 0; i < positionList.Count - 1; i++) {
            //fill the list
            heroPositionsList.Add(gridPathSystem.GetWorldPosition(positionList[i]));

            if (i == gridPathPositionList.Count - 2)
                heroPositionsList.Add(gridPathSystem.GetWorldPosition(positionList[i + 1]));

            Debug.DrawLine(
                gridPathSystem.GetWorldPosition(positionList[i]),
                gridPathSystem.GetWorldPosition(positionList[i + 1]),
                Color.red,
                10f
            );
        }
        int dist = heroPositionsList.Count - 1;
        return heroPositionsList.Count-1;

    }

    public int CalculateSimpleDistance(Vector3 positionA, Vector3 positionB) {
        int MOVE_STRAIGHT_COST = 10;
        int MOVE_DIAGONAL_COST = 14;
        GridPosition gridPositionA = PathFinding.Instance.Grid().GetGridPosition(positionA);
        GridPosition gridPositionB = PathFinding.Instance.Grid().GetGridPosition(positionB);
        
        GridPosition gridPositionDistance = (gridPositionA - gridPositionB);

        /*int distance = Mathf.Abs(gridPositionDistance.x) + Mathf.Abs(gridPositionDistance.z);
        return distance * MOVE_STRAIGHT_COST;*/

        int xDistance = Mathf.Abs(gridPositionDistance.x);
        int zDistance = Mathf.Abs(gridPositionDistance.z);
        int remaining = Mathf.Abs(xDistance - zDistance);

        return (MOVE_DIAGONAL_COST * Mathf.Min(xDistance, zDistance) + MOVE_STRAIGHT_COST * remaining)/10;

    }

}
