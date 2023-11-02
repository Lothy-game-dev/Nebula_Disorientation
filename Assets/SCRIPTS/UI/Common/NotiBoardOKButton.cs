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
        // Check if there are any collider on mouse
        // If there is this collider, disable the background
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
        FindObjectOfType<SoundSFXGeneratorController>().GenerateSound("ButtonClick");
        // Cases
        if ("OKButton".Equals(name) || "CancelButton".Equals(name))
        {
            Destroy(NotiBoard);
        } else if ("CertainButton".Equals(name))
        {
            FindObjectOfType<NotificationBoardController>().ConfirmOnConfirmationBoard();
            Destroy(NotiBoard);
        } else if ("ConvertButton".Equals(name))
        {
            if (NotiBoard.transform.GetChild(0).GetComponent<ConvertBoard>()==null)
            {
                ConvertSessionBoard csb = NotiBoard.transform.GetChild(0).GetComponent<ConvertSessionBoard>();
                if (csb!=null)
                {
                    csb.Convert();
                }
            } else
            {
                ConvertBoard cb = NotiBoard.transform.GetChild(0).GetComponent<ConvertBoard>();
                if (cb != null)
                {
                    cb.Convert();
                }
            }
        } else if ("BackButton".Equals(name))
        {
            Destroy(NotiBoard);
        } else if ("ConfirmButton".Equals(name))
        {
            RechargeBoard rb = NotiBoard.transform.GetChild(0).GetComponent<RechargeBoard>();
            if (rb!=null)
            {
                rb.Recharge();
            }
        } else if ("ReturnButton".Equals(name))
        {
            Destroy(NotiBoard);
        } else
        {
            if ("RenameButton".Equals(name))
            {
                PersonalAreaRename rn = NotiBoard.transform.GetChild(0).GetComponent<PersonalAreaRename>();
                if (rn != null)
                {
                    rn.RenameAction();
                }
            }
        }
    }

    private void OnDestroy()
    {
        if ("OKButton".Equals(name) && MoveToUEC)
        {
            int n = FindObjectOfType<AccessDatabase>().GetCurrentSessionPlayerId();
            Dictionary<string, object> Data = FindObjectOfType<AccessDatabase>().GetPlayerInformationById(n);
            if ((int)Data["CurrentSession"]==-1)
            {
                FindObjectOfType<MainMenuCameraController>().MoveToUEC();
            } else
            {
                string place = FindObjectOfType<AccessDatabase>().GetCurrentPlaceOfSession(n);
                if (place.ToLower().Equals("uec"))
                {
                    FindObjectOfType<MainMenuCameraController>().MoveToUECSession(n);
                } 
                else if (place.ToLower().Equals("lotw"))
                {
                    FindObjectOfType<MainMenuCameraController>().MoveToLOTWScene(n);
                }
                else if (place.ToLower().Equals("gameplay"))
                {
                    FindObjectOfType<MainMenuCameraController>().MoveToGameplay(n);
                }
                else
                {
                    FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Camera.main.transform.position,
                        "Cannot fetch data for this player. Please contact to our email!", 3f);
                }
            }
        }
    }
    #endregion
}
