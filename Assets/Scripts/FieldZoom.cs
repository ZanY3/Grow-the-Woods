using UnityEngine;
using UnityEngine.InputSystem;

public class FieldZoom : MonoBehaviour
{
    [SerializeField] private float minZoom = 0.8f;
    [SerializeField] private float maxZoom = 1.6f;
    [SerializeField] private float zoomSpeed = 0.15f;
    [SerializeField] private float smoothSpeed = 8f;

    private float targetZoom = 1f;
    private float currentZoom = 1f;

    private RectTransform fieldContainer;

    private void Start()
    {
        fieldContainer = GetComponent<RectTransform>();
        currentZoom = targetZoom;
    }

    void Update()
    {
        if (InteractionManager.Instance.canZoomCam)
        {
            float scroll = Mouse.current.scroll.ReadValue().y;

            if (scroll != 0)
            {
                targetZoom += scroll * zoomSpeed * Time.deltaTime * 10f;
                targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
            }

            currentZoom = Mathf.Lerp(currentZoom, targetZoom, Time.deltaTime * smoothSpeed);

            fieldContainer.localScale = Vector3.one * currentZoom;
        }
    }
}
