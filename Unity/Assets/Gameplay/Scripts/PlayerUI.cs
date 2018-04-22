using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerUI : MonoBehaviour
{
    [SerializeField] 
    private GameObject[] inventorySlotUI = new GameObject[5];

    [SerializeField] 
    private Sprite defaultSprite;

    [SerializeField]
    private Camera camera;

    private int currentActiveSlotIndex = 0;

    public void Start()
    {
        SetActiveUISlot(null, 0);
    }

    // - enlight the active current slot item
    public void SetActiveUISlot(BattleRobo.PlayerObjectScript obj, int index)
    {
        inventorySlotUI[currentActiveSlotIndex].transform.GetChild(0).GetComponent<Image>().color = new Color32(255, 255, 255, 96);
        inventorySlotUI[index].transform.GetChild(0).GetComponent<Image>().color = new Color32(255, 0, 0, 100);
        currentActiveSlotIndex = index;
    }

    // - set image associated to the object in the UI slot item
    public void SetItemUISlot(BattleRobo.PlayerObjectScript obj, int index)
    {
        inventorySlotUI[index].transform.GetChild(0).GetComponent<Image>().sprite = (obj != null) ? obj.GetSprite() : defaultSprite;
    }

    public Camera GetCamera()
    {
        return camera;
    }
}