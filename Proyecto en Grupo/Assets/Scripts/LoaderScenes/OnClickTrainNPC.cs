using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class OnClickTrainNPC : MonoBehaviour
{
    // Start is called before the first frame update

    void Start()
    {
        Button trainNPCButton = GetComponent<Button>();
        trainNPCButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.TrainingAI);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
