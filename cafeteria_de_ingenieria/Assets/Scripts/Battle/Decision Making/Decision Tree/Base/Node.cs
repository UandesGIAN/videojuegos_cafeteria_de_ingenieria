using UnityEngine;


public abstract class Node
{
    // debe ser publica para que nodo padre pueda acceder al Decide de nodos hijos
    public abstract Node Decide(GameObject obj, GameObject world);
}
