using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnClickStartGame : MonoBehaviour
{
    public GameObject infoTextGameObject;
    float time;
    // Start is called before the first frame update
    void Start()
    {
        time = 0;
        Button startGameBtn = GetComponent<Button>();
        startGameBtn.onClick.AddListener(() =>
        {
            bool existsOutputModelFile = File.Exists("Assets/WekaData/output_model.model");
            if (existsOutputModelFile)
            {
                Loader.Load(Loader.Scene.Simulation);
            }
            else
            {
                if (infoTextGameObject != null)
                {
                    Text txt = infoTextGameObject.GetComponent<Text>();
                    txt.text = "Primero debes realizar un entrenamiento a tu NPC!\nPulsa en \"Entrenar NPC\"";
                }
            }

        });
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time >= 5)
        {
            if (infoTextGameObject != null)
            {
                Text txt = infoTextGameObject.GetComponent<Text>();
                txt.text = "";
            }
            time = 0;
        }
    }
}
