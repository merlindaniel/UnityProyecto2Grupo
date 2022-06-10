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

public class JumpingAIJose : MonoBehaviour
{
    [Header("Basic settings")]
    public string nombreArchivoDeDatosInicial;
    public string nombreArchivoDeDatosFinal;    //Se guardara los datos en este archivo si se desea realizar un entrenamiento (bool realizarEntrenamiento)
    public bool generateModel = true;
    public string modelFileName;

    [Header("Force Limit Settings")]
    public int limiteFuerzaZSuperior;
    public int limiteAlturaInferior;    //Como minimo debe ser 14
    public int limiteAlturaSuperior;
    public int slope = 20;

    [Header("NPC Prefab")]
    public GameObject NPCPrefab;


    //IA
    weka.classifiers.functions.MultilayerPerceptron AIModelFZ;
    weka.core.Instances casosEntrenamiento;


    GameObject instanciaNPC;
    float npcHeight;

    float alturaDeseada, posInicialZ, posInicialX, calculatedDistance;

    bool isDistanceCalculated;

    //Factores
    const float factorIncAltura = 2f;
    

    void Start()
    {
        Regex regex = new Regex(@"[a-zA-Z]+\w*\.arff");
        if (!regex.IsMatch(nombreArchivoDeDatosInicial))
            nombreArchivoDeDatosInicial = "Experiencia_Inicial.arff";

        if (!regex.IsMatch(nombreArchivoDeDatosFinal))
            nombreArchivoDeDatosFinal = "Experiencia_Final.arff";

        Regex regexModel = new Regex(@"[a-zA-Z]+\w*\.model");
        if (!regexModel.IsMatch(modelFileName))
            modelFileName = "Modelo_Multilayer_Perceptron.model";

        GameObject iNPC = Instantiate(NPCPrefab);
        npcHeight = iNPC.GetComponent<Collider>().bounds.size.y;
        Destroy(iNPC);

        print("Altura NPC: " + npcHeight);

        StartCoroutine("Entrenamiento");
    }

    IEnumerator Entrenamiento()
    {
        yield return new WaitForSeconds(3.0f);

        //CONSTRUCCION DE CASOS DE ENTRENAMIENTO

        casosEntrenamiento = new weka.core.Instances(new java.io.FileReader("Assets/WekaData/" + nombreArchivoDeDatosInicial));
        
        print("Fase de entrenamiento: Inicializada");

        for (int fuerzaZ=0; fuerzaZ <= limiteFuerzaZSuperior; fuerzaZ+=slope)
        {
            for (float altura=limiteAlturaInferior; altura <= limiteAlturaSuperior; altura++)
            {
                instanciaNPC = Instantiate(NPCPrefab);
                instanciaNPC.transform.position = transform.position;
                Rigidbody rb = instanciaNPC.GetComponent<Rigidbody>();
                alturaDeseada = altura;
                    
                posInicialZ = transform.position.z;
                posInicialX = transform.position.x;
                float fuerzaY = obtenerFuerzaY(rb.mass, altura+(npcHeight/2));

                rb.AddForce(new Vector3(0, fuerzaY, fuerzaZ), ForceMode.Impulse);

                //print("Esperamos...");
                isDistanceCalculated = false;
                yield return new WaitUntil(() => (isDistanceCalculated));        //... y espera a que la pelota llegue al suelo
                print("Fuerza en Z: " + fuerzaZ + ". Fuerza en Y: " + fuerzaY + ". Altura: " + altura + ". Distancia calculada Z: " + calculatedDistance);

                //Guardamos los datos
                Instance casoAdecidir = new Instance(casosEntrenamiento.numAttributes());
                casoAdecidir.setDataset(casosEntrenamiento);
                casoAdecidir.setValue(0, fuerzaZ);
                casoAdecidir.setValue(1, fuerzaY);
                casoAdecidir.setValue(2, altura);
                casoAdecidir.setValue(3, calculatedDistance);
                casosEntrenamiento.add(casoAdecidir);

                Destroy(instanciaNPC);
            }
        }


        print("Se crearon " + casosEntrenamiento.numInstances() + " casos de entrenamiento");



        //GUARDADO DATASET
        File salida = new File("Assets/WekaData/" + nombreArchivoDeDatosFinal);
        if (!salida.exists())
            System.IO.File.Create(salida.getAbsoluteFile().toString()).Dispose();
        ArffSaver saver = new ArffSaver();
        saver.setInstances(casosEntrenamiento);
        saver.setFile(salida);
        saver.writeBatch();
        print("---------- FIN ENTRENAMIENTO ----------");


        //GENERACION DEL MODELO
        if (generateModel)
        {
            print("----EMPIEZA GENERACION DEL MODELO");
            casosEntrenamiento.setClassIndex(0);
            AIModelFZ = new MultilayerPerceptron();
            AIModelFZ.setHiddenLayers("7,4");
            AIModelFZ.setTrainingTime(2000);

            AIModelFZ.buildClassifier(casosEntrenamiento);                        //CREAR MODELO
            SerializationHelper.write("Assets/WekaData/" + modelFileName, AIModelFZ);
            print("----TERMINA GENERACION DEL MODELO");
        }
    }


    // Obtiene la Fuerza a aplicar en el eje Y aplicando la formula de lanzamiento verticial y la segunda ley de Newton
    float obtenerFuerzaY(float masa, float alturaObjetivo)
    {
        if (alturaObjetivo >= 2)
            return masa * Mathf.Sqrt(alturaObjetivo * 2.0f * 9.81f * factorIncAltura);
        else
            return masa * Mathf.Sqrt(2f * 2.0f * 9.81f * factorIncAltura);
    }

    // Update is called once per frame
    
    
    void Update()
    {

    }

    void FixedUpdate()
    {
        if (instanciaNPC != null)
        {
            if (!isDistanceCalculated && (instanciaNPC.transform.position.y-(npcHeight/2)) <= alturaDeseada && instanciaNPC.GetComponent<Rigidbody>().velocity.y < 0)
            {
                instanciaNPC.GetComponent<Rigidbody>().isKinematic = true;
                float actPosX = instanciaNPC.transform.position.x;
                float actPosZ = instanciaNPC.transform.position.z;
                calculatedDistance = Mathf.Sqrt(Mathf.Pow(Mathf.Abs(posInicialX - actPosX), 2f) + Mathf.Pow(Mathf.Abs(posInicialZ - actPosZ), 2f));
                isDistanceCalculated = true;
            }
        }
    }
}
