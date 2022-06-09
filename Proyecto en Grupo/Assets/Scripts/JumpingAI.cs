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
//using java.lang;
//using java.util;





public class JumpingAI : MonoBehaviour
{
    //IA
    weka.classifiers.functions.MultilayerPerceptron saberPredecirFuerzaZ;
    // weka.classifiers.trees.M5P saberPredecirFuerzaZ;
    public bool realizarEntrenamiento;
    public int numAlturaDistanciaDistinta;     //Numero de veces que se desea que la plataforma tenga una altura-distancia distinta
    public string nombreArchivoDeDatosInicial;
    public string nombreArchivoDeDatosFinal;    //Se guardara los datos en este archivo si se desea realizar un entrenamiento (bool realizarEntrenamiento)
    public int numSaltosDespues;                //Numero de saltos que realizar� despues de caer en dentro de la plataforma interna

    public int limiteFuerzaZSuperior;
    public int limiteAlturaInferior;    //Como minimo debe ser 14
    public int limiteAlturaSuperior;
    


    //UI
    string texto;

    //Factores
    //const float factorFuerzaX = 0.005f;
    const float factorIncAltura = 2f;
    //const float valorMaximoX = 10000;

    weka.core.Instances casosEntrenamiento;

    PrincipalNPC principalNpc;
    Rigidbody rb;

    void OnGUI()
    {
        GUI.Label(new Rect(10, 20, 600, 20), texto);
    }

