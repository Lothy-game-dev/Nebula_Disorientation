using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuButtons : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public PauseMenuOption Options;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Function group 1
    // Group all function that serve the same algorithm
    private void OnMouseDown()
    {
        FindObjectOfType<SoundSFXGeneratorController>().GenerateSound("ButtonClick");
        if (name.Contains("Back"))
        {
            Options.SaveData();
            FindObjectOfType<CameraController>().PauseGame();
        } else if (name.Contains("Quit"))
        {
            Time.timeScale = 1;
            PlayerPrefs.SetFloat("CreateLoading", 1f);
            SceneManager.UnloadSceneAsync("GameplayInterior");
            SceneManager.LoadSceneAsync("MainMenu");
        }
    }
    #endregion
}
