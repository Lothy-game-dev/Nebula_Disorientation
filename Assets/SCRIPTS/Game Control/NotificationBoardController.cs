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
    #endregion
    #region NormalVariables
    public delegate void VoidFunctionPass();
    public VoidFunctionPass VoidReturnFunction;
    private float InitScale;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        InitScale = NotificationBoard.transform.GetChild(0).localScale.x;
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
            Destroy(go.transform.parent.gameObject, autoCloseTimer);
        }
    }

    public void CreateNormalConfirmBoard(Vector2 Position, string text)
    {
        GameObject notiBoard = Instantiate(ConfirmationBoard, new Vector3(Position.x, Position.y, ConfirmationBoard.transform.position.z), Quaternion.identity);
        notiBoard.transform.GetChild(0).localScale = new Vector2(notiBoard.transform.GetChild(0).localScale.x / 10, notiBoard.transform.GetChild(0).localScale.y / 10);
        notiBoard.transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>().text = text;
        notiBoard.SetActive(true);
        StartCoroutine(NotiBoardAnim(0f, notiBoard.transform.GetChild(0).gameObject));
    }

    public void ConfirmOnConfirmationBoard()
    {
        VoidReturnFunction();
    }
    #endregion
}
