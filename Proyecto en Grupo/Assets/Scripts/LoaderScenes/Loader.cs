using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
    public enum Scene
    {
        MainMenu,
        Main,
        TrainingAI
    }

    public static void Load(Scene scene)
    {
        SceneManager.LoadScene(scene.ToString());
        // SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
    } 
}
