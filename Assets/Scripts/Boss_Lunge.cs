using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Lunge : StateMachineBehaviour
{
    Rigidbody2D rb;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb = animator.GetComponentInParent<Rigidbody2D>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb.gravityScale = 0;
        int _dir = TheHollowKnight.Instance.facingRight ? 1 : -1;
        rb.velocity = new Vector2(_dir * (TheHollowKnight.Instance.speed * 5), 0f);

        if (Vector2.Distance(PlayerController.Instance.transform.position, rb.transform.position) < TheHollowKnight.Instance.attackRange
        && !TheHollowKnight.Instance.damagedPlayer)
        {
            PlayerController.Instance.TakeDamage(TheHollowKnight.Instance.damage);
            TheHollowKnight.Instance.damagedPlayer = true;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
