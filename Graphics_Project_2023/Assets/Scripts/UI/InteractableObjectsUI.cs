using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObjectsUI : MonoBehaviour {

    [SerializeField] private GameObject buttonOpen;
    [SerializeField] private InteractableObject interactable;

    private void Awake() {
        this.buttonOpen.SetActive(false);
    }
    public void Start() {
        MouseClick.instance.OnInteractableObjectSelection += MouseClick_OnInteractableObjectSelection;
    }
    /* If we select this interactable object, then activate the open button */
    private void MouseClick_OnInteractableObjectSelection(object sender, MouseClick.OnInteractableObjectSelectionEventArgs e) {
        if (e.selectedInteractableObject == this.interactable) {
            this.buttonOpen.SetActive(true);
        }
        else {
            this.buttonOpen.SetActive(false);
        }
    }

    public void Button_Open() {
        Debug.Log("Button Open");
        Heroes heroToOpenIt = MouseClick.instance.GetSelectedHero();
        if (heroToOpenIt != null) {
            heroToOpenIt.ObjectInteract();
        }
    }



}
