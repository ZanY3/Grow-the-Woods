using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] private int width = 5;
    [SerializeField] private int height = 5;

    [SerializeField] private float cellSize = 64f;

    [SerializeField] private bool drawGizmosForScene = true;

    [SerializeField] private GameObject cellPrefab;

    void Start()
    {
        drawGizmosForScene = false;
        GenerateGrid();
    }

    void GenerateGrid()
    {
        float startX = -(width - 1) * cellSize / 2f;
        float startY = (height - 1) * cellSize / 2f;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                GameObject cell = Instantiate(cellPrefab, transform);

                RectTransform rect = cell.GetComponent<RectTransform>();

                rect.anchoredPosition = new Vector2(
                    startX + x * cellSize,
                    startY - y * cellSize
                );
            }
        }
    }

    void OnDrawGizmos()
    {
        if (!drawGizmosForScene) return;

        Gizmos.color = Color.green;

        float startX = -(width - 1) / 2f;
        float startY = (height - 1) / 2f;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 pos = new Vector3(startX + x, startY - y, 0);
                Gizmos.DrawWireCube(transform.position + pos, Vector3.one);
            }
        }
    }
}
