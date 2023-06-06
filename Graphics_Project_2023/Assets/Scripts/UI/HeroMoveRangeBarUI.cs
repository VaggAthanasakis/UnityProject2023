using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroMoveRangeBarUI : MonoBehaviour
{
    [SerializeField] private Image barImage;
    [SerializeField] private Heroes hero;

    // Start is called before the first frame update
    void Start() {
        hero.OnRemainingMoveRangeChanged += Hero_OnRemainingMoveRangeChanged;
        TurnSystem.Instance.OnRoundEnded += TurnSystem_OnRoundEnded;
        barImage.fillAmount = 1f;
        this.HideBar();
    }

    private void Update() {
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
    }

    private void Hero_OnRemainingMoveRangeChanged(object sender, Heroes.OnRemainingMoveRangeChangedEventArgs e) {

        barImage.fillAmount = e.remainingSteps;
        if (this.hero.GetIsPlayersTurn()) {
            ShowBar();
        }
        else {
            HideBar();
        }
    }

    private void TurnSystem_OnRoundEnded(object sender, TurnSystem.OnRoundEndedEventArgs e) {
        barImage.fillAmount = 1f;
        HideBar();
    }

    private void ShowBar() {
        if(this != null)
            gameObject.SetActive(true);
    }
    private void HideBar() {
        if (this != null)
            gameObject.SetActive(false);
    }


}
