using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrincipalNPC : MonoBehaviour
{
    GameObject nextPlatform;
    bool finished;
    public bool isJumping;

    

    private void Awake()
    {
        nextPlatform = GameObject.FindGameObjectWithTag("Respawn").GetComponent<Platform>().nextPlatform;
        Transform spawnTransform = GameObject.FindGameObjectWithTag("Respawn").transform;
        transform.position = new Vector3(spawnTransform.position.x, spawnTransform.position.y + 10, spawnTransform.position.z);
    }

    // Start is called before the first frame update
    void Start()
    {
        finished = false;
        isJumping = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void goToSpawn()
    {
        Transform spawnTransform = GameObject.FindGameObjectWithTag("Respawn").transform;
        transform.position = new Vector3(spawnTransform.position.x, spawnTransform.position.y + 10, spawnTransform.position.z);
        nextPlatform = GameObject.FindGameObjectWithTag("Respawn").GetComponent<Platform>().nextPlatform;
        isJumping = false;
    }

    public void lookAtNextPlatform(GameObject nextPlatform)
    {
        transform.LookAt(nextPlatform.transform);
        this.nextPlatform = nextPlatform;
        isJumping = false;
    }

    public void setFinished()
    {
        finished = true;
    }

    public bool isFinished()
    {
        return finished;
    }

    public GameObject getNextPlatform()
    {
        return nextPlatform;
    }
}
