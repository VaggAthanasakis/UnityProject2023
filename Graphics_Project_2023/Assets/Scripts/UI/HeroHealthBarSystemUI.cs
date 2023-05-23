using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroHealthBarSystemUI : MonoBehaviour {

    [SerializeField] private Image barImage;
    [SerializeField] private Heroes hero;

    private void Start() {
        hero.OnHealthChanged += Hero_OnHealthChanged;
        barImage.fillAmount = 1f;
        this.HideBar();
        
    }

    public void Update() {
        /* Fix the bar in order to always face the camera */
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,Camera.main.transform.rotation*Vector3.up);
    }
   

    private void Hero_OnHealthChanged(object sender, Heroes.OnHealthChangedEventArgs e) {
        barImage.fillAmount = e.healthNormalized;
        /* Check if the hero has full health or is dead
         * then hide the health bar
           else show the health bar*/
        if (e.healthNormalized == 0f || e.healthNormalized == 1f) {
            HideBar();
        }
        else {
            ShowBar();
        }
    }

    private void ShowBar() {
        gameObject.SetActive(true);
    }
    private void HideBar() {
        gameObject.SetActive(false);
    }



}
