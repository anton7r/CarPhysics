using UnityEngine;

public class Wheel
{
    public static UnityEngine.Object wheelPrefab;
    public static UnityEngine.Object smokePrefab;

    public WheelCollider collider;
    public GameObject tire;
    public GameObject smoke;
    public ParticleSystem smokeParticleSystem;
    private static Quaternion rot = new Quaternion(0f, 0f, 0f, 1);

    private float defaultSideStiffness = 2f;
    private float driftSideStiffness = 1.5f;

    public Wheel(GameObject parent, WheelCollider collider)
    {
        this.collider = collider;

        Vector3 _pos;
        Quaternion _quat;

        collider.GetWorldPose(out _pos, out _quat);
        tire = MonoBehaviourX.InstantiateAsChild(parent, wheelPrefab, _pos, _quat);
        smoke = MonoBehaviourX.InstantiateAsChild(parent, smokePrefab, _pos, rot);

        smokeParticleSystem = smoke.GetComponent<ParticleSystem>();
        defaultSideStiffness = collider.sidewaysFriction.stiffness;
        driftSideStiffness = defaultSideStiffness * 0.75f;
    }

    public void Update()
    {
        Vector3 _pos = tire.transform.position;
        Quaternion _quat = tire.transform.rotation;

        collider.GetWorldPose(out _pos, out _quat);

        tire.transform.position = _pos;
        tire.transform.rotation = _quat;
        smoke.transform.position = _pos - new Vector3(0, collider.radius, 0);

        WheelHit wheelHit;
        collider.GetGroundHit(out wheelHit);
        //Debug.Log(wheelHit.sidewaysSlip);

        var em = smokeParticleSystem.emission;
        float rate = (wheelHit.forwardSlip * 300) + (wheelHit.sidewaysSlip * 300) + (collider.brakeTorque / 100);
        if (rate < 10)
        {
            rate = 0;
        }


        float sideStiff = defaultSideStiffness;
        if (collider.brakeTorque > 1000)
        {
            //Debug.Log("Drifting");
            sideStiff = driftSideStiffness;
        }
        float prevStiff = getSideStiffness();
        setSideStiffness(Mathf.Lerp(prevStiff, sideStiff, prevStiff > sideStiff ? 0.1f :  0.01f));
        //Debug.Log(getSideStiffness());
        em.rateOverTime = rate;
    }

    float getSideStiffness() {
        return collider.sidewaysFriction.stiffness;
    }

    void setSideStiffness(float newStiffness)
    {
        WheelFrictionCurve frictionCurve = collider.sidewaysFriction;
        frictionCurve.stiffness = newStiffness;
        collider.sidewaysFriction = frictionCurve;
    }
}