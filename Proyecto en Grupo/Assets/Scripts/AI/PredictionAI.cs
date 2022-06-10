using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using UnityEngine;
using weka.core;
using weka.core.converters;
using weka.classifiers.lazy;
using weka.classifiers.functions;
using java.io;
using System;

// Clase abstracta general para predecir con modelos de IA
public abstract class PredictionAI : MonoBehaviour
{
    [Header("Model Settings")]
    public bool loadModel = false;
    public string modelFileName;
    public string datasetFile;

    // IA
    protected Instances trainingDataset;
    protected MultilayerPerceptron AIModelFZ;

    protected virtual void Start()
    {
        // Cargamos el dataset
        LoadAndBuildModel();
    }

    protected virtual void LoadAndBuildModel()
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

    protected virtual double Predict(Instance instance)
    {
        return AIModelFZ.classifyInstance(instance);
    }

    public virtual void PredictAndExecute()
    {
        // Predict

        // Execute
    }
}
