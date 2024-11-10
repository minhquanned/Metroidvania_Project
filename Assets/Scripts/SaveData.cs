using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

[System.Serializable]
public struct SaveData
{
    public static SaveData Instance;

    // Map stuff
    public HashSet<string> sceneNames;

    // Bench Stuff
    public string benchSceneName;
    public Vector2 benchPos;

    // Player Stuff
    public int playerHealth;
    public int playerMaxHealth;
    public int playerHeartShards;
    public float playerMana;
    public int playerManaOrbs;
    public int playerOrbShard;
    public float playerOrb0Fill, playerOrb1Fill, playerOrb2Fill;
    public bool playerHalfMana;
    public Vector2 playerPosition;
    public string lastScene;

    public bool playerUnlockedWallJump, playerUnlockedDash, playerUnlockedVarJump;
    public bool playerUnlockedSideCast, playerUnlockedUpCast, playerUnlockedDownCast;

    // Enemy stuff
    // Shade stuff
    public Vector2 shadePos;
    public string sceneWithShade;
    public Quaternion shadeRot;

    public void Initialize()
    {
        if (!File.Exists(Application.persistentDataPath + "/save.bench.data"))
        {
            BinaryWriter writer = new BinaryWriter(File.Create(Application.persistentDataPath + "/save.bench.data"));
        }

        if (!File.Exists(Application.persistentDataPath + "/save.player.data"))
        {
            BinaryWriter writer = new BinaryWriter(File.Create(Application.persistentDataPath + "/save.player.data"));
        }

        if (!File.Exists(Application.persistentDataPath + "/save.shade.data"))
        {
            BinaryWriter writer = new BinaryWriter(File.Create(Application.persistentDataPath + "/save.shade.data"));
        }

        if (sceneNames == null)
        {
            sceneNames = new HashSet<string>();
        }
    }

    public void SaveBench()
    {
        using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/save.bench.data")))
        {
            writer.Write(benchSceneName);
            writer.Write(benchPos.x);
            writer.Write(benchPos.y);
        }
    }

