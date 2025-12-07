using Assets.Scripts.Enemies;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Wave;
using DG.Tweening;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    // Singleton instance
    [HideInInspector] public static WaveSpawner Instance { get; private set; }
    private NextWaveManager nextWaveManager;
    private WaveTimer waveTimer;

    [Header("Enemies Prefabs")]
    [SerializeField] private GameObject[] enemyPrefabs;
    private GameObject enemiesContainer;

    [Header("Spawn Settings")]
    [SerializeField] private Transform spawnStartTransform;
    [SerializeField] private Transform spawnEndTransform;

    private float xSpawnStart;
    private float xSpawnEnd;

    private float ySpawnStart;
    private float ySpawnEnd;

    private float yToTravel = -4f;

    [SerializeField] private float timeBetweenSpawnsPerPacket = 1f;
    [SerializeField] private float nextWaveDelay = 3f;
    [SerializeField] private float spawnAnimationDuration = 1f;

    private int enemiesPerWave;
    private int enemiesToSpawn;
    [SerializeField] private int enemiesPerPacket;
    [SerializeField] private float maxNumberOfEnemies = 10;
    private int waveNumber = 0;

    // Enemy positions
    private Dictionary<int, Vector2> enemyEndPositions = new Dictionary<int, Vector2>();
    [SerializeField] private float minDistanceBetweenEnemies = 0.5f;

    private Tween animationTween;
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI waveNumberText;
    int BudgetCurve() {
        return waveNumber * 20;
    }


    GameObject GetRandomEnemiesPrefab()
    {
        int index = UnityEngine.Random.Range(0, enemyPrefabs.Length);
        return enemyPrefabs[index];
    }

    Vector2 GetSpawnPoint()
    {
        float xPos = UnityEngine.Random.Range(xSpawnStart, xSpawnEnd);
        float yPos = UnityEngine.Random.Range(ySpawnStart, ySpawnEnd);

        Vector2 potentialPosition = new Vector2(xPos, yPos);

        // Check if the position is already taken
        foreach (Vector2 pos in enemyEndPositions.Values)
        {
            if (Vector2.Distance(pos, potentialPosition) < minDistanceBetweenEnemies) // in some tolerance
            {
                return GetSpawnPoint();
            }
        }
        return potentialPosition;
    }

    // Create Singleton
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        SpawnWave();
        enemiesContainer = GameObject.Find("EnemiesContainer");
        nextWaveManager = NextWaveManager.Instance;
        waveTimer = WaveTimer.Instance;
        enemiesToSpawn = 0;

        // spawn area boundaries
        xSpawnStart = spawnStartTransform.position.x;
        xSpawnEnd = spawnEndTransform.position.x;
        ySpawnStart = spawnStartTransform.position.y;
        ySpawnEnd = spawnEndTransform.position.y;
    }

    public void SpawnWave()
    {
        if(enemiesToSpawn > 0) return;

        StartCoroutine(SpawnWaveWithDelay());
    }

    IEnumerator SpawnWaveWithDelay()
    {
        yield return new WaitForSeconds(nextWaveDelay);

        enemyEndPositions = new Dictionary<int, Vector2>();

        waveNumber++;
        enemiesPerWave = BudgetCurve();
        enemiesToSpawn = enemiesPerWave;

        waveTimer.StartTimer(enemiesToSpawn + 1); // each wave lasts for enemiesToSpawn seconds

        waveNumberText.text = $"Level: {waveNumber}";

        // spawn enemies in packets
        int packets = Mathf.CeilToInt((float)enemiesPerWave / enemiesPerPacket);
        for (int i = 0; i < packets; i++)
        {
            for (int j = 0; j < enemiesPerPacket; j++)
            {
                // limit of enemies to spawn
                if (enemiesToSpawn <= 0) break;

                // wait if max number of enemies is reached
                yield return new WaitUntil(() => nextWaveManager.GetAliveEnemiesCount() < maxNumberOfEnemies);

                // check if wave time ended
                if (waveTimer.IsWaveTimeOver)
                {
                    enemiesToSpawn = 0;
                    break;
                }

                // spawn enemy
                GameObject enemyPrefab = GetRandomEnemiesPrefab();
                Vector2 spawnPos = GetSpawnPoint();

                GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.Euler(0f, 0f, 180f));
                enemy.transform.parent = enemiesContainer.transform;

                // register enemy to NextWaveManager 
                nextWaveManager.RegisterEnemies(1);
                // register enemy end position
                BaseEnemy baseEnemy = enemy.GetComponentInChildren<BaseEnemy>();
                if(baseEnemy != null)
                {
                    //Debug.Log($"Registering enemy {baseEnemy.GetID()} at position {spawnPos}.");
                    enemyEndPositions.Add(baseEnemy.GetID(), spawnPos);
                }


                UpDownAnim(enemy, spawnPos);
                enemiesToSpawn--;
            }
            yield return new WaitForSeconds(timeBetweenSpawnsPerPacket);
        }
    }

    void UpDownAnim(GameObject enemyInstance, Vector2 startPos)
    {
        float targetY = startPos.y + yToTravel;

        animationTween = enemyInstance.transform.DOMoveY(targetY, spawnAnimationDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                // Notify that the enemy has arrived at its destination
                IArrivable arrivable = enemyInstance.GetComponentInChildren<IArrivable>();
                if (arrivable != null)
                {
                    arrivable.OnArrive();
                }
            });
    }

    public void UnregisterEnemy(int enemyId)
    {
        if(enemyEndPositions.ContainsKey(enemyId))
        {
            //Debug.Log($"Unregistering enemy {enemyId} from WaveSpawner.");
            enemyEndPositions.Remove(enemyId);
        }
    }
    void OnDestroy()
    {
        if (animationTween != null && animationTween.IsActive())
        {
            animationTween.Kill();
        }
    }
}
