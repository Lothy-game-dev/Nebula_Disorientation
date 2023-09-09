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
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
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
    }
    private void OnMouseExit()
    {
        transform.localScale = new Vector3(initScaleX, initScaleY, 0);
        GetComponent<SpriteRenderer>().sortingOrder = 1;
    }
    private void OnMouseDown()
    {
        if ("Quit".Equals(name))
        {
            PlayerPrefs.SetFloat("CreateLoading", 1f);
            SceneManager.UnloadSceneAsync("UECMainMenu");
            SceneManager.LoadSceneAsync("MainMenu");
        } else if ("Options".Equals(name))
        {
            PlayerPrefs.SetFloat("CreateLoading", 1f);
            PlayerPrefs.SetInt("ToOption", 1);
            PlayerPrefs.SetInt("BackToUEC", 1);
            SceneManager.UnloadSceneAsync("UECMainMenu");
            SceneManager.LoadSceneAsync("MainMenu");
        }
    }
    #endregion
}
