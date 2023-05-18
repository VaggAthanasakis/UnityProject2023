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
    private GridPosition startGridPosition;
    private Heroes hero;

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
        //startGridPosition = PathFinding.Instance.GetGridPosition(hero.transform.position);
        //Debug.Log("Starting Hero Position: " + startGridPosition.ToString());
        //startGridPosition = new GridPosition(0, 0);
        //startGridPosition = PathFinding.Instance.GetGridPosition(hero.transform.position);
    }

    /* Get The Selected Player */ 
    private void Instance_OnHeroSelectAction(object sender, MouseClick.OnHeroSelectActionEventArgs e) {
        this.hero = e.selectedHero;
        startGridPosition = PathFinding.Instance.GetGridPosition(hero.transform.position);
        Debug.Log("StartGridPath Position: "+startGridPosition);
        //this.hero.SetIsSelected(true);
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

    private void MoveHero() {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        /* if we have press the mouse button and we do not point to a hero */
        if (Input.GetMouseButtonDown(0) && !Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, heroesLayerMask)) {

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
            if (hero != null) {
                if (hero.currentPositionIndex == 0) {
                    Debug.Log("HEre");
                    startGridPosition = PathFinding.Instance.GetGridPosition(hero.transform.position);
                    
                }
            }

            /* Find the path that the player must follow */
            gridPathPositionList = gridPathSystem.FindPath(startGridPosition, mouseGridPosition);

            if (gridPathPositionList != null) {
                GridPosition end = gridPathPositionList[gridPathPositionList.Count-1];
                GridPosition start = gridPathPositionList[0];
                PathNode startNode = gridPathSystem.GetPathNode(start);
                PathNode endNode = gridPathSystem.GetPathNode(end);

                Debug.Log("Start Node: "+ startNode.IsWalkable()+" End Node: "+ endNode.IsWalkable());
                
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
            //Update Unit's list
            Debug.Log(heroPositionsList.Count);
            /* if the hero is selected, then move it from the path calculated above */
            if (hero.GetIsSelected()) {
                hero.SetPositionsList(heroPositionsList);
            }
            //selectedHeroStartingPosition = hero.transform.position;
            //Debug.Log("Hero Transform: " + PathFinding.Instance.GetGridPosition(hero.transform.position));
            //startGridPosition = PathFinding.Instance.GetGridPosition(hero.transform.position);
        }
    }

}
