using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadOutGoButton : MonoBehaviour
{
    #region ComponentVariables
    #endregion
    #region InitializeVariables
    public LoadoutScene Scene;
    public GameObject Outer;
    public GameObject Inner;
    #endregion
    #region NormalVariables
    private float initScale;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void OnEnable()
    {
        // Initialize variables
        initScale = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Mouse Check
    private void OnMouseEnter()
    {
        transform.localScale = new Vector2(initScale * 1.1f, initScale * 1.1f);
    }

    private void OnMouseOver()
    {
        // Rotate circles
        Inner.transform.Rotate(new Vector3(0, 0, 1));
        Outer.transform.Rotate(new Vector3(0, 0, -1));
    }

    private void OnMouseExit()
    {
        transform.localScale = new Vector2(initScale, initScale);
    }

    private void OnMouseDown()
    {
        if (Scene.GetComponent<LoadoutScene>().CurrentFuelCells>0)
        {
            //Data
            FindObjectOfType<NotificationBoardController>().VoidReturnFunction = SetDataToScene;
            FindObjectOfType<NotificationBoardController>().CreateNormalConfirmBoard(Scene.transform.position,
                    "Will these items be your final decision for this session?");
        } else
        {
            FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Scene.transform.position,
                "More Fuel Cell required!", 5f);
        }

    }

    public void SetDataToScene()
    {
        string check = Scene.GetComponent<LoadoutScene>().SetDataToDb();
        if ("Success".Equals(check))
        {
            FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Scene.transform.position,
                "Load out successfully!\nYour session will start soon!",3f);
            PlayerPrefs.SetInt("PlayerID", FindObjectOfType<UECMainMenuController>().PlayerId);
            SceneManager.LoadSceneAsync("GameplayExterior");
            SceneManager.UnloadSceneAsync("UECMainMenu");
        } else if ("Fail".Equals(check))
        {
            FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Scene.transform.position,
                "Cannot create session!\nPlease contact to our email!", 3f);
        } else if ("No Exist".Equals(check))
        {
            FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Scene.transform.position,
                "Cannot fetch data for this pilot!\nPlease try again!", 3f);
        }
    }
    #endregion
}
