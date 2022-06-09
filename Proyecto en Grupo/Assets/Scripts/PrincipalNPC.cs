using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrincipalNPC : MonoBehaviour
{
    GameObject respawn;

    GameObject nextPlatform;
    GameObject actualPlatform;

    Rigidbody rb;

    bool finished; //Si el NPC llego a la meta
    float npcHeight; //Altura del NPC
    public bool inNextPlatform;
    public float time;
    
    public bool isJumping; //Si el NPC se encuentra en el aire
    //bool internalPlatformPressed;    //Indica si la plataforma interna fue pisada alguna vez. Nos ayudara a conocer si el NPC pis� o no la plataforma. Nota: Hay que resetar esto cada vez que saltemos

    public float timeScaleGame;

    public bool manualJump = false;

    private void Awake()
    {
        //internalPlatformPressed = false;
        finished = false;
        isJumping = false;
        inNextPlatform = false;
        npcHeight = GetComponent<Collider>().bounds.size.y;

        respawn = GameObject.FindGameObjectWithTag("Respawn");

        actualPlatform = respawn;
        nextPlatform = respawn.GetComponent<SpawnPlatform>().nextPlatformPrediction;
        //nextPlatform = respawn.GetComponent<Platform>().nextPlatform;
        float actSpawnHeight = respawn.GetComponent<Collider>().bounds.size.y;
        transform.position = new Vector3(respawn.transform.position.x, respawn.transform.position.y + (actSpawnHeight / 2f) + (npcHeight / 2f), respawn.transform.position.z);

        if (nextPlatform != null)
        {
            transform.LookAt(nextPlatform.transform.position);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        // Time.timeScale = timeScaleGame;
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
        actualPlatform = respawn;

        nextPlatform = respawn.GetComponent<SpawnPlatform>().nextPlatformPrediction;


        float actSpawnHeight = respawn.GetComponent<Collider>().bounds.size.y;
        transform.position = new Vector3(respawn.transform.position.x, respawn.transform.position.y + (actSpawnHeight / 2f) + (npcHeight / 2f), respawn.transform.position.z);
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

    //public bool InternalPlatformPressed()
    //{
    //    return internalPlatformPressed;
    //}

    //public void ResetInternalPlatformPressed()
    //{
    //    internalPlatformPressed = false;
    //}

    //public void SetInternalPlatformPressed()
    //{
    //    internalPlatformPressed = true;
    //}

}
