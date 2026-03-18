using UnityEngine;
using UnityEngine.UI;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance;

    [HideInInspector] public bool plantsCanEarn = false;
    [HideInInspector] public bool canPressBtns = true;
    [HideInInspector] public bool canZoomCam = true;
    [SerializeField] private Button[] buttons;

    private void Awake()
    {
        Instance = this;
        plantsCanEarn = false;
    }

    private void Update()
    {
        if (canPressBtns)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].interactable = true;
            }
        }
        else
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].interactable = false;
            }
        }
    }
}
