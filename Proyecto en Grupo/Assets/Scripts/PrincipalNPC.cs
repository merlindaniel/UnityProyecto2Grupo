using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrincipalNPC : MonoBehaviour
{
    GameObject respawn;

    GameObject nextPlatform;
    GameObject actualPlatform;

    bool finished; //Si el NPC llego a la meta
    public bool isJumping; //Si el NPC se encuentra en el aire

    Rigidbody rb;

    float npcHeight;

    private void Awake()
    {
        Time.timeScale = 3;
        respawn = GameObject.FindGameObjectWithTag("Respawn");

        actualPlatform = respawn;
        nextPlatform = respawn.GetComponent<Platform>().nextPlatform;
        transform.position = new Vector3(respawn.transform.position.x, respawn.transform.position.y + 10, respawn.transform.position.z);
        if (nextPlatform != null)
        {
            transform.LookAt(nextPlatform.transform.position);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
            

        npcHeight = GetComponent<Collider>().bounds.size.y;
    }

    // Start is called before the first frame update
    void Start()
    {
        finished = false;
        isJumping = false;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoToSpawn()
    {
        isJumping = true; //Spawnea en al aire. Tomaremos esto como un salto
        //Transform spawnTransform = GameObject.FindGameObjectWithTag("Respawn").transform;
        //transform.position = new Vector3(spawnTransform.position.x, spawnTransform.position.y + 10, spawnTransform.position.z);
        //nextPlatform = GameObject.FindGameObjectWithTag("Respawn").GetComponent<Platform>().nextPlatform;
        actualPlatform = respawn;
        nextPlatform = respawn.GetComponent<Platform>().nextPlatform;
        transform.position = new Vector3(respawn.transform.position.x, respawn.transform.position.y + 10, respawn.transform.position.z);
    }


    public void GoToActualPlatform()
    {
        isJumping = true;
        float actPlatformHeight = actualPlatform.GetComponent<Collider>().bounds.size.y;
        transform.position = new Vector3(actualPlatform.transform.position.x, actualPlatform.transform.position.y + (actPlatformHeight/2f) + (npcHeight/2f) + 1, actualPlatform.transform.position.z);
    }

    public void ChangeNextPlatform(GameObject nextPlatform)
    {
        this.actualPlatform = this.nextPlatform;
        this.nextPlatform = nextPlatform;
        if (nextPlatform != null)
        {
            transform.LookAt(nextPlatform.transform.position);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
    }

    public void jumpRelative(float forceX, float forceY, float forceZ)
    {
        isJumping = true;
        rb.AddRelativeForce(new Vector3(forceX, forceY, forceZ), ForceMode.Impulse);
    }


    public void SetFinished()
    {
        finished = true;
        actualPlatform = nextPlatform;
        nextPlatform = null;
        rb.isKinematic = true;
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
}
