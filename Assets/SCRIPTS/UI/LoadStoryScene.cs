using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadStoryScene : MainMenuSceneShared
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject Board;
    public GameObject BoardContent;
    public GameObject ItemBoxTop;
    public GameObject ItemBoxBottom;
    public GameObject ItemBoxInitPos;
    public ScrollRect ScrollRect;
    public GameObject TopArrow;
    public GameObject BottomArrow;
    #endregion
    #region NormalVariables
    public GameObject CurrentPressPilot;
    private float distanceToTop;
    private float distanceToBottom;
    private float maximumHeight;
    private Vector2 currentClonePos;
    private List<GameObject> currentPilotList;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        distanceToTop = ItemBoxTop.transform.position.y - ItemBoxInitPos.transform.position.y;
        distanceToBottom = ItemBoxInitPos.transform.position.y - ItemBoxBottom.transform.position.y;
        currentPilotList = new List<GameObject>();
        CurrentPressPilot = null;
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        CheckShowArrows();
        CheckPilotList();
    }
    #endregion
    #region Anim
    public override void StartAnimation()
    {
        GetData();
    }
    #endregion
    #region Get Data
    public void GetData()
    {
        // Delete All Data
        DeleteCurrentList();
        // Get Data from DB
        List<List<string>> NameAndRank = FindObjectOfType<AccessDatabase>().GetAllNameAndRankFromPlayerProfile();
        maximumHeight = (Mathf.Abs(distanceToTop) + Mathf.Abs(distanceToBottom)) * NameAndRank[0].Count;
        ScrollRect.vertical = false;
        BoardContent.GetComponent<RectTransform>().sizeDelta
            = new Vector2(BoardContent.GetComponent<RectTransform>().sizeDelta.x,
            maximumHeight);
        StartCoroutine(GeneratePilotContract(NameAndRank[0].Count, NameAndRank));
    }

    private IEnumerator GeneratePilotContract(int count, List<List<string>> NameAndRank)
    {
        currentClonePos = ItemBoxInitPos.transform.position;
        for (int i = 0; i < count; i++)
        {
            GameObject boardClone = Instantiate(ItemBoxInitPos, currentClonePos, Quaternion.identity);
            boardClone.GetComponent<PilotNameBar>().SetText(i + 1, NameAndRank[0][i], NameAndRank[1][i]);
            boardClone.GetComponent<PilotNameBar>().Scene = gameObject;
            boardClone.transform.SetParent(BoardContent.transform);
            currentPilotList.Add(boardClone);
            boardClone.SetActive(true);
            currentClonePos.y -= (Mathf.Abs(distanceToTop) + Mathf.Abs(distanceToBottom));
            yield return new WaitForSeconds(0.1f);
        }
        Vector3 pos = BoardContent.transform.position;
        pos.y = 0;
        BoardContent.transform.position = pos;
        ScrollRect.vertical = true;
    }

    private void DeleteCurrentList()
    {
        int i = 0;
        while (i < currentPilotList.Count)
        {
            GameObject temp = currentPilotList[i];
            if (temp != null)
            {
                currentPilotList.RemoveAt(i);
                Destroy(temp);
            }
            else
            {
                i++;
            }
        }
    }

    void CheckShowArrows()
    {
        float posY = BoardContent.transform.localPosition.y;
        if (posY > (Mathf.Abs(distanceToTop) + Mathf.Abs(distanceToBottom)))
        {
            if (!TopArrow.activeSelf)
            {
                TopArrow.SetActive(true);
            }
        }
        else
        {
            if (TopArrow.activeSelf)
            {
                TopArrow.SetActive(false);
            }
        }
        if (posY < (maximumHeight - (Mathf.Abs(distanceToTop) + Mathf.Abs(distanceToBottom)) * 4))
        {
            if (!BottomArrow.activeSelf)
            {
                BottomArrow.SetActive(true);
            }
        }
        else
        {
            if (BottomArrow.activeSelf)
            {
                BottomArrow.SetActive(false);
            }
        }
    }
    #endregion
    #region Check Pilot List
    void CheckPilotList()
    {
        if (CurrentPressPilot != null)
        {
            Color c = CurrentPressPilot.GetComponent<Image>().color;
            c.r = 0;
            c.b = 0;
            CurrentPressPilot.GetComponent<Image>().color = c;
            foreach (var go in currentPilotList)
            {
                if (go.GetComponent<Image>().color != Color.white && go != CurrentPressPilot)
                {
                    go.GetComponent<Image>().color = Color.white;
                }
            }
        }
        else
        {
            foreach (var go in currentPilotList)
            {
                if (go.GetComponent<Image>().color != Color.white)
                {
                    go.GetComponent<Image>().color = Color.white;
                }
            }
        }
    }
    #endregion
    #region Button Activate
    public void CheckBeforeDelete()
    {
        if (CurrentPressPilot != null)
        {
            FindObjectOfType<NotificationBoardController>().VoidReturnFunction = DeleteSelected;
            PilotNameBar pn = CurrentPressPilot.GetComponent<PilotNameBar>();
            string name = "";
            string rank = "";
            if (pn != null)
            {
                name = pn.PilotName;
                rank = pn.PilotRank;
            }
            string confirmText = "Do you want to delete " + rank + " pilot - " + name + "?";
            FindObjectOfType<NotificationBoardController>().CreateNormalConfirmBoard(transform.position,
                confirmText);
        }
        else
        {
            FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(transform.position,
                "Please Select a pilot contract to perform actions!", 5f);
        }
    }
    public void DeleteSelected()
    {
        PilotNameBar pn = CurrentPressPilot.GetComponent<PilotNameBar>();
        string check = "";
        if (pn != null)
        {
            check = FindObjectOfType<AccessDatabase>().DeletePlayerProfileByName(pn.PilotName);
        }
        if ("Success".Equals(check))
        {
            FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(transform.position,
            "Delete pilot contract successfully!", 5f);
            CurrentPressPilot = null;
        }
        else if ("Fail".Equals(check))
        {
            FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(transform.position,
            "Delete pilot contract fail. Please try again!", 5f);
        }
        else if ("No Exist".Equals(check))
        {
            FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(transform.position,
            "Pilot contract does not exist. Please try again!", 5f);
        }
        GetData();
    }

    public void CheckOnward()
    {
        if (CurrentPressPilot != null)
        {
            FindObjectOfType<NotificationBoardController>().VoidReturnFunction = OnwardSeleted;
            PilotNameBar pn = CurrentPressPilot.GetComponent<PilotNameBar>();
            string name = "";
            string rank = "";
            if (pn != null)
            {
                name = pn.PilotName;
                rank = pn.PilotRank;
            }
            string confirmText = "Continue " + rank + " - " + name + "'s contract onward?";
            FindObjectOfType<NotificationBoardController>().CreateNormalConfirmBoard(transform.position,
                confirmText);
        }
        else
        {
            FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(transform.position,
                "Please Select a pilot contract to perform actions!", 5f);
        }
    }
    public void OnwardSeleted()
    {
        PilotNameBar pn = CurrentPressPilot.GetComponent<PilotNameBar>();
        if (pn!=null)
        {
            FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(transform.position,
                        "Welcome back to Nebula Disorientation, " + pn.PilotName + "!\n(Auto close in 5 seconds)", 5f);
            CurrentPressPilot = null;
            FindObjectOfType<MainMenuCameraController>().MoveToUEC(8f);
        }
    }
    #endregion
}