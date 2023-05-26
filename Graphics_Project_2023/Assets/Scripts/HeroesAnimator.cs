using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroesAnimator : MonoBehaviour {
    
    private const string IS_WALKING = "isWalking";
    private const string IS_DEAD = "isDead";
    private const string IS_SELECTED = "isSelected";
    private const string IS_HEALING = "isHealing";
    private const string IS_ATTACKING = "isAttacking";
    private const string GETS_HIT = "getsHit";
    private const string IS_BEGGING = "isBegging";
    private const string IS_CASTSPELLING = "isCastSpelling";
    
    [SerializeField] private Heroes hero;
    private Animator animator;
      
    private void Awake() {
        animator = GetComponent<Animator>();
        animator.SetBool(IS_WALKING,hero.GetIsWalking());
        animator.SetBool(IS_DEAD, hero.GetIsDead());
        animator.SetBool(IS_SELECTED, hero.GetIsSelected());
        animator.SetBool(IS_HEALING, hero.GetIsHealing());
        animator.SetBool(IS_ATTACKING, hero.GetIsAttacking());
        animator.SetBool(GETS_HIT, hero.GetGetsHit());
        animator.SetBool(IS_BEGGING, hero.GetIsBegging());
        animator.SetBool(IS_CASTSPELLING, hero.GetIsCastSpelling());
        
    }

    private void Update() {
        animator.SetBool(IS_WALKING, hero.GetIsWalking());  
        animator.SetBool(IS_DEAD, hero.GetIsDead());
        animator.SetBool(IS_SELECTED, hero.GetIsSelected());
        animator.SetBool(IS_HEALING, hero.GetIsHealing());
        animator.SetBool(IS_ATTACKING, hero.GetIsAttacking());
        animator.SetBool(GETS_HIT, hero.GetGetsHit());
        animator.SetBool(IS_BEGGING, hero.GetIsBegging());
        animator.SetBool(IS_CASTSPELLING, hero.GetIsCastSpelling());
    }


}
