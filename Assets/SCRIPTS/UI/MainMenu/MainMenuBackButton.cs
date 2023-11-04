using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuBackButton : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    public GameObject CurrentScene;
    public GameObject BackScene;
    public OptionMenu OptionMenu;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<Collider2D>() == null)
        {
            gameObject.AddComponent<BoxCollider2D>();
        }
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
        if (CurrentScene.name.Contains("Option"))
        {
            FindAnyObjectByType<AccessDatabase>().UpdateOptionSetting(Mathf.RoundToInt(float.Parse(OptionMenu.Master)),
                                    Mathf.RoundToInt(float.Parse(OptionMenu.Music)), Mathf.RoundToInt(float.Parse(OptionMenu.Sound)), Mathf.RoundToInt(float.Parse(OptionMenu.FpsCounter)), OptionMenu.Resol);
            OptionMenu.IsSaved = true;
            Camera.main.GetComponent<SoundController>().CheckSoundVolumeByDB();
        }
        if (PlayerPrefs.GetInt("BackToUEC") == 1)
        {
            PlayerPrefs.SetInt("BackToUEC", 0);
            FindObjectOfType<MainMenuCameraController>().MoveToUEC();
        } else
        if (BackScene != null && BackScene.name != "MainMenuScene")
        {
            FindObjectOfType<MainMenuCameraController>().ChangeToScene(CurrentScene, BackScene);
        } else
        {
            FindObjectOfType<MainMenuCameraController>().BackToMainMenu(CurrentScene);
        }
    }
    #endregion
    #region Function group ...
    // Group all function that serve the same algorithm
    #endregion
}
