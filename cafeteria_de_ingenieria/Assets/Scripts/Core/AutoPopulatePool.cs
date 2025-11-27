using UnityEngine;
using System.Collections.Generic;
public class AutoPopulatePool : MonoBehaviour
{
    [Header("Configuración Automática")]
    [Tooltip("Carpeta donde buscar prefabs (ej: Prefabs/Items, Prefabs/Skills, Prefabs/Upgrades)")]
    public string prefabsFolderPath = "Resources/Resources/Prefabs/Items";
    
    [Tooltip("Usar detección automática de carpeta basada en el nombre del GameObject")]
    public bool autoDetectFolderFromName = true;

    [Header("Configuración Manual (Opcional)")]
    public GameObject[] manualPrefabsToInclude;
    public bool autoPopulateOnStart = true;
    
    [Tooltip("Limpiar pool antes de llenarlo")]
    public bool clearBeforePopulate = true;
    public bool showDebugInfo = true;
    
    void Start()
    {
        if (autoDetectFolderFromName)
        {
            DetectFolderFromGameObjectName();
        }
        
        if (autoPopulateOnStart)
        {
            PopulatePool();
        }
    }
    
    private void DetectFolderFromGameObjectName()
    {
        string objectName = gameObject.name.ToLower();
        
        if (objectName.Contains("item"))
        {
            prefabsFolderPath = "Resources/Prefabs/Items";
        }
        else if (objectName.Contains("skill"))
        {
            prefabsFolderPath = "Resources/Prefabs/Skills";
        }
        else if (objectName.Contains("upgrade"))
        {
            prefabsFolderPath = "Resources/Prefabs/Upgrades";
        }
        
        if (showDebugInfo)
        {
            Debug.Log($"AutoPopulatePool [{gameObject.name}]: Auto-detectado path '{prefabsFolderPath}'");
        }
    }
    
    [ContextMenu("Populate Pool")]
    public void PopulatePool()
    {
        if (clearBeforePopulate)
        {
            ClearPool();
        }
        
        GameObject[] prefabsToUse = GetPrefabsToPopulate();
        
        if (prefabsToUse.Length == 0)
        {
            Debug.LogWarning($"AutoPopulatePool [{gameObject.name}]: No se encontraron prefabs para poblar el pool");
            return;
        }
        
        int populatedCount = 0;
        
        foreach (GameObject prefab in prefabsToUse)
        {
            if (prefab != null)
            {
                GameObject instance = Instantiate(prefab, transform);
                instance.name = prefab.name; // Quitar "(Clone)" del nombre
                instance.SetActive(false); // Mantener inactivo para que sea encontrado por GetComponentsInChildren(true)
                populatedCount++;
            }
            else
            {
                Debug.LogWarning($"AutoPopulatePool [{gameObject.name}]: Prefab nulo encontrado");
            }
        }
        
        if (showDebugInfo)
        {
            Debug.Log($"AutoPopulatePool [{gameObject.name}]: Pool populado con {populatedCount} prefabs desde '{prefabsFolderPath}'");
        }
    }

    private GameObject[] GetPrefabsToPopulate()
    {
        GameObject[] autoPrefabs = LoadPrefabsFromFolder();
        
        if (autoPrefabs.Length > 0)
        {
            return autoPrefabs;
        }
        
        if (manualPrefabsToInclude != null && manualPrefabsToInclude.Length > 0)
        {
            if (showDebugInfo)
            {
                Debug.Log($"AutoPopulatePool [{gameObject.name}]: Usando prefabs manuales como fallback");
            }
            return manualPrefabsToInclude;
        }
        
        return new GameObject[0];
    }
    
    private GameObject[] LoadPrefabsFromFolder()
    {
#if UNITY_EDITOR
        try
        {
            string fullPath = "Assets/" + prefabsFolderPath;
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:GameObject", new[] { fullPath });
            List<GameObject> foundPrefabs = new List<GameObject>();
            
            foreach (string guid in guids)
            {
                string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                
                if (prefab != null)
                {
                    foundPrefabs.Add(prefab);
                }
            }
            
            if (showDebugInfo && foundPrefabs.Count > 0)
            {
                Debug.Log($"AutoPopulatePool [{gameObject.name}]: Encontrados {foundPrefabs.Count} prefabs en '{fullPath}'");
            }
            
            return foundPrefabs.ToArray();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"AutoPopulatePool [{gameObject.name}]: Error cargando prefabs desde '{prefabsFolderPath}': {e.Message}");
            return new GameObject[0];
        }
#else
        // En builds, usar Resources si la carpeta está en Resources
        if (prefabsFolderPath.StartsWith("Resources/"))
        {
            string resourcePath = prefabsFolderPath.Substring("Resources/".Length);
            GameObject[] resourcePrefabs = Resources.LoadAll<GameObject>(resourcePath);
            
            if (showDebugInfo)
            {
                Debug.Log($"AutoPopulatePool [{gameObject.name}]: Cargados {resourcePrefabs.Length} prefabs desde Resources/{resourcePath}");
            }
            
            return resourcePrefabs;
        }
        else
        {
            Debug.LogWarning($"AutoPopulatePool [{gameObject.name}]: La carga automática solo funciona en el Editor o con carpetas Resources en builds");
            return new GameObject[0];
        }
#endif
    }
    
    [ContextMenu("Clear Pool")]
    public void ClearPool()
    {
        int childCount = transform.childCount;
        
        // Limpiar en orden inverso para evitar problemas de índices
        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            
            if (Application.isPlaying)
            {
                Destroy(child.gameObject);
            }
            else
            {
                DestroyImmediate(child.gameObject);
            }
        }
        
        if (showDebugInfo && childCount > 0)
        {
            Debug.Log($"AutoPopulatePool [{gameObject.name}]: Limpiados {childCount} objetos del pool");
        }
    }
    
    public PoolInfo GetPoolInfo()
    {
        return new PoolInfo
        {
            poolName = gameObject.name,
            detectedPath = prefabsFolderPath,
            configuredPrefabs = manualPrefabsToInclude?.Length ?? 0,
            currentChildren = transform.childCount,
            isPopulated = transform.childCount > 0,
            isAutoDetectionEnabled = autoDetectFolderFromName
        };
    }
    
    [System.Serializable]
    public struct PoolInfo
    {
        public string poolName;
        public string detectedPath;
        public int configuredPrefabs;
        public int currentChildren;
        public bool isPopulated;
        public bool isAutoDetectionEnabled;
        
        public override string ToString()
        {
            return $"Pool '{poolName}': {currentChildren} objetos, Path: '{detectedPath}', AutoDetect: {isAutoDetectionEnabled}";
        }
    }
    
    // Métodos para debugging en el Inspector
    void OnValidate()
    {
        // Auto-detectar carpeta si está habilitado
        if (autoDetectFolderFromName && Application.isPlaying == false)
        {
            DetectFolderFromGameObjectName();
        }
    }
    
    [ContextMenu("Test Auto Detection")]
    public void TestAutoDetection()
    {
        DetectFolderFromGameObjectName();
        Debug.Log($"AutoPopulatePool [{gameObject.name}]: Path detectado: '{prefabsFolderPath}'");
        
#if UNITY_EDITOR
        GameObject[] foundPrefabs = LoadPrefabsFromFolder();
        Debug.Log($"AutoPopulatePool [{gameObject.name}]: Encontrados {foundPrefabs.Length} prefabs");
        
        for (int i = 0; i < foundPrefabs.Length; i++)
        {
            Debug.Log($"  {i + 1}. {foundPrefabs[i].name}");
        }
#endif
    }
}