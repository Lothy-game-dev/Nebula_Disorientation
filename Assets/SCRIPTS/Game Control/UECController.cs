using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UECController : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject LeftBorder;
    public GameObject RightBorder;
    public GameObject TopBorder;
    public GameObject BottomBorder;
    public GameObject Background;
    public GameObject mainCamera;
    public GameObject Cash;
    public GameObject TimelessShard;
    public GameObject FuelCell;
    public GameObject FuelEnergy;
    public GameObject NameRankBox;
    public GameObject DailyMissionBar;
    public GameObject EnterBattleButton;
    #endregion
    #region NormalVariables
    public bool isPlanetMoving;
    private AccessDatabase ad;
    private int currentId;
    private float BGMovingSpeed;
    private Vector2 BGveloc;
    private float HalfCamHeight;
    private float HalfCamWidth;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    private void OnEnable()
    {
        FindObjectOfType<MainMenuCameraController>().GenerateLoadingScene(1f);
    }
    void Start()
    {
        // Initialize variables
        isPlanetMoving = true;
        ad = FindObjectOfType<AccessDatabase>();
        BGMovingSpeed = 0.25f;
        HalfCamHeight = mainCamera.GetComponent<Camera>().orthographicSize;
        HalfCamWidth = HalfCamHeight * mainCamera.GetComponent<Camera>().aspect;
        BGveloc = new Vector2((Random.Range(0, 2) - 0.5f) * 2 * BGMovingSpeed, (Random.Range(0, 2) - 0.5f) * 2 * BGMovingSpeed);
        Background.GetComponent<Rigidbody2D>().velocity = BGveloc;
        CheckDailyMission();
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        DefineBackgroundVelocity();
        Background.GetComponent<Rigidbody2D>().velocity = BGveloc;
        CheckDailyMission();
    }
    #endregion
    #region Background moving
    private void DefineBackgroundVelocity()
    {
        float x = 0;
        float y = 0;
        if (TopBorder.transform.position.y <= mainCamera.transform.position.y + HalfCamHeight)
        {
            y = Random.Range(0, BGMovingSpeed);
        } else if (BottomBorder.transform.position.y >= mainCamera.transform.position.y - HalfCamHeight)
        {
            y = Random.Range(-BGMovingSpeed, 0);
        }
        if (LeftBorder.transform.position.x >= mainCamera.transform.position.x - HalfCamWidth)
        {
            x = Random.Range(-BGMovingSpeed, 0);
        }
        else if (RightBorder.transform.position.x <= mainCamera.transform.position.x + HalfCamWidth)
        {
            x = Random.Range(0, BGMovingSpeed);
        }
        if (x != 0 && y == 0)
        {
            BGveloc = new Vector2(x, BGveloc.y);
        }
        else if (x == 0 && y != 0)
        {
            BGveloc = new Vector2(BGveloc.x, y);
        }
        else if (x != 0 && y != 0)
        {
            BGveloc = new Vector2(x, y);
        }
    }
    #endregion
    #region Data
    public void SetDataToView(Dictionary<string, object> datas)
    {
        currentId = (int)datas["ID"];
        Cash.transform.GetChild(1).GetComponent<TextMeshPro>().text = ((int)datas["Cash"]).ToString();
        TimelessShard.transform.GetChild(1).GetComponent<TextMeshPro>().text = ((int)datas["TimelessShard"]).ToString();
        FuelEnergy.transform.GetChild(1).GetComponent<TextMeshPro>().text = ((int)datas["FuelEnergy"]).ToString();
        FuelCell.transform.GetChild(0).GetChild(0).GetComponent<Slider>().maxValue = 10;
        FuelCell.transform.GetChild(0).GetChild(0).GetComponent<Slider>().value = (int)datas["FuelCell"];
        NameRankBox.transform.GetChild(0).GetComponent<TextMeshPro>().text = GenerateNameRankText((string)datas["Name"], (string)datas["Rank"], (datas.ContainsKey("RankColor")? (string)datas["RankColor"]:"") );
    }

    private string GenerateNameRankText(string name, string rank, string rankColor)
    {
        System.DateTime currentTime = System.DateTime.Now;
        string text = "";
        string ranking = "";
        if ("unranked".Equals(rank.ToLower()))
        {
            ranking = "Pilot";
        }
        else
        {
            ranking = "<color=" + rankColor + ">" + rank + "</color>";
        }
        string time = currentTime.ToString("HH:mm");
        int hour;
        try
        {
            hour = int.Parse(time.Split(":")[0]);
        } catch
        {
            hour = -1;
        }
        string[] morningQuotes = { "Good morning", "Have a nice day,", "Getting up too early, huh" };
        string[] morningQuotesAfter = { "!", "!", "?" };
        string[] afternoonQuotes = { "Good afternoon", "What an afternoon to be at the UEC, right", "Get some nap," };
        string[] afternoonQuotesAfter = { "!", "?", "!" };
        string[] eveningQuotes = { "Good evening", "Have you got your dinner yet, ", "Time to study some new mechanics," };
        string[] eveningQuotesAfter = { "!", "?", "!" };
        string[] nightQuotes = { "Nighty night,", "Have you done your homework yet,", "Sleep when," };
        string[] nightQuotesAfter = { "!", "?", "?" };
        if (hour>=6 && hour < 11)
        {
            int n = Random.Range(0, 3);
            text += morningQuotes[n] + " " + ranking + " " + name + morningQuotesAfter[n];
        } else if (hour >= 11 && hour < 17)
        {
            int n = Random.Range(0, 3);
            text += afternoonQuotes[n] + " " + ranking + " " + name + afternoonQuotesAfter[n];
        } else if (hour >= 17 && hour < 21)
        {
            int n = Random.Range(0, 3);
            text += eveningQuotes[n] + " " + ranking + " " + name + eveningQuotesAfter[n];
        } else if (hour >= 21 || hour<6)
        {
            int n = Random.Range(0, 3);
            text += nightQuotes[n] + " " + ranking + " " + name + nightQuotesAfter[n];
        }
        return text;
    }

    private void CheckDailyMission()
    {
        int n = ad.NumberOfDailyMissionById(currentId);
        bool checkDM = true;
        if (n<2)
        {
            string check = ad.GenerateDailyMission(currentId, 2 - n);
            if ("Fail".Equals(check))
            {
                checkDM = false;
                FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(transform.position,
                    "Failed to fetch daily missions' datas.\n Please contact to our email for more help.", 5f);
            }
        }
        if (checkDM)
        {
            List<List<string>> listDM = ad.GetListDailyMissionUndone(currentId);
            List<string> Missions = new List<string>();
            for (int i = 0; i < listDM[0].Count; i++)
            {
                string mission = "";
                switch (listDM[0][i])
                {
                    case "KE":
                        mission = "Kill " + listDM[1][i] + " enemy(s).";
                        break;
                    case "KB":
                        mission = "Kill " + listDM[1][i] + " boss enemy(s).";
                        break;
                    case "C":
                        mission = "Complete " + listDM[1][i] + " spacezone(s).";
                        break;
                    case "S":
                        mission = "Spend " + listDM[1][i] + " cash(s) during a session.";
                        break;
                    case "P":
                        mission = "Play for at least " + listDM[1][i] + " minute(s).";
                        break;
                    case "CD":
                        mission = "Complete " + listDM[1][i] + " Defend/Escort Spacezone(s).";
                        break;
                    case "CA":
                        mission = "Complete " + listDM[1][i] + " Assault Spacezone(s).";
                        break;
                    case "CAA":
                        mission = "Complete " + listDM[1][i] + " Assault Advanced Spacezone(s).";
                        break;
                    case "B":
                        mission = "Purchase " + listDM[1][i] + " item(s).";
                        break;
                    default: break;
                }
                mission += "\n(0/" + listDM[1][i] + ")";
                Missions.Add(mission);
            }
            DailyMissionBar.GetComponent<UECDailyMissions>().missions = Missions;
        }
    }
    #endregion
}
