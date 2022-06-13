using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class Simulation : MonoBehaviour
{

    [Header("Basic Simulation Settings")]
    public float timeScaleGame;
    public int numNPCs = 2;
    public int numDragons = 1;

    // Prefabs
    [Header("Prefabs Settings")]
    public GameObject Dragon;
    public GameObject NPC;

    // Platform
    [Header("Platform Settings")]
    public bool drawLinesBetweenPlatforms = false;
    public Color lineColor = Color.white;

    [Header("UI Settings")]
    public GameObject BackButtonGameObject;

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

        if (BackButtonGameObject != null)
        {
            Button btn = BackButtonGameObject.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                // SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
                Loader.Load(Loader.Scene.MainMenu);
            });
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
