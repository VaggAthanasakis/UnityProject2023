using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuIU : MonoBehaviour {

    //private const string MAIN_GAME_SCENE = "MainGameScene";

    [SerializeField] GameObject buttonsMainMenu;

    private void Start() {
       // SoundManager.Instance.PlaySoundWithoutFade(SoundManager.MAIN_MENU_CHAR_SELECTION_MUSIC);

    }

    public void Button_StartGame() {
        Debug.Log("Start Game Pushed!");
        SoundManager.Instance.PlaySoundWithoutFade(SoundManager.BUTTON_PRESS);
        SceneLoader.LoadScene(SceneLoader.Scene.CharacterSelectionScene);
    }

    public void Button_Credits() {
        SoundManager.Instance.PlaySoundWithoutFade(SoundManager.BUTTON_PRESS);
        buttonsMainMenu.SetActive(!buttonsMainMenu.activeSelf);

    }

    public void Button_Quit() {
        SoundManager.Instance.PlaySoundWithoutFade(SoundManager.BUTTON_PRESS);
        Application.Quit();
    }



}
