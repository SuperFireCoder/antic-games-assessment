using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum Turn {
        Idle, Player, Enemy
    }

    public enum UpdateType {
        PlayerScore, EnemyScore, Move
    }
    public int move = 10;
    public int playerCorrect = 0, playerWrong = 0;
    public int enemyCorrect = 0, enemyWrong = 0;
    public bool isStarted = false;
    public Turn turn = Turn.Idle;
    public Vector3 playerPos, enemyPos;

    public void StartGame() {
        move = 10;
        playerCorrect = 0; playerWrong = 0;
        enemyCorrect = 0; enemyWrong = 0;
        BallSpawner ballSpawner = FindObjectOfType<BallSpawner>();
        ballSpawner.StartGame();
        transform.Find("Player").position = playerPos;
        transform.Find("Hint").position = playerPos;
        transform.Find("Enemy").position = enemyPos;
        isStarted = true;
    }

    public void ExitGame() {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void UpdateValue(UpdateType type, int value) {
        if (type == UpdateType.PlayerScore) {
            if (value == 1) playerCorrect ++;
            else playerWrong ++;
        } else if (type == UpdateType.EnemyScore) {
            if (value == 1) enemyCorrect ++;
            else enemyWrong ++;
        } else if (type == UpdateType.Move) {
            move --;
        }
        UIManager uIManager = FindObjectOfType<UIManager>();
        uIManager.UpdateHUD();
    }

    public int PlayerScore() {
        return playerCorrect - playerWrong;
    }

    public int EnemyScore() {
        return enemyCorrect - enemyWrong;
    }

    public void EnemyMove() {
        turn = GameManager.Turn.Enemy;
        AIPlayerController aIPlayerController = FindObjectOfType<AIPlayerController>();
        aIPlayerController.StartMove();
    }
}