    void Start()
    {
        principalNpc = GetComponent<PrincipalNPC>();
        rb = GetComponent<Rigidbody>();

        Regex regex = new Regex(@"[a-zA-Z]+\w*\.arff");
        if (!regex.IsMatch(nombreArchivoDeDatosInicial))
            nombreArchivoDeDatosInicial = "Experiencia_Inicial.arff";

        if (!regex.IsMatch(nombreArchivoDeDatosFinal))
            nombreArchivoDeDatosFinal = "Experiencia_Final.arff";


        principalNpc.SetLearing();
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

            for (int fuerzaZ=0; fuerzaZ <= limiteFuerzaZSuperior; fuerzaZ++)
            {
                for (int altura=limiteAlturaInferior; altura <= limiteAlturaSuperior; altura++)
                {
                    float posInicialZ = transform.position.z;
                    float posInicialY = transform.position.y;
                    //float fuerzaX = Random.Range(0, 2000);
                    //float altura = Random.Range(14.0f, 50.0f);

                    float alturaRespectoNpc = altura - (posInicialY - (principalNpc.GetNpcHeight() / 2));
                    float fuerzaY = obtenerFuerzaY(rb.mass, alturaRespectoNpc);

                    //print("Masa: " + rb.mass + ". AlturaObjetivo: " + altura + ". DistanciaObjetivo: " + distancia);
                    print("Fuerza en Z: " + fuerzaZ + ". Fuerza en Y: " + fuerzaY + ". Alt respecto NPC: " + alturaRespectoNpc);
                    principalNpc.jumpRelative(0, fuerzaY, fuerzaZ);

                    print("Esperamos...");
                    yield return new WaitUntil(() => (altura >= transform.position.y && rb.velocity.y < 0));

                    print("Llega a la altura " + altura);
                    float distanciaZ = Mathf.Abs(posInicialZ - transform.position.z);


                    //Guardamos los datos
                    Instance casoAdecidir = new Instance(casosEntrenamiento.numAttributes());
                    casoAdecidir.setDataset(casosEntrenamiento);
                    casoAdecidir.setValue(0, fuerzaZ);
                    casoAdecidir.setValue(1, fuerzaY);
                    casoAdecidir.setValue(2, alturaRespectoNpc);
                    casoAdecidir.setValue(3, distanciaZ);
                    casosEntrenamiento.add(casoAdecidir);


                    principalNpc.GoToSpawn();
                    yield return new WaitUntil(() => (principalNpc.isJumping == false)); //Esperamos a que vuelva al Spawn
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




        //APRENDIZAJE A PARTIR DE LOS CASOS DE ENTRENAMIENTO
        print("----EMPIEZA GENERACION DEL MODELO");
        saberPredecirFuerzaZ = new MultilayerPerceptron();
        // saberPredecirFuerzaZ = new M5P();                                               //Algoritmo Arbol de Regresion M5P
        saberPredecirFuerzaZ.setHiddenLayers("7,5,3");
        saberPredecirFuerzaZ.setTrainingTime(1000);
        saberPredecirFuerzaZ.setLearningRate(0.2);
        //saberPredecirFuerzaZ.setOptions(Utils.splitOptions("-L 0.3 -M 0.2 -N 5500 -V 0 -S 0 -E 20 -H 5,5,5 -R"));
        casosEntrenamiento.setClassIndex(0);                                             //Aprendemos la Fuerza en Z
        saberPredecirFuerzaZ.buildClassifier(casosEntrenamiento);                        //REALIZAR EL APRENDIZAJE
        print("----TERMINA GENERACION DEL MODELO");



        //Prueba(ESTO DEBERIA DE ESTAR DENTRO DEL UPDATE):
        //principalNpc.SetFinished(false);
        principalNpc.SetPrediction();
        principalNpc.GoToSpawn();
        

        //yield return new WaitUntil(() => (principalNpc.isJumping == false));
        Time.timeScale = 1;

        Vector3 pnp = principalNpc.GetNextPlatform().transform.position;

        //float alt = pnp.y - (transform.position.y - (collider.bounds.size.y / 2.0f)); //altura desde los pies del NPC hasta el medio de la plataforma
        float alt = pnp.y;
        //float dis = Vector3.Distance(transform.position, new Vector3(pnp.x, transform.position.y, pnp.z)); //Distancia del NPC hasta el objetivo ignorando la altura (cateto contuguo desde el NPC)
        float dis = Mathf.Abs(pnp.x-transform.position.x);
        float fY = obtenerFuerzaY(rb.mass, alt);

        Instance casoPrueba = new Instance(casosEntrenamiento.numAttributes());
        casoPrueba.setDataset(casosEntrenamiento);
        casoPrueba.setValue(1, fY);
        casoPrueba.setValue(2, alt);
        casoPrueba.setValue(3, dis);
        //casoPrueba.setValue(4, 1);
        float fZ = (float)saberPredecirFuerzaZ.classifyInstance(casoPrueba);                          //Predice FuerzaZ

        print("Fuerza en Z: " + fZ + ". Fuerza en Y: " + fY + ". Distancia: " + dis + ". Altura: " + alt);
        principalNpc.jumpRelative(0, fY, fZ);

        //yield return new WaitUntil(() => (principalNpc.isJumping == false)); //Esperamos a que toque el terreno

    }


    /// <summary>
    ///     Obtiene la Fuerza a aplicar en el eje Y aplicando la formula de lanzamiento verticial y la segunda ley de Newton
    /// </summary>
    /// <returns></returns>
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
        texto = "Alt actual: " + transform.position.y; 
        //if (pausarCalculoAlturas)
        //{
        //    alturaMax = -10000f;
        //    texto = "Alt max(world) esta iteracion: - " + ". Alt max(world) general: " + alturaMaxGeneral; //Nota: Aqui muestra la altura del mundo. Realmente nosotros trabajamos con la diferencia de altura entre la plataforma actual y la siguiente
        //} else
        //{
        //    float alturaActual = (transform.position.y - (collider.bounds.size.y / 2f));
        //    if (alturaActual > alturaMax)
        //    {
        //        alturaMax = alturaActual;
        //        texto = "Alt max(world) esta iteracion: " + alturaMax + ". Alt max(world) general: " + alturaMaxGeneral;
        //    }


        //    if (alturaActual > alturaMaxGeneral)
        //    {
        //        alturaMaxGeneral = alturaActual;
        //        texto = "Alt max(world) esta iteracion: " + alturaMax + ". Alt max(world) general: " + alturaMaxGeneral;
        //    }
        //}






    }
}
