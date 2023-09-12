using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UECEnterSession : MonoBehaviour
{
    #region ComponentVariables
    private Animator anim;
    #endregion
    #region InitializeVariables
    public string InfoText;
    public GameObject UECScene;
    public GameObject LoadoutScene;
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
        anim = GetComponent<Animator>();
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
        anim.ResetTrigger("Stop");
        anim.SetTrigger("Move");
        FindObjectOfType<NotificationBoardController>().CreateNormalInformationBoard(gameObject, InfoText);
    }

    private void OnMouseDown()
    {
        FindObjectOfType<UECMainMenuController>().TeleportToScene(UECScene, LoadoutScene);
        FindObjectOfType<MainMenuCameraController>().GenerateLoadingSceneAtPos(LoadoutScene.transform.position, 1f);
    }

    private void OnMouseExit()
    {
        anim.ResetTrigger("Move");
        anim.SetTrigger("Stop");
        FindObjectOfType<NotificationBoardController>().DestroyCurrentInfoBoard();
    }
    #endregion
}
