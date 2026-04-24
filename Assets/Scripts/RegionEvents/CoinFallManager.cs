using UnityEngine;
using System.Collections;

public class CoinFallManager : MonoBehaviour
{
    [SerializeField] private RectTransform[] spawnPlaces;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private RectTransform canvasTransform;

    [Header("Parameters")]
    [SerializeField] private float spawnRate = 0.2f;
    [SerializeField] private float duration = 5.0f;
    [SerializeField][Range(0, 100)] private float chanceToStart;
    [SerializeField] private float checkInterval;

    private float _currentSpawnTimer;
    private float _currentDurationTimer;
    private bool _isActive;

    private void Start()
    {
        StartCoroutine(ChanceCheckRoutine());
    }

    private IEnumerator ChanceCheckRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(checkInterval);
            if (_isActive) continue;

            if (Random.Range(0f, 100f) <= chanceToStart)
            {
                StartCoinFall();
            }
        }
    }
    public void StartCoinFall()
    {
        _currentDurationTimer = duration;
        _currentSpawnTimer = 0;
        _isActive = true;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (!_isActive) return;

        if (_currentDurationTimer > 0)
        {
            _currentDurationTimer -= Time.deltaTime;
            _currentSpawnTimer -= Time.deltaTime;

            if (_currentSpawnTimer <= 0)
            {
                SpawnCoin();
                _currentSpawnTimer = spawnRate;
            }
        }
        else
        {
            StopCoinFall();
        }
    }

    private void SpawnCoin()
    {
        if (spawnPlaces.Length == 0) return;
        Transform spawnPoint = spawnPlaces[Random.Range(0, spawnPlaces.Length)];
        GameObject coin = Instantiate(coinPrefab, canvasTransform);
        coin.transform.position = spawnPoint.position;
    }

    private void StopCoinFall()
    {
        _isActive = false;
        //gameObject.SetActive(false);
    }
}
