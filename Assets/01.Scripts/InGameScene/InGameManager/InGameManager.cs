﻿using System.Collections;
using System.Collections.Generic;
using EventTools.Event;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class InGameManager : MonoBehaviour
{
    
    public static InGameManager instance;
    
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
    
    
    [Header("Events")]
    [SerializeField]
    private UniEvent<Color,Color,float> backgroundColorChangeEvent;
    
    
    private void Awake(){
        if(instance is null){
            instance = this;
        }

        scoreManager = gameObject.GetComponent<ScoreManager>();
        nodeCreator = gameObject.GetComponent<NodeCreator>();
        metronome = gameObject.GetComponent<Metronome>();
        centerEffectorController = gameObject.GetComponent<CenterEffectorController>();
        nodeInteractionController = gameObject.GetComponent<NodeInteractionController>();
    }

    private void Start(){
        GameManager.instance.touchManager.touchType = TouchType.InGame;
        GameManager.instance.gameResult = new GameResult();
        GameManager.instance.CurrentSceneType = SceneType.INGAME;
    }

    public void ChangeBackgroundColor(Color topColor, Color bottomColor, float duration) {
        backgroundColorChangeEvent.Invoke(topColor,bottomColor,duration);
    }    

    public void GameEnd() {
        instance = null;

        GameManager.instance.gameResult.Score = scoreManager.TotalScore;
        GameManager.instance.PlayerSetting.totalScore += (ulong) scoreManager.TotalScore;

        var judgeCounts = GameManager.instance.gameResult.JudgeCounts;
        var accuracy = (judgeCounts[4] * 1.0f)
                       + (judgeCounts[3] * 0.8f)
                       + (judgeCounts[2] * 0.4f)
                       + (judgeCounts[0] * 0.0f);

        accuracy /= GameManager.instance.gameResult.NodeTotalCount;
        accuracy *= 100.0f;

        GameManager.instance.gameResult.Accuracy = accuracy;

        var rank = "";
        
        switch (accuracy) {
            case var v when accuracy == 100.0f:
                rank = "SSS";
                GameManager.instance.PlayerSetting.perfectPlay++;
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
            case var v when accuracy > 30.0f:
                rank = "F";
                break;
        }

        GameManager.instance.gameResult.Rank = rank;
        
        GameManager.instance.selectSongItem.HighScore
            = GameManager.instance.selectSongItem.HighScore < scoreManager.TotalScore
                ? scoreManager.TotalScore
                : GameManager.instance.selectSongItem.HighScore;

        GameManager.instance.PlayerSetting.highClearLevel
            = GameManager.instance.PlayerSetting.highClearLevel < GameManager.instance.songData.currentSelectDifficulty
                ? GameManager.instance.songData.currentSelectDifficulty
                : GameManager.instance.PlayerSetting.highClearLevel;

        SceneManager.LoadScene("00.MainScene");
    }


}
