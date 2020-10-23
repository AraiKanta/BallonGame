﻿using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// ゲーム中の時間を管理する
/// ゲームはタイムリミット制。
/// </summary>
[RequireComponent(typeof(ScoreManager))]
public class TimeScheduler : SingletonMonoBehavior<TimeScheduler>
{
    [SerializeField] Text m_timeLimitText;
    [SerializeField] float m_timeLimit = 300f;

    /// <summary>ゲーム中かどうか判定する </summary>
    public bool InGame { get; set; }

    // Update is called once per frame
    void Update()
    {
        if (!InGame) return;

        //タイムリミット
        m_timeLimit -= Time.deltaTime;
        m_timeLimitText.text = m_timeLimit.ToString("F2") + "秒";

        if (m_timeLimit <= 0)
        {
            m_timeLimit = 0;
            GameOver();
        }
    }

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
        int timeScore = Mathf.FloorToInt(m_timeLimit) * 100;//修正ポイント(どういう計算になるかは未定)
        ScoreManager.Instance.AddTimeScore(timeScore);
    }

    /// <summary>
    /// タイムオーバーか、バルーンが障害物に当たったら呼ばれる
    /// </summary>
    public void GameOver()
    {
        InGame = false;
        Debug.Log("GameOver");
    }
}
