using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UECPersonalArea : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject Inner;
    public GameObject Outer;
    public string InfoText;
    public GameObject UECScene;
    public GameObject PersonalAreaScene;
    #endregion
    #region NormalVariables
    private float initScale;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
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
    #region MouseCheck
    private void OnMouseEnter()
    {
        FindObjectOfType<NotificationBoardController>().CreateNormalInformationBoard(gameObject, InfoText);
    }
    private void OnMouseOver()
    {
        Inner.transform.Rotate(new Vector3(0, 0, 2f));
        Outer.transform.Rotate(new Vector3(0, 0, -2f));
        transform.localScale = new Vector3(initScale * 1.1f, initScale * 1.1f, initScale);
    }
    private void OnMouseExit()
    {
        transform.localScale = new Vector3(initScale, initScale, initScale);
        FindObjectOfType<NotificationBoardController>().DestroyCurrentInfoBoard();
    }
    private void OnMouseDown()
    {
        FindObjectOfType<UECMainMenuController>().TeleportToScene(UECScene, PersonalAreaScene);
        FindObjectOfType<MainMenuCameraController>().GenerateLoadingSceneAtPos(PersonalAreaScene.transform.position, 1f);
    }
    #endregion
}
