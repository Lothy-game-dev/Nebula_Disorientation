using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using UnityEngine.UI;
using TMPro;

public class FuelCellBar : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    public GameObject FuelCellInfo;
    private AccessDatabase ad;
    private string LastTimeFuel;
    private Dictionary<string, object> Data;
    private float timer;
    private string RegenFuelTime;
    private int PlayerID;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        ad = FindObjectOfType<AccessDatabase>();
        if (PlayerPrefs.GetInt("PlayerID")==0)
        {
            PlayerID = FindObjectOfType<UECMainMenuController>().PlayerId;
        } else
        {
            PlayerID = PlayerPrefs.GetInt("PlayerID");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        if (Time.timeScale == 0)
        {
            timer -= 1 / 30f;
        }
        else
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            timer = 1f;
            UpdateCheckFuel();
        }
    }
    #endregion
    #region UpdateCheckFuel
    // Group all function that serve the same algorithm
    private void UpdateCheckFuel()
    {
        if (PlayerID != 0)
        {
            Data = ad.GetPlayerInformationById(PlayerID);
            LastTimeFuel = (string)Data["LastFuelCellUsed"];
            if (LastTimeFuel == null || LastTimeFuel.Length == 0)
            {
                RegenFuelTime = "";
            }
            else
            {
                CultureInfo culture = CultureInfo.InvariantCulture;
                System.DateTime LastTime = System.DateTime.ParseExact(LastTimeFuel, "dd/MM/yyyy HH:mm:ss", culture);
                System.DateTime date = System.DateTime.Now;
                double Result = (date - LastTime).TotalSeconds;
                if (Result <= 0)
                {
                    RegenFuelTime = "";
                }
                else
                {
                    int minutes = 15;
                    while (Result > minutes*60)
                    {
                        Result = Result - minutes * 60;
                        // Add Fuel cell
                        ad.AddFuelCell(PlayerID);
                        Data = ad.GetPlayerInformationById(PlayerID);
                        if ((int)Data["FuelCell"] == 10)
                        {
                            break;
                        }
                    }
                    Result = (int)(minutes * 60 - Result);
                    if (Result == 0)
                    {
                        // Add Fuel cell
                        ad.AddFuelCell(PlayerID);
                    }
                    Data = ad.GetPlayerInformationById(PlayerID);
                    transform.GetChild(0).GetChild(0).GetComponent<Slider>().value = (int)Data["FuelCell"];
                    if ((int)Data["FuelCell"] == 10)
                    {
                        if (transform.childCount > 2)
                        transform.GetChild(2).gameObject.SetActive(false);
                    } else
                    {
                        if (transform.childCount > 2)
                            transform.GetChild(2).gameObject.SetActive(true);
                    }
                    int hour = (int)Result / 3600;
                    int minute = (int)(Result - hour*3600) / 60;
                    int second = (int)(Result - hour * 3600 - minute * 60);
                    RegenFuelTime = "Restore 1 in<br>" + "0" + hour + ":" + minute + ":" + second;
                }
            }
            if (FuelCellInfo != null)
            {
                FuelCellInfo.transform.GetChild(1).GetComponent<TextMeshPro>().text =
                    (RegenFuelTime != "" ?
                     "Fuel Core. Players need Fuel Core to traverse their journey back and forth.<br>" + RegenFuelTime
                     : "Fuel Core. Players need Fuel Core to traverse their journey back and forth.");
            }
        }
    }
    #endregion
    #region MouseCheck
    private void OnMouseEnter()
    {
        FindObjectOfType<NotificationBoardController>().CreateNormalInformationBoard(gameObject, "Fuel Core. Players need Fuel Core to traverse their journey back and forth.<br>" + RegenFuelTime);
    }

    private void OnMouseExit()
    {
        FindObjectOfType<NotificationBoardController>().DestroyCurrentInfoBoard();
    }
    #endregion
}
