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
    public static List<string> selectedCharacters = new List<string>();

    /* When we try to change scene, at first we load the loading scene for a single frame */
    public static void LoadScene(Scene targetScene) {
        SceneLoader.targetScene = targetScene;
        SceneManager.LoadScene(Scene.LoadingScene.ToString());
        Debug.Log("Target Scene: "+targetScene.ToString());
    }

    public static void LoadingSceneWait() { // this function will run at the first update of the LoadingScene
        /* After the first frame at the loading scene, we load the next one */
        if (targetScene == Scene.MainGameScene) {
            selectedCharacters = CharacterSelectionUI.Instance.GetSelectedCharacters(); // we inform the character list with the selected
                                                                                        // by the user characters
        }
        SceneManager.LoadScene(targetScene.ToString());
    }

}
