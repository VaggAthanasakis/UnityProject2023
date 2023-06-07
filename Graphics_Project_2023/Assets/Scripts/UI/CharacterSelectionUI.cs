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
    [SerializeField] TextMeshProUGUI summonerCounterText;


    private int fighterCounter = 0;
    private int rangerCounter = 0;
    private int mageCounter = 0;
    private int priestCounter = 0;
    private int musicianCounter = 0;
    private int summonerCounter = 0;
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
            if (summonerCounter > 0) {
                selectedHeroes.Add(Summoner.HERO_CLASS);
                summonerCounter--;
            }

        }
    }

    public List<string> GetSelectedCharacters() { 
        return this.selectedHeroes;
    }


    /* Start Game Button */
    public void Button_StartGame() {
        SoundManager.Instance.PlaySoundWithoutFade(SoundManager.BUTTON_PRESS);
        if (charactersCounter > 0 && charactersCounter <= 4) {
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
        SoundManager.Instance.PlaySoundWithoutFade(SoundManager.BUTTON_PRESS);
        if (fighterCounter < 4) {
            fighterCounter++;
            fighterCounterText.text = fighterCounter.ToString();
            charactersCounter++;
        }
    }
    public void Button_RemoveFighter() {
        SoundManager.Instance.PlaySoundWithoutFade(SoundManager.BUTTON_PRESS);
        if (fighterCounter > 0) {
            fighterCounter--;
            fighterCounterText.text = fighterCounter.ToString();
            charactersCounter--;
        }    
    }
    /* Add/Remove Ranger */
    public void Button_AddRanger() {
        SoundManager.Instance.PlaySoundWithoutFade(SoundManager.BUTTON_PRESS);
        if (rangerCounter < 4) {
            rangerCounter++;
            rangerCounterText.text = rangerCounter.ToString();
            charactersCounter++;
        }
    }
    public void Button_RemoveRanger() {
        SoundManager.Instance.PlaySoundWithoutFade(SoundManager.BUTTON_PRESS);
        if (rangerCounter > 0) {
            rangerCounter--;
            rangerCounterText.text = rangerCounter.ToString();
            charactersCounter--;
        }
    }
    /* Add/Remove Mage */
    public void Button_AddMage() {
        SoundManager.Instance.PlaySoundWithoutFade(SoundManager.BUTTON_PRESS);
        if (mageCounter < 4) {
            mageCounter++;
            mageCounterText.text = mageCounter.ToString();
            charactersCounter++;
        }
    }
    public void Button_RemoveMage() {
        SoundManager.Instance.PlaySoundWithoutFade(SoundManager.BUTTON_PRESS);
        if (mageCounter > 0) {
            mageCounter--;
            mageCounterText.text = mageCounter.ToString();
            charactersCounter--;
        }
    }
    /* Add/Remove Priest */
    public void Button_AddPriest() {
        SoundManager.Instance.PlaySoundWithoutFade(SoundManager.BUTTON_PRESS);
        if (priestCounter < 4) {
            priestCounter++;
            priestCounterText.text = priestCounter.ToString();
            charactersCounter++;
        }
    }
    public void Button_RemovePriest() {
        SoundManager.Instance.PlaySoundWithoutFade(SoundManager.BUTTON_PRESS);
        if (priestCounter > 0) {
            priestCounter--;
            priestCounterText.text = priestCounter.ToString();
            charactersCounter--;
        }
    }
    /* Add/Remove Musician */
    public void Button_AddMusician() {
        SoundManager.Instance.PlaySoundWithoutFade(SoundManager.BUTTON_PRESS);
        if (musicianCounter < 4) {
            musicianCounter++;
            musicianCounterText.text = musicianCounter.ToString();
            charactersCounter++;
        }
    }
    public void Button_RemoveMusician() {
        SoundManager.Instance.PlaySoundWithoutFade(SoundManager.BUTTON_PRESS);
        if (musicianCounter > 0) {
            musicianCounter--;
            musicianCounterText.text = musicianCounter.ToString();
            charactersCounter--;
        }
    }

    /* Add/Remove Summoner */
    public void Button_AddSummoner() {
        SoundManager.Instance.PlaySoundWithoutFade(SoundManager.BUTTON_PRESS);
        if (summonerCounter < 4) {
            summonerCounter++;
            summonerCounterText.text = summonerCounter.ToString();
            charactersCounter++;
        }
    }
    public void Button_RemoveSummoner() {
        SoundManager.Instance.PlaySoundWithoutFade(SoundManager.BUTTON_PRESS);
        if (summonerCounter > 0) {
            summonerCounter--;
            summonerCounterText.text = summonerCounter.ToString();
            charactersCounter--;
        }
    }

}
