using UnityEngine;
using UnityEngine.UI;

public class PackShimmer : MonoBehaviour
{
    [SerializeField] private float speed = 400f;
    [SerializeField] private float startX;
    [SerializeField] private float endX;
    [SerializeField] private float fadeSpeed = 4f;

    private RectTransform shimmer;
    private Image image;

    private bool fadingOut;
    private float startAlpha;

    void Start()
    {
        shimmer = GetComponent<RectTransform>();
        image = GetComponent<Image>();

        startAlpha = image.color.a;
    }

    void Update()
    {
        shimmer.anchoredPosition += Vector2.right * speed * Time.deltaTime;

        if (shimmer.anchoredPosition.x > endX && !fadingOut)
        {
            fadingOut = true;
        }

        if (fadingOut)
        {
            Color c = image.color;
            c.a -= fadeSpeed * Time.deltaTime;
            image.color = c;

            if (c.a <= 0f)
            {
                shimmer.anchoredPosition = new Vector2(startX, shimmer.anchoredPosition.y);

                c.a = startAlpha;
                image.color = c;

                fadingOut = false;
            }
        }
    }
}
