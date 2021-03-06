﻿#if !ECS
using UnityEngine;
using UnityEngine.UI;
public class UI : MonoBehaviour
{
    private Text enemyCountText;
    private EnemySpawn enemySpawn;

    void Start()
    {
        enemyCountText = GameObject.Find("EnemyCountText").GetComponent<Text>();
        enemySpawn = GameObject.Find("EnemySpawn").GetComponent<EnemySpawn>();
    }

    void Update() => enemyCountText.text = "敌人数量:" + enemySpawn.EnemyCount;
}
#endif