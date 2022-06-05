using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using weka.core;


public class JumpingAI : MonoBehaviour
{
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


    void Start()
    {
        Time.timeScale = 5;
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

                float altura = principalNpc.getNextPlatform().transform.position.y - transform.position.y;
                float distancia = Mathf.Abs(transform.position.x - principalNpc.getNextPlatform().transform.position.x);
                float fuerzaY = obtenerFuerzaY(rb.mass, altura, distancia);

                print("Altura: " + altura + ". Distancia: " + distancia);
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
                    principalNpc.goToSpawn();
                    yield return new WaitUntil(() => (principalNpc.isJumping == false)); //Esperamos a que toque el terreno
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
            return masa * Mathf.Sqrt(alturaObjetivo * 2 * 9.81f * factorIncAltura); //Mathf.Sqrt(masa * Mathf.Sqrt(alturaObjetivo * 2 * 9.81f)); //factorIncFuerzaYPorDistancia; //(distanciaObjetivo* factorIncFuerzaYPorDistancia);
        else
            return masa * 9.81f + ((masa * 9.81f) / 2.0f); //factorIncFuerzaYPorDistancia; //(distanciaObjetivo * factorIncFuerzaYPorDistancia);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
