using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingSceneWait : MonoBehaviour
{

    /* Our Goal Is To Wait For The First Update And Then To Load The Next Scene */

    private bool isFirstUpdate = true;

    private void Update() {
        if (isFirstUpdate) { // If we are at the first update then make the variable false
            this.isFirstUpdate = false;
            SceneLoader.LoadingSceneWait();
            
        }
 
    }


}
