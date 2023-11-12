using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotificationBoardController : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject NotificationBoard;
    public GameObject ConfirmationBoard;
    public GameObject ConvertBoard;
    public GameObject RechargeBoard;
    public GameObject InformationBoard;
    public GameObject SmallHUDInfoBoard;
    public GameObject RenameBoard;
    public GameObject MissionCompeletedBoard;
    public GameObject RankUpBoard;
    #endregion
    #region NormalVariables
    public delegate void VoidFunctionPass();
    public VoidFunctionPass VoidReturnFunction;
    private float InitScale;
    private GameObject currentInfoBoard;
    private List<GameObject> currentHUDSmallBoard;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        InitScale = NotificationBoard.transform.GetChild(0).localScale.x;
        currentHUDSmallBoard = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible

    }
    #endregion
    #region Create Noti Board
    public void CreateNormalNotiBoard(Vector2 Position, string text, float autoCloseTimer)
    {
        GameObject notiBoard = Instantiate(NotificationBoard, new Vector3(Position.x,Position.y,NotificationBoard.transform.position.z), Quaternion.identity);
        notiBoard.transform.GetChild(0).localScale = new Vector2(notiBoard.transform.GetChild(0).localScale.x / 10, notiBoard.transform.GetChild(0).localScale.y / 10);
        notiBoard.transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>().text = text;
        notiBoard.SetActive(true);
        StartCoroutine(NotiBoardAnim(autoCloseTimer, notiBoard.transform.GetChild(0).gameObject));
    }

    public void CreateUECMovingNotiBoard(Vector2 Position, string text, float autoCloseTimer)
    {
        GameObject notiBoard = Instantiate(NotificationBoard, new Vector3(Position.x, Position.y, NotificationBoard.transform.position.z), Quaternion.identity);
        notiBoard.transform.GetChild(0).localScale = new Vector2(notiBoard.transform.GetChild(0).localScale.x / 10, notiBoard.transform.GetChild(0).localScale.y / 10);
        notiBoard.transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>().text = text;
        notiBoard.transform.GetChild(0).GetChild(1).GetComponent<NotiBoardOKButton>().MoveToUEC = true;
        notiBoard.SetActive(true);
        StartCoroutine(NotiBoardAnim(autoCloseTimer, notiBoard.transform.GetChild(0).gameObject));
    }
    public void CreateCustomNotiBoard(Vector2 Position, string text, float autoCloseTimer, bool isRed, Color textColor)
    {
        GameObject notiBoard = Instantiate(NotificationBoard, new Vector3(Position.x, Position.y, NotificationBoard.transform.position.z), Quaternion.identity);
        notiBoard.transform.GetChild(0).localScale = new Vector2(notiBoard.transform.GetChild(0).localScale.x / 10, notiBoard.transform.GetChild(0).localScale.y / 10);
        notiBoard.transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>().text = text;
        notiBoard.transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>().color = textColor;
        if (isRed)
        {
            Color c = notiBoard.GetComponent<SpriteRenderer>().color;
            c.g = 0;
            c.b = 0;
            notiBoard.transform.GetChild(0).GetComponent<SpriteRenderer>().color = c;
        }
        // Loading Scene
        notiBoard.SetActive(true);
        StartCoroutine(NotiBoardAnim(autoCloseTimer, notiBoard.transform.GetChild(0).gameObject));
    }

    private IEnumerator NotiBoardAnim(float autoCloseTimer, GameObject go)
    {
        for (int i=0;i<20;i++)
        {
            go.transform.localScale = new Vector2(go.transform.localScale.x + InitScale * 4.5f/100, go.transform.localScale.y + InitScale * 4.5f / 100);
            yield return new WaitForSeconds(0.01f);
        }
        if (autoCloseTimer>0)
        {
            if (go.transform.parent.gameObject!=null)
            {
                Destroy(go.transform.parent.gameObject, autoCloseTimer);
            }
        }
    
    }

    private IEnumerator RankUpBoardAnim(float autoCloseTimer, GameObject go) {
        for (int i = 0; i < 20; i++)
        {
            go.transform.localScale = new Vector2(go.transform.localScale.x + InitScale * 4.5f / 100, go.transform.localScale.y + InitScale * 4.5f / 100);
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(autoCloseTimer);
        for (int i = 0; i < 20; i++)
        {
            Color c1 = go.transform.parent.gameObject.GetComponent<SpriteRenderer>().color;
            c1.a -= 1 / 20f;
            go.transform.parent.gameObject.GetComponent<SpriteRenderer>().color = c1;
            Color c = go.GetComponent<TextMeshPro>().color;
            c.a -= 1 / 20f;
            go.GetComponent<TextMeshPro>().color = c;
            yield return new WaitForSeconds(0.01f);
        }
        Destroy(go.transform.parent.gameObject);
            
        
    }

    public void CreateNormalConfirmBoard(Vector2 Position, string text)
    {
        GameObject notiBoard = Instantiate(ConfirmationBoard, new Vector3(Position.x, Position.y, ConfirmationBoard.transform.position.z), Quaternion.identity);
        notiBoard.transform.GetChild(0).localScale = new Vector2(notiBoard.transform.GetChild(0).localScale.x / 10, notiBoard.transform.GetChild(0).localScale.y / 10);
        notiBoard.transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>().text = text;
        notiBoard.SetActive(true);
        StartCoroutine(NotiBoardAnim(0f, notiBoard.transform.GetChild(0).gameObject));
    }

    public void CreateNormalConvertBoard(Vector2 Position, string ConvertFrom, string ConvertTo, float Rate)
    {
        GameObject notiBoard = Instantiate(ConvertBoard, new Vector3(Position.x, Position.y, ConvertBoard.transform.position.z), Quaternion.identity);
        notiBoard.transform.GetChild(0).localScale = new Vector2(notiBoard.transform.GetChild(0).localScale.x / 10, notiBoard.transform.GetChild(0).localScale.y / 10);
        if (ConvertFrom == "Fuel Energy")
        {
            notiBoard.transform.GetChild(0).GetComponent<ConvertBoard>().isRevert = true;
        }
        if (notiBoard.transform.GetChild(0).GetComponent<ConvertBoard>() != null)
        {
            notiBoard.transform.GetChild(0).GetComponent<ConvertBoard>().SetConvertItem(ConvertFrom, ConvertTo, Rate);
        }
        else
        {
            notiBoard.transform.GetChild(0).GetComponent<ConvertSessionBoard>().SetConvertItem(ConvertFrom, ConvertTo, Rate);
        }
        notiBoard.SetActive(true);
        StartCoroutine(NotiBoardAnim(0f, notiBoard.transform.GetChild(0).gameObject));
    }

    public void CreateNormalRechargeBoard(Vector2 Position)
    {
        GameObject notiBoard = Instantiate(RechargeBoard, new Vector3(Position.x, Position.y, RechargeBoard.transform.position.z), Quaternion.identity);
        notiBoard.transform.GetChild(0).localScale = new Vector2(notiBoard.transform.GetChild(0).localScale.x / 10, notiBoard.transform.GetChild(0).localScale.y / 10);
        notiBoard.SetActive(true);
        StartCoroutine(NotiBoardAnim(0f, notiBoard.transform.GetChild(0).gameObject));
    }

    public void CreateNormalInformationBoard(GameObject go, string text)
    {
        if (text!="")
        {
            GameObject infoBoard = Instantiate(InformationBoard, new Vector3(go.transform.position.x, go.transform.position.y, InformationBoard.transform.position.z), Quaternion.identity);
            infoBoard.transform.GetChild(1).GetComponent<TextMeshPro>().text = text;
            infoBoard.transform.localScale = new Vector3(infoBoard.transform.localScale.x * Camera.main.transform.localScale.x,
                infoBoard.transform.localScale.y * Camera.main.transform.localScale.y, infoBoard.transform.localScale.z);
            infoBoard.GetComponent<InformationBoard>().SetPosition(go);
            currentInfoBoard = infoBoard;
            if (go.name.Contains("FuelCell"))
            {
                go.GetComponent<FuelCellBar>().FuelCellInfo = infoBoard;
            }
        }
    }

    public void CreateHUDSmallInfoBoard(GameObject go, string text, string LeftRightTopBottom)
    {
        if (text!="")
        {
            GameObject smallHUDBoard = Instantiate(SmallHUDInfoBoard,
                new Vector3(go.transform.position.x, go.transform.position.y, SmallHUDInfoBoard.transform.position.z), Quaternion.identity);
            smallHUDBoard.transform.GetChild(0).GetComponent<TextMeshPro>().text = text;
            smallHUDBoard.transform.localScale = new Vector3(smallHUDBoard.transform.localScale.x * Camera.main.transform.localScale.x,
                smallHUDBoard.transform.localScale.y * Camera.main.transform.localScale.y, smallHUDBoard.transform.localScale.z);
            smallHUDBoard.GetComponent<HUDSmallBoard>().SetPosition(go.transform.position, LeftRightTopBottom);
            smallHUDBoard.transform.SetParent(go.transform);
            currentHUDSmallBoard.Add(smallHUDBoard);
        }
    }

    public void CreateNormalRenameBoard(Vector2 Position, string NewName)
    {
        GameObject notiBoard = Instantiate(RenameBoard, new Vector3(Position.x, Position.y, RenameBoard.transform.position.z), Quaternion.identity);
        notiBoard.transform.GetChild(0).localScale = new Vector2(notiBoard.transform.GetChild(0).localScale.x / 10, notiBoard.transform.GetChild(0).localScale.y / 10);
        notiBoard.SetActive(true);
        StartCoroutine(NotiBoardAnim(0f, notiBoard.transform.GetChild(0).gameObject));
    }

    public void CreateMissionCompletedNotiBoard(string text, float autoCloseTimer)
    {
        Vector2 Position = Camera.main.transform.position;
        int pos = 0;
        if (FindAnyObjectByType<UECMainMenuController>() != null)
        {
            pos = 4;
        }
        else
        {
            pos = 250;
        }
        GameObject notiBoard = Instantiate(MissionCompeletedBoard, new Vector3(Position.x, Position.y - pos, MissionCompeletedBoard.transform.position.z), Quaternion.identity);
        notiBoard.transform.GetChild(0).localScale = new Vector2(notiBoard.transform.GetChild(0).localScale.x / 1.5f, notiBoard.transform.GetChild(0).localScale.y);
        notiBoard.transform.GetChild(0).GetComponent<TextMeshPro>().text = text + "<br> Mission Completed!";
        notiBoard.SetActive(true);
        if (FindAnyObjectByType<UECMainMenuController>() == null)
        {
            notiBoard.transform.SetParent(Camera.main.transform);
        }
        StartCoroutine(NotiBoardAnim(autoCloseTimer, notiBoard.transform.GetChild(0).gameObject));
    }
    public void CreateRankUpNotiBoard(string text, float autoCloseTimer)
    {
        Vector2 Position = Camera.main.transform.position;
        int pos = 0;
        if (FindAnyObjectByType<UECMainMenuController>() != null || FindAnyObjectByType<SessionSummary>() != null)
        {
            pos = 3;
        } else
        {
            pos = 500;
        }
        GameObject notiBoard = Instantiate(RankUpBoard, new Vector3(Position.x, Position.y + pos, RankUpBoard.transform.position.z), Quaternion.identity);
        notiBoard.transform.GetChild(0).localScale = new Vector2(notiBoard.transform.GetChild(0).localScale.x / 2f, notiBoard.transform.GetChild(0).localScale.y / 2f);
        notiBoard.transform.GetChild(0).GetComponent<TextMeshPro>().text = "Congratulation! <br>You have reach " + text;
        notiBoard.SetActive(true);
        if (FindAnyObjectByType<UECMainMenuController>() == null)
        {          
            notiBoard.transform.SetParent(Camera.main.transform);
        }
        StartCoroutine(RankUpBoardAnim(autoCloseTimer, notiBoard.transform.GetChild(0).gameObject));
    }

    public void DestroyCurrentInfoBoard()
    {
        currentInfoBoard.GetComponent<InformationBoard>().Close();
    }

    public void DestroyAllCurrentHUDSmallBoard()
    {
        foreach (var board in currentHUDSmallBoard)
        {
            Destroy(board);
        }
    }

    public void ConfirmOnConfirmationBoard()
    {
        VoidReturnFunction();
        VoidReturnFunction = null;
    }
    #endregion
}
