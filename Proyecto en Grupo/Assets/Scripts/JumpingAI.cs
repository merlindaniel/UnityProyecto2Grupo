using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using UnityEngine;
using weka.core;
using weka.core.converters;
//using weka.classifiers;
using weka.classifiers.trees;
using weka.classifiers.functions;
using java.io;
using System;
//using java.lang;
//using java.util;





public class JumpingAI : MonoBehaviour
{
    //IA
    weka.classifiers.functions.MultilayerPerceptron AIModelFZ;
    // weka.classifiers.trees.M5P AIModelFZ;

    [Header("Model Settings")]
    public bool loadModel = false;
    public string modelFile;
    
    public string datasetFile;

    //UI
    string text;

    //Factores
    //const float factorFuerzaX = 0.005f;
    const float incHeightFactor = 2f;
    //const float valorMaximoX = 10000;

    weka.core.Instances trainingDataset;

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
        if (!regex.IsMatch(modelFile))
            modelFile = "Modelo_Multilayer_Perceptron.model";

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
            // AIModelFZ = new M5P();                                               //Algoritmo Arbol de Regresion M5P
            AIModelFZ.setHiddenLayers("7,5,3");
            AIModelFZ.setTrainingTime(1000);
            AIModelFZ.setLearningRate(0.2);
            //AIModelFZ.setOptions(Utils.splitOptions("-L 0.3 -M 0.2 -N 5500 -V 0 -S 0 -E 20 -H 5,5,5 -R"));
                                                         
            AIModelFZ.buildClassifier(trainingDataset);                        //CREAR MODELO
            SerializationHelper.write("Assets/WekaData/" + modelFile, AIModelFZ);
            print("----TERMINA GENERACION DEL MODELO");
        }
        else
        {
            print("----LECTURA DEL MODELO");
            AIModelFZ = (MultilayerPerceptron) SerializationHelper.read("Assets/WekaData/" + modelFile);
            print("----TERMINA LECTURA DEL MODELO");
        }
        
    }

    ///     Obtiene la Fuerza a aplicar en el eje Y aplicando la formula de lanzamiento verticial y la segunda ley de Newton
    float CalculateFY(float mass, float targetHeight)
    {
        if (targetHeight > 0)
            return mass * Mathf.Sqrt(targetHeight * 2.0f * 9.81f * incHeightFactor); //Mathf.Sqrt(masa * Mathf.Sqrt(alturaObjetivo * 2 * 9.81f)); //factorIncFuerzaYPorDistancia; //(distanciaObjetivo* factorIncFuerzaYPorDistancia);
        else
            return mass * 9.81f; //+ ((masa * 9.81f) / 2.0f); //factorIncFuerzaYPorDistancia; //(distanciaObjetivo * factorIncFuerzaYPorDistancia);
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
        //instance.setValue(4, 1);
        float fZ = (float)AIModelFZ.classifyInstance(instance);                          //Predice FuerzaZ

        return fZ;
    }

    public void PredictAndJumpToNextPlatform()
    {
        mainNPC.LookNextPlatform();
        Vector3 pnp = mainNPC.GetNextPlatform().transform.position;
        //float height = pnp.y;
        float height = pnp.y - (transform.position.y - (mainNPC.GetNpcHeight() / 2));
        float distance = Mathf.Abs(pnp.x-transform.position.x);
        float fY = CalculateFY(rb.mass, height);
        float fZ = PredictFZ(fY, height, distance);
        print("Fuerza en Z: " + fZ + ". Fuerza en Y: " + fY + ". Distancia: " + distance + ". Altura: " + height);
        mainNPC.jumpRelative(0, fY, fZ);
    }
}
