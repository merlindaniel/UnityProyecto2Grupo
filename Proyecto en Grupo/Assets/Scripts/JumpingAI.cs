using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using UnityEngine;
using weka.core;
using weka.core.converters;
//using weka.classifiers;
//using weka.classifiers.trees;
using weka.classifiers.lazy;
using weka.classifiers.functions;
using java.io;
using System;
//using java.lang;
//using java.util;





public class JumpingAI : MonoBehaviour
{
    [Header("Model Settings")]
    public bool loadModel = false;
    public string modelFileName;
    
    public string datasetFile;


    //IA
    weka.core.Instances trainingDataset;
    weka.classifiers.functions.MultilayerPerceptron AIModelFZ;
    //weka.classifiers.lazy.IBk AIModelFZ;

    //UI
    string text;

    //Factores
    const float incHeightFactor = 2f;

    //Otros
    PrincipalNPC mainNPC;
    Rigidbody rb;

    void OnGUI()
    {
        GUI.Label(new Rect(10, 20, 600, 20), text);
    }

    void Start()
    {
        mainNPC = GetComponent<PrincipalNPC>();
        rb = GetComponent<Rigidbody>();

        Regex regex = new Regex(@"[a-zA-Z]+\w*\.model");
        if (!regex.IsMatch(modelFileName))
            modelFileName = "Modelo_Multilayer_Perceptron.model";

        //Cargamos el dataset
        LoadAndBuildModel();
    }

    private void LoadAndBuildModel()
    {
        trainingDataset = new weka.core.Instances(new java.io.FileReader("Assets/WekaData/" + datasetFile));
        trainingDataset.setClassIndex(0);   //Queremos obtener Fuerza en Z

        //CONSTRUCCION DE CASOS DE ENTRENAMIENTO
        if (!loadModel)
        {
            //APRENDIZAJE A PARTIR DE LOS CASOS DE ENTRENAMIENTO
            print("----EMPIEZA GENERACION DEL MODELO");
            AIModelFZ = new MultilayerPerceptron();
            AIModelFZ.setHiddenLayers("7,4");//7,5,3
            AIModelFZ.setTrainingTime(2000);
            //AIModelFZ = new IBk();
            //AIModelFZ.setKNN(3);

            AIModelFZ.buildClassifier(trainingDataset);                        //CREAR MODELO
            SerializationHelper.write("Assets/WekaData/" + modelFileName, AIModelFZ);
            print("----TERMINA GENERACION DEL MODELO");
        }
        else
        {
            print("----LECTURA DEL MODELO");
            AIModelFZ = (MultilayerPerceptron)SerializationHelper.read("Assets/WekaData/" + modelFileName);
            //AIModelFZ = (IBk)SerializationHelper.read("Assets/WekaData/" + modelFileName);
            print("----TERMINA LECTURA DEL MODELO");
        }
        
    }

    ///     Obtiene la Fuerza a aplicar en el eje Y aplicando la formula de lanzamiento verticial y la segunda ley de Newton
    float CalculateFY(float mass, float targetHeight)
    {
        if (targetHeight >= 2)
            return mass * Mathf.Sqrt(targetHeight * 2.0f * 9.81f * incHeightFactor);
        else
            return mass * Mathf.Sqrt(2f * 2.0f * 9.81f * incHeightFactor);
    }

    // Update is called once per frame
    void Update()
    {
        text = "Alt actual: " + transform.position.y; 
    }

    float PredictFZ(float fY, float height, float distance)
    {
        Instance instance = new Instance(trainingDataset.numAttributes());
        instance.setDataset(trainingDataset);
        instance.setValue(1, fY);
        instance.setValue(2, height);
        instance.setValue(3, distance);
        float fZ = (float)AIModelFZ.classifyInstance(instance);                          //Predice FuerzaZ

        return fZ;
    }

    public void PredictAndJumpToNextPlatform()
    {
        mainNPC.LookNextPlatform();
        Vector3 pnp = mainNPC.GetNextPlatform().transform.position;
        float platformHeight = mainNPC.GetNextPlatform().GetComponent<Collider>().bounds.size.y; //Tenemos en cuenta la altuar de la plataforma
        float height = (pnp.y + (platformHeight/2f)) - (transform.position.y - (mainNPC.GetNpcHeight() / 2f));
        float distance = Mathf.Sqrt(Mathf.Pow(Mathf.Abs(pnp.x - transform.position.x), 2f) + Mathf.Pow(Mathf.Abs(pnp.z - transform.position.z), 2f));
        float fY = CalculateFY(rb.mass, height);
        float fZ = PredictFZ(fY, height, distance);
        print("Fuerza en Z: " + fZ + ". Fuerza en Y: " + fY + ". Distancia: " + distance + ". Altura: " + height);
        mainNPC.jumpRelative(0, fY, fZ);
    }
}
