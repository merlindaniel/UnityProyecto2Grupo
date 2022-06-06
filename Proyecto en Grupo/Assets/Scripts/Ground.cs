using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "NPC") {
            other.gameObject.GetComponent<PrincipalNPC>().GoToActualPlatform();
            //Transform spawnTransform = GameObject.FindGameObjectWithTag("Respawn").transform;
            //other.transform.position = new Vector3(spawnTransform.position.x, spawnTransform.position.y + 10, spawnTransform.position.z);
        }
        
    }
}
