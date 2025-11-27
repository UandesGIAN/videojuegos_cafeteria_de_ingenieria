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

    void Update()
    {
        // Cheat: CTRL + I + NUM
        if (Input.GetKey(KeyCode.LeftControl) &&
        Input.GetKey(KeyCode.I))
        {
            int number = GetNumberPressed();

            if (number != -1)
            {
                Debug.Log("Número presionado: " + number);
                GiveRandomItems(number);
            }
        }
    }

    // Aux
    private int GetNumberPressed()
    {
        for (int i = 1; i <= 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))   // Teclado superior
                return i;

            if (Input.GetKeyDown(KeyCode.Keypad0 + i))  // Teclado numérico
                return i;
        }

        return -1; // Nada presionado
    }

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

    public void GiveRandomItems(int amount)
    {
        Item[] allItems = itemPoolContainer.GetComponentsInChildren<Item>(true);

        if (allItems.Length == 0)
        {
            Debug.LogWarning("No hay items en el itemPoolContainer.");
            return;
        }

        for (int i = 0; i < amount; i++)
        {
            Item randomItem = allItems[Random.Range(0, allItems.Length)];
            AddItemToPlayer(randomItem.itemName);
        }

        Debug.Log($"Se dieron {amount} item(s) aleatorio(s) a {player.fightername}");
    }
}
