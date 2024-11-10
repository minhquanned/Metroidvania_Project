using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] GameObject[] maps;

    Bench bench;

    // Start is called before the first frame update
    void OnEnable()
    {
        bench = FindObjectOfType<Bench>();
        if (bench != null)
        {
            if (bench.interacted)
            {
                UpdateMap();
                // Debug.Log("Map updated");
            }
        }
    }

    void UpdateMap()
    {
        var savedScenes = SaveData.Instance.sceneNames;
        for (int i = 0; i < maps.Length; i++)
        {
            if (savedScenes.Contains("Cave_" + (i + 1)))
            {
                maps[i].SetActive(true);
                // Debug.Log("Cave_" + (i + 1) + " true");
            }
            else
            {
                maps[i].SetActive(false);
            }
        }
    }
}
