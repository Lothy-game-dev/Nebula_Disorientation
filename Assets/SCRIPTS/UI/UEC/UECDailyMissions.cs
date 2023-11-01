using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UECDailyMissions : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject DMText;
    public GameObject DMBG;
    public GameObject DM1;
    public GameObject DM2;
    public GameObject DM3;
    public GameObject DM4;
    public GameObject DMBGBeforePos;
    public GameObject DMBG1MissionsPos;
    public GameObject DMBG2MissionsPos;
    public GameObject DMBG3MissionsPos;
    public GameObject DMBG4MissionsPos;
    public string InfoText;
    #endregion
    #region NormalVariables
    public List<string> missions;
    private Vector2 BGToPos;
    private bool BGGoingTo;
    private Vector2 DM1Pos;
    private Vector2 DM2Pos;
    private Vector2 DM3Pos;
    private Vector2 DM4Pos;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        List<string> missions = new List<string>();
        DMBG.transform.position = DMBGBeforePos.transform.position;
        BGGoingTo = false;
        DM1Pos = DM1.GetComponent<RectTransform>().anchoredPosition;
        DM2Pos = DM2.GetComponent<RectTransform>().anchoredPosition;
        DM3Pos = DM3.GetComponent<RectTransform>().anchoredPosition;
        DM4Pos = DM4.GetComponent<RectTransform>().anchoredPosition;
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        SetDailyMission();
        if (BGGoingTo)
        {
            if (DMBG.transform.position.y < BGToPos.y)
            {
                DMBG.transform.position = new Vector3(BGToPos.x, BGToPos.y, DMBG.transform.position.z);
            }
        } else
        {
            if (DMBG.transform.position.y > DMBGBeforePos.transform.position.y)
            {
                DMBG.transform.position = new Vector3(DMBGBeforePos.transform.position.x, DMBGBeforePos.transform.position.y, DMBG.transform.position.z);
            }
        }
    }
    #endregion
    #region Check Daily Mission
    public void SetDailyMission()
    {
 
        if (missions.Count == 4)
        {
            if (!DM1.activeSelf)
            {
                DM1.SetActive(true);
            }
            if (!DM2.activeSelf)
            {
                DM2.SetActive(true);
            }
            if (!DM3.activeSelf)
            {
                DM3.SetActive(true);
            }
            if (!DM4.activeSelf)
            {
                DM4.SetActive(true);
            }
            DM1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = missions[0];
            DM2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = missions[1];
            DM3.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = missions[2];
            DM4.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = missions[3];
            DMText.GetComponent<TextMeshPro>().text = "Daily missions <color=\"red\">(0/4)</color>";
            BGToPos = DMBG3MissionsPos.transform.position;
        } else if (missions.Count == 3)
        {
            if (DM1.activeSelf)
            {
                DM1.SetActive(false);
            }
            if (!DM2.activeSelf)
            {
                DM2.SetActive(true);
            }
            if (!DM3.activeSelf)
            {
                DM3.SetActive(true);
            }
            if (!DM4.activeSelf)
            {
                DM4.SetActive(true);
            }
            DM1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
            DM2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = missions[0];
            DM3.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = missions[1];
            DM4.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = missions[2];
            DMText.GetComponent<TextMeshPro>().text = "Daily missions <color=\"yellow\">(1/4)</color>";
            DM2.GetComponent<RectTransform>().anchoredPosition = DM1Pos;
            DM3.GetComponent<RectTransform>().anchoredPosition = DM2Pos;
            DM4.GetComponent<RectTransform>().anchoredPosition = DM3Pos;
            BGToPos = DMBG3MissionsPos.transform.position;
        } else if (missions.Count == 2)
        {
            if (DM1.activeSelf)
            {
                DM1.SetActive(false);
            }
            if (DM2.activeSelf)
            {
                DM2.SetActive(false);
            }
            if (!DM3.activeSelf)
            {               
                DM3.SetActive(true);             
            }
            if (!DM4.activeSelf)
            {
                DM4.SetActive(true);               
            }
            DM1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
            DM2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
            DM3.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = missions[0];
            DM4.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = missions[1];
            DMText.GetComponent<TextMeshPro>().text = "Daily missions <color=\"yellow\">(2/4)</color>";
            BGToPos = DMBG3MissionsPos.transform.position;
            DM3.GetComponent<RectTransform>().anchoredPosition = DM1Pos;
            DM4.GetComponent<RectTransform>().anchoredPosition = DM2Pos;
        } else
        {
            if (missions.Count == 1)
            {
                if (DM1.activeSelf)
                {
                    DM1.SetActive(false);
                }
                if (DM2.activeSelf)
                {
                    DM2.SetActive(false);
                }
                if (DM3.activeSelf)
                {
                    DM3.SetActive(false);
                }
                if (!DM4.activeSelf)
                {
                    DM4.SetActive(true);
                }
                DM1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
                DM2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
                DM3.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
                DM4.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = missions[0];
                DMText.GetComponent<TextMeshPro>().text = "Daily missions <color=\"yellow\">(3/4)</color>";
                BGToPos = DMBG3MissionsPos.transform.position;
                DM4.GetComponent<RectTransform>().anchoredPosition = DM1Pos;
            } else
            {
                if (missions.Count == 0)
                {
                    if (DM1.activeSelf)
                    {
                        DM1.SetActive(false);
                    }
                    if (DM2.activeSelf)
                    {
                        DM2.SetActive(false);
                    }
                    if (DM3.activeSelf)
                    {
                        DM3.SetActive(false);
                    }
                    if (DM4.activeSelf)
                    {
                        DM4.SetActive(false);
                    }
                    DM1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
                    DM2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
                    DM3.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
                    DM4.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
                    DMText.GetComponent<TextMeshPro>().text = "<color=\"green\">completed!</color>";
                    BGToPos = DMBGBeforePos.transform.position;
                }
            }
        }
    }
    #endregion
    #region Mouse Check
    private void OnMouseDown()
    {
        FindObjectOfType<SoundSFXGeneratorController>().GenerateSound("ButtonClick");
        // Animation
        if (!BGGoingTo)
        {
            BGGoingTo = true;
            Vector2 Veloc = (BGToPos - new Vector2(DMBG.transform.position.x, DMBG.transform.position.y));
            DMBG.GetComponent<Rigidbody2D>().velocity = Veloc;
        } else
        {
            BGGoingTo = false;
            Vector2 Veloc = (new Vector2(DMBGBeforePos.transform.position.x, DMBGBeforePos.transform.position.y)
                - new Vector2(DMBG.transform.position.x, DMBG.transform.position.y));
            DMBG.GetComponent<Rigidbody2D>().velocity = Veloc;
        }
        
    }
    private void OnMouseEnter()
    {
        FindObjectOfType<NotificationBoardController>().CreateNormalInformationBoard(gameObject, InfoText);
    }
    private void OnMouseExit()
    {
        FindObjectOfType<NotificationBoardController>().DestroyCurrentInfoBoard();
    }
    #endregion
}
