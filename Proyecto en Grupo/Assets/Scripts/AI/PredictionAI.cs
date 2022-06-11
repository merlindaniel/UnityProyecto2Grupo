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
    public string initialExperienceFilename;

    // IA
    protected Instances trainingDataset;
    protected MultilayerPerceptron AIModel;

    protected virtual void Start()
    {
        // Cargamos el dataset
        LoadAndBuildModel();
    }

    protected virtual void LoadAndBuildModel()
    {
        trainingDataset = new weka.core.Instances(new java.io.FileReader("Assets/WekaData/" + initialExperienceFilename));
        trainingDataset.setClassIndex(0);   //Queremos obtener Fuerza en Z

        //CONSTRUCCION DE CASOS DE ENTRENAMIENTO
        if (!loadModel)
        {
            //APRENDIZAJE A PARTIR DE LOS CASOS DE ENTRENAMIENTO
            AIModel = new MultilayerPerceptron();
            AIModel.setHiddenLayers("7,4");//7,5,3
            AIModel.setTrainingTime(2000);

            AIModel.buildClassifier(trainingDataset);                        //CREAR MODELO
            SerializationHelper.write("Assets/WekaData/" + modelFileName, AIModel);
        }
        else
        {
            AIModel = (MultilayerPerceptron)SerializationHelper.read("Assets/WekaData/" + modelFileName);
        }
    }

    protected virtual double Predict(Instance instance)
    {
        return AIModel.classifyInstance(instance);
    }

    public virtual void PredictAndExecute()
    {
        // Predict

        // Execute
    }
}
