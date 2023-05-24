using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestAnimator : MonoBehaviour {

    private const string IS_OPEN = "isOpen";

    [SerializeField] private InteractableObject chest;
    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
        animator.SetBool(IS_OPEN,chest.isOpen);
    }

    // Update is called once per frame
   private void Update() {
        animator.SetBool(IS_OPEN, chest.isOpen);
    }
}
