using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScriptHandler : MonoBehaviour
{
    public bool isGamePaused = false;
    public bool isGameOver = false;
    public GameObject pauseMenuUI;
    public GameObject gameOverUI;
    public GameObject player;
    public EnergyBar energyBar;
    public Camera mainCamera;

    Vector2 respawnPosition;

    [Header("Inimigos Oceano")]
    [SerializeField] float spawnRaiaTimerMax;
    private float spawnRaiaTimer;
    [SerializeField] float spawnCrabTimerMax;
    private float spawnCrabTimer;
    [SerializeField] float spawnOctopusTimerMax;
    private float spawnOctopusTimer;
    [SerializeField] float raiaInicialChance = 0.20f;
    private float raiaCurrentChance;
    [SerializeField] float crabInicialChance = 0.20f;
    private float crabCurrentChance;
    [SerializeField] float octopusInicialChance = 0.20f;
    private float octopusCurrentChance;
    [SerializeField] float increaseChanceByFail = 0.10f;

    public GameObject raiaPrefab;
    public GameObject crabPrefab;
    public GameObject octopusPrefab;
    //public GameObject weakspotPrefab;

    private int gameState;

    

    void Start()
    {
        //Coloquei porque por algum motivo ele não volta ao normal na primeira vez reiniciando e depois funciona normal, estranho...
        Time.timeScale = 1f;

        switch (SceneManager.GetActiveScene().name)
        {
            case "Menu":
                gameState = 0;
                break;

            case "Floresta":
                gameState = 1;
                break;

            case "Oceano":
                gameState = 2;
                spawnRaiaTimer = spawnRaiaTimerMax;
                raiaCurrentChance = raiaInicialChance;
                spawnCrabTimer = spawnCrabTimerMax + 1.5f;
                crabCurrentChance = crabInicialChance;
                spawnOctopusTimer = spawnOctopusTimerMax + 3f;
                octopusCurrentChance = octopusInicialChance;
                break;

            case "Cidade":
                gameState = 3;
                break;
        }
    }

    void Update()
    {
        //Debug.Log(respawnPosition);
        if(gameState == 2)
        {
            if(spawnRaiaTimer <= 0)
            {
                spawnRaiaTimer = spawnRaiaTimerMax;

                if(Random.Range(0f, 1f) <= raiaCurrentChance)
                {
                    spawnRaiaTimer = spawnRaiaTimer + 5;
                    GameObject newRaia = Instantiate(raiaPrefab, new Vector3(mainCamera.transform.position.x + 13.17f, mainCamera.transform.position.y + 0.23f, 0), Quaternion.identity);
                    raiaCurrentChance = raiaInicialChance;
                }
                else
                {
                    raiaCurrentChance = raiaCurrentChance + increaseChanceByFail;
                }
            }
            if (spawnCrabTimer <= 0)
            {
                spawnCrabTimer = spawnCrabTimerMax;

                if (Random.Range(0f, 1f) <= crabCurrentChance)
                {
                    spawnCrabTimer = spawnCrabTimer + 5;
                    GameObject newCrab = Instantiate(crabPrefab, new Vector3(mainCamera.transform.position.x + 13.02f, mainCamera.transform.position.y - 7.3f, 0), Quaternion.identity);
                    crabCurrentChance = crabInicialChance;
                }
                else
                {
                    crabCurrentChance = crabCurrentChance + increaseChanceByFail;
                }
            }
            if (spawnOctopusTimer <= 0)
            {
                spawnOctopusTimer = spawnOctopusTimerMax;

                if (Random.Range(0f, 1f) <= octopusCurrentChance)
                {
                    spawnOctopusTimer = spawnOctopusTimer + 5;
                    GameObject newOctopus = Instantiate(octopusPrefab, new Vector3(mainCamera.transform.position.x + 13.49f, mainCamera.transform.position.y - 3.14f, 0), Quaternion.identity);
                    octopusCurrentChance = octopusInicialChance;
                }
                else
                {
                    octopusCurrentChance = octopusCurrentChance + increaseChanceByFail;
                }
            }

            spawnRaiaTimer = spawnRaiaTimer - Time.deltaTime;
            spawnCrabTimer = spawnCrabTimer - Time.deltaTime;
            spawnOctopusTimer = spawnOctopusTimer - Time.deltaTime;
        }
        //Debug.Log(spawnTimer);

        //Pausa o jogo
        if(Input.GetKeyDown(KeyCode.Escape) && isGameOver == false)
        {
            if(isGamePaused)
            {
                pauseMenuUI.SetActive(false);
                Resume();
            }
            else
            {
                pauseMenuUI.SetActive(true);
                Pause();
            }
        }

        //Reinicia o jogo caso em Game Over
        else if(Input.GetKeyDown(KeyCode.R) && isGameOver == true)
        {
            //Coloquei porque o novo codigo quebrou o Game Over da fase do Oceano, precisa ser arrumado depois
            //if(SceneManager.GetActiveScene().name == "Oceano")
            //{
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            //}

            //player.transform.position = respawnPosition;
            //energyBar.SetMaxEnergy();
            //isGameOver = false;
            //Time.timeScale = 1f;
            //gameOverUI.SetActive(false);
        }
    }

    //Resume o jogo completamente
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isGamePaused = false;
    }

    //Pausa o jogo completamente
    public void Pause()
    {
        Time.timeScale = 0f;
        isGamePaused = true;
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void GameOver()
    {
        isGameOver = true;
        gameOverUI.SetActive(true);
        Pause();
    }


    public void CheckpointHit(Vector2 spawnPosition)
    {
        respawnPosition = spawnPosition;
    }

    public int GetGameState()
    {
        return gameState;
    }
}
