using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private GameObject mainMenu, gameoverMenu, gameHUD, setColorMenu;
    private TextMeshProUGUI labelPlayerScore;
    private TextMeshProUGUI labelEnemyScore;
    private TextMeshProUGUI labelMove;
    private TextMeshProUGUI labelResult;
    private TextMeshProUGUI labelColorCnt;
    private GameManager gameManager;
    private PlayerController playerController;
    private BallSpawner ballSpawner;
    private Slider sliderColorCnt;
    
    // Start is called before the first frame update
    void Start() {
        mainMenu = GameObject.Find("Main_Menu");
        gameoverMenu = GameObject.Find("Gameover_Menu");
        gameHUD = GameObject.Find("Game_HUD");
        setColorMenu = GameObject.Find("Setcolor_Menu");
        labelPlayerScore = GameObject.Find("Label_Score_Player").GetComponent<TextMeshProUGUI>();
        labelEnemyScore = GameObject.Find("Label_Score_Enemy").GetComponent<TextMeshProUGUI>();
        labelMove = GameObject.Find("Label_Move").GetComponent<TextMeshProUGUI>();
        labelResult = GameObject.Find("Label_Result").GetComponent<TextMeshProUGUI>();
        labelColorCnt = GameObject.Find("Label_ColorCnt").GetComponent<TextMeshProUGUI>();
        sliderColorCnt = GameObject.Find("Slider_ColorCnt").GetComponent<Slider>();
        gameoverMenu?.SetActive(false);
        gameHUD?.SetActive(false);
        setColorMenu?.SetActive(false);
        gameManager = FindObjectOfType<GameManager>();
        ballSpawner = FindObjectOfType<BallSpawner>();
        playerController = FindObjectOfType<PlayerController>();
    }

    public void StartGame() {
        mainMenu?.SetActive(false);
        gameHUD?.SetActive(true);
        gameManager?.StartGame();
        playerController.isPlay = true;
    }

    public void UpdateHUD() {
        labelPlayerScore.SetText($"Player Score: {gameManager.PlayerScore()}");
        labelEnemyScore.SetText($"Enemy Score: {gameManager.EnemyScore()}");
        labelMove.SetText($"Move: {gameManager.move}");
        if(gameManager.move == 0) {
            gameHUD?.SetActive(false);
            gameoverMenu?.SetActive(true);
            playerController.isPlay = false;
            labelResult.SetText($"No move moves\n\nYou earned {gameManager.PlayerScore()} points in {10 - gameManager.move} turns");
        } else if(gameManager.playerCorrect == ballSpawner.sameColorCnt) {
            gameHUD?.SetActive(false);
            gameoverMenu?.SetActive(true);
            playerController.isPlay = false;
            labelResult.SetText($"Victory\n\nYou earned {gameManager.PlayerScore()} points in 10 turns");
        }
    }

    public void SetColor() {
        mainMenu?.SetActive(false);
        setColorMenu?.SetActive(true);
    }

    public void SetColorCount() {
        ballSpawner.SetColorCount((int)sliderColorCnt.value);
        labelColorCnt.SetText($"Color Count: {(int)sliderColorCnt.value}");
    }

    public void Return() {
        mainMenu?.SetActive(true);
        setColorMenu?.SetActive(false);
    }

    public void Restart() {
        gameoverMenu?.SetActive(false);
        mainMenu?.SetActive(true);
    }
}
