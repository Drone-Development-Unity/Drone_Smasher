using Assets.Scripts.Enemies;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Wave;
using DG.Tweening;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    // Singleton instance
    [HideInInspector] public static WaveSpawner Instance { get; private set; }
    private NextWaveManager _nextWaveManager;
    private WaveTimer _waveTimer;

    [Header("Enemies Prefabs")]
    [SerializeField] private DifficultyClass[] enemyDifficultyClasses;
    private int currentBudget;
    private float[] currSetOfProbDiffClass;
    [SerializeField] private int maxSignificantWaveNumber;

    private GameObject enemiesContainer;

    [Header("Difficulty Scaling")]
    [SerializeField] private int addSecondsPerEveryWave;
    [SerializeField] private float enemyHpMultiplier;
    private float currHpMult;
    [SerializeField] private float enemyDmgMultiplier;
    private float currDmgMult;

    [Header("Spawn Settings")]
    // spawn area transforms
    [SerializeField] private Transform spawnStartTransform;
    [SerializeField] private Transform spawnEndTransform;

    private float xSpawnStart;
    private float xSpawnEnd;

    private float ySpawnStart;
    private float ySpawnEnd;

    private float yToTravel = -4f;

    // spawn timing
    [SerializeField] private float timeBetweenSpawnsPerPacket = 1f;
    [SerializeField] private float spawnAnimationDuration = 1f;

    // enemies counters
    private int enemiesPerWave;
    private int enemiesToSpawn;
    public int GetEnemiesToSpawn() { return enemiesToSpawn; }

    // spawn limits
    [SerializeField] private int enemiesPerPacket;
    [SerializeField] private float maxNumberOfEnemies = 10;

    // wave counter
    private int avaliableWaveNumber = 1;
    public int GetAvaliableWaveNumber() { return avaliableWaveNumber; }

    // Enemy positions
    private Dictionary<int, Vector2> enemyEndPositions = new Dictionary<int, Vector2>();
    [SerializeField] private float minDistanceBetweenEnemies = 0.5f;

    private Tween animationTween;

    int BudgetCurve(int wave) {
        return wave * 10;
    }


    GameObject GetRandomEnemiesPrefab()
    {
        int diffIndex = PickDifficultyClassIndex();
        DifficultyClass diffClass = enemyDifficultyClasses[diffIndex];

        // check for budget
        if (diffClass.cost <= currentBudget)
        {
            // random prefab
            int prefabIndex = UnityEngine.Random.Range(0, diffClass.enemyPrefabs.Length);
            GameObject prefab = diffClass.enemyPrefabs[prefabIndex];

            currentBudget -= diffClass.cost;
            //Debug.Log($"Selected enemy of difficulty '{diffClass.name}' with cost {diffClass.cost}. Remaining budget: {currentBudget}.");
            return prefab;
        }
        return null;
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
        //SpawnWave();
        enemiesContainer = GameObject.Find("EnemiesContainer");
        _nextWaveManager = NextWaveManager.Instance;
        _waveTimer = WaveTimer.Instance;
        enemiesToSpawn = 0;

        // spawn area boundaries
        xSpawnStart = spawnStartTransform.position.x;
        xSpawnEnd = spawnEndTransform.position.x;
        ySpawnStart = spawnStartTransform.position.y;
        ySpawnEnd = spawnEndTransform.position.y;
    }

    public void SpawnWave(int waveNumber)
    {
        if(enemiesToSpawn > 0) return;


        StartCoroutine(SpawnWaveWithDelay(waveNumber));
    }

    IEnumerator SpawnWaveWithDelay(int waveNumber)
    {
        PrepareWave(waveNumber);

        enemyEndPositions = new Dictionary<int, Vector2>();

        if(waveNumber == avaliableWaveNumber)
        {
            avaliableWaveNumber++;
        }

        enemiesPerWave = BudgetCurve(waveNumber);
        enemiesToSpawn = enemiesPerWave;

        //Debug.Log($"Spawning Wave {waveNumber} with {enemiesPerWave} enemies.");


        // spawn enemies in packets
        int packets = Mathf.CeilToInt((float)enemiesPerWave / enemiesPerPacket);
        for (int i = 0; i < packets; i++)
        {
            for (int j = 0; j < enemiesPerPacket; j++)
            {
                // limit of enemies to spawn
                if (enemiesToSpawn <= 0) break;

                // wait if max number of enemies is reached
                yield return new WaitUntil(() => _nextWaveManager.GetAliveEnemiesCount() < maxNumberOfEnemies);

                // check if wave time ended
                if (_waveTimer.IsWaveTimeOver)
                {
                    enemiesToSpawn = 0;
                    break;
                }

                // check budget
                GameObject enemyPrefab = GetRandomEnemiesPrefab();
                if(enemyPrefab == null) { 
                  // no more budget
                    enemiesToSpawn = 0;
                    break;
                }

                // spawn enemy
                Vector2 spawnPos = GetSpawnPoint();

                GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.Euler(0f, 0f, 180f));
                enemy.transform.parent = enemiesContainer.transform;

                // register enemy to NextWaveManager 
                _nextWaveManager.RegisterEnemies(1);
                // register enemy end position
                BaseEnemy baseEnemy = enemy.GetComponentInChildren<BaseEnemy>();
                if(baseEnemy != null)
                {
                    //Debug.Log($"Registering enemy {baseEnemy.GetID()} at position {spawnPos}.");
                    enemyEndPositions.Add(baseEnemy.GetID(), spawnPos);
                    // hp multiplier
                    baseEnemy.SetHpMutiplier(currHpMult);
                }

                BaseEnemyShoot baseEnemyShoot = enemy.GetComponentInChildren<BaseEnemyShoot>();
                if (baseEnemyShoot != null) 
                {
                    baseEnemyShoot.SetDmgMultiplier(currDmgMult);
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

        animationTween = enemyInstance.transform
            .DOMoveY(targetY, spawnAnimationDuration)
            .SetEase(Ease.OutQuad)
            .SetLink(enemyInstance)
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

    // set up budget, probabilities for classes
    private void PrepareWave(int waveNumber)
    {
        currentBudget = BudgetCurve(waveNumber);

        // prepare probabilities for difficulty classes
        int numberOfDiffClasses = enemyDifficultyClasses.Length;
        currSetOfProbDiffClass = new float[numberOfDiffClasses];

        float probDiff = (maxSignificantWaveNumber <= 1) ? 0f : (float)waveNumber / (float)(maxSignificantWaveNumber - 1);

        for (int i = 0; i < numberOfDiffClasses; i++)
        {
            float start = enemyDifficultyClasses[i].firstWaveProbability;
            float end = enemyDifficultyClasses[i].infWaveProbability;

            float prob = Mathf.Lerp(start, end, probDiff);
            currSetOfProbDiffClass[i] = prob;
        }

        // diffulty multipliers
        currHpMult = (float)Math.Pow(enemyHpMultiplier, waveNumber - 1);
        currDmgMult = (float)Math.Pow(enemyDmgMultiplier, waveNumber - 1);

        // let know menagers about new wave
        int timerDuration = waveNumber * addSecondsPerEveryWave + 1;
        _waveTimer.StartTimer(timerDuration);
        _nextWaveManager.OnNewWave();

        //Debug.Log($"Prepared wave {waveNumber} with budget {currentBudget}.");
        //Debug.Log($"Difficulty class probabilities: {string.Join(", ", currSetOfProbDiffClass)}");
    }

    // get random difficulty class 
    private int PickDifficultyClassIndex()
    {
        float totalProb = 0f;
        int numberOfDiffClasses = enemyDifficultyClasses.Length;

        for (int i = 0; i < numberOfDiffClasses; i++)
            totalProb += currSetOfProbDiffClass[i];

        float randomProb = UnityEngine.Random.Range(0f, totalProb);
        float cumulative = 0f;

        for (int i = 0; i < numberOfDiffClasses; i++)
        {
            cumulative += currSetOfProbDiffClass[i];
            if (randomProb <= cumulative)
                return i;
        }

        return numberOfDiffClasses - 1;
    }

    [System.Serializable]
    public struct DifficultyClass
    {
        public string name;
        public int cost;
        public GameObject[] enemyPrefabs;
        [UnityEngine.Range(0f, 1f)] public float firstWaveProbability;
        [UnityEngine.Range(0f, 1f)] public float infWaveProbability;
    }
}
