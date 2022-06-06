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

    Rigidbody rigidbody;

    float npcHeight;

    private void Awake()
    {
        respawn = GameObject.FindGameObjectWithTag("Respawn");

        actualPlatform = respawn;
        nextPlatform = respawn.GetComponent<Platform>().nextPlatform;
        transform.position = new Vector3(respawn.transform.position.x, respawn.transform.position.y + 10, respawn.transform.position.z);
        if (nextPlatform != null)
            transform.LookAt(nextPlatform.transform.position);

        npcHeight = GetComponent<Collider>().bounds.size.y;
    }

    // Start is called before the first frame update
    void Start()
    {
        finished = false;
        isJumping = false;
        rigidbody = GetComponent<Rigidbody>();
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
        //if (!this.nextPlatform.GetInstanceID().Equals(nextPlatform.GetInstanceID()))
        //{
        this.actualPlatform = this.nextPlatform;
        this.nextPlatform = nextPlatform;
        if (nextPlatform != null)
            transform.LookAt(nextPlatform.transform.position);
        //}
    }

    public void jumpRelative(float forceX, float forceY, float forceZ)
    {
        isJumping = true;
        rigidbody.AddRelativeForce(new Vector3(forceX, forceY, forceZ), ForceMode.Impulse);
    }


    public void SetFinished()
    {
        finished = true;
        actualPlatform = nextPlatform;
        nextPlatform = null;
        rigidbody.isKinematic = true;
    }

    public bool IsFinished()
    {
        return finished;
    }

    public GameObject GetNextPlatform()
    {
        return nextPlatform;
    }
}
