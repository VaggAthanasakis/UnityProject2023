using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HeroAttributesUI : MonoBehaviour {

    [SerializeField] Heroes hero;
    [SerializeField] GameObject gameObjectCanvas;
    [SerializeField] TextMeshProUGUI attributesInfo;

    private void Start() {
        MouseClick.instance.OnHeroPointingAction += MouseClick_OnHeroPointingAction;
        this.gameObjectCanvas.SetActive(false);
    }

    private void Update() {
        this.gameObjectCanvas.transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
        if (hero.IsPointedByMouse()) {
            this.gameObjectCanvas.SetActive(true);
            this.attributesInfo.text = hero.attributesToString;
        }
        else {
            this.gameObjectCanvas.SetActive(false);
        }
    }

    /* Event that arrives when the hero is pointed */
    private void MouseClick_OnHeroPointingAction(object sender, MouseClick.OnHeroPointingActionEventArgs e) {

        if (this == null) {
            return;
        }
        else if (e.pointedHero != null && e.pointedHero == this.hero) {
            if (!this.gameObjectCanvas.activeSelf) { // if it is not already active     
                this.gameObjectCanvas.SetActive(true);
                this.attributesInfo.text = e.pointedHero.attributesToString;
                //Debug.Log("SHOULD HAVE OPENED");
            }
        }
        else if (e.pointedHero != this || !e.pointedHero.GetIsDead()) {
            this.gameObjectCanvas.SetActive(false);
        }
    }




}
