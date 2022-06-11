using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingNPC : MonoBehaviour
{
    GameObject respawnPlatform;
    GameObject nextPlatform;
    GameObject currentPlatform;

    Rigidbody rbNPC;

    bool finished; //Si el NPC llego a la meta
    float npcHeight; //Altura del NPC
    [HideInInspector] public bool isSetToJump;

    float time;

    // Saltar manualmente / Respawnear al NPC
    public bool manualJump = false, respawn = false;

    private void Awake()
    {
        finished = false;
        isSetToJump = false;
        npcHeight = GetComponent<Collider>().bounds.size.y;

        respawnPlatform = GameObject.FindGameObjectWithTag("Respawn");
        currentPlatform = respawnPlatform;
        nextPlatform = respawnPlatform;
        
        float actSpawnHeight = respawnPlatform.GetComponent<Collider>().bounds.size.y;
        transform.position = new Vector3(respawnPlatform.transform.position.x, respawnPlatform.transform.position.y + (actSpawnHeight / 2f) + (npcHeight / 2f), respawnPlatform.transform.position.z);
    }

    // Start is called before the first frame update
    void Start()
    {
        time = 0;
        rbNPC = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -100)
        {
            rbNPC.isKinematic = true;
            rbNPC.isKinematic = false;
            GoToSpawn();
        }

        // Preparar salto
        if (isSetToJump)
        {
            time += Time.deltaTime;
            if (time >= 0.1) // Mirar a la plataforma
            {
                rbNPC.isKinematic = false;
                LookNextPlatform();
            }
            if (time >= 0.3) // Predecir y saltar
            {
                isSetToJump = false;
                time = 0;
                GetComponent<JumpingAI>().PredictAndExecute();
            }
        }

        if (manualJump)
        {
            LookNextPlatform();
            GetComponent<JumpingAI>().PredictAndExecute();
            manualJump = false;
        }

        if (respawn)
        {
            rbNPC.isKinematic = true;
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
        currentPlatform = respawnPlatform;

        float actSpawnHeight = respawnPlatform.GetComponent<Collider>().bounds.size.y;
        transform.position = new Vector3(respawnPlatform.transform.position.x, respawnPlatform.transform.position.y + (actSpawnHeight / 2f) + (npcHeight / 2f) + 1, respawnPlatform.transform.position.z);
    }

    public void GoToCurrentPlatform()
    {
        float actPlatformHeight = currentPlatform.GetComponent<Collider>().bounds.size.y;
        transform.position = new Vector3(currentPlatform.transform.position.x, currentPlatform.transform.position.y + (actPlatformHeight/2f) + (npcHeight/2f) + 1, currentPlatform.transform.position.z);
    }

    public void SetNextPlatform(GameObject nextPlatform)
    {
        this.currentPlatform = this.nextPlatform;
        this.nextPlatform = nextPlatform;
    }

    public void jumpRelative(float forceX, float forceY, float forceZ)
    {
        rbNPC.AddRelativeForce(new Vector3(forceX, forceY, forceZ), ForceMode.Impulse);
    }

    public void SetFinished(bool finish)
    {
        this.finished = finish;
        rbNPC.isKinematic = finish;
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
        return currentPlatform;
    }

    public float GetNpcHeight()
    {
        return npcHeight;
    }
}
