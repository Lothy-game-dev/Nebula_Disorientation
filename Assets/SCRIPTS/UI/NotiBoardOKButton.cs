using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotiBoardOKButton : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject NotiBoard;
    #endregion
    #region NormalVariables
    public bool MoveToUEC;
    private float currentTimer;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        currentTimer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        currentTimer += Time.deltaTime;
        RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        bool check = false;
        foreach (var hit in hits)
        {
            if (hit.collider!=null)
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
        } else
        {
            NotiBoard.GetComponent<NotificationBoard>().DisableCollider *= 1;
        }
    }
    #endregion
    #region Mouse Check
    private void OnMouseDown()
    {
        Debug.Log("A");
        if ("OKButton".Equals(name) || "CancelButton".Equals(name))
        {
            Destroy(NotiBoard);
        } else if ("CertainButton".Equals(name))
        {
            FindObjectOfType<NotificationBoardController>().ConfirmOnConfirmationBoard();
            Destroy(NotiBoard);
        } else if ("ConvertButton".Equals(name))
        {
            ConvertBoard cb = NotiBoard.transform.GetChild(0).GetComponent<ConvertBoard>();
            if (cb!=null)
            {
                cb.Convert();
            }
        } else if ("BackButton".Equals(name))
        {
            Destroy(NotiBoard);
        }
    }

    private void OnDestroy()
    {
        if ("OKButton".Equals(name) && MoveToUEC)
        {
            FindObjectOfType<MainMenuCameraController>().MoveToUEC();
        }   
    }
    #endregion
}
