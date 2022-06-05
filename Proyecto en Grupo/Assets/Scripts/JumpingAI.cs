using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using weka.core;


public class JumpingAI : MonoBehaviour
{
    Collider collider;
    float alturaMax = -10000f;
    float alturaMaxGeneral = -10000f;
    bool pausarCalculoAlturas = false;
    public string texto;
    // Start is called before the first frame update

    //Factores
    const float factorFuerzaX = 0.005f;
    const float factorIncAltura = 2f;
    const float valorMaximoX = 10000;

    //---ESTADOS
    const string ENTRENAMIENTO = "Entrenamiento";
    const string PREDICCION = "Prediccion";

    string estado;

    weka.core.Instances casosEntrenamiento;

    PrincipalNPC principalNpc;
    Rigidbody rb;

    void OnGUI()
    {
        GUI.Label(new Rect(10, 20, 600, 20), texto);
    }

    void Start()
    {
        collider = GetComponent<Collider>();
        //Time.timeScale = 5;
        principalNpc = GetComponent<PrincipalNPC>();
        rb = GetComponent<Rigidbody>();

        estado = ENTRENAMIENTO;
        StartCoroutine("Entrenamiento");
    }

    IEnumerator Entrenamiento()
    {
        casosEntrenamiento = new weka.core.Instances(new java.io.FileReader("Assets/WekaData/Experiencia_Inicial.arff"));

        //CONSTRUCCION DE CASOS DE ENTRENAMIENTO
        print("Fase de entrenamiento: Inicializada");

        yield return new WaitForSeconds(3.0f);

        while (principalNpc.getNextPlatform() != null)  //Si no hay mas plataformas terminamos el bucle
        {
            print("--Entra while");
            //Platform p = ;
            //int nextNextPlatformId = -1;
            //if (p.nextPlatform != null)
            //    nextNextPlatformId = principalNpc.getNextPlatform().GetComponent<Platform>().nextPlatform.GetInstanceID();
            //else
            //    break;

            for (float fuerzaX = 0; fuerzaX < valorMaximoX; fuerzaX = fuerzaX + factorFuerzaX * valorMaximoX)
            {
                print("--Entra for");
                int nextNextPlatformId = principalNpc.getNextPlatform().GetComponent<Platform>().nextPlatform.GetInstanceID();

                Vector3 positionNextPlatform = principalNpc.getNextPlatform().transform.position;

                float altura = principalNpc.getNextPlatform().transform.position.y - (transform.position.y - (collider.bounds.size.y / 2f));//altura desde los pies del NPC
                float distancia = Vector3.Distance(transform.position, new Vector3(positionNextPlatform.x, transform.position.y, positionNextPlatform.z)); //Distancia del NPC hasta el objetivo ignorando la altura (cateto contuguo desde el NPC)
                float fuerzaY = obtenerFuerzaY(rb.mass, altura, distancia);

                print("Masa: " + rb.mass + ". AlturaObjetivo: " + altura + ". DistanciaObjetivo: " + distancia);
                print("Fuerza en X: " + fuerzaX + ". Fuerza en Y: " + fuerzaY);
                rb.AddRelativeForce(new Vector3(0, fuerzaY, fuerzaX), ForceMode.Impulse);
                principalNpc.isJumping = true;

                yield return new WaitUntil(() => (principalNpc.isJumping == false)); //Esperamos a que toque el terreno


                int actualNextPlatformId = principalNpc.getNextPlatform().GetInstanceID();

                int resultado = nextNextPlatformId == actualNextPlatformId ? 1 : 0;

                if (resultado == 1)
                    print("-----El NPC SI LLEGO!");
                else
                    print("-----El NPC NO LLEGO"); 


                Instance casoAdecidir = new Instance(casosEntrenamiento.numAttributes());
                casoAdecidir.setDataset(casosEntrenamiento);
                casoAdecidir.setValue(0, fuerzaX);
                casoAdecidir.setValue(1, fuerzaY);
                casoAdecidir.setValue(2, altura);
                casoAdecidir.setValue(3, distancia);
                casoAdecidir.setValue(4, resultado);

                casosEntrenamiento.add(casoAdecidir);

                if (resultado == 1) {
                    break;  //El NPC llego a la plataforma. Empezamos ahora con la siguiente plataforma.
                }
                else
                {
                    yield return new WaitForSeconds(0.5f);  //Para ver donde cayó
                    pausarCalculoAlturas = true;
                    principalNpc.goToSpawn();
                    yield return new WaitUntil(() => (principalNpc.isJumping == false)); //Esperamos a que toque el terreno
                    pausarCalculoAlturas = false;
                }
                //yield return new WaitForSeconds(1.5f);
            }
        }
        print("Se crearon " + casosEntrenamiento.numInstances() + " casos de entrenamiento");
    }


    /// <summary>
    ///     Obtiene la Fuerza a aplicar en el eje Y aplicando la formula de lanzamiento verticial y la segunda ley de Newton
    /// </summary>
    /// <returns></returns>
    float obtenerFuerzaY(float masa, float alturaObjetivo, float distanciaObjetivo)
    {
        if (alturaObjetivo > 0)
            return masa * Mathf.Sqrt(alturaObjetivo * 2.0f * 9.81f); //Mathf.Sqrt(masa * Mathf.Sqrt(alturaObjetivo * 2 * 9.81f)); //factorIncFuerzaYPorDistancia; //(distanciaObjetivo* factorIncFuerzaYPorDistancia);
        else
            return masa * 9.81f; //+ ((masa * 9.81f) / 2.0f); //factorIncFuerzaYPorDistancia; //(distanciaObjetivo * factorIncFuerzaYPorDistancia);
    }

    // Update is called once per frame
    
    
    void Update()
    {
        if (pausarCalculoAlturas)
        {
            alturaMax = -10000f;
            texto = "Alt max esta iteracion: - " + ". Alt max general: " + alturaMaxGeneral;
        } else
        {
            float alturaActual = (transform.position.y - (collider.bounds.size.y / 2f));
            if (alturaActual > alturaMax)
            {
                alturaMax = alturaActual;
                texto = "Alt max esta iteracion: " + alturaMax + ". Alt max general: " + alturaMaxGeneral;
            }


            if (alturaActual > alturaMaxGeneral)
            {
                alturaMaxGeneral = alturaActual;
                texto = "Alt max esta iteracion: " + alturaMax + ". Alt max general: " + alturaMaxGeneral;
            }
        }
            

        



    }
}
