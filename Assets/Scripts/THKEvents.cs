using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class THKEvents : MonoBehaviour
{
    void SlashDamagePlayer()
    {
        if (PlayerController.Instance.transform.position.x > transform.position.x
        || PlayerController.Instance.transform.position.x < transform.position.x)
        {
            Hit(TheHollowKnight.Instance.SideAttackTransform, TheHollowKnight.Instance.SideAttackArea);
        }
        else if (PlayerController.Instance.transform.position.y > transform.position.y)
        {
            Hit(TheHollowKnight.Instance.UpAttackTransform, TheHollowKnight.Instance.UpAttackArea);
        }
        else if (PlayerController.Instance.transform.position.y < transform.position.y)
        {
            Hit(TheHollowKnight.Instance.DownAttackTransform, TheHollowKnight.Instance.DownAttackArea);
        }
    }

    void Hit(Transform _attackTransform, Vector2 _attackArea)
    {
        Collider2D[] _objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0);
        for (int i = 0; i < _objectsToHit.Length; i++)
        {
            if (_objectsToHit[i].GetComponent<PlayerController>() != null && !PlayerController.Instance.pState.invincible)
            {
                _objectsToHit[i].GetComponent<PlayerController>().TakeDamage(TheHollowKnight.Instance.damage);
                if (PlayerController.Instance.pState.alive)
                {
                    PlayerController.Instance.HitStopTime(0, 5, 0.5f);
                }
            }
        }
    }

    void Parrying()
    {
        TheHollowKnight.Instance.parrying = true;
    }

    void BendDownCheck()
    {
        if (TheHollowKnight.Instance.barrageAttack)
        {
            StartCoroutine(BarrageAttackTransition());
        }
        if (TheHollowKnight.Instance.outbreakAttack)
        {
            StartCoroutine(OutbreakAttackTransition());
        }
        if (TheHollowKnight.Instance.bounceAttack)
        {
            TheHollowKnight.Instance.anim.SetTrigger("Bounce1");
        }
    }

    void BarrageOrOutbreak()
    {
        if (TheHollowKnight.Instance.barrageAttack)
        {
            TheHollowKnight.Instance.StartCoroutine(TheHollowKnight.Instance.Barrage());
        }

        if (TheHollowKnight.Instance.outbreakAttack)
        {
            TheHollowKnight.Instance.StartCoroutine(TheHollowKnight.Instance.OutBreak());
            Debug.Log("Called Outbreak()");
        }
    }

    IEnumerator BarrageAttackTransition()
    {
        yield return new WaitForSeconds(1f);
        TheHollowKnight.Instance.anim.SetBool("Cast", true);
    }

    IEnumerator OutbreakAttackTransition()
    {
        yield return new WaitForSeconds(1f);
        TheHollowKnight.Instance.anim.SetBool("Cast", true);
    }

    void DestroyAfterDeath()
    {
        TheHollowKnight.Instance.DestroyAfterDeath();
    }
}
