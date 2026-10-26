using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerInventoryController : MonoBehaviour
{
    [SerializeField] private Image[] inventorySlotArr;
    private List<BasePickUp> pickUpList = new List<BasePickUp>();
    public void AddPickUp(BasePickUp target)
    {
        pickUpList.Add(target);
        for(int i = 0; i < inventorySlotArr.Length; i++)
        {
            if (inventorySlotArr[i].sprite == null)
            {
                inventorySlotArr[i].sprite = target.GetSprite;
                inventorySlotArr[i].enabled = true;
                return;
            }
        }
    }
    public void RemovePickUp(BasePickUp target)
    {
        pickUpList.Remove(target);
        for (int i = 0; i < inventorySlotArr.Length; i++)
        {
            if (inventorySlotArr[i].sprite == target.GetSprite)
            {
                inventorySlotArr[i].sprite = null;
                inventorySlotArr[i].enabled = false;
                return;
            }
        }
    }
    public BasePickUp TryGetPickUpInventory(BasePickUp target)
    {
        foreach (BasePickUp item in pickUpList)
        {
            if(item == target)
            {
                return item;
            }
        }
        return null;
    }
}
