using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroActionsUI : MonoBehaviour {

    [SerializeField] private Button AttackButton;

    private void Awake() {
        AttackButton.onClick.AddListener(() => {
            /* Code to run when the attack button is pushed */
            Heroes selectedHero = MouseClick.instance.GetSelectedHero();
            Debug.Log("UI Selected: "+selectedHero.ToString());
            //selectedHero.Interact();
            //TurnSystem.Instance
            Debug.Log("Attack Button Pushed!");
        }); 
    } 
}
