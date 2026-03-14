using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ShovelSlot : MonoBehaviour, IPointerClickHandler
{
    public static ShovelSlot Instance;

    [SerializeField] private RectTransform shovelObj;
    [SerializeField] private GameObject shovelClueTxt;
    [SerializeField] private Canvas canvas;

    [HideInInspector] public bool waitingForAction = false;
    private Vector2 shovelStartPos;
    private Transform startParent;
    private Image shovelImage;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        shovelStartPos = shovelObj.anchoredPosition;
        startParent = shovelObj.parent;
        shovelImage = shovelObj.GetComponent<Image>();
    }

    void Update()
    {
        if (!waitingForAction) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            mousePos,
            canvas.worldCamera,
            out Vector2 canvasPoint
        );

        shovelObj.anchoredPosition = canvasPoint;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                ReturnShowel();
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        waitingForAction = !waitingForAction;

        shovelClueTxt.SetActive(waitingForAction);

        if (waitingForAction)
        {
            shovelObj.SetParent(canvas.transform, false);
            shovelImage.raycastTarget = false;
            InteractionManager.Instance.canPressBtns = false;
        }
        else
        {
            ReturnShowel();
        }
    }
    public void ReturnShowel()
    {
        waitingForAction = false;
        shovelClueTxt.SetActive(false);
        shovelObj.SetParent(startParent, false);
        shovelObj.anchoredPosition = shovelStartPos;
        shovelImage.raycastTarget = true;
        InteractionManager.Instance.canPressBtns = true;
    }
}
