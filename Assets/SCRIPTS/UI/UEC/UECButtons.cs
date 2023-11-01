using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UECButtons : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public string InfoText;
    #endregion
    #region NormalVariables
    private float initScaleX;
    private float initScaleY;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        initScaleX = transform.localScale.x;
        initScaleY = transform.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Check Mouse
    private void OnMouseEnter()
    {
        transform.localScale = new Vector3(initScaleX * 1.1f, initScaleY * 1.1f, 0);
        GetComponent<SpriteRenderer>().sortingOrder = 2;
        FindObjectOfType<NotificationBoardController>().CreateNormalInformationBoard(gameObject, InfoText);
    }
    private void OnMouseExit()
    {
        transform.localScale = new Vector3(initScaleX, initScaleY, 0);
        GetComponent<SpriteRenderer>().sortingOrder = 1;
        FindObjectOfType<NotificationBoardController>().DestroyCurrentInfoBoard();
    }
    private void OnMouseDown()
    {
        FindObjectOfType<SoundSFXGeneratorController>().GenerateSound("ButtonClick");
        if ("Quit".Equals(name))
        {
            FindObjectOfType<MainMenuCameraController>().GenerateBlackFadeClose(0.5f, 3f);
            // Back to main menu
            PlayerPrefs.SetFloat("CreateLoading", 1f);
            SceneManager.UnloadSceneAsync("UECMainMenu");
            SceneManager.LoadSceneAsync("MainMenu");
        } else if ("Options".Equals(name))
        {
            FindObjectOfType<MainMenuCameraController>().GenerateBlackFadeClose(0.5f, 3f);
            // Back to main menu and go to option
            PlayerPrefs.SetFloat("CreateLoading", 1f);
            PlayerPrefs.SetInt("ToOption", 1);
            PlayerPrefs.SetInt("BackToUEC", 1);
            SceneManager.UnloadSceneAsync("UECMainMenu");
            SceneManager.LoadSceneAsync("MainMenu");
        } else if ("Encyclopedia".Equals(name))
        {
            FindObjectOfType<MainMenuCameraController>().GenerateBlackFadeClose(0.5f, 3f);
            // Back to main menu and go to encyc
            PlayerPrefs.SetFloat("CreateLoading", 1f);
            PlayerPrefs.SetInt("ToEncyclopedia", 1);
            PlayerPrefs.SetInt("BackToUEC", 1);
            SceneManager.UnloadSceneAsync("UECMainMenu");
            SceneManager.LoadSceneAsync("MainMenu");
        }
    }
    #endregion
}
