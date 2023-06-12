using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InteractableObject : MonoBehaviour {

    [SerializeField] GameObject interactableObject;
    [SerializeField] string objectType;
    [SerializeField] GameObject selectedChestVisual;

    public string mysteryBoxAction;

    private Type currentType;

    public bool isSelected = false;
    public bool isOpen = false;
    public int selectedCounter = 0;
    public enum Type { 
        Chest,
        Door,  
    }
    private void Awake() {
        SetType();
    }
    private void Start() {
        MouseClick.instance.OnInteractableObjectSelection += Instance_OnInteractableObjectSelection;
        //GameManager.Instance.SetNoWalkableAreaAtObjectInstantiation(this.gameObject);
    }

    private void SetType() {
        if (objectType.Equals("Chest")) {
            this.currentType = Type.Chest;
        }        
    }

    public Type GetObjectType() {
        return this.currentType;
    }


    private void Instance_OnInteractableObjectSelection(object sender, MouseClick.OnInteractableObjectSelectionEventArgs e) {
        if (e.selectedInteractableObject == this) {
            this.isSelected = true;
            selectedCounter++;
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
            SoundManager.Instance.PlaySoundWithoutFade(SoundManager.NO_ACTION);
            return false;
        }

        this.isOpen = true;
        int randomNumber = Random.Range(1, 101);
        /* 25% heal, 25% damage, 25% decrease attack range decrease, 25% range increase */
        if (randomNumber <= 25) { // then heal else damage
            mysteryBoxAction = "HEAL";
            heroInteracted.GetHeal(randomNumber / 2 + 1, null);
            SoundManager.Instance.PlaySoundWithoutFade(SoundManager.HEAL_MUSIC);
        }
        else if (randomNumber <= 50) {
            mysteryBoxAction = "DAMAGE";
            heroInteracted.TakeDamage(randomNumber/2 - 8, null);
            /* if this is the last hero and dies */
            if (GameManager.Instance.aliveHeroes.Count == 0) {
                GameManager.Instance.SetCurrentState(GameManager.State.GameOver);
                SoundManager.Instance.StopSoundWithoutFade(SoundManager.COMBAT_MODE_MUSIC);
                SoundManager.Instance.PlaySoundWithoutFade(SoundManager.DEFEAT_MUSIC);
                SoundManager.Instance.PlaySoundWithoutFade(SoundManager.MAIN_MENU_CHAR_SELECTION_MUSIC);
                Debug.Log("DEFEAT");
            }
            SoundManager.Instance.PlaySoundWithoutFade(SoundManager.ATTACK_MUSIC);
        }
        else if (randomNumber <= 75) {
            mysteryBoxAction = "RANGE DOWN";
            heroInteracted.SetAttackRange(heroInteracted.GetAttackRange()/2 +1); // decrease attack range
            SoundManager.Instance.PlaySoundWithoutFade(SoundManager.ATTACK_MUSIC);
        }
        else {
            mysteryBoxAction = "RANGE UP";
            heroInteracted.SetAttackRange(heroInteracted.GetAttackRange() * 2 - 2); // increase attack range
            SoundManager.Instance.PlaySoundWithoutFade(SoundManager.HEAL_MUSIC);
        }
        UI_Manager.Instance.selectedObjectPanel.SetActive(false);
        return true;
    }

    /* Info about the interactable game object */
    public string InteractableObjectToString() {
        return "Chest Contain Either A Heal Potion,a Hurmful Poison And Can Even Decrease/Increase Attack Range. Click Again To Open It...";
    }
}
