using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrincipalNPC : MonoBehaviour
{
    GameObject respawnGameObject;

    GameObject nextPlatform;
    GameObject actualPlatform;

    Rigidbody rb;

    bool finished; //Si el NPC llego a la meta
    float npcHeight; //Altura del NPC
    [HideInInspector] public bool inNextPlatform;
    [HideInInspector] public bool isJumping; //Si el NPC se encuentra en el aire

    float time;
    public float timeScaleGame;

    // Saltar manualmente / Respawnear al NPC
    public bool manualJump = false, respawn = false;

    private void Awake()
    {
        //internalPlatformPressed = false;
        finished = false;
        isJumping = false;
        inNextPlatform = false;
        npcHeight = GetComponent<Collider>().bounds.size.y;

        respawnGameObject = GameObject.FindGameObjectWithTag("Respawn");

        actualPlatform = respawnGameObject;
        nextPlatform = respawnGameObject.GetComponent<SpawnPlatform>().nextPlatformPrediction;
        float actSpawnHeight = respawnGameObject.GetComponent<Collider>().bounds.size.y;
        transform.position = new Vector3(respawnGameObject.transform.position.x, respawnGameObject.transform.position.y + (actSpawnHeight / 2f) + (npcHeight / 2f), respawnGameObject.transform.position.z);

        if (nextPlatform != null)
        {
            transform.LookAt(nextPlatform.transform.position);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        time = 0;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (transform.position.y < -100)
        {
            rb.isKinematic = true;
            rb.isKinematic = false;
            GoToSpawn();
        }


        if (inNextPlatform)
        {
            time += Time.deltaTime;
            if (time >= 0.1)
            {
                rb.isKinematic = false;
                LookNextPlatform();
            }
            if (time >= 0.3)
            {
                inNextPlatform = false;
                time = 0;
                GetComponent<JumpingAI>().PredictAndJumpToNextPlatform();
            }
        }

        if (manualJump)
        {
            LookNextPlatform();
            GetComponent<JumpingAI>().PredictAndJumpToNextPlatform();
            manualJump = false;
        }

        if (respawn)
        {
            GoToSpawn();
            respawn = false;
        }
    }

    public void LookNextPlatform()
    {
        if (nextPlatform != null)
        {
            transform.LookAt(nextPlatform.transform.position);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
    }

    public void GoToSpawn()
    {
        isJumping = true; //Spawnea en al aire. Tomaremos esto como un salto
        actualPlatform = respawnGameObject;

        nextPlatform = respawnGameObject.GetComponent<SpawnPlatform>().nextPlatformPrediction;


        float actSpawnHeight = respawnGameObject.GetComponent<Collider>().bounds.size.y;
        transform.position = new Vector3(respawnGameObject.transform.position.x, respawnGameObject.transform.position.y + (actSpawnHeight / 2f) + (npcHeight / 2f), respawnGameObject.transform.position.z);
        LookNextPlatform();
        GetComponent<JumpingAI>().PredictAndJumpToNextPlatform();
    }


    public void GoToActualPlatform()
    {
        isJumping = true;
        float actPlatformHeight = actualPlatform.GetComponent<Collider>().bounds.size.y;
        transform.position = new Vector3(actualPlatform.transform.position.x, actualPlatform.transform.position.y + (actPlatformHeight/2f) + (npcHeight/2f) + 1, actualPlatform.transform.position.z);
        LookNextPlatform(); //Para arreglar las microrotaciones que se producen en el salto anterior
    }

    //Cambia la plataforma 
    public void SetNextPlatform(GameObject nextPlatform)
    {
        this.actualPlatform = this.nextPlatform;
        this.nextPlatform = nextPlatform;
        //LookNextPlatform();
    }

    public void jumpRelative(float forceX, float forceY, float forceZ)
    {
        isJumping = true;
        rb.AddRelativeForce(new Vector3(forceX, forceY, forceZ), ForceMode.Impulse);
    }

    public void jump(float forceX, float forceY, float forceZ)
    {
        isJumping = true;
        rb.AddForce(new Vector3(forceZ, forceY, 0), ForceMode.Impulse);
    }

    public void SetFinished(bool finish)
    {
        this.finished = finish;
        rb.isKinematic = finish;
    }

    public bool IsFinished()
    {
        return finished;
    }

    public GameObject GetNextPlatform()
    {
        return nextPlatform;
    }

    public GameObject GetActualPlatform()
    {
        return actualPlatform;
    }

    public float GetNpcHeight()
    {
        return npcHeight;
    }


}
