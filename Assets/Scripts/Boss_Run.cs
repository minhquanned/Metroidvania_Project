using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Run : StateMachineBehaviour
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
        TargetPlayerPosition(animator);

        if (TheHollowKnight.Instance.attackCountdown <= 0)
        {
            TheHollowKnight.Instance.AttackHandler();
            TheHollowKnight.Instance.attackCountdown =
                Random.Range(TheHollowKnight.Instance.attackTimer - 1, TheHollowKnight.Instance.attackTimer + 1);
        }

        if (!TheHollowKnight.Instance.Grounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, -25); //if knight is not grounded, fall to ground
        }
    }

    void TargetPlayerPosition(Animator animator)
    {
        if (TheHollowKnight.Instance.Grounded())
        {
            TheHollowKnight.Instance.Flip();
            Vector2 _target = new Vector2(PlayerController.Instance.transform.position.x, rb.position.y);
            Vector2 _newPos = Vector2.MoveTowards(rb.position, _target, TheHollowKnight.Instance.runSpeed * Time.fixedDeltaTime);
            // TheHollowKnight.Instance.runSpeed = TheHollowKnight.Instance.speed; // temporary speed
            rb.MovePosition(_newPos);
        }
        else
        {
            rb.velocity = new Vector2(rb.velocity.x, -25);
        }

        if (Vector2.Distance(PlayerController.Instance.transform.position, rb.position) <= TheHollowKnight.Instance.attackRange)
        {
            animator.SetBool("Running", false);
        }
        else
        {
            return;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("Running", false);
    }
}
