using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class IntroManager : MonoBehaviour
{
    [SerializeField] private GameObject firstRegionObjects;
    [SerializeField] private CanvasGroup panel;
    [SerializeField] private TMP_Text text;
    [SerializeField] private AudioClip nextTextSound;
    [SerializeField] private AudioClip introEndSound;

    [TextArea]
    [SerializeField] private string[] lines;

    private int currentLine = 0;
    private bool canClick = false;
    private bool firstTimeShowLine = true;
    private bool ended = false;

    private void Start()
    {
        ShowLine();
        firstTimeShowLine = false;
    }

    private void Update()
    {
        if (canClick && Mouse.current.leftButton.wasPressedThisFrame && !ended)
        {
            NextLine();
        }
    }

    private void ShowLine()
    {
        if(!firstTimeShowLine)
            AudioManager.Instance.PlaySfxSound(nextTextSound, 0.15f, 0.9f, 1.1f);
        canClick = false;

        text.text = lines[currentLine];
        text.alpha = 0;
        text.transform.localScale = Vector3.one * 0.95f;

        Sequence seq = DOTween.Sequence();

        seq.Append(text.DOFade(1, 0.5f));
        seq.Join(text.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack));

        seq.AppendInterval(0.8f);

        seq.OnComplete(() => canClick = true);
    }

    private void NextLine()
    {
        currentLine++;

        if (currentLine >= lines.Length)
        {
            EndIntro();
            return;
        }

        Sequence seq = DOTween.Sequence();

        seq.Append(text.DOFade(0, 0.3f));
        seq.AppendCallback(() => ShowLine());
    }

    private void EndIntro()
    {
        AudioManager.Instance.PlaySfxSound(introEndSound, 0.15f);
        panel.DOFade(0, 0.5f).OnComplete(() =>
        {
            panel.gameObject.SetActive(false);
            InteractionManager.Instance.plantsCanEarn = true;
            InteractionManager.Instance.canStartEvents = true;
        });
        firstRegionObjects.SetActive(true);
        ended = true;
    }
}
