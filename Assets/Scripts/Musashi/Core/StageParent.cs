﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// ステージ情報を管理する
/// セレクト画面でボタンを押した時に確定させ、
/// 次のゲームシーンで参照させる
/// </summary>
public class StageParent : SingletonMonoBehavior<StageParent>
{
    /// <summary>ステージデータ</summary>
    [SerializeField] StageData[] m_stageDatas;
    /// <summary>インスタンス化したstagePrefabが入ったリスト(Hierarchy上に生成されたプレハブ)</summary>
    private readonly List<GameObject> m_stagePrefabList = new List<GameObject>();
    /// <summary>出現させるステージデータ </summary>
    public StageData GetAppearanceStageData { get; private set; }
    /// <summary>出現させるステージプレハブ</summary>
    public GameObject GetAppearanceStagePrefab { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        var json = new SaveAndLoadWithJSON();
        for (int i = 0; i < m_stageDatas.Length; i++)
        {
            //各ステージデータをロードする
            m_stageDatas[i] = json.LoadStageData(m_stageDatas[i]);
        }


        //最初のダミーシーンでステージを生成して、アクティブを非表示にする
        foreach (var data in m_stageDatas)
        {
            var go = Instantiate(data.StagePrefab);
            go.transform.SetParent(this.transform);
            go.SetActive(false);
            m_stagePrefabList.Add(go);
        }
        Initialization();
    }

    public void Initialization()
    {
        GetAppearanceStageData = null;
        GetAppearanceStagePrefab = null;
    }

    /// <summary>
    /// ステージセレクトボタンを押した時に、次のゲームシーンで出現させる
    /// ステージ情報を確定させる
    /// </summary>
    public void SetStageInfo(GameObject stage)
    {
        if (stage == null)
        {
            Debug.LogError("ステージプレハブがセットされていません！！");
            return;
        }

        //stageを検索
        for (int i = 0; i < m_stagePrefabList.Count(); i++)
        {
            if (m_stagePrefabList[i].Equals(stage))
            {
                //stageをセットする
                GetAppearanceStageData = m_stageDatas[i];
                GetAppearanceStagePrefab = m_stagePrefabList[i];
                //ゲームシーンをロード
                SceneLoader.Instance.LoadGameScene();
                return;
            }
        }

        Debug.LogError("該当するStagePrefabがStageDataに存在しません");
    }


    public void AppearanceStageObject(Transform stageParent)
    {
        //ステージの非表示な子オブジェクトを表示する
        foreach (Transform item in stageParent)
        {
            if (item.gameObject.activeSelf == false)
            {
                item.gameObject.SetActive(true);
            }
            //さらに子供が非表示なら表示する
            AppearanceStageObject(item);
        }
        GetAppearanceStagePrefab.SetActive(true);
    }
}
