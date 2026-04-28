using UnityEngine;
using UnityEngine.InputSystem;

public class ArtefactsManager : MonoBehaviour
{
    [SerializeField] private ArtefactSlot[] slots;
    [SerializeField] private GameObject artefactPackPanel;

    private void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            artefactPackPanel.SetActive(true);
        }
    }

    public void AddArtefact(ArtefactData artefact)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (!slots[i].isOccupied)
            {
                slots[i].SetArtefact(artefact);
                break;
            }
        }
        Debug.Log($"Artifact added: {artefact.name}");
    }
    public bool ExistEmptySlots()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (!slots[i].isOccupied)
            {
                return true;
            }
        }
        return false;
    }

}
