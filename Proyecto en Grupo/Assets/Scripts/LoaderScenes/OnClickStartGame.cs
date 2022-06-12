using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnClickStartGame : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Button startGameBtn = GetComponent<Button>();
        startGameBtn.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.Main);
        });
    }

    // Update is called once per frame
    void Update()
    {

    }
}
