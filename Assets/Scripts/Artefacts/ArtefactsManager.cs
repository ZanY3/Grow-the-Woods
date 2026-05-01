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
        if(artefact.type == ArtefactData.Type.CoinMultiplier)
        {
            StatsManager.Instance.coinMultiplier += artefact.value;
        }
        else if (artefact.type == ArtefactData.Type.CoinAdder)
        {
            StatsManager.Instance.coinAdder += (int)artefact.value;
        }
        else if (artefact.type == ArtefactData.Type.ShopDiscount)
        {
            StatsManager.Instance.shopDiscount += artefact.value;
        }
        else if(artefact.type == ArtefactData.Type.ChanceUpgrader)
        {
            StatsManager.Instance.chanceAdder += (int)artefact.value;
        }
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
