using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InteractableObject : MonoBehaviour {

    [SerializeField] GameObject interactableObject;
    [SerializeField] string objectType;
    [SerializeField] GameObject objectInsideChest;
    [SerializeField] GameObject selectedChestVisual;

    public string mysteryBoxAction;

    private Type currentType;

    public bool isSelected = false;
    public bool isOpen = false;
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

    private void SelectedObjectVisual() {
        selectedChestVisual.SetActive(isSelected);
   
    }
    /* This function is called in order to open the mystery box
     * if we can open it, return true else false */
    public bool ChestOpen(Heroes heroInteracted) {
        int interactableDistance = 30;
        int distance =  PathFinding.Instance.CalculateSimpleDistance(this.interactableObject.transform.position,heroInteracted.transform.position);
      
        Debug.Log("Distance "+distance);
        /* Check if the hero is far away */
        if (distance > interactableDistance) {
            Debug.Log("Hero Far Away");
            return false;
        }

        this.isOpen = true;
        int randomNumber = Random.Range(1, 11);
        /* 60% heal, 40% damage */
        if (randomNumber < 7) { // then heal else damage
            mysteryBoxAction = "HEAL";
            heroInteracted.GetHeal(randomNumber,null);
        }
        else {
            mysteryBoxAction = "DAMAGE";
            heroInteracted.TakeDamage(randomNumber-2,null);
        }
        UI_Manager.Instance.selectedObjectPanel.SetActive(false);
        return true;
    }

    /* Info about the interactable game object */
    public string InteractableObjectToString() {
        return "This Box Contain Either A Heal Potion Or Poison That Will Hurm Your Hero! Open it Wisely";
    }
}
