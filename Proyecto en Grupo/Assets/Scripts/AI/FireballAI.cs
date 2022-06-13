using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using weka.core;

public class FireballAI : PredictionAI
{
    private Transform target;

    const float incHeightFactor = 2f;

    private bool setToLaunch = false;
    private float launchTimer = 0f;

    public float destroyTime = 8f;
    private float destroyTimer = 0f;

    protected override void Start()
    {
        modelFileName = "fireball_ai_model.model";
        base.Start(); // Cargar modelo
    }

    // Obtiene la Fuerza a aplicar en el eje Y aplicando la formula de lanzamiento vertical y la segunda ley de Newton
    private float CalculateFY(float mass, float targetHeight)
    {
        if (targetHeight >= 2)
            return mass * Mathf.Sqrt(targetHeight * 2.0f * 9.81f * incHeightFactor);
        else
            return mass * Mathf.Sqrt(2f * 2.0f * 9.81f * incHeightFactor);
    }

    // Predecir y saltar
    public override void PredictAndExecute()
    {
        if (target != null)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            Vector3 targetPosition = target.position;
            float platformHeight = target.GetComponent<Collider>().bounds.size.y; //Tenemos en cuenta la altuar del objetivo
            float height = (targetPosition.y + (platformHeight/2f)) - (transform.position.y - (target.GetComponent<Collider>().bounds.size.y / 2f));
            float distance = Mathf.Sqrt(Mathf.Pow(Mathf.Abs(targetPosition.x - transform.position.x), 2f) + Mathf.Pow(Mathf.Abs(targetPosition.z - transform.position.z), 2f));
            float fY = CalculateFY(rb.mass, height);

            Instance instance = new Instance(trainingDataset.numAttributes());
            instance.setDataset(trainingDataset);
            instance.setValue(1, fY);
            instance.setValue(2, height);
            instance.setValue(3, distance);
            float fZ = (float) Predict(instance);

            rb.AddRelativeForce(new Vector3(0, fY, fZ), ForceMode.Impulse);
        }
    }

    public void Update()
    {
        if (setToLaunch && target != null)
        {
            launchTimer += Time.deltaTime;

            transform.localRotation = Quaternion.Euler(target.position.x, target.position.y, target.position.z);
            transform.LookAt(target);

            if (launchTimer > 0.1f)
            {
                PredictAndExecute();
                setToLaunch = false;
                launchTimer = 0;
            }
        }

        destroyTimer += Time.deltaTime;
        if (destroyTimer >= destroyTime || transform.position.y < -200f)
        {
            Destroy(gameObject);
        }
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    public void SetToLaunch()
    {
        setToLaunch = true;
    }
}
