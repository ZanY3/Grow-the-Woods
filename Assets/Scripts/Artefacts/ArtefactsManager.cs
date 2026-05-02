using UnityEngine;
using UnityEngine.InputSystem;

public class ArtefactsManager : MonoBehaviour
{
    [SerializeField] private ArtefactSlot[] slots;
    [SerializeField] private GameObject artefactPackPanel;
    [SerializeField] private ShopManager shopManager;

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

        StatsManager.Instance.ApplyArtefact(artefact);

        // Refresh displayed prices immediately if it's a discount artefact
        if (artefact.type == ArtefactData.Type.ShopDiscount)
            shopManager.RefreshAllPrices();
    }

    // Called from ArtefactSlot when player deletes an artefact
    public void OnArtefactRemoved(ArtefactData artefact)
    {
        if (artefact.type == ArtefactData.Type.ShopDiscount)
            shopManager.RefreshAllPrices();
    }

    public bool ExistEmptySlots()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (!slots[i].isOccupied)
                return true;
        }
        return false;
    }
}