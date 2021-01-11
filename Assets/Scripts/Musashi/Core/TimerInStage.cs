﻿using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// ゲーム中のタイマーの制御（カウントダウン）とそれに伴う
/// ゲームシーンの状態（Start,Goal,GameOver）を監視するクラス
/// </summary>
[RequireComponent(typeof(ScoreManager))]
public class TimerInStage : EventReceiver<TimerInStage>
{
    [SerializeField] UISetActiveControl m_UISetActiveControl;
    ScoreManager m_scoreManager;
    float m_timeLimit;

    /// <summary>ゲーム中かどうか判定する </summary>
    public bool InGame { get; set; }
    private float m_oldSeconds;//1フレーム前の秒数

    private void Start()
    {
        m_scoreManager = GetComponent<ScoreManager>();
        if (StageParent.Instance)
        {
            //ステージを出現させる
            StageParent.Instance.AppearanceStageObject(StageParent.Instance.GetAppearanceStagePrefab.transform);
            //制限時間をセットする
            m_timeLimit = StageParent.Instance.GetAppearanceStageData.SetTimeLimit;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!InGame) return;

        //タイムリミット
        m_timeLimit -= Time.deltaTime;
        //分と秒とミリ秒を設定
        int minutes = (int)m_timeLimit / 60;
        float seconds = m_timeLimit - minutes * 60;
        float mseconds = m_timeLimit * 1000 % 1000;

        //UIに00:00:00.00形式で表示する
        //m_UISetActiveControl.TimerText.text = minute.ToString("00") + ":" + ((int)seconds).ToString("00") + ":" + ((int)ms).ToString("F0");
        //if ((int)seconds != (int)m_oldSeconds)
        //{
        //}
        //m_oldSeconds = seconds;
        m_UISetActiveControl.TimerText.text = string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, mseconds);


        //タイマーリミット!
        if (m_timeLimit <= 0)
        {
            m_timeLimit = 0;
            m_eventSystemInGameScene.ExecuteGameOverEvent();
        }
    }

    //memo
    //int mseconds = Mathf.FloorToInt((timer - minutes * 60 - seconds) * 1000);
    //string niceTime = string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, mseconds);

    public void StartGame()
    {
        InGame = true;
    }

    /// <summary>
    /// ゴール時のイベントから呼ばれる
    /// </summary>
    public void OnGoal()
    {
        InGame = false;
        //残り時間をScoreManagerに渡す
        m_scoreManager.Result(m_timeLimit);
    }

    /// <summary>
    /// タイムオーバーか、バルーンが障害物に当たったら呼ばれる
    /// </summary>
    public void GameOver()
    {
        InGame = false;
        if (StageParent.Instance)
        {
            //ステージを非表示にする
            StageParent.Instance.GetAppearanceStagePrefab.SetActive(false);
            //ステージを初期化する
            StageParent.Instance.Initialization();
        }

        if (SceneLoader.Instance)
        {
            //タップしたらセレクト画面に戻る(タップしてください。みたいなテキストを出す)
            SceneLoader.Instance.LoadSelectSceneWithTap();
        }
    }

    protected override void OnEnable()
    {
        m_eventSystemInGameScene.GameStartEvent += StartGame;
        m_eventSystemInGameScene.GameClearEvent += OnGoal;
        m_eventSystemInGameScene.GameOverEvent += GameOver;
    }

    protected override void OnDisable()
    {
        m_eventSystemInGameScene.GameStartEvent -= StartGame;
        m_eventSystemInGameScene.GameClearEvent -= OnGoal;
        m_eventSystemInGameScene.GameOverEvent -= GameOver;
    }
}
