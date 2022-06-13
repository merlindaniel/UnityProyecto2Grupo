using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using weka.core;
using weka.core.converters;
using weka.classifiers.functions;
using java.io;

public class TrainingAI : MonoBehaviour
{

    [Header("Basic Settings")]
    public string outputFilename = "output.arff";    //Se guardara los datos en este archivo si se desea realizar un entrenamiento (bool realizarEntrenamiento)
    public bool generateModel = true;
    public string modelFilename = "Modelo_Multilayer_Perceptron.model";
    private string inputFilename = "Experiencia_Inicial.arff";

    [Header("Force Limit Settings")]
    public int maxFZ = 3000;
    public int minHeight = 0;
    public int maxHeight = 40;
    public int slopeFZ = 20, slopeHeight = 1;

    [Header("NPC Prefab")]
    public GameObject NPCPrefab;

    [Header("UI Settings")]
    public GameObject buttonGameObject;

    //IA
    weka.classifiers.functions.MultilayerPerceptron AIModelFZ;
    weka.core.Instances dataset;

    GameObject instanceNPC;
    float npcHeight;

    float desiredHeight, initialPosZ, initialPosX, calculatedDistance;

    bool isDistanceCalculated;

    //Factores
    const float incHeightFactor = 2f;

    void Start()
    {
        Regex regex = new Regex(@"[a-zA-Z]+\w*\.arff");
        if (!regex.IsMatch(inputFilename))
            inputFilename = "Experiencia_Inicial.arff";

        if (!regex.IsMatch(outputFilename))
            outputFilename = "output_dataset.arff";

        Regex regexModel = new Regex(@"[a-zA-Z]+\w*\.model");
        if (!regexModel.IsMatch(modelFilename))
            modelFilename = "output_model.model";

        GameObject iNPC = Instantiate(NPCPrefab);
        npcHeight = iNPC.GetComponent<Collider>().bounds.size.y;
        Destroy(iNPC);

        if (buttonGameObject != null)
        {
            Button saveAndBackBtn = buttonGameObject.GetComponent<Button>();
            saveAndBackBtn.onClick.AddListener(SaveAndGoMainMenu);
        }


        StartCoroutine("Entrenamiento");
    }

    IEnumerator Entrenamiento()
    {
        yield return new WaitForSeconds(3.0f);

        //CONSTRUCCION DE CASOS DE ENTRENAMIENTO

        dataset = new weka.core.Instances(new java.io.FileReader("Assets/WekaData/" + inputFilename));
        
        print("Fase de entrenamiento: Inicializada");

        for (int fZ=0; fZ <= maxFZ; fZ+=slopeFZ)
        {
            for (float height=minHeight; height <= maxHeight; height+=slopeHeight)
            {
                instanceNPC = Instantiate(NPCPrefab);
                instanceNPC.transform.position = transform.position;
                Rigidbody rb = instanceNPC.GetComponent<Rigidbody>();
                desiredHeight = height;
                    
                initialPosZ = transform.position.z;
                initialPosX = transform.position.x;
                float fY = CalculateFY(rb.mass, height+(npcHeight/2));

                rb.AddForce(new Vector3(0, fY, fZ), ForceMode.Impulse);

                //print("Esperamos...");
                isDistanceCalculated = false;
                yield return new WaitUntil(() => (isDistanceCalculated));        //... y espera a que la pelota llegue al suelo
                print("Fuerza en Z: " + fZ + ". Fuerza en Y: " + fY + ". Altura: " + height + ". Distancia calculada Z: " + calculatedDistance);

                //Guardamos los datos
                Instance instance = new Instance(dataset.numAttributes());
                instance.setDataset(dataset);
                instance.setValue(0, fZ);
                instance.setValue(1, fY);
                instance.setValue(2, height);
                instance.setValue(3, calculatedDistance);
                dataset.add(instance);

                Destroy(instanceNPC);
            }
        }

        //print("Se crearon " + dataset.numInstances() + " casos de entrenamiento");

        ////GUARDADO DATASET
        //File output = new File("Assets/WekaData/" + outputFilename);
        //if (!output.exists())
        //    System.IO.File.Create(output.getAbsoluteFile().toString()).Dispose();
        //ArffSaver saver = new ArffSaver();
        //saver.setInstances(dataset);
        //saver.setFile(output);
        //saver.writeBatch();
        //print("---------- FIN ENTRENAMIENTO ----------");


        ////GENERACION DEL MODELO
        //if (generateModel)
        //{
        //    print("----EMPIEZA GENERACION DEL MODELO");
        //    dataset.setClassIndex(0);
        //    AIModelFZ = new MultilayerPerceptron();
        //    AIModelFZ.setHiddenLayers("7,4");
        //    AIModelFZ.setTrainingTime(2000);

        //    AIModelFZ.buildClassifier(dataset);                        //CREAR MODELO
        //    SerializationHelper.write("Assets/WekaData/" + modelFilename, AIModelFZ);
        //    print("----TERMINA GENERACION DEL MODELO");
        //}
        GenerateDatasetAndModel();
    }


    // Obtiene la Fuerza a aplicar en el eje Y aplicando la formula de lanzamiento verticial y la segunda ley de Newton
    float CalculateFY(float mass, float desiredHeight)
    {
        if (desiredHeight >= 2)
            return mass * Mathf.Sqrt(desiredHeight * 2.0f * 9.81f * incHeightFactor);
        else
            return mass * Mathf.Sqrt(2f * 2.0f * 9.81f * incHeightFactor);
    }

    void GenerateDatasetAndModel()
    {
        print("Se crearon " + dataset.numInstances() + " casos de entrenamiento");
        //GUARDADO DATASET
        File output = new File("Assets/WekaData/" + outputFilename);
        if (!output.exists())
            System.IO.File.Create(output.getAbsoluteFile().toString()).Dispose();
        ArffSaver saver = new ArffSaver();
        saver.setInstances(dataset);
        saver.setFile(output);
        saver.writeBatch();
        print("---------- FIN ENTRENAMIENTO ----------");


        //GENERACION DEL MODELO
        if (generateModel)
        {
            print("----EMPIEZA GENERACION DEL MODELO");
            dataset.setClassIndex(0);
            AIModelFZ = new MultilayerPerceptron();
            AIModelFZ.setHiddenLayers("7,4");
            AIModelFZ.setTrainingTime(2000);

            AIModelFZ.buildClassifier(dataset);                        //CREAR MODELO
            SerializationHelper.write("Assets/WekaData/" + modelFilename, AIModelFZ);
            print("----TERMINA GENERACION DEL MODELO");
        }
    }

    void SaveAndGoMainMenu()
    {
        StopCoroutine("Entrenamiento");
        if(dataset!=null && dataset.numInstances() > 0)
        {
            GenerateDatasetAndModel();
            Loader.Load(Loader.Scene.MainMenu);
        }

    }

    void FixedUpdate()
    {
        if (instanceNPC != null)
        {
            if (!isDistanceCalculated && (instanceNPC.transform.position.y-(npcHeight/2)) <= desiredHeight && instanceNPC.GetComponent<Rigidbody>().velocity.y < 0)
            {
                instanceNPC.GetComponent<Rigidbody>().isKinematic = true;
                float actPosX = instanceNPC.transform.position.x;
                float actPosZ = instanceNPC.transform.position.z;
                calculatedDistance = Mathf.Sqrt(Mathf.Pow(Mathf.Abs(initialPosX - actPosX), 2f) + Mathf.Pow(Mathf.Abs(initialPosZ - actPosZ), 2f));
                isDistanceCalculated = true;
            }
        }
    }
}
