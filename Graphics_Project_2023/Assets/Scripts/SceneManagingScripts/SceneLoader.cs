using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader {

    public enum Scene { 
        MainMenuScene,
        LoadingScene,
        CharacterSelectionScene,
        MainGameScene,
    }

    private static Scene targetScene;

    /* When we try to change scene, at first we load the loading scene for a single frame */
    public static void LoadScene(Scene targetScene) {
        SceneLoader.targetScene = targetScene;
        SceneManager.LoadScene(Scene.LoadingScene.ToString());
        Debug.Log("Target Scene: "+targetScene.ToString());
    }

    public static void LoadingSceneWait() { // this function will run at the first update of the LoadingScene
       /* After the first frame at the loading scene, we load the next one */
        SceneManager.LoadScene(targetScene.ToString());
        Debug.Log("Exiting Switch");
        switch (SceneLoader.targetScene) {
            case Scene.MainMenuScene:
                GameManager.Instance.SetCurrentState(GameManager.State.MainMenu);
                Debug.Log("State From Load: "+GameManager.Instance.GetCurrentState());
                break;
            case Scene.CharacterSelectionScene:
                GameManager.Instance.SetCurrentState(GameManager.State.CharacterSelection);
                break;
            case Scene.MainGameScene:
                GameManager.Instance.SetCurrentState(GameManager.State.FreeRoam);
                break;
         

        }
    }

}
