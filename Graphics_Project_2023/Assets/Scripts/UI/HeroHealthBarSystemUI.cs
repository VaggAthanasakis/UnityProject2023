using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroHealthBarSystemUI : MonoBehaviour {

    [SerializeField] private Image HeroBarImage;
    [SerializeField] private Image EnemyBarImage;
    [SerializeField] private Heroes hero;

    private void Start() {
        hero.OnHealthChanged += Hero_OnHealthChanged;
        if (!hero.GetIsEnemy()) {
            HeroBarImage.fillAmount = 1f;
            EnemyBarImage.fillAmount = 0f;
        }
        else {
            EnemyBarImage.fillAmount = 1f;
            HeroBarImage.fillAmount = 0f;
        }
        
        this.HideBar();
        
    }

    public void Update() {
        /* Fix the bar in order to always face the camera */
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,Camera.main.transform.rotation*Vector3.up);
    }
   

    private void Hero_OnHealthChanged(object sender, Heroes.OnHealthChangedEventArgs e) {
        if (!hero.GetIsEnemy()) { // hero
            HeroBarImage.fillAmount = e.healthNormalized;
            EnemyBarImage.fillAmount = 0f;
        }
        else { // enemy
            EnemyBarImage.fillAmount = e.healthNormalized;
            HeroBarImage.fillAmount = 0f;
        }

        //HeroBarImage.fillAmount = e.healthNormalized;
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
