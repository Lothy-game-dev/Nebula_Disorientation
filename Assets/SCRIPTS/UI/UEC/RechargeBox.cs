using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class RechargeBox : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject NotiBoard;
    public RechargeBoard board;
    public TextMeshPro text;
    public RechargeBox[] OtherBoxes;
    #endregion
    #region NormalVariables
    private bool isChosen;
    private string rechargeAmount;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        isChosen = false;
        rechargeAmount = text.text.Split("->")[1].Replace(" ", "");
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        bool check = false;
        foreach (var hit in hits)
        {
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.name == name)
                {
                    check = true;
                }
            }
        }
        if (check)
        {
            NotiBoard.GetComponent<NotificationBoard>().DisableCollider *= 0;
        }
        else
        {
            NotiBoard.GetComponent<NotificationBoard>().DisableCollider *= 1;
        }
    }
    #endregion
    #region MouseCheck
    private void OnMouseDown()
    {
        if (!isChosen)
        {
            isChosen = true;
            Color c = GetComponent<SpriteRenderer>().color;
            c.r = 0;
            c.b = 0.5f;
            GetComponent<SpriteRenderer>().color = c;
            foreach (var but in OtherBoxes)
            {
                but.Uncheck();
            }
            board.CurrentShardRecharging = int.Parse(rechargeAmount);
        } else
        {
            isChosen = false;
            Color c = GetComponent<SpriteRenderer>().color;
            c.r = 1;
            c.b = 1;
            GetComponent<SpriteRenderer>().color = c;
            board.CurrentShardRecharging = 0;
        }
    }

    public void Uncheck()
    {
        if (isChosen)
        {
            isChosen = false;
            Color c = GetComponent<SpriteRenderer>().color;
            c.r = 1;
            c.b = 1;
            GetComponent<SpriteRenderer>().color = c;
        }
    }
    #endregion
}