    public void LoadBench()
    {
        if (File.Exists(Application.persistentDataPath + "/save.bench.data"))
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(Application.persistentDataPath + "/save.bench.data")))
            {
                benchSceneName = reader.ReadString();
                benchPos.x = reader.ReadSingle();
                benchPos.y = reader.ReadSingle();
            }
        }
    }

    public void SavePlayerData()
    {
        using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/save.player.data")))
        {
            playerHealth = PlayerController.Instance.Health;
            writer.Write(playerHealth);
            playerMaxHealth = PlayerController.Instance.maxHealth;
            writer.Write(playerMaxHealth);
            playerHeartShards = PlayerController.Instance.heartShards;
            writer.Write(playerHeartShards);

            playerMana = PlayerController.Instance.Mana;
            writer.Write(playerMana);
            playerHalfMana = PlayerController.Instance.halfMana;
            writer.Write(playerHalfMana);
            playerManaOrbs = PlayerController.Instance.manaOrbs;
            writer.Write(playerManaOrbs);
            playerOrbShard = PlayerController.Instance.orbShard;
            writer.Write(playerOrbShard);
            playerOrb0Fill = PlayerController.Instance.manaOrbsHandler.orbFills[0].fillAmount;
            writer.Write(playerOrb0Fill);
            playerOrb1Fill = PlayerController.Instance.manaOrbsHandler.orbFills[1].fillAmount;
            writer.Write(playerOrb1Fill);
            playerOrb2Fill = PlayerController.Instance.manaOrbsHandler.orbFills[2].fillAmount;
            writer.Write(playerOrb2Fill);

            playerUnlockedWallJump = PlayerController.Instance.unlockedWallJump;
            writer.Write(playerUnlockedWallJump);
            playerUnlockedDash = PlayerController.Instance.unlockedDash;
            writer.Write(playerUnlockedDash);
            playerUnlockedVarJump = false;
            writer.Write(playerUnlockedVarJump);

            playerUnlockedSideCast = PlayerController.Instance.unlockedSideCast;
            writer.Write(playerUnlockedSideCast);
            playerUnlockedUpCast = PlayerController.Instance.unlockedUpCast;
            writer.Write(playerUnlockedUpCast);
            playerUnlockedDownCast = PlayerController.Instance.unlockedDownCast;
            writer.Write(playerUnlockedDownCast);

            playerPosition = PlayerController.Instance.transform.position;
            writer.Write(playerPosition.x);
            writer.Write(playerPosition.y);

            lastScene = SceneManager.GetActiveScene().name;
            writer.Write(lastScene);
        }
    }

    public void LoadPlayerData()
    {
        if (File.Exists(Application.persistentDataPath + "/save.player.data"))
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(Application.persistentDataPath + "/save.player.data")))
            {
                playerHealth = reader.ReadInt32();
                playerMaxHealth = reader.ReadInt32();
                playerHeartShards = reader.ReadInt32();

                playerMana = reader.ReadSingle();
                playerHalfMana = reader.ReadBoolean();
                playerManaOrbs = reader.ReadInt32();
                playerOrbShard = reader.ReadInt32();
                playerOrb0Fill = reader.ReadSingle();
                playerOrb1Fill = reader.ReadSingle();
                playerOrb2Fill = reader.ReadSingle();

                playerUnlockedWallJump = reader.ReadBoolean();
                playerUnlockedDash = reader.ReadBoolean();
                playerUnlockedVarJump = reader.ReadBoolean();

                playerUnlockedSideCast = reader.ReadBoolean();
                playerUnlockedUpCast = reader.ReadBoolean();
                playerUnlockedDownCast = reader.ReadBoolean();

                playerPosition.x = reader.ReadSingle();
                playerPosition.y = reader.ReadSingle();
                lastScene = reader.ReadString();

                SceneManager.LoadScene(lastScene);
                PlayerController.Instance.transform.position = playerPosition;

                PlayerController.Instance.Health = playerHealth;
                PlayerController.Instance.maxHealth = playerMaxHealth;
                PlayerController.Instance.heartShards = playerHeartShards;

                PlayerController.Instance.Mana = playerMana;
                PlayerController.Instance.halfMana = playerHalfMana;
                PlayerController.Instance.manaOrbs = playerManaOrbs;
                PlayerController.Instance.orbShard = playerOrbShard;
                PlayerController.Instance.manaOrbsHandler.orbFills[0].fillAmount = playerOrb0Fill;
                PlayerController.Instance.manaOrbsHandler.orbFills[1].fillAmount = playerOrb1Fill;
                PlayerController.Instance.manaOrbsHandler.orbFills[2].fillAmount = playerOrb2Fill;

                PlayerController.Instance.unlockedWallJump = playerUnlockedWallJump;
                PlayerController.Instance.unlockedDash = playerUnlockedDash;
                PlayerController.Instance.unlockedVarJump = playerUnlockedVarJump;

                PlayerController.Instance.unlockedSideCast = playerUnlockedSideCast;
                PlayerController.Instance.unlockedUpCast = playerUnlockedUpCast;
                PlayerController.Instance.unlockedDownCast = playerUnlockedDownCast;
            }
            Debug.Log("Load Player Data");
        }
        else
        {
            Debug.Log("File doesn't exist");
            PlayerController.Instance.halfMana = false;
            PlayerController.Instance.Health = PlayerController.Instance.maxHealth;
            PlayerController.Instance.Mana = 0.5f;
            PlayerController.Instance.heartShards = 0;

            PlayerController.Instance.unlockedWallJump = false;
            PlayerController.Instance.unlockedDash = false;
            PlayerController.Instance.unlockedVarJump = false;

            PlayerController.Instance.unlockedSideCast = false;
            PlayerController.Instance.unlockedUpCast = false;
            PlayerController.Instance.unlockedDownCast = false;
        }
    }

    public void StartWithNewPlayerData()
    {
        if (File.Exists(Application.persistentDataPath + "/save.player.data"))
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(Application.persistentDataPath + "/save.player.data")))
            {
                playerHealth = PlayerController.Instance.health;
                playerMaxHealth = PlayerController.Instance.maxHealth;
                playerHeartShards = 0;

                playerMana = PlayerController.Instance.Mana;
                playerHalfMana = false;
                playerManaOrbs = 0;
                playerOrbShard = 0;
                playerOrb0Fill = 0;
                playerOrb1Fill = 0;
                playerOrb2Fill = 0;

                playerUnlockedWallJump = false;
                playerUnlockedDash = false;
                playerUnlockedVarJump = false;

                playerUnlockedSideCast = false;
                playerUnlockedUpCast = false;
                playerUnlockedDownCast = false;

                playerPosition.x = PlayerController.Instance.transform.position.x;
                playerPosition.y = PlayerController.Instance.transform.position.y;
                lastScene = SceneManager.GetActiveScene().name;

                SceneManager.LoadScene(lastScene);
                PlayerController.Instance.transform.position = playerPosition;

                PlayerController.Instance.Health = playerHealth;
                PlayerController.Instance.maxHealth = playerMaxHealth;
                PlayerController.Instance.heartShards = playerHeartShards;

                PlayerController.Instance.Mana = playerMana;
                PlayerController.Instance.halfMana = playerHalfMana;
                PlayerController.Instance.manaOrbs = playerManaOrbs;
                PlayerController.Instance.orbShard = playerOrbShard;
                PlayerController.Instance.manaOrbsHandler.orbFills[0].fillAmount = playerOrb0Fill;
                PlayerController.Instance.manaOrbsHandler.orbFills[1].fillAmount = playerOrb1Fill;
                PlayerController.Instance.manaOrbsHandler.orbFills[2].fillAmount = playerOrb2Fill;

                PlayerController.Instance.unlockedWallJump = playerUnlockedWallJump;
                PlayerController.Instance.unlockedDash = playerUnlockedDash;
                PlayerController.Instance.unlockedVarJump = playerUnlockedVarJump;

                PlayerController.Instance.unlockedSideCast = playerUnlockedSideCast;
                PlayerController.Instance.unlockedUpCast = playerUnlockedUpCast;
                PlayerController.Instance.unlockedDownCast = playerUnlockedDownCast;
            }
            Debug.Log("Reset Player Data");
        }
    }

    public void SaveShadeData()
    {
        using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/save.shade.data")))
        {
            sceneWithShade = SceneManager.GetActiveScene().name;
            shadePos = Shade.Instance.transform.position;
            shadeRot = Shade.Instance.transform.rotation;

            writer.Write(sceneWithShade);

            writer.Write(shadePos.x);
            writer.Write(shadePos.y);

            writer.Write(shadeRot.x);
            writer.Write(shadeRot.y);
            writer.Write(shadeRot.z);
            writer.Write(shadeRot.w);
        }
    }

    public void LoadShadeData()
    {
        if (File.Exists(Application.persistentDataPath + "/save.shade.data"))
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(Application.persistentDataPath + "/save.shade.data")))
            {
                sceneWithShade = reader.ReadString();
                shadePos.x = reader.ReadSingle();
                shadePos.y = reader.ReadSingle();

                float rotationX = reader.ReadSingle();
                float rotationY = reader.ReadSingle();
                float rotationZ = reader.ReadSingle();
                float rotationW = reader.ReadSingle();
                shadeRot = new Quaternion(rotationX, rotationY, rotationZ, rotationW);
            }
        }
        else
        {
            Debug.Log("Shade doesn't exist");
        }
    }
}
