using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterSelectionUI : MonoBehaviour {

    public static CharacterSelectionUI Instance { get; private set; }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } 
    }

    [SerializeField] GameObject errorMessage;
    private List<string> selectedHeroes = new List<string>();

    /**************************************************/
    [SerializeField] TextMeshProUGUI fighterCounterText;
    [SerializeField] TextMeshProUGUI rangerCounterText;
    [SerializeField] TextMeshProUGUI mageCounterText;
    [SerializeField] TextMeshProUGUI priestCounterText;
    [SerializeField] TextMeshProUGUI musicianCounterText;
   // [SerializeField] TextMeshProUGUI newCharCounterText;


    private int fighterCounter = 0;
    private int rangerCounter = 0;
    private int mageCounter = 0;
    private int priestCounter = 0;
    private int musicianCounter = 0;
    //private int newcharCounter = 0;
    private int charactersCounter = 0;
    /* The Heroes That The User Has Select */
    private void SelectedCharactersToSpawn() {
        for (int i = 0; i < charactersCounter; i++) { // we need to spawn charactersCounter heroes
            if (fighterCounter > 0) {
                selectedHeroes.Add(Fighter.HERO_CLASS);
                fighterCounter--;
            }
            if (rangerCounter > 0) {
                selectedHeroes.Add(Ranger.HERO_CLASS);
                rangerCounter--;
            }
            if (mageCounter > 0) {
                selectedHeroes.Add(Mage.HERO_CLASS);
                mageCounter--;
            }
            if (priestCounter > 0) {
                selectedHeroes.Add(Priest.HERO_CLASS);
                priestCounter--;
            }
            if (musicianCounter > 0) {
                selectedHeroes.Add(Musician.HERO_CLASS);
                musicianCounter--;
            }

        }
    }

    public List<string> GetSelectedCharacters() { 
        return this.selectedHeroes;
    }


    /* Start Game Button */
    public void Button_StartGame() {
        if (charactersCounter > 0 && charactersCounter <= 3) {
            /* Then We Can Start The Game */
            this.errorMessage.SetActive(false);
            Debug.Log("Game Can Start!");
            SelectedCharactersToSpawn();
            SceneLoader.LoadScene(SceneLoader.Scene.MainGameScene);
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
    /* Add/Remove Musician */
    public void Button_AddMusician() {
        if (musicianCounter < 3) {
            musicianCounter++;
            musicianCounterText.text = musicianCounter.ToString();
            charactersCounter++;
        }
    }

    public void Button_RemoveMusician() {
        if (musicianCounter > 0) {
            musicianCounter--;
            musicianCounterText.text = musicianCounter.ToString();
            charactersCounter--;
        }

    }

}
