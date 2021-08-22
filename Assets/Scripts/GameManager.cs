using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [Header("Controls")]
    public GameObject Controls;
    public GameObject Controls_W;
    public GameObject Controls_A;
    public GameObject Controls_S;
    public GameObject Controls_D;
    public float removeTutorialSpeed = 1f;

    [Header("Score")]
    public List<TextMeshProUGUI> ScoreTexts;

    [Header("Spawners")]
    public List<ShipSpawner> Spawners;
    public int StartingSimultaneousShips = 1;
    public int ScoreToIncrementSimultaneousShips = 30;

    [Header("UI")]
    public GameObject GameOverScreen;

    private bool wPressed;
    private bool aPressed;
    private bool sPressed;
    private bool dPressed;

    private int currentScore;
    private int maxSimultaneousShips;

    public static GameManager _;

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        
        _ = this;
    }

    private void Start()
    {
        Controls.SetActive(true);
        maxSimultaneousShips = StartingSimultaneousShips;
    }

    private void Update()
    {
        if (!wPressed && Input.GetKeyDown(KeyCode.W))
        {
            wPressed = true;
            
            StartCoroutine(RemoveTutorialKey(Controls_W));
        }
        
        if (!aPressed && Input.GetKeyDown(KeyCode.A))
        {
            aPressed = true;
            
            StartCoroutine(RemoveTutorialKey(Controls_A));
        }
        
        if (!sPressed && Input.GetKeyDown(KeyCode.S))
        {
            sPressed = true;
            
            StartCoroutine(RemoveTutorialKey(Controls_S));
        }
        
        if (!dPressed && Input.GetKeyDown(KeyCode.D))
        {
            dPressed = true;
            
            StartCoroutine(RemoveTutorialKey(Controls_D));
        }

        foreach (var txt in ScoreTexts)
        {
            txt.text = currentScore.ToString();
        }
        
        ManageSpawners();
    }

    private IEnumerator RemoveTutorialKey(GameObject control)
    {
        var images = control.GetComponentsInChildren<Image>();
        var texts = control.GetComponentsInChildren<TextMeshProUGUI>();

        while (images[0].color.a > 0)
        {
            foreach (var img in images)
            {
                var color = img.color;
                color.a -= Time.deltaTime * removeTutorialSpeed;

                img.color = color;
            }

            foreach (var txt in texts)
            {
                var color = txt.color;
                color.a -= Time.deltaTime * removeTutorialSpeed;

                txt.color = color;
            }

            yield return null;
        }
    }

    private void ManageSpawners()
    {
        if (GetShipNumber() < maxSimultaneousShips)
        {
            var spawner = Spawners[Random.Range(0, Spawners.Count)];
            spawner.SpawnShip();
        }
    }

    private int GetShipNumber()
    {
        return FindObjectsOfType<EnemyShip>().Length;
    }

    public void AddScore(int score)
    {
        currentScore += score;

        maxSimultaneousShips = StartingSimultaneousShips + (currentScore / ScoreToIncrementSimultaneousShips);
    }

    public void ShowGameOverScreen()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        GameOverScreen.SetActive(true);
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
