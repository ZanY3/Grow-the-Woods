using UnityEngine;
using UnityEngine.InputSystem;

public class FieldZoom : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private bool followCursor = true;
    [SerializeField] private float moveAmount = 40f;
    [SerializeField] private float smoothTime = 0.35f;

    [Header("Zoom")]
    [SerializeField] private float minZoom = 0.8f;
    [SerializeField] private float maxZoom = 1.6f;
    [SerializeField] private float zoomSpeed = 0.15f;
    [SerializeField] private float smoothSpeed = 8f;

    private RectTransform fieldContainer;

    private float targetZoom = 1f;
    private float currentZoom = 1f;

    private Vector2 startPos;
    private Vector2 velocity;

    void Start()
    {
        fieldContainer = GetComponent<RectTransform>();

        currentZoom = targetZoom;
        startPos = fieldContainer.anchoredPosition;
    }

    void Update()
    {
        HandleZoom();

        if (followCursor)
            HandleMouseMove();
    }

    void HandleZoom()
    {
        if (!InteractionManager.Instance.canZoomCam) return;

        float scroll = Mouse.current.scroll.ReadValue().y;

        if (scroll != 0)
        {
            targetZoom += scroll * zoomSpeed * Time.deltaTime * 10f;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        }

        currentZoom = Mathf.Lerp(currentZoom, targetZoom, Time.deltaTime * smoothSpeed);

        fieldContainer.localScale = Vector3.one * currentZoom;
    }

    void HandleMouseMove()
    {
        Vector2 mouse = Mouse.current.position.ReadValue();

        float normalizedX = (mouse.x / Screen.width - 0.5f) * 2f;
        float normalizedY = (mouse.y / Screen.height - 0.5f) * 2f;

        float deadzone = 0.1f;

        if (Mathf.Abs(normalizedX) < deadzone) normalizedX = 0;
        if (Mathf.Abs(normalizedY) < deadzone) normalizedY = 0;

        Vector2 offset = new Vector2(
            -normalizedX * moveAmount,
            -normalizedY * moveAmount
        );

        Vector2 target = startPos + offset;

        fieldContainer.anchoredPosition = Vector2.SmoothDamp(
            fieldContainer.anchoredPosition,
            target,
            ref velocity,
            smoothTime
        );
    }
}
