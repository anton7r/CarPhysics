using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoBehaviourX : MonoBehaviour
{

    public static UnityEngine.Object LoadPrefab(string fileName) {
        return Resources.Load<GameObject>("Prefabs/" + fileName);
    }

    public static GameObject InstantiateAsChild(GameObject parentGameObject, UnityEngine.Object prefab, Vector3 position, Quaternion rotation) {
        GameObject obj = Instantiate(prefab, position, rotation) as GameObject;
        obj.transform.parent = parentGameObject.transform;
        return obj;
    }

}
