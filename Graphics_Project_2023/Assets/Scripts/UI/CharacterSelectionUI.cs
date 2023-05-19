using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectionUI : MonoBehaviour {

    private void Awake() {
        
    }

    private void Start() {
        GameManager.Instance.SetCurrentState(GameManager.State.CharacterSelection);
    }

}
