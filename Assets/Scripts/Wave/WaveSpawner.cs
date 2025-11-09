using Assets.Scripts.Enemies;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Wave;
using DG.Tweening;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    // Singleton instance
    [HideInInspector] public static WaveSpawner Instance { get; private set; }
    private NextWaveManager nextWaveManager;

    [Header("Enemies Prefabs")]
    [SerializeField] private GameObject[] enemyPrefabs;
    private GameObject enemiesContainer;

    [Header("Spawn Settings")]
    private float xSpawnStart = -8.5f;
    private float xSpawnEnd = 0.5f;

    private float ySpawnStart = 8f;
    private float ySpawnEnd = 6f;

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
    int BudgetCurve() {
        return waveNumber * 20;
    }

    GameObject GetRandomEnemiesPrefab()
    {
        int index = Random.Range(0, enemyPrefabs.Length);
        return enemyPrefabs[index];
    }

    Vector2 GetSpawnPoint()
    {
        float xPos = Random.Range(xSpawnStart, xSpawnEnd);
        float yPos = Random.Range(ySpawnStart, ySpawnEnd);

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
        enemiesToSpawn = 0;
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

        // spawn enemies in packets
        int packets = Mathf.CeilToInt((float)enemiesPerWave / enemiesPerPacket);
        for (int i = 0; i < packets; i++)
        {
            for (int j = 0; j < enemiesPerPacket; j++)
            {
                if (enemiesToSpawn <= 0) break;

                yield return new WaitUntil(() => nextWaveManager.GetAliveEnemiesCount() < maxNumberOfEnemies);


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

        enemyInstance.transform.DOMoveY(targetY, spawnAnimationDuration)
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
}
