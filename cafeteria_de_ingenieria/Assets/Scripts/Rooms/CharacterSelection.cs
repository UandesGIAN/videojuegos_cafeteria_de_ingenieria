using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CharacterSelection : MonoBehaviour
{
    [Header("Referencias desde el Editor")]
    public List<Button> characterButtons; // Los botones de personajes
    public FighterStats playerStats;      // El jugador
    public List<Skill> initialSkills;     // Habilidades base

    // Estructura interna para definir estadísticas
    private class CharacterData
    {
        public string name;
        public ElementType elementType;
        public float Health;
        public float IQ;
        public float Attack;
        public float IQattack;
        public float PhysicalArmor;
        public float IQArmor;
        public Skill baseSkill;

        public CharacterData(
            string name,
            ElementType elementType,
            float Health,
            float IQ,
            float Attack,
            float IQattack,
            float PhysicalArmor,
            float IQArmor,
            Skill baseSkill)
        {
            this.name = name;
            this.elementType = elementType;
            this.Health = Health;
            this.IQ = IQ;
            this.Attack = Attack;
            this.IQattack = IQattack;
            this.PhysicalArmor = PhysicalArmor;
            this.IQArmor = IQArmor;
            this.baseSkill = baseSkill;
        }
    }

    // Diccionario interno de personajes
    private Dictionary<string, CharacterData> characterDataDict;

    private void Awake()
    {
        InicializarPersonajes();
        ConfigurarBotones();
    }

    private void InicializarPersonajes()
    {
        // Definir todos los personajes
        characterDataDict = new Dictionary<string, CharacterData>()
        {
            {
                "BORJA",
                new CharacterData(
                "BORJA",
                ElementType.CEMENTO,
                220, 110, 10, 15, 8, 4, initialSkills[0]
                )
            },
            {
                "VICE",
                new CharacterData(
                    "VICE",
                    ElementType.PLANTA,
                    170, 120, 20, 18, 6, 3, initialSkills[1]
                )
            },
            {
                "GIAN",
                new CharacterData(
                    "GIAN",
                    ElementType.ELECTRONICO,
                    150, 150, 20, 25, 5, 7, initialSkills[2] 
                )
            },
            {
                "PEDE",
                new CharacterData(
                    "PEDE",
                    ElementType.FUEGO,
                    180, 130, 10, 22, 6, 5, initialSkills[3] 
                )
            },
            {
                "LUCAS",
                new CharacterData(
                    "LUCAS",
                    ElementType.AGUA,
                    200, 140, 20, 20, 4, 6, initialSkills[4] 
                )
            }
        };
    }

    private void ConfigurarBotones()
    {
        foreach (var button in characterButtons)
        {
            // Agregar componente de sonido si no existe
            if (button.GetComponent<UIButtonSound>() == null)
            {
                UIButtonSound buttonSound = button.gameObject.AddComponent<UIButtonSound>();
                buttonSound.soundType = UIButtonSound.ButtonSoundType.Confirm;
            }
            
            // Buscar los elementos hijos "Title" y "Icon" en el botón
            TMP_Text titleText = button.transform.Find("title")?.GetComponent<TMP_Text>();
            Image iconImage = button.transform.Find("Icon")?.GetComponent<Image>();

            if (titleText == null)
            {
                Debug.LogWarning($"El botón {button.name} no tiene 'title'.");
                continue;
            }

            string characterName = titleText.text.ToUpper().Trim();

            if (!characterDataDict.ContainsKey(characterName))
            {
                Debug.LogWarning($"No hay datos definidos para el personaje '{characterName}'.");
                continue;
            }

            // Capturar el nombre dentro de la lambda
            string capturedName = characterName;

            // Eliminar listeners anteriores
            button.onClick.RemoveAllListeners();

            // Asignar listener
            button.onClick.AddListener(() => SeleccionarPersonaje(capturedName, iconImage));
        }
    }

    private void SeleccionarPersonaje(string characterName, Image icon)
    {
        if (!characterDataDict.ContainsKey(characterName))
        {
            Debug.LogError($"Intento de seleccionar personaje inexistente: {characterName}");
            return;
        }

        CharacterData data = characterDataDict[characterName];

        // Asignar valores al FighterStats del jugador
        if (playerStats != null)
        {
            playerStats.fightername = data.name;
            playerStats.elementType = data.elementType;
            playerStats.health = data.Health;
            playerStats.startHealth = data.Health;
            playerStats.IQ = data.IQ;
            playerStats.startIQ = data.IQ;
            playerStats.attack = data.Attack;
            playerStats.IQattack = data.IQattack;
            playerStats.physicalArmor = data.PhysicalArmor;
            playerStats.IQArmor = data.IQArmor;
            playerStats.img = icon != null ? icon.sprite : null;
            
            Debug.Log($"📋 Stats asignadas: Attack={data.Attack}, IQattack={data.IQattack}");
            
            // Habilidad base
            if (data.baseSkill != null)
            {
                Transform skillContainer = playerStats.transform.Find("Skills");
                if (skillContainer != null)
                {
                    Skill skillCopy = Instantiate(data.baseSkill, skillContainer);
                    skillCopy.name = data.baseSkill.name;
                    skillCopy.gameObject.SetActive(true);

                    playerStats.RefreshSkills();
                    Debug.Log($"Skill '{data.baseSkill.skillName}' asignada a {playerStats.fightername}");
                }
                else
                {
                    Debug.LogWarning($"No se encontró el objeto 'Skills' en {playerStats.fightername}");
                }
            }

            Debug.Log($"✅ Personaje seleccionado: {data.name}");
            RoomManager.Instance.GoToNextRoom();
        }
        else
        {
            Debug.LogError("CharacterSelection: No se ha asignado el FighterStats del jugador.");
        }
    }
}
