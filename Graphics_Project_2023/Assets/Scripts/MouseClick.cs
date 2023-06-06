using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class MouseClick : MonoBehaviour {
    public static MouseClick instance { get; private set; }

    [SerializeField] private LayerMask planeLayerMask;
    [SerializeField] private LayerMask heroLayerMask;
    [SerializeField] private LayerMask interactableObjectsLayerMask;
    [SerializeField] private Transform mouseIndicator;


    private Heroes selectedHero = null;
    private Heroes selectedEnemy = null;
    private Heroes pointedHero = null;
    private InteractableObject selectedObject = null;

    public Heroes GetSelectedHero() {
        return this.selectedHero;
    }

    public void SetSelectedHero(Heroes hero) {
        this.selectedHero = hero;
    }

    public Heroes GetSelectedEnemy() {
        return this.selectedEnemy;
    }
    public InteractableObject GetSelectedInteractableObject() {
        return this.selectedObject;
    }

    /* Hero Selection Event */
    public event EventHandler<OnHeroSelectActionEventArgs> OnHeroSelectAction; // event that occurs when a player is selected
    public class OnHeroSelectActionEventArgs : EventArgs {
        public Heroes selectedHero;
    }

    /* Object Selection Event */
    public event EventHandler<OnInteractableObjectSelectionEventArgs> OnInteractableObjectSelection;
    public class OnInteractableObjectSelectionEventArgs : EventArgs {
        public InteractableObject selectedInteractableObject;
    }

    /* Object Pointed Event */
    public event EventHandler<OnHeroPointingActionEventArgs> OnHeroPointingAction;
    public class OnHeroPointingActionEventArgs : EventArgs {
        public Heroes pointedHero;
    }

    /*****************************************************************************/
    private void Awake() {
        instance = this;
    }

    private void Update() {
        MouseSelectHero();
        MousePointingHero();
        SelectInteractableObject();

    }
    public static Vector3 GetPosition() {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, instance.planeLayerMask);
        return raycastHit.point;
    }

    /* Check if we point at hero */
    private void MousePointingHero() {
        //mouseIndicator.position = GetPosition();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        /* Check Where The Mouse Is Pointing */
        // if we point to a hero
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, instance.heroLayerMask)) {
            if (raycastHit.transform.TryGetComponent<Heroes>(out Heroes pointedHero)) {
                this.pointedHero = pointedHero;
                this.pointedHero.SetIsPointedByMouse(true);
                OnHeroPointingAction?.Invoke(this, new OnHeroPointingActionEventArgs {
                    pointedHero = pointedHero
                });
            }
        }
        else {
            this.pointedHero = null;
            OnHeroPointingAction?.Invoke(this, new OnHeroPointingActionEventArgs {
                pointedHero = null
            });
        }
    }

    /* Check if we select a hero */
    private void MouseSelectHero() {
        mouseIndicator.position = GetPosition();
        /* Check if either the enemy or the hero is dead and if so make them null */
        if (this.selectedEnemy != null) {
            if (this.selectedEnemy.GetIsDead()) {
                Debug.Log("IS DEAD");
                this.selectedEnemy.SetIsSelected(false);
                this.selectedEnemy = null;
            }
        }
        if (this.selectedHero != null) {
            if (this.selectedHero.GetIsDead()) {
                Debug.Log("IS DEAD");
                this.selectedHero.SetIsSelected(false);
                this.selectedHero = null;
            }
        }
        /* Check if we select a hero/enemy */
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, instance.heroLayerMask)) { // if we select a hero                
                if (raycastHit.transform.TryGetComponent(out Heroes selected)) {                // if we have selected a hero
                    if (!selected.GetIsEnemy()) {
                        this.selectedHero = selected;
                        Debug.Log("Selected Hero: " + selected);
                        OnHeroSelectAction?.Invoke(this, new OnHeroSelectActionEventArgs {
                            selectedHero = this.selectedHero
                        });
                    }
                    else if (selected.GetIsEnemy()) {
                        this.selectedEnemy = selected;
                        Debug.Log("Selected Enemy: " + selected);
                        OnHeroSelectAction?.Invoke(this, new OnHeroSelectActionEventArgs {
                            selectedHero = this.selectedEnemy
                        });
                    }
                }
            }
        }
        //Debug.Log("SELECTED ENEMY: "+this.GetSelectedEnemy());
    }

    private void SelectInteractableObject() {
        mouseIndicator.position = GetPosition();
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, instance.interactableObjectsLayerMask)) { // if we select a hero                
                Debug.Log(raycastHit.transform);
                if (raycastHit.transform.TryGetComponent(out InteractableObject selectedObject)) {
                    this.selectedObject = selectedObject;
                    OnInteractableObjectSelection?.Invoke(this, new OnInteractableObjectSelectionEventArgs {
                        selectedInteractableObject = selectedObject
                    });
                }
                else {
                    //this.selectedObject = null;
                }
            }


        }
    }


}










