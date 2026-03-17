using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndingManager : MonoBehaviour
{
    public static EndingManager Instance;

    [Header("Progress")]
    [SerializeField] private GameObject progressUI;
    [SerializeField] private TMP_Text progressTxt;
    [SerializeField] private Image progressFill;

    [Header("Ending UI")]
    [SerializeField] private CanvasGroup endingPanel;
    [SerializeField] private GameObject bg;
    [SerializeField] private TMP_Text titleTxt;
    [SerializeField] private Transform restartBtn;

    [SerializeField] private int totalCells = 40;

    private int cellsFilled = 0;
    private bool endingStarted = false;

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
        // изначально всё выключено
        endingPanel.gameObject.SetActive(false);
        bg.SetActive(false);

        endingPanel.alpha = 0;
        titleTxt.alpha = 0;
        restartBtn.localScale = Vector3.zero;

        UpdateProgress(1);
    }

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            UpdateProgress(47);
        }
    }

    public void UpdateProgress(int amountToAdd)
    {
        if (endingStarted) return;

        cellsFilled += amountToAdd;

        progressTxt.text = cellsFilled + " / " + totalCells;

        float progress = (float)cellsFilled / totalCells;
        progressFill.fillAmount = progress;

        if (cellsFilled >= totalCells)
        {
            PlayEnding();
        }
    }

    public void ChangeProgressState(bool state)
    {
        progressUI.SetActive(state);
    }

    public void PlayEnding()
    {
        if (endingStarted) return;
        endingStarted = true;

        ChangeProgressState(false);

        StartCoroutine(EndingSequence());
    }

    private IEnumerator EndingSequence()
    {
        // 1. включаем панель и фейдим
        endingPanel.gameObject.SetActive(true);
        endingPanel.alpha = 0;
        endingPanel.DOFade(1f, 0.8f);

        yield return new WaitForSeconds(0.3f);

        // 2. волна по клеткам
        var cells = Grid.Instance.GetCells();

        for (int i = 0; i < cells.Length; i++)
        {
            var cell = cells[i];

            cell.transform
                .DOScale(1.1f, 0.15f)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);

            yield return new WaitForSeconds(0.03f);
        }

        yield return new WaitForSeconds(0.5f);

        // 3. включаем BG и фейдим
        bg.SetActive(true);

        Image img = bg.GetComponent<Image>();
        if (img != null)
        {
            Color c = img.color;
            c.a = 0;
            img.color = c;

            img.DOFade(1f, 0.8f).SetEase(Ease.InOutSine);
        }

        yield return new WaitForSeconds(0.8f);

        // 4. текст
        titleTxt.DOFade(1, 1f);
        titleTxt.transform.DOScale(1, 0.5f).From(0.8f);

        yield return new WaitForSeconds(0.5f);

        // 5. кнопка
        restartBtn
            .DOScale(1, 0.4f)
            .From(0f)
            .SetEase(Ease.OutBack);
    }

    public void Restart()
    {
        SceneManager.LoadScene("Game");
    }
}
