using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject ballFlyingPrefab;
    private int points;
    private float gameTimer;
    [SerializeField] private float gameTimerMax;
    private bool gameOver;
    [SerializeField] private GameObject[] gameObjectsToDeleteWhenTimerEnds;
    [SerializeField] private Text pointsText, timerText;
    [SerializeField] private GameObject pressToRestart;
    private int gameTimerInt;

    public static GameManager Instance;

    public bool GameOver => gameOver;
    public void AddPoints(int pointsToAdd) => points += pointsToAdd;

    private void Awake() {
        //singleton logic
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Instantiate(ballFlyingPrefab);
        gameTimer = gameTimerMax;
    }

    void Update()
    {
        //POST-GAME LOGIC
        if (gameTimer < 0) {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Start")) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            if (!gameOver) {
                for (int i = 0; i < gameObjectsToDeleteWhenTimerEnds.Length; i++) {
                    Destroy(gameObjectsToDeleteWhenTimerEnds[i]);
                }
                pressToRestart.SetActive(true);
                gameOver = true;
            }
        }
        else gameTimer -= Time.deltaTime;
        
        //GAME TIMER LOGIC
        gameTimerInt = (int)gameTimer;
        if (gameTimerInt < 0) gameTimerInt = 0;
        
        //SET UI TEXT
        pointsText.text = points.ToString();
        timerText.text = gameTimerInt.ToString();
    }
}
