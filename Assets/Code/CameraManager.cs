using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager INSTANCE;

    public GameObject targetPosition;
    public GameObject lookTarget; 

    private float speed = 15;

    void Awake() {
        INSTANCE = this;
    }

    void FixedUpdate()
    {
        if(targetPosition == null) {
            return;
        }

        follow();
        /*
        if(Input.GetMouseButton(1)) {
            Debug.Log("CLICK");
        }
        */
    }

    void follow() {
        gameObject.transform.position = Vector3.Lerp(transform.position, targetPosition.transform.position, Time.deltaTime * speed);
        gameObject.transform.LookAt(lookTarget.gameObject.transform.position);
    }

    public static void setCameraTargetPosition(GameObject target) {
        INSTANCE.targetPosition = target;
    }

    public static void setCameraLookTarget(GameObject lookTarget) {
        INSTANCE.lookTarget = lookTarget;
    }

}
