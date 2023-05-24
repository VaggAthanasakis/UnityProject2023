using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HeroAttributesUI : MonoBehaviour {

    [SerializeField] Heroes hero;
    [SerializeField] GameObject gameObjectCanvas;
    [SerializeField] TextMeshProUGUI attributesInfo;

    public void Update() {
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
        MouseClick.instance.OnHeroPointingAction += MouseClick_OnHeroPointingAction;
    }

    private void MouseClick_OnHeroPointingAction(object sender, MouseClick.OnHeroPointingActionEventArgs e) {
        if (e.pointedHero != null && e.pointedHero == this.hero) {
            if (!this.gameObjectCanvas.activeSelf) { // if it is not already active
                this.gameObjectCanvas.SetActive(true);
                this.attributesInfo.text = e.pointedHero.attributesToString;
                Debug.Log("SHOULD HAVE OPENED");
            }

        }
        else {
            this.gameObjectCanvas.SetActive(false);
        }
    }

    private void PointedHeroVisuals() {
        if (hero.IsPointedByMouse()) { 
            
        }
    
    }




}
