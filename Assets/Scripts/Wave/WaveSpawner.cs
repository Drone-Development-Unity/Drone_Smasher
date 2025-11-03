using Assets.Scripts.Interfaces;
using Assets.Scripts.Wave;
using DG.Tweening;
using System.Collections;
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
    private int waveNumber = 0;

    int BudgetCurve() {
        return waveNumber * 10;
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
        return new Vector2(xPos, yPos);
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

                // spawn enemy
                GameObject enemyPrefab = GetRandomEnemiesPrefab();
                Vector2 spawnPos = GetSpawnPoint();

                GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.Euler(0f, 0f, 180f));
                enemy.transform.parent = enemiesContainer.transform;
                // register enemy to NextWaveManager
                nextWaveManager.RegisterEnemies(1);

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
}
