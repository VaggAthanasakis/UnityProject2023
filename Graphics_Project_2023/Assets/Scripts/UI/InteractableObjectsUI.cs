using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractableObjectsUI : MonoBehaviour {

    [SerializeField] private GameObject buttonOpen;
    [SerializeField] private InteractableObject interactable;
    [SerializeField] TextMeshProUGUI InsideObjectText;
    [SerializeField] private GameObject actionPanel;


    private string healString = "HEAL";
    private string damageString = "DAMAGE";
    public string actionInsideMysteryBox;


    private void Awake() {
        this.buttonOpen.SetActive(false);
        this.actionPanel.SetActive(false);
    }
    public void Start() {
        MouseClick.instance.OnInteractableObjectSelection += MouseClick_OnInteractableObjectSelection;
    }

    private void Update() {
        /* Fix the bar in order to always face the camera */
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
    }


    /* If we select this interactable object, then activate the open button */
    private void MouseClick_OnInteractableObjectSelection(object sender, MouseClick.OnInteractableObjectSelectionEventArgs e) {
        if (e.selectedInteractableObject == this.interactable) {
            /* If it is already open and it is a chest, return */
            if (interactable.isOpen && interactable.GetObjectType() == InteractableObject.Type.Chest) { return; }
            //this.buttonOpen.SetActive(true);
            if (this.interactable.selectedCounter > 0) { // if we click again at the chest
                Heroes heroToOpenIt = MouseClick.instance.GetSelectedHero();
                OpenChest();
            }
        }
        else {
            //this.buttonOpen.SetActive(false);
        }
    }

    private void OpenChest() {
        Heroes heroToOpenIt = MouseClick.instance.GetSelectedHero();
        if (heroToOpenIt != null) {
            heroToOpenIt.PointAtTheInteractedObject(this.interactable.gameObject);
            /* If the hero cannot interact, return */
            if (!heroToOpenIt.ObjectInteract()) { return; }
            /* else */
            this.interactable.isOpen = true;
            this.buttonOpen.SetActive(false);
            this.actionPanel.SetActive(true);
            if (interactable.mysteryBoxAction == healString) {
                this.InsideObjectText.text = healString;
            }
            else if (interactable.mysteryBoxAction == damageString) {
                this.InsideObjectText.text = damageString;

            }
            else {
                Debug.Log("STRING HAS NOT BEEN SET");
            }
        }

    }

    // is not used
    public void Button_Open() {
        Debug.Log("Button Open");
        SoundManager.Instance.PlaySoundWithoutFade(SoundManager.BUTTON_PRESS);
        Heroes heroToOpenIt = MouseClick.instance.GetSelectedHero();
        if (heroToOpenIt != null) {
            heroToOpenIt.PointAtTheInteractedObject(this.interactable.gameObject);
            /* If the hero cannot interact, return */
            if (!heroToOpenIt.ObjectInteract()) { return; }
            /* else */
            this.interactable.isOpen = true;
            this.buttonOpen.SetActive(false);
            this.actionPanel.SetActive(true);
            if (interactable.mysteryBoxAction == healString) {
                this.InsideObjectText.text = healString;
            }
            else if (interactable.mysteryBoxAction == damageString) {
                this.InsideObjectText.text = damageString;

            }
            else {
                Debug.Log("STRING HAS NOT BEEN SET");
            }
        }
    }



}
