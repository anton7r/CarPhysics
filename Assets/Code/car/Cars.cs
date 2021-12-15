using UnityEngine;
using System.Collections.Generic;

public class Cars : MonoBehaviourX
{
    public List<GameObject> CarList = new List<GameObject>();
    private static Dictionary<string, GameObject> cars = new Dictionary<string, GameObject>();
    private static List<GameObject> list = new List<GameObject>();

    //Loads cars into memory then kills it self

    void Awake() {
        cars.Clear();
        list.Clear();
        list = CarList;

        foreach(GameObject car in list) {
            cars.Add(car.name, car);
        }
        Destroy(gameObject);
    }

    public static GameObject instantiateCar(string prefabName, Vector3 pos) {
        GameObject carPrefab;
        cars.TryGetValue(prefabName, out carPrefab);
        if(carPrefab != null) {
            return Instantiate(carPrefab, pos, Quaternion.identity);
        }
        return null;
    }

    public static void assureLoaded() {
        if(list.Count == 0) {
            //list not loaded
            //try load list

            Instantiate(LoadPrefab("CarList"), new Vector3(), new Quaternion());
        }
    }

}