using UnityEngine;
using static Enums;

[CreateAssetMenu(fileName = "ShopItemData", menuName = "Data/ShopItemData")]
public class ShopItemData : ScriptableObject {
    public ShopItemType shopItemType;
    public string itemID;
    public string itemName;
    public Sprite itemShopImage;
    public GameObject shopItemPrefab;
}
