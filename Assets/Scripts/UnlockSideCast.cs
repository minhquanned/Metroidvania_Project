using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockSideCast : MonoBehaviour
{
    [SerializeField] GameObject particles;
    [SerializeField] GameObject canvasUI;
    bool used;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerController.Instance.unlockedSideCast)
        {
            Destroy(gameObject);
        }
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

        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(0.5f);

        canvasUI.SetActive(true);

        yield return new WaitForSeconds(4f);
        PlayerController.Instance.unlockedSideCast = true;
        canvasUI.SetActive(false);
        Destroy(gameObject);
    }
}
