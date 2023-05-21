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
    [SerializeField] private Transform mouseIndicator;


    private Heroes selectedHero = null;
    private Heroes selectedEnemy = null;

    public Heroes GetSelectedHero() {
        return this.selectedHero;
    }

    public Heroes GetSelectedEnemy() {
        return this.selectedEnemy;
    }

    /* Hero Selection Event */
    public event EventHandler<OnHeroSelectActionEventArgs> OnHeroSelectAction; // event that occurs when a player is selected
    public class OnHeroSelectActionEventArgs : EventArgs {
        public Heroes selectedHero;
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

    }
    public static Vector3 GetPosition() { 

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, instance.planeLayerMask);
        return raycastHit.point;
    }

    private void MousePointingHero() {
        //mouseIndicator.position = GetPosition();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        /* Check Where The Mouse Is Pointing */
        // if we point to a hero
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, instance.heroLayerMask)) {
            //Debug.Log("1.Pointig on a hero");
            if (raycastHit.transform.TryGetComponent<Heroes>(out Heroes pointedHero)) {
                //Debug.Log("2.Pointig on a hero");
                OnHeroPointingAction?.Invoke(this, new OnHeroPointingActionEventArgs {
                    pointedHero = pointedHero
                });
            }
        }

    }

    private void MouseSelectHero() {
        mouseIndicator.position = GetPosition();
        //if (EventSystem.current.IsPointerOverGameObject()) return;
        if (this.selectedEnemy != null) {
            if (this.selectedEnemy.GetIsDead())
                this.selectedEnemy.SetIsSelected(false);
                this.selectedEnemy = null;
        }
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, instance.heroLayerMask)) { // if we select a hero                
                Debug.Log(raycastHit.transform);
                if (raycastHit.transform.TryGetComponent(out Heroes selectedHero)) { // if we have selected a hero
                    if (this.selectedHero != selectedHero && !selectedHero.GetIsEnemy()) {
                        this.selectedHero = selectedHero;
                        OnHeroSelectAction?.Invoke(this, new OnHeroSelectActionEventArgs {
                            selectedHero = selectedHero
                        });
                    }
                    else if(this.selectedEnemy != selectedHero && selectedHero.GetIsEnemy()) {
                        this.selectedEnemy = selectedHero;
                        OnHeroSelectAction?.Invoke(this, new OnHeroSelectActionEventArgs {
                            selectedHero = selectedEnemy
                        }) ;
                    }
                }
            }
        }
 
    }


   
 }













