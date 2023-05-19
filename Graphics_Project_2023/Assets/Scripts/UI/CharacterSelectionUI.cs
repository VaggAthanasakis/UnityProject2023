using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterSelectionUI : MonoBehaviour {

    private void Start() {
        GameManager.Instance.SetCurrentState(GameManager.State.CharacterSelection);
    }

    [SerializeField] GameObject errorMessage;

    /**************************************************/
    [SerializeField] TextMeshProUGUI fighterCounterText;
    [SerializeField] TextMeshProUGUI rangerCounterText;
    [SerializeField] TextMeshProUGUI mageCounterText;
    [SerializeField] TextMeshProUGUI priestCounterText;

    private int fighterCounter = 0;
    private int rangerCounter = 0;
    private int mageCounter = 0;
    private int priestCounter = 0;
    private int charactersCounter = 0;

    /* Start Game Button */
    public void Button_StartGame() {
        if (charactersCounter > 0 && charactersCounter <= 3) {
            /* Then We Can Start The Game */
            this.errorMessage.SetActive(false);
            Debug.Log("Game Can Start!");
        }
        else {
            this.errorMessage.SetActive(true);
        }
    }





    /* Add/Remove Fighter */
    public void Button_AddFighter() {
        if (fighterCounter < 3) {
            fighterCounter++;
            fighterCounterText.text = fighterCounter.ToString();
            charactersCounter++;
        }
    }

    public void Button_RemoveFighter() {
        if (fighterCounter > 0) {
            fighterCounter--;
            fighterCounterText.text = fighterCounter.ToString();
            charactersCounter--;
        }
           
    }
    /* Add/Remove Ranger */
    public void Button_AddRanger() {
        if (rangerCounter < 3) {
            rangerCounter++;
            rangerCounterText.text = rangerCounter.ToString();
            charactersCounter++;

        }
    }

    public void Button_RemoveRanger() {
        if (rangerCounter > 0) {
            rangerCounter--;
            rangerCounterText.text = rangerCounter.ToString();
            charactersCounter--;
        }

    }
    /* Add/Remove Mage */
    public void Button_AddMage() {
        if (mageCounter < 3) {
            mageCounter++;
            mageCounterText.text = mageCounter.ToString();
            charactersCounter++;
        }
    }

    public void Button_RemoveMage() {
        if (mageCounter > 0) {
            mageCounter--;
            mageCounterText.text = mageCounter.ToString();
            charactersCounter--;
        }

    }
    /* Add/Remove Priest */
    public void Button_AddPriest() {
        if (priestCounter < 3) {
            priestCounter++;
            priestCounterText.text = priestCounter.ToString();
            charactersCounter++;
        }
    }

    public void Button_RemovePriest() {
        if (priestCounter > 0) {
            priestCounter--;
            priestCounterText.text = priestCounter.ToString();
            charactersCounter--;
        }

    }

}
