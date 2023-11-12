using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

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
    public int MissionUndone;
    private string DMDone;
    private TimeSpan ResetTime;
    public GameObject DMInfo;
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
        if (missions.Count > 0)
        {
            SetDailyMission();
        }
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
        ResetTime = System.DateTime.Now.AddDays(1).Date + new TimeSpan(0, 0, 0) - System.DateTime.Now;
        Debug.Log(string.Format("{0:D2}:{1:D2}:{2:D2}", ResetTime.Hours, ResetTime.Minutes, ResetTime.Seconds));
        InfoText = "Reward for each mission <br><br> 2500 <sprite=3>  5 <sprite=0>  100 <sprite=1> <br><br> Total Reset in " + string.Format("{0:D2}:{1:D2}:{2:D2}", ResetTime.Hours, ResetTime.Minutes, ResetTime.Seconds) + "";
        DMInfo.transform.GetChild(1).GetComponent<TextMeshPro>().text = InfoText;
    }
    #endregion
    #region Check Daily Mission
    public void SetDailyMission()
    {
        DM1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = missions[0];
        DM2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = missions[1];
        DM3.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = missions[2];
        DM4.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = missions[3];
        if (missions.Count - MissionUndone == 4)
        {
            DMText.GetComponent<TextMeshPro>().text = "<color=#008000>Completed!</color>";
        } else
        {
            DMText.GetComponent<TextMeshPro>().text = "Daily missions <color=red>("+(missions.Count - MissionUndone)+"/4)</color>";
        }
        BGToPos = DMBG3MissionsPos.transform.position;       
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
