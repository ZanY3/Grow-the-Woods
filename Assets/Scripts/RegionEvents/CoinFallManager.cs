using DG.Tweening;
using System.Collections;
using UnityEngine;

public class CoinFallManager : MonoBehaviour
{
    [SerializeField] private RectTransform[] spawnPlaces;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private RectTransform parentTransform;

    [Header("Clue Settings")]
    [SerializeField] private CanvasGroup coinFallStartClue;
    [SerializeField] private float clueFadeTime = 0.5f;
    [SerializeField] private float clueDisplayTime = 1.5f;

    [Header("Parameters")]
    [SerializeField] private float spawnRate = 0.2f;
    [SerializeField] private float duration = 5.0f;
    [SerializeField][Range(0, 100)] private float chanceToStart;
    [SerializeField] private float checkInterval;

    [HideInInspector] public bool canLaunchEvent = true;
    private float _currentSpawnTimer;
    private float _currentDurationTimer;
    private bool _isActive;

    private void Start()
    {
        if (coinFallStartClue != null)
        {
            coinFallStartClue.alpha = 0;
            coinFallStartClue.gameObject.SetActive(false);
        }

        StartCoroutine(ChanceCheckRoutine());
    }

    private IEnumerator ChanceCheckRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(checkInterval);
            if (_isActive) continue;

            if (Random.Range(0f, 100f) <= chanceToStart && canLaunchEvent)
            {
                StartCoroutine(ShowClueAndStartSequence());
            }
        }
    }

    private IEnumerator ShowClueAndStartSequence()
    {
        _isActive = true;

        if (coinFallStartClue != null)
        {
            coinFallStartClue.gameObject.SetActive(true);

            coinFallStartClue.transform.localScale = Vector3.zero;
            coinFallStartClue.DOFade(1, clueFadeTime);
            coinFallStartClue.transform.DOScale(1.2f, clueFadeTime).SetEase(Ease.OutBack);

            yield return new WaitForSeconds(clueDisplayTime);

            coinFallStartClue.DOFade(0, clueFadeTime);
            yield return coinFallStartClue.transform.DOScale(0, clueFadeTime).SetEase(Ease.InBack).WaitForCompletion();

            coinFallStartClue.gameObject.SetActive(false);
        }
        StartCoinFall();
    }

    public void StartCoinFall()
    {
        _currentDurationTimer = duration;
        _currentSpawnTimer = 0;
        _isActive = true;
    }

    private void Update()
    {
        if (!_isActive || _currentDurationTimer <= 0) return;

        _currentDurationTimer -= Time.deltaTime;
        _currentSpawnTimer -= Time.deltaTime;

        if (_currentSpawnTimer <= 0)
        {
            SpawnCoin();
            _currentSpawnTimer = spawnRate;
        }

        if (_currentDurationTimer <= 0)
        {
            StopCoinFall();
        }
    }

    private void SpawnCoin()
    {
        if (spawnPlaces.Length == 0) return;
        Transform spawnPoint = spawnPlaces[Random.Range(0, spawnPlaces.Length)];
        GameObject coin = Instantiate(coinPrefab, parentTransform);
        coin.transform.position = spawnPoint.position;

        coin.transform.localScale = Vector3.zero;
        coin.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBounce);
    }

    private void StopCoinFall()
    {
        _isActive = false;
    }
}
