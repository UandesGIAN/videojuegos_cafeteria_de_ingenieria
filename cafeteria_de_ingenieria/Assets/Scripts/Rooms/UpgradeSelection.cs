using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeSelection : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject[] upgradeButtons;
    public TMP_Text[] upgradeNames;
    public TMP_Text[] upgradeDescriptions;
    public Image[] upgradeIcons;
    public RoomController currentRoom;

    [Header("Pool de mejoras posibles")]
    public Upgrade[] allUpgrades;
    private Upgrade[] selectedUpgrades;

    public void ShowUpgrades()
    {
        gameObject.SetActive(true);
        /*
        // Seleccionar 3 mejoras aleatorias
        selectedUpgrades = new Upgrade[3];
        for (int i = 0; i < 3; i++)
        {
            selectedUpgrades[i] = allUpgrades[Random.Range(0, allUpgrades.Length)];

            upgradeButtons[i].SetActive(true);
            upgradeNames[i].text = selectedUpgrades[i].upgradeName;
            upgradeDescriptions[i].text = selectedUpgrades[i].description;
            upgradeIcons[i].sprite = selectedUpgrades[i].icon;

            int index = i;
            upgradeButtons[i].GetComponent<Button>().onClick.RemoveAllListeners();
            upgradeButtons[i].GetComponent<Button>().onClick.AddListener(() => SelectUpgrade(index));
        }*/

        // 3 mejoras de prueba para probar la lógica
        selectedUpgrades = new Upgrade[3];
        for (int i = 0; i < 3; i++)
        {
            selectedUpgrades[i] = ScriptableObject.CreateInstance<Upgrade>();
            selectedUpgrades[i].upgradeName = "Prueba " + (i + 1);
            selectedUpgrades[i].description = "Descripción de prueba " + (i + 1);
            selectedUpgrades[i].icon = null; // o asigna un sprite de prueba

            upgradeButtons[i].SetActive(true);
            upgradeNames[i].text = selectedUpgrades[i].upgradeName;
            upgradeDescriptions[i].text = selectedUpgrades[i].description;
            upgradeIcons[i].sprite = selectedUpgrades[i].icon;

            int index = i;
            upgradeButtons[i].GetComponent<Button>().onClick.RemoveAllListeners();
            upgradeButtons[i].GetComponent<Button>().onClick.AddListener(() => SelectUpgrade(index));
        }
    }

    public void SelectUpgrade(int index)
    {
        var chosenUpgrade = selectedUpgrades[index];
        //FighterStats playerStats = BattleManager.Instance.player.GetComponent<FighterStats>();

        //chosenUpgrade.Apply(playerStats);

        Debug.Log($"Seleccionada mejora: {chosenUpgrade.upgradeName}");

        gameObject.SetActive(false);
        currentRoom.OnUpgradeSelected(); // siguiente paso → seleccionar sala
    }
}

