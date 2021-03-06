﻿using System.Collections;
using System.Collections.Generic;
using EventTools.Event;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class InGameManager : MonoBehaviour
{
    
    public static InGameManager Instance;
    
    [HideInInspector]
    public ScoreManager scoreManager;

    [HideInInspector]
    public NodeCreator nodeCreator;

    [HideInInspector]
    public CenterEffectorController centerEffectorController;

    [HideInInspector]
    public NodeInteractionController nodeInteractionController;

    [HideInInspector]
    public Metronome metronome;
    
    [SerializeField]
    private GameObject pauseView;
    
    [Header("Events")]
    [SerializeField]
    private UniEvent<Color,Color,float> backgroundColorChangeEvent;
    
    
    private void Awake(){
        if(Instance is null){
            Instance = this;
        }

        scoreManager = gameObject.GetComponent<ScoreManager>();
        nodeCreator = gameObject.GetComponent<NodeCreator>();
        metronome = gameObject.GetComponent<Metronome>();
        centerEffectorController = gameObject.GetComponent<CenterEffectorController>();
        nodeInteractionController = gameObject.GetComponent<NodeInteractionController>();
    }

    private void Start(){
        GameManager.Instance.touchManager.touchType = TouchType.InGame;
        GameManager.Instance.gameResult = new GameResult();
        GameManager.Instance.CurrentSceneType = SceneType.INGAME;
    }

    public void ChangeBackgroundColor(Color topColor, Color bottomColor, float duration) {
        backgroundColorChangeEvent.Invoke(topColor,bottomColor,duration);
    }    

    public void GameEnd() {
        Instance = null;

        Time.timeScale = 1.0f;
        
        GameManager.Instance.gameResult.Score = scoreManager.TotalScore;
        GameManager.Instance.PlayerSetting.AchieveRequireData.AddValueToRequire("Score", scoreManager.TotalScore);
        GameManager.Instance.PlayerSetting.totalScore += (ulong) scoreManager.TotalScore;

        var judgeCounts = GameManager.Instance.gameResult.JudgeCounts;
        var accuracy = (judgeCounts[4] * 1.0f)
                       + (judgeCounts[3] * 0.8f)
                       + (judgeCounts[2] * 0.4f)
                       + (judgeCounts[0] * 0.0f);

        accuracy /= GameManager.Instance.gameResult.NodeTotalCount;
        accuracy *= 100.0f;

        GameManager.Instance.gameResult.Accuracy = accuracy;

        var rank = "";
        
        switch (accuracy) {
            case var v when accuracy == 100.0f:
                rank = "SSS";
                GameManager.Instance.PlayerSetting.perfectPlay++;
                break;
            case var v when accuracy > 95.0f:
                rank = "SS";
                break;
            case var v when accuracy > 90.0f:
                rank = "S";
                break;
            case var v when accuracy > 80.0f:
                rank = "A";
                break;
            case var v when accuracy > 70.0f:
                rank = "B";
                break;
            case var v when accuracy > 60.0f:
                rank = "C";
                break;
            case var v when accuracy > 50.0f:
                rank = "D";
                break;
            case var v when accuracy > 40.0f:
                rank = "E";
                break;
            default:
                rank = "F";
                break;
        }

        var difficulty = GameManager.Instance.songData.currentSelectDifficulty;
        
        GameManager.Instance.gameResult.Rank = rank;
        
        GameManager.Instance.selectSongItem.HighScore[difficulty]
            = GameManager.Instance.selectSongItem.HighScore[difficulty] < scoreManager.TotalScore
                ? scoreManager.TotalScore
                : GameManager.Instance.selectSongItem.HighScore[difficulty];

        GameManager.Instance.PlayerSetting.highClearLevel
            = GameManager.Instance.PlayerSetting.highClearLevel < difficulty
                ? difficulty
                : GameManager.Instance.PlayerSetting.highClearLevel;

        GameManager.Instance.PlayerSetting.AchieveRequireData.SetValueToRequire("HighClearLevel", GameManager.Instance.PlayerSetting.highClearLevel);
        GameManager.Instance.PlayerSetting.AchieveRequireData.AddValueToRequire($"ClearDiff{difficulty + 1}", GameManager.Instance.PlayerSetting.highClearLevel);
        
        var totalExp = (difficulty * 50) * (int)(accuracy);
        GameManager.Instance.PlayerSetting.currentExp += totalExp;
        
        SceneManager.LoadScene("00.MainScene");
    }


    public void GamePause() {
        Time.timeScale = 0.0f;
        metronome.Pause();
        pauseView.gameObject.SetActive(true);
    }

    public void GameResume() {
        Time.timeScale = 1.0f;
        metronome.Resume();
        pauseView.gameObject.SetActive(false);
    }
    
}
