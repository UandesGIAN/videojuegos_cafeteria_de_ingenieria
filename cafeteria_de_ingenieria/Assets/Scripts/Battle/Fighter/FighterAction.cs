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

    public void SelectOption(string option_name)
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
            meleePrefab.GetComponent<AttackScript>().Attack(target);
        
        // skill (por ahora randomamente)
        else if (option_name.CompareTo(BattleConstants.MenuAttackOptions.Skill.ToString()) == 0)
            meleePrefab.GetComponent<AttackScript>().UseSkillRandomly(target);
    }

    // por ahora, no existen Decision Trees especificos para jefes; solo hay uno global! pero hacerlos especificos es parte del balanceo ejejeje
    private void InitializeDecisionTree()
    {
        // evaluadores
        IsBossHealthBelowXPercentEvaluator ownerHealthEvaluator = new IsBossHealthBelowXPercentEvaluator(targetHealthPercent: 0.3f);
        DoesBossHaveElixirEvaluator ownerElixirEvaluator = new DoesBossHaveElixirEvaluator();

        // decisiones
        ObjectDecisionNode isOwnerHealthBelow30Percent = new ObjectDecisionNode(evaluator: ownerHealthEvaluator);
        ObjectDecisionNode doesOwnerHaveElixir = new ObjectDecisionNode(evaluator: ownerElixirEvaluator);

        // acciones
        ActionNode skillAction = new ActionNode(name: BattleConstants.MenuAttackOptions.Skill.ToString());
        ActionNode elixirItemAction = new ActionNode(name: "Elixir");

        // construccion del arbol!!
        isOwnerHealthBelow30Percent.NoNode = skillAction;
        isOwnerHealthBelow30Percent.YesNode = doesOwnerHaveElixir;

        doesOwnerHaveElixir.NoNode = skillAction;
        doesOwnerHaveElixir.YesNode = elixirItemAction;

        // definir root
        rootNode = isOwnerHealthBelow30Percent;
    }

    // llamado desde TurnController
    public void EvaluateDecisionTree()
    {
        ActionNode resultNode = (ActionNode) rootNode.Decide(obj: this.gameObject, world: new GameObject());

        switch (resultNode.name)
        {
            case "Skill":
                SelectOption(BattleConstants.MenuAttackOptions.Skill.ToString());
                break;

            case "Elixir":
                UseItem(itemName: "Elixir");
                break;
        }
    }

    private void UseItem(string itemName)
    {
        // no es necesario chequear si el item es null, porque el evaluador del Decision Tree ya lo hizo
        Debug.Log($"{ownerStats.name} está bajo de vida! Usando Elixir.");

        Item item = ownerStats.GetItems().FirstOrDefault(item => item.itemName == itemName);

        // Establecer usuario del item
        item.SetUser(ownerStats);
        
        item.Run(); 
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
