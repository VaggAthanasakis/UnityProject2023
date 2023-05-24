using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour {

    [SerializeField] GameObject interactableObject;
    [SerializeField] string objectType;
    private Type currentType;
    [SerializeField] GameObject objectInsideChest;
    [SerializeField] GameObject selectedChestVisual;

    public bool isSelected = false;
    public enum Type { 
        Chest,
        Door,  
    }
    private void Awake() {
        SetType();
    }
    private void Start() {
        MouseClick.instance.OnInteractableObjectSelection += Instance_OnInteractableObjectSelection;
        GameManager.Instance.SetNoWalkableAreaAtObjectInstantiation(this.gameObject);
    }

    private void SetType() {
        if (objectType.Equals("Chest")) {
            this.currentType = Type.Chest;
        }
        else if (objectType.Equals("Door")) {
            this.currentType = Type.Door;
            this.objectInsideChest = null;
        }
        
    }

    public Type GetObjectType() {
        return this.currentType;
    }


    private void Instance_OnInteractableObjectSelection(object sender, MouseClick.OnInteractableObjectSelectionEventArgs e) {
        if (e.selectedInteractableObject == this) {
            this.isSelected = true;
            SelectedObjectVisual();
        }
        else {
            this.isSelected = false;
            SelectedObjectVisual();
        }
    }

    /* This method is called when a character interacts with the object */
    public void Interact() {
        /* If the object is a Chest */
        if (currentType == Type.Chest) {

        }
        /* If the object is a Door */
        else if (currentType == Type.Door) { 
        
        
        }
    }

    private void SelectedObjectVisual() {
        selectedChestVisual.SetActive(isSelected);
    }


}
