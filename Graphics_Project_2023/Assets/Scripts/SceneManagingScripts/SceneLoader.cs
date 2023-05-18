using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader {

    public enum Scene { 
        MainMenuScene,
        MainGameScene,
        LoadingScene     
    }

    private static Scene targetScene;

    /* When we try to change scene, at first we load the loading scene for a single frame */
    public static void LoadScene(Scene targetScene) {
        SceneLoader.targetScene = targetScene;
        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }

    public static void LoadingSceneWait() { // this function will run at the first update of the LoadingScene
       /* After the first frame at the loading scene, we load the next one */
        SceneManager.LoadScene(targetScene.ToString());
    }

}
