using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddManaOrb : MonoBehaviour
{
    [SerializeField] GameObject particles;
    [SerializeField] GameObject canvasUI;

    [SerializeField] OrbShard orbShard;

    bool used;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerController.Instance.manaOrbs >= 3)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !used)
        {
            used = true;

            StartCoroutine(ShowUI());
        }
    }

    IEnumerator ShowUI()
    {
        GameObject _particles = Instantiate(particles, transform.position, Quaternion.identity);
        Destroy(_particles, 0.5f);
        yield return new WaitForSeconds(0.5f);

        canvasUI.SetActive(true);
        orbShard.initialFillAmount = PlayerController.Instance.orbShard * 0.34f;
        PlayerController.Instance.orbShard++;
        orbShard.targetFillAmount = PlayerController.Instance.orbShard * 0.34f;

        StartCoroutine(orbShard.LerpFill());

        yield return new WaitForSeconds(2.5f);
        PlayerController.Instance.unlockedWallJump = true;
        canvasUI.SetActive(false);
        Destroy(gameObject);
    }
}
