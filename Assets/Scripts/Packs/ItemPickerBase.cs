using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ItemPickerBase : MonoBehaviour
{
    [Header("Card Picker Base")]
    [SerializeField] protected GameObject allPanel;
    [SerializeField] protected GameObject openPanel;
    [SerializeField] protected GameObject choosePanel;
    [SerializeField] protected Button confirmBtn;
    [SerializeField] protected AudioClip openSound;
    [SerializeField] protected AudioClip chooseSound;
    [SerializeField] protected AudioClip confirmSound;

    protected int selectedIndex = -1;
    protected RectTransform[] cardRects;
    protected Dictionary<GameObject, Vector3> originalScales = new();
    private const float selectedMultiplier = 1.15f;

    private void Start()
    {
        InteractionManager.Instance.canStartEvents = false;
    }
    public void OpenPicker()
    {
        AudioManager.Instance.PlaySfxSound(openSound, 0.4f);
        openPanel.SetActive(false);
        choosePanel.SetActive(true);
        confirmBtn.interactable = false;
        selectedIndex = -1;

        FillCards();
        CacheScales();
        AnimateCards();
    }

    public void SelectCard(int index)
    {
        AudioManager.Instance.PlaySfxSound(chooseSound, 0.25f, 0.85f, 1.15f);

        if (selectedIndex == index)
        {
            ResetScale(cardRects[index].gameObject);
            selectedIndex = -1;
            confirmBtn.interactable = false;
            OnDeselected();
            return;
        }

        if (selectedIndex >= 0)
            ResetScale(cardRects[selectedIndex].gameObject);

        selectedIndex = index;
        SetScale(cardRects[index].gameObject);
        confirmBtn.interactable = true;
        OnSelected(index);
    }

    public void ConfirmChoice()
    {
        if (selectedIndex >= 0)
            ResetScale(cardRects[selectedIndex].gameObject);

        openPanel.SetActive(true);
        choosePanel.SetActive(false);
        allPanel.SetActive(false);
        InteractionManager.Instance.canStartEvents = true;

        AudioManager.Instance.PlaySfxSound(confirmSound, 0.3f, 0.9f, 1.1f);
        OnConfirmed(selectedIndex);
    }

    // — Subclasses implement these —
    protected abstract void FillCards();
    protected abstract void OnSelected(int index);
    protected abstract void OnDeselected();
    protected abstract void OnConfirmed(int index);

    private void CacheScales()
    {
        originalScales.Clear();
        foreach (var r in cardRects)
            originalScales[r.gameObject] = r.localScale;
    }

    private void SetScale(GameObject obj)
    {
        var r = obj.GetComponent<RectTransform>();
        var o = originalScales[obj];
        r.localScale = new Vector3(o.x * selectedMultiplier, o.y * selectedMultiplier, 1f);
    }

    private void ResetScale(GameObject obj)
    {
        var r = obj.GetComponent<RectTransform>();
        r.localScale = originalScales[obj];
    }

    private void AnimateCards()
    {
        for (int i = 0; i < cardRects.Length; i++)
        {
            var card = cardRects[i];
            Vector3 origScale = originalScales[card.gameObject];
            Vector2 targetPos = card.anchoredPosition;

            card.localScale = new Vector3(0f, 0f, 1f);
            card.anchoredPosition = targetPos + Vector2.up * 20f;

            var seq = DOTween.Sequence();
            seq.Append(card.DOScale(origScale, 0.35f).SetEase(Ease.OutBack));
            seq.Join(card.DOAnchorPos(targetPos, 0.35f).SetEase(Ease.OutCubic));
            seq.SetDelay(i * 0.05f);
        }
    }
}
