using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Simulation : MonoBehaviour
{
    public float timeScaleGame;

    public int numNPCs = 2;
    public int numDragons = 1;

    // Prefabs
    public GameObject Dragon, NPC;

    // Platform
    public bool drawLinesBetweenPlatforms = false;
    public Color lineColor = Color.white;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = timeScaleGame;

        for (int i = 0; i < numNPCs; i++)
        {
            GameObject npc = Instantiate(NPC);
        }

        for (int i = 0; i < numDragons; i++)
        {
            GameObject dragon = Instantiate(
                Dragon, 
                transform.localPosition + Vector3.right * Random.Range(-10f, 10f) + Vector3.forward * Random.Range(-10f, 10f), 
                Quaternion.identity
            );
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale != timeScaleGame)
        {
            Time.timeScale = timeScaleGame;
        }

        if (drawLinesBetweenPlatforms)
        {
            FindObjectsOfType<Platform>()
                .ToList()
                .ForEach(platform =>
                {
                    platform.DrawLine(lineColor);
                });
        }
    }
}
