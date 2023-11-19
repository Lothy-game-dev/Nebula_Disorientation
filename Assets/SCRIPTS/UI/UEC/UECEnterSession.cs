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
    public GameObject FactoryScene;
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
        FindObjectOfType<SoundSFXGeneratorController>().GenerateSound("ButtonClick");
        List<string> check = FindObjectOfType<AccessDatabase>().GetAllOwnedModel(FindObjectOfType<UECMainMenuController>().PlayerId);
        if (check.Count > 0)
        {
            FindObjectOfType<MainMenuCameraController>().GenerateBlackFadeClose(1f, 5f);
            StartCoroutine(MoveToLoadout());
        }
        else
        {
            FindObjectOfType<NotificationBoardController>().VoidReturnFunction = MoveToFactory;
            FindObjectOfType<NotificationBoardController>().CreateNormalConfirmBoard(UECScene.transform.position,
                "You don't own a fighter yet. Get your first one in the Factory!");
        }
    }

    private IEnumerator MoveToLoadout()
    {
        yield return new WaitForSeconds(1.5f);
        FindObjectOfType<UECMainMenuController>().TeleportToScene(UECScene, LoadoutScene);
        FindObjectOfType<MainMenuCameraController>().GenerateLoadingSceneAtPos(LoadoutScene.transform.position, 1f);
    }

    private void OnMouseExit()
    {
        anim.ResetTrigger("Move");
        anim.SetTrigger("Stop");
        FindObjectOfType<NotificationBoardController>().DestroyCurrentInfoBoard();
    }

    public void MoveToFactory()
    {
        FactoryScene.GetComponent<FactoryScene>().FromLoadout = true;
        FindObjectOfType<UECMainMenuController>().TeleportToScene(UECScene, FactoryScene);
    }
    #endregion
}
