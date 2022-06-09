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
    //IA
    weka.classifiers.functions.MultilayerPerceptron saberPredecirFuerzaZ;
    // weka.classifiers.trees.M5P saberPredecirFuerzaZ;
    public bool realizarEntrenamiento;
    public string nombreArchivoDeDatosInicial;
    public string nombreArchivoDeDatosFinal;    //Se guardara los datos en este archivo si se desea realizar un entrenamiento (bool realizarEntrenamiento)

    public int limiteFuerzaZSuperior;
    public int limiteAlturaInferior;    //Como minimo debe ser 14
    public int limiteAlturaSuperior;
    
    public GameObject NPC, instanciaNPC;

    public float alturaActual, posInicialZ, distanciaCalculadaZ;

    //Factores
    //const float factorFuerzaX = 0.005f;
    const float factorIncAltura = 2f;

    public int slope = 20;
    //const float valorMaximoX = 10000;

    weka.core.Instances casosEntrenamiento;

    void Start()
    {
        Regex regex = new Regex(@"[a-zA-Z]+\w*\.arff");
        if (!regex.IsMatch(nombreArchivoDeDatosInicial))
            nombreArchivoDeDatosInicial = "Experiencia_Inicial.arff";

        if (!regex.IsMatch(nombreArchivoDeDatosFinal))
            nombreArchivoDeDatosFinal = "Experiencia_Final.arff";

        StartCoroutine("Entrenamiento");
    }

    IEnumerator Entrenamiento()
    {
        yield return new WaitForSeconds(3.0f);

        //CONSTRUCCION DE CASOS DE ENTRENAMIENTO

        casosEntrenamiento = new weka.core.Instances(new java.io.FileReader("Assets/WekaData/" + nombreArchivoDeDatosInicial));
        if (realizarEntrenamiento)
        {
            print("Fase de entrenamiento: Inicializada");

            for (int fuerzaZ=0; fuerzaZ <= limiteFuerzaZSuperior; fuerzaZ+=slope)
            {
                for (float altura=limiteAlturaInferior; altura <= limiteAlturaSuperior; altura++)
                {
                    instanciaNPC = Instantiate(NPC) as GameObject;
                    instanciaNPC.transform.position = transform.position;
                    // PrincipalNPC principalNpc = instanciaNPC.GetComponent<PrincipalNPC>();
                    Rigidbody rb = instanciaNPC.GetComponent<Rigidbody>();
                    alturaActual = altura;
                    
                    posInicialZ = transform.position.z;
                    float posInicialY = transform.position.y;
                    float fuerzaY = obtenerFuerzaY(rb.mass, altura);

                    rb.AddForce(new Vector3(0, fuerzaY, fuerzaZ), ForceMode.Impulse);
                    // principalNpc.jump(0, fuerzaY, fuerzaZ);

                    print("Esperamos...");
                    yield return new WaitUntil(() => (distanciaCalculadaZ != -float.MaxValue));        //... y espera a que la pelota llegue al suelo
                    print("Fuerza en Z: " + fuerzaZ + ". Fuerza en Y: " + fuerzaY + ". Altura: " + altura + ". Distancia calculada Z: " + distanciaCalculadaZ);

                    //Guardamos los datos
                    Instance casoAdecidir = new Instance(casosEntrenamiento.numAttributes());
                    casoAdecidir.setDataset(casosEntrenamiento);
                    casoAdecidir.setValue(0, fuerzaZ);
                    casoAdecidir.setValue(1, fuerzaY);
                    casoAdecidir.setValue(2, altura);
                    casoAdecidir.setValue(3, distanciaCalculadaZ);
                    casosEntrenamiento.add(casoAdecidir);

                    Destroy(instanciaNPC);
                    posInicialZ = -float.MaxValue;
                    alturaActual = -float.MaxValue;
                    distanciaCalculadaZ = -float.MaxValue;
                }
            }


            print("Se crearon " + casosEntrenamiento.numInstances() + " casos de entrenamiento");



            //GUARDADO DE LA EXPERIENCIA
                
            File salida = new File("Assets/WekaData/" + nombreArchivoDeDatosFinal); // Experiencia_Final_Perceptron_Neuronal.arff
            if (!salida.exists())
                System.IO.File.Create(salida.getAbsoluteFile().toString()).Dispose();
            ArffSaver saver = new ArffSaver();
            saver.setInstances(casosEntrenamiento);
            saver.setFile(salida);
            saver.writeBatch();
        }
        print("---------- FIN ENTRENAMIENTO ----------");
    }


    // Obtiene la Fuerza a aplicar en el eje Y aplicando la formula de lanzamiento verticial y la segunda ley de Newton
    float obtenerFuerzaY(float masa, float alturaObjetivo)
    {
        if (alturaObjetivo > 0)
            return masa * Mathf.Sqrt(alturaObjetivo * 2.0f * 9.81f * factorIncAltura); //Mathf.Sqrt(masa * Mathf.Sqrt(alturaObjetivo * 2 * 9.81f)); //factorIncFuerzaYPorDistancia; //(distanciaObjetivo* factorIncFuerzaYPorDistancia);
        else
            return masa * 9.81f; //+ ((masa * 9.81f) / 2.0f); //factorIncFuerzaYPorDistancia; //(distanciaObjetivo * factorIncFuerzaYPorDistancia);
    }

    // Update is called once per frame
    
    
    void Update()
    {

    }

    void FixedUpdate()
    {
        if (instanciaNPC != null)
        {
            if (distanciaCalculadaZ == -float.MaxValue && instanciaNPC.transform.position.y < alturaActual && instanciaNPC.GetComponent<Rigidbody>().velocity.y < 0)
            {
                distanciaCalculadaZ = Mathf.Abs(posInicialZ - instanciaNPC.transform.position.z);
            }
        }
    }
}
