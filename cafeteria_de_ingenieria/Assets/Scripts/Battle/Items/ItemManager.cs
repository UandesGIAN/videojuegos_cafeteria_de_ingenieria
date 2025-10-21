using UnityEngine;
using System.Linq;

public class ItemManager : MonoBehaviour
{
    [Header("Item Pool")]
    public GameObject itemPoolContainer;

    [Header("Player Configuration")]
    public FighterStats player;

    [Header("Battle UI Reference")]
    public BattleUI battleUI;

    public void AddItemToPlayer(string itemName)
    {
        Transform playerItemsContainer = player.transform.Find("Items");
        if (playerItemsContainer == null) return;

        Item[] allItems = itemPoolContainer.GetComponentsInChildren<Item>(true);
        Item itemTemplate = allItems.FirstOrDefault(i => i.itemName == itemName);

        // Instanciar el item
        GameObject itemCopy = Instantiate(itemTemplate.gameObject, playerItemsContainer);
        itemCopy.name = itemName;
        itemCopy.SetActive(true);

        // Añadir a la lista interna del jugador
        Item itemComponent = itemCopy.GetComponent<Item>();
        if (itemComponent != null)
        {
            player.AddItem(itemComponent);
        }

        // Refrescar HUD
        battleUI.SetupItemList(player);

        Debug.Log($"Item '{itemName}' agregado a {player.fightername}");
        Debug.Log($"'{player.GetItemCount(itemName)}'");
    }

    public void RemoveItemFromPlayerInstance(Item itemInstance)
    {
        if (player == null || itemInstance == null) return;

        Transform playerItemsContainer = player.transform.Find("Items");
        if (playerItemsContainer == null) return;

        // Quitar de la lista interna
        player.RemoveItem(itemInstance);

        // Eliminar la instancia visual
        Destroy(itemInstance.gameObject);

        // Refrescar HUD
        battleUI.SetupItemList(player);

        Debug.Log($"Item '{itemInstance.itemName}' usado y removido de {player.fightername}");
    }
}
