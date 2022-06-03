using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{

    [Header("Components")]
    public Animator animator;
    public Transform attackPoint;
    public LayerMask enemyLayers;

    [Header("Stats")]
    public float attackRange = 0.5f;
    public int attackDamage = 40;
    public float attackRate = 1f;
    //float nextAttackTime = 0f;

    void Update()
    {
        //if(Time.time >= nextAttackTime){
            if(Input.GetKeyDown(KeyCode.J)){
                Attack();
                //nextAttackTime = Time.time + 1f / attackRange;
            }
        //}
    }

    void Attack(){
        //PLAY AN ATTACK ANIMATION
        animator.SetBool("attack", true);

        //DETECT ENEMIES IN RANGE OF ATTACK
        //create a circle and collects all objects that circle hits
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        
        //DAMAGE THEM
        foreach(Collider2D enemy in hitEnemies){
            Debug.Log("We hit " + enemy.name);
            //enemy.GetComponent<Skeleton>().TakeDamage(attackDamage);
        }
    }

    void OnDrawGizmosSelected() {
        if(attackPoint == null){
            return;
        }

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    void EndAttackAnimation(){
        animator.SetBool("attack", false);
    }
}
