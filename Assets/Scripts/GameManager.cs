using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public string transitionedFromScene;

    public Vector2 platformingRespawnPoint;
    public Vector2 respawnPoint;
    [SerializeField] Bench bench;

    public GameObject shade;

    [SerializeField] private FadeUI pauseMenu;
    [SerializeField] private float fadeTime;
    public bool gameIsPaused;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        SaveData.Instance.Initialize();
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        if (PlayerController.Instance != null)
        {
            if (PlayerController.Instance.halfMana)
            {
                SaveData.Instance.LoadShadeData();
                if (SaveData.Instance.sceneWithShade == SceneManager.GetActiveScene().name || SaveData.Instance.sceneWithShade == "")
                {
                    Instantiate(shade, SaveData.Instance.shadePos, SaveData.Instance.shadeRot);
                }
            }
        }

        SaveScene();

        DontDestroyOnLoad(gameObject);
        bench = FindObjectOfType<Bench>();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SaveData.Instance.SavePlayerData();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && !gameIsPaused)
        {
            pauseMenu.FadeUIIn(fadeTime);
            Time.timeScale = 0f;
            gameIsPaused = true;
        }
    }

    public void UnPauseGame()
    {
        Time.timeScale = 1f;
        gameIsPaused = false;
    }

    public void SaveGame()
    {
        SaveData.Instance.SavePlayerData();
    }

    public void SaveScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SaveData.Instance.sceneNames.Add(currentSceneName);
    }

    public void RespawnPlayer()
    {
        SaveData.Instance.LoadBench();

        if (SaveData.Instance.benchSceneName != null)   // Load scene if exists.
        {
            SceneManager.LoadScene(SaveData.Instance.benchSceneName);
        }

        if (SaveData.Instance.benchPos != null) // Set the respawn point to the bench's position
        {
            respawnPoint = SaveData.Instance.benchPos;
        }
        else
        {
            respawnPoint = platformingRespawnPoint;
        }

        PlayerController.Instance.transform.position = respawnPoint;

        StartCoroutine(UIManager.Instance.DeactivateDeathScreen());
        PlayerController.Instance.Respawned();
    }
}
