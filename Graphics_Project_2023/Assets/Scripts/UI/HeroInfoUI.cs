using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HeroInfoUI : MonoBehaviour {

    [SerializeField] private Transform container;
    [SerializeField] private Transform heroTemplate;
    [SerializeField] private TextMeshProUGUI heroInfo;
    private Heroes selectedHero = null;

    private void Awake() {
        /* At the start we want the template not to be visible */
        heroTemplate.gameObject.SetActive(false);

    }

    private void Start() {
        MouseClick.instance.OnHeroSelectAction += Instance_OnHeroSelectAction;
        UpdateVisual();
    }

    private void Instance_OnHeroSelectAction(object sender, MouseClick.OnHeroSelectActionEventArgs e) {
        //Debug.Log("Event Arrived");
        selectedHero = e.selectedHero;
        //UpdateVisual();
        UpdateVisual();
    }

    private void UpdateVisual() {
        //Debug.Log("Inside update visual");
        /* Destroy everything inside the container except the template */
        foreach (Transform child in container) {
            if (child == heroTemplate)
                continue;
            Destroy(child.gameObject);
        
        }
        
        if (selectedHero != null) {
            heroInfo.text = selectedHero.HeroStatisticsToString();
            Transform heroTransform = Instantiate(heroTemplate, container);
            heroTransform.gameObject.SetActive(true);
            //Debug.Log("SelectedHero Name From UpdateVisual: "+selectedHero.ToString());
        } 
    }




}
