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
    public GameObject DMBGBeforePos;
    public GameObject DMBG1MissionsPos;
    public GameObject DMBG2MissionsPos;
    public string InfoText;
    #endregion
    #region NormalVariables
    public List<string> missions;
    private Vector2 BGToPos;
    private bool BGGoingTo;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        List<string> missions = new List<string>();
        DMBG.transform.position = DMBGBeforePos.transform.position;
        BGGoingTo = false;
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
        if (missions.Count == 2)
        {
            if (!DM1.activeSelf)
            {
                DM1.SetActive(true);
            }
            if (!DM2.activeSelf)
            {
                DM2.SetActive(true);
            }
            DM1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = missions[0];
            DM2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = missions[1];
            DMText.GetComponent<TextMeshPro>().text = "Daily missions <color=\"red\">(0/2)</color>";
            BGToPos = DMBG2MissionsPos.transform.position;
        } else if (missions.Count == 1)
        {
            if (DM1.activeSelf)
            {
                DM1.SetActive(false);
            }
            if (!DM2.activeSelf)
            {
                DM2.SetActive(true);
            }
            DM1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
            DM2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = missions[0];
            DMText.GetComponent<TextMeshPro>().text = "Daily missions <color=\"yellow\">(1/2)</color>";
            BGToPos = DMBG1MissionsPos.transform.position;
        } else if (missions.Count == 0)
        {
            if (DM1.activeSelf)
            {
                DM1.SetActive(false);
            }
            if (DM2.activeSelf)
            {
                DM2.SetActive(false);
            }
            DM1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
            DM2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
            DMText.GetComponent<TextMeshPro>().text = "<color=\"green\">completed!</color>";
            BGToPos = DMBGBeforePos.transform.position;
        }
    }
    #endregion
    #region Mouse Check
    private void OnMouseDown()
    {
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
