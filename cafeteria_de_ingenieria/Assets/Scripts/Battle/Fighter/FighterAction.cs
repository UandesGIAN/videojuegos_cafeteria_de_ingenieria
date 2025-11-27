using System.Linq;
using UnityEngine;


public class FighterAction : MonoBehaviour
{
    // clase usada para seleccionar entre ataque melee y habilidades, tanto para el prota como enemigos 

    private GameObject player;
    private GameObject enemy;
    private FighterStats ownerStats;

    private ObjectDecisionNode rootNode; // nodo inicial de Decision Tree

    [SerializeField]
    private GameObject meleePrefab;


    public void Awake()
    {
        player = GameObject.FindGameObjectWithTag(BattleConstants.CharacterRole.Player.ToString());
        enemy = GameObject.FindGameObjectWithTag(BattleConstants.CharacterRole.Enemy.ToString());

        ownerStats = gameObject.GetComponent<FighterStats>();

        // si el dueño de este FighterAction script es un Jefe, se inicializa el Decision Tree
        if (ownerStats.fighterHierarchy == FighterHierarchy.Boss)
        {
            InitializeDecisionTree();
            Debug.Log("Decision Tree del Jefe inicializado.");
        }
    }

    public void SelectOption(string option_name, Skill playerSkill = null)
    {
        // recibe el nombre de la opcion elegida en el ActionMenu desde los metodos llamados por BattleManager.ActivateOption()
        GameObject target = gameObject.CompareTag(BattleConstants.CharacterRole.Player.ToString()) ? enemy : player;
        
        if (target == null)
        {
            Debug.LogWarning("Target ya no existe, no se puede atacar.");
            return; // No hay objetivo
        }

        Debug.Log("Current Fighter: " + gameObject.tag);
        Debug.Log("Target is " + target.GetComponent<FighterStats>().fightername + ":");

        // ataque melee (normal)
        if (option_name.CompareTo(BattleConstants.MenuAttackOptions.Melee.ToString()) == 0)
            meleePrefab.GetComponent<AttackScript>().Attack(target: target);
        
        // skill (por ahora randomamente)
        else if (option_name.CompareTo(BattleConstants.MenuAttackOptions.Skill.ToString()) == 0)
        {
            // si target es el jugador, entonces enemigo (solo boss xd) usa una skill al azar
            if (target.CompareTag("Player"))
                meleePrefab.GetComponent<AttackScript>().UseSkillRandomly(target: target);
            else // e.o.c., jugador esta usando habilidad; skill no es nulo
                meleePrefab.GetComponent<AttackScript>().UseSkill(skill: playerSkill, target: target);
        }
    }

    // no existen Decision Trees especificos para jefes; solo hay uno global!
    private void InitializeDecisionTree()
    {
        // evaluadores
        IsBossHealthBelowXPercentEvaluator ownerHealthEvaluator = new IsBossHealthBelowXPercentEvaluator(targetHealthPercent: 0.3f);
        DoesBossHaveElixirEvaluator ownerElixirEvaluator = new DoesBossHaveElixirEvaluator();

        IsBossIQMeterBelowXPercentEvaluator ownerIQMeterEvaluator = new IsBossIQMeterBelowXPercentEvaluator(targetIQPercent: 0.3f);
        DoesBossHaveItemOfTypeEvaluator ownerIQItemEvaluator = new DoesBossHaveItemOfTypeEvaluator(itemType: StatsType.IQ);


        // decisiones
        ObjectDecisionNode isOwnerHealthBelow30Percent = new ObjectDecisionNode(evaluator: ownerHealthEvaluator);
        ObjectDecisionNode doesOwnerHaveElixir = new ObjectDecisionNode(evaluator: ownerElixirEvaluator);

        ObjectDecisionNode isOwnerIQBelow30Percent = new ObjectDecisionNode(evaluator: ownerIQMeterEvaluator);
        ObjectDecisionNode doesOwnerHaveIQItem = new ObjectDecisionNode(evaluator: ownerIQItemEvaluator);


        // acciones
        ActionNode skillAction = new ActionNode(name: BattleConstants.MenuAttackOptions.Skill.ToString());
        ActionNode elixirItemAction = new ActionNode(name: "Elixir");
        ActionNode iQItemAction = new ActionNode(name: "IQ Item");


        // construccion del arbol!!
        isOwnerHealthBelow30Percent.NoNode = isOwnerIQBelow30Percent;
        isOwnerHealthBelow30Percent.YesNode = doesOwnerHaveElixir;

        doesOwnerHaveElixir.NoNode = isOwnerIQBelow30Percent;
        doesOwnerHaveElixir.YesNode = elixirItemAction;

        isOwnerIQBelow30Percent.NoNode = skillAction;
        isOwnerIQBelow30Percent.YesNode = doesOwnerHaveIQItem;

        doesOwnerHaveIQItem.NoNode = skillAction;
        doesOwnerHaveIQItem.YesNode = iQItemAction;


        // definir root
        rootNode = isOwnerHealthBelow30Percent;
    }

    // llamado desde TurnController
    public void EvaluateDecisionTree()
    {
        ActionNode resultNode = (ActionNode) rootNode.Decide(obj: this.gameObject, world: new GameObject());

        switch (resultNode.name)
        {
            case "Elixir":
                UseItem(itemName: "Elixir");
                break;
            
            case "IQ Item":
                UseItemOfType(itemType: StatsType.IQ);
                break;
            
            case "Skill":
                SelectOption(BattleConstants.MenuAttackOptions.Skill.ToString());
                break;            
        }
    }

    private void UseItem(string itemName)
    {
        // no es necesario chequear si el item es null, porque el evaluador del Decision Tree ya lo hizo
        Debug.Log($"{ownerStats.name} está bajo de vida! Usando {itemName}.");
        
        Item item = ownerStats.GetItems().FirstOrDefault(item => item.itemName == itemName);

        // Establecer usuario del item
        item.SetUser(ownerStats);
        
        item.Run(); 

        DialogueManager.Instance.ShowDialogue($"{ownerStats.fightername} usó {itemName}!");
        ownerStats.RemoveItem(item);
    }

    private void UseItemOfType(StatsType itemType)
    {
        Debug.Log($"{ownerStats.name} está bajo de IQ! Usando item de {itemType}.");

        Item item = ownerStats.GetItems().FirstOrDefault(item => item is StatsModItems statsModItem && statsModItem.statType == itemType);

        // Establecer usuario del item
        item.SetUser(ownerStats);
        item.Run(); 

        DialogueManager.Instance.ShowDialogue($"{ownerStats.fightername} usó {item.itemName}!");
        ownerStats.RemoveItem(item);
    }

    public GameObject GetEnemy()
    {
        return this.enemy;
    }

    public void SetEnemy(GameObject enemy)
    {
        this.enemy = enemy;
    }
}
