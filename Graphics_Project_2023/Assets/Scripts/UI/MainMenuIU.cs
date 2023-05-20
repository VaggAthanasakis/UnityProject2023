using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuIU : MonoBehaviour {

    //private const string MAIN_GAME_SCENE = "MainGameScene";

    [SerializeField] GameObject buttonsMainMenu;

    public void Button_StartGame() {
        Debug.Log("Start Game Pushed!");
        SceneLoader.LoadScene(SceneLoader.Scene.CharacterSelectionScene);
    }

    public void Button_Credits() {
        buttonsMainMenu.SetActive(!buttonsMainMenu.activeSelf);

    }

    public void Button_Quit() {
        Application.Quit();
    }



}
