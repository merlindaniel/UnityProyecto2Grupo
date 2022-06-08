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
    //weka.classifiers.functions.MultilayerPerceptron saberPredecirFuerzaZ;
    weka.classifiers.trees.M5P saberPredecirFuerzaZ;
    public bool realizarEntrenamiento;
    public int numAlturaDistanciaDistinta;     //Numero de veces que se desea que la plataforma tenga una altura-distancia distinta
    public string nombreArchivoDeDatosInicial;
    public string nombreArchivoDeDatosFinal;    //Se guardara los datos en este archivo si se desea realizar un entrenamiento (bool realizarEntrenamiento)
    public int numSaltosDespues;                //Numero de saltos que realizará despues de caer en dentro de la plataforma interna
    int contadorSaltosDespues;

    //UI
    Collider collider;
    float alturaMax = -10000f;
    float alturaMaxGeneral = -10000f;
    bool pausarCalculoAlturas = false;
    string texto;

    //Factores
    const float factorFuerzaX = 0.005f;
    const float factorIncAltura = 2f;
    const float valorMaximoX = 10000;

    weka.core.Instances casosEntrenamiento;

    PrincipalNPC principalNpc;
    Rigidbody rb;

    void OnGUI()
    {
        GUI.Label(new Rect(10, 20, 600, 20), texto);
    }

    void Start()
    {
        contadorSaltosDespues = 0;

        collider = GetComponent<Collider>();
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
            for (int i=0; i<numAlturaDistanciaDistinta; i++)
            {
                //print("--Entra while");
                int nextPlatformId = principalNpc.GetNextPlatform().GetInstanceID();
                bool plataformaInternaFuePisada = false;   //Para saber si la plataforma fue pisada al menos 1 vez
                contadorSaltosDespues = 0;

                for (float fuerzaX = 0; fuerzaX < valorMaximoX; fuerzaX = fuerzaX + factorFuerzaX * valorMaximoX)
                {
                    //print("--Entra for");
                    


                    Vector3 positionNextPlatform = principalNpc.GetNextPlatform().transform.position;

                    float altura = positionNextPlatform.y - (transform.position.y - (collider.bounds.size.y / 2f)); //altura desde los pies del NPC hasta el medio de la plataforma
                    float distancia = Vector3.Distance(transform.position, new Vector3(positionNextPlatform.x, transform.position.y, positionNextPlatform.z)); //Distancia del NPC hasta el objetivo ignorando la altura (cateto contuguo desde el NPC)
                    float fuerzaY = obtenerFuerzaY(rb.mass, altura, distancia);

                    //print("Masa: " + rb.mass + ". AlturaObjetivo: " + altura + ". DistanciaObjetivo: " + distancia);
                    print("Fuerza en X: " + fuerzaX + ". Fuerza en Y: " + fuerzaY);
                    principalNpc.jumpRelative(0, fuerzaY, fuerzaX);

                    yield return new WaitUntil(() => (principalNpc.isJumping == false)); //Esperamos a que toque el terreno
    

                    bool platformChanged = nextPlatformId == principalNpc.GetActualPlatform().GetInstanceID();    //Comprobamos si sigue en la misma plataforma

                    if (platformChanged) {
                        print("-----El NPC SI LLEGO!");
                        yield return new WaitForSeconds(1f);  //Debug: Para ver donde cayó
                        plataformaInternaFuePisada = true;
                        //principalNpc.SetFinished(false);
                    }
                    else
                    {
                        print("-----El NPC NO LLEGO");
                    }

                    
                    //Guardamos los datos
                    Instance casoAdecidir = new Instance(casosEntrenamiento.numAttributes());
                    casoAdecidir.setDataset(casosEntrenamiento);
                    casoAdecidir.setValue(0, fuerzaX);
                    casoAdecidir.setValue(1, fuerzaY);
                    casoAdecidir.setValue(2, altura);
                    casoAdecidir.setValue(3, distancia);
                    casoAdecidir.setValue(4, platformChanged ? 1 : 0);
                    casosEntrenamiento.add(casoAdecidir);


                    principalNpc.GoToSpawn();
                    yield return new WaitUntil(() => (principalNpc.isJumping == false)); //Esperamos a que vuelva al Spawn


                    //Si al menos 1 vez la plataforma interna fue pisada pero en el salto actual ya no lo fue superando el contador de numSaltosDespues, termina el bucle y vamos a la siguiente plataforma
                    if (plataformaInternaFuePisada && contadorSaltosDespues >= numSaltosDespues+1)
                        break;


                    if (plataformaInternaFuePisada)
                        contadorSaltosDespues++;

                }

                //Cambiamos posicion y altura de la plataforma de aprendizaje
                print("-Cambiando posicion plataforma de entrenamiento");
                GameObject learningInternalPlatform = principalNpc.GetNextPlatform();
                GameObject learningPlatform = learningInternalPlatform.transform.parent.gameObject;
                float z = learningPlatform.transform.position.z;
                learningPlatform.transform.position = new Vector3(Random.Range(-245.0f, -145.5f), Random.Range(14.0f, 50.0f), z);//Area de entrenamiento


                //principalNpc.NextPlatform();
                //if (principalNpc.GetNextPlatform() != null)
                //{
                //    principalNpc.GoToActualPlatform();
                //    yield return new WaitUntil(() => (principalNpc.isJumping == false));
                //}

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
        //saberPredecirFuerzaZ = new MultilayerPerceptron();
        saberPredecirFuerzaZ = new M5P();                                               //Algoritmo Arbol de Regresion M5P
        //saberPredecirFuerzaZ.setHiddenLayers("8,5");
        //saberPredecirFuerzaZ.setTrainingTime(1000);
        //saberPredecirFuerzaZ.setOptions(Utils.splitOptions("-L 0.3 -M 0.2 -N 5500 -V 0 -S 0 -E 20 -H 5,5,5 -R"));
        casosEntrenamiento.setClassIndex(0);                                             //Aprendemos la Fuerza en Z
        saberPredecirFuerzaZ.buildClassifier(casosEntrenamiento);                        //REALIZAR EL APRENDIZAJE
        print("----TERMINA GENERACION DEL MODELO");



        //Prueba(ESTO DEBERIA DE ESTAR DENTRO DEL UPDATE):
        //principalNpc.SetFinished(false);
        principalNpc.SetPrediction();
        principalNpc.GoToSpawn();
        

        yield return new WaitUntil(() => (principalNpc.isJumping == false));
        Time.timeScale = 1;

        Vector3 pnp = principalNpc.GetNextPlatform().transform.position;

        float alt = pnp.y - (transform.position.y - (collider.bounds.size.y / 2.0f)); //altura desde los pies del NPC hasta el medio de la plataforma
        float dis = Vector3.Distance(transform.position, new Vector3(pnp.x, transform.position.y, pnp.z)); //Distancia del NPC hasta el objetivo ignorando la altura (cateto contuguo desde el NPC)
        float fY = obtenerFuerzaY(rb.mass, alt, dis);

        Instance casoPrueba = new Instance(casosEntrenamiento.numAttributes());
        casoPrueba.setDataset(casosEntrenamiento);
        casoPrueba.setValue(1, fY);
        casoPrueba.setValue(2, alt);
        casoPrueba.setValue(3, dis);
        casoPrueba.setValue(4, 1);
        float fZ = (float)saberPredecirFuerzaZ.classifyInstance(casoPrueba);                          //Predice FuerzaZ

        print("Fuerza en Z: " + fZ + ". Fuerza en Y: " + fY);
        principalNpc.jumpRelative(0, fY, fZ);

        //yield return new WaitUntil(() => (principalNpc.isJumping == false)); //Esperamos a que toque el terreno

    }


    /// <summary>
    ///     Obtiene la Fuerza a aplicar en el eje Y aplicando la formula de lanzamiento verticial y la segunda ley de Newton
    /// </summary>
    /// <returns></returns>
    float obtenerFuerzaY(float masa, float alturaObjetivo, float distanciaObjetivo)
    {
        if (alturaObjetivo > 0)
            return masa * Mathf.Sqrt(alturaObjetivo * 2.0f * 9.81f * factorIncAltura); //Mathf.Sqrt(masa * Mathf.Sqrt(alturaObjetivo * 2 * 9.81f)); //factorIncFuerzaYPorDistancia; //(distanciaObjetivo* factorIncFuerzaYPorDistancia);
        else
            return masa * 9.81f; //+ ((masa * 9.81f) / 2.0f); //factorIncFuerzaYPorDistancia; //(distanciaObjetivo * factorIncFuerzaYPorDistancia);
    }

    // Update is called once per frame
    
    
    void Update()
    {
        if (pausarCalculoAlturas)
        {
            alturaMax = -10000f;
            texto = "Alt max(world) esta iteracion: - " + ". Alt max(world) general: " + alturaMaxGeneral; //Nota: Aqui muestra la altura del mundo. Realmente nosotros trabajamos con la diferencia de altura entre la plataforma actual y la siguiente
        } else
        {
            float alturaActual = (transform.position.y - (collider.bounds.size.y / 2f));
            if (alturaActual > alturaMax)
            {
                alturaMax = alturaActual;
                texto = "Alt max(world) esta iteracion: " + alturaMax + ". Alt max(world) general: " + alturaMaxGeneral;
            }


            if (alturaActual > alturaMaxGeneral)
            {
                alturaMaxGeneral = alturaActual;
                texto = "Alt max(world) esta iteracion: " + alturaMax + ". Alt max(world) general: " + alturaMaxGeneral;
            }
        }
            

        



    }
}
