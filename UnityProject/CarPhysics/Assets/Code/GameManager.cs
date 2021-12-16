using UnityEngine;

public class GameManager : MonoBehaviourX
{

    [SerializeField]
    public TMPro.TMP_Text txtSpeed;
    [SerializeField]
    public TMPro.TMP_Text txtRPM;
    [SerializeField]
    public TMPro.TMP_Text txtGear;
    [SerializeField]
    public TMPro.TMP_Text txtFPS;

    static GameManager INSTANCE;

    private static bool showFPS = false;
    private float deltaTime = 0;

    private BaseCarController playerCar;

    void Awake()
    {
        INSTANCE = this;
        Cars.assureLoaded();
        loadPrefabs();
        setShowFps(showFPS);
    }

    private BaseCarController loadCar(string prefabName) {
        return Cars.instantiateCar(prefabName, new Vector3(0, 2, 0)).GetComponent<BaseCarController>();
    }

    private void loadPrefabs()
    {
        Wheel.wheelPrefab = LoadPrefab("Wheel");
        Wheel.smokePrefab = LoadPrefab("TireSmoke");
    }


    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        playerCar = loadCar("TeslaModel3");
        CameraManager.setCameraTargetPosition(playerCar.getCameraTargetPosition());
        CameraManager.setCameraLookTarget(playerCar.getCameraLookTarget());
    }

    public static void UpdateSpeed(float speed)
    {
        INSTANCE.txtSpeed.text = Mathf.Round(speed) + " KM/H";
    }

    public static void UpdateRPM(float rpm) 
    {
        INSTANCE.txtRPM.text = Mathf.Round(rpm) + " RPM";
    }

    public static void UpdateGear(int gear)
    {
        INSTANCE.txtGear.text = (gear + 1).ToString() + " Gear";
    }

    void FixedUpdate() {
        if(playerCar.transform.position.y < -100) {
            playerCar.transform.position = new Vector3(0, 2, 0);
            playerCar.transform.rotation = Quaternion.identity;
            Rigidbody rb = playerCar.GetComponent<Rigidbody>();
            rb.angularVelocity = rb.velocity = new Vector3(0, 0, 0);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown("f3"))
        {
            setShowFps(!showFPS);
        }

        if (showFPS)
        {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            float fps = 1.0f / deltaTime;
            txtFPS.text = Mathf.Ceil(fps).ToString() + " FPS";
        }

    }

    void toggleShowFps()
    {
        setShowFps(!showFPS);
    }

    void setShowFps(bool state)
    {
        showFPS = state;
        txtFPS.gameObject.SetActive(state);
    }

}
