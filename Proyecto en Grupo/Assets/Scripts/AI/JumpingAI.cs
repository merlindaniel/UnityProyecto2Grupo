using System.Text.RegularExpressions;

using UnityEngine;
using weka.core;

public class JumpingAI : PredictionAI
{
    //UI
    [HideInInspector] string text;

    //Factores
    const float incHeightFactor = 2f;

    //Otros
    [HideInInspector] JumpingNPC mainNPC;
    [HideInInspector] Rigidbody rb;

    protected override void Start()
    {
        mainNPC = GetComponent<JumpingNPC>();
        rb = GetComponent<Rigidbody>();

        Regex regex = new Regex(@"[a-zA-Z]+\w*\.model");
        if (!regex.IsMatch(modelFileName))
            modelFileName = "output_model.model";
            
        base.Start(); // Cargar modelo
    }

    // Update is called once per frame
    void Update()
    {
        text = "Alt actual: " + transform.position.y; 
    }

    // Obtiene la Fuerza a aplicar en el eje Y aplicando la formula de lanzamiento vertical y la segunda ley de Newton
    float CalculateFY(float mass, float targetHeight)
    {
        if (targetHeight >= 2)
            return mass * Mathf.Sqrt(targetHeight * 2.0f * 9.81f * incHeightFactor);
        else
            return mass * Mathf.Sqrt(2f * 2.0f * 9.81f * incHeightFactor);
    }

    // Predecir y saltar
    public override void PredictAndExecute()
    {
        mainNPC.LookNextPlatform();
        Vector3 pnp = mainNPC.GetNextPlatform().transform.position;
        float platformHeight = mainNPC.GetNextPlatform().GetComponent<Collider>().bounds.size.y; //Tenemos en cuenta la altuar de la plataforma
        float height = (pnp.y + (platformHeight/2f)) - (transform.position.y - (mainNPC.GetNpcHeight() / 2f));
        float distance = Mathf.Sqrt(Mathf.Pow(Mathf.Abs(pnp.x - transform.position.x), 2f) + Mathf.Pow(Mathf.Abs(pnp.z - transform.position.z), 2f));
        float fY = CalculateFY(rb.mass, height);

        Instance instance = new Instance(trainingDataset.numAttributes());
        instance.setDataset(trainingDataset);
        instance.setValue(1, fY);
        instance.setValue(2, height);
        instance.setValue(3, distance);
        float fZ = (float) Predict(instance);

        mainNPC.jumpRelative(0, fY, fZ);
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 20, 600, 20), text);
    }
}
