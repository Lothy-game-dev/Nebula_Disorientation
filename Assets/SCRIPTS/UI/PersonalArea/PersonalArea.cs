using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PersonalArea : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    public GameObject Template;
    public GameObject Content;
    public GameObject RankDesc;
    public GameObject RankSalary;
    public GameObject CurrentSalary;
    public GameObject PlayerName;
    public GameObject PlayerRank;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    public List<List<string>> RankList;
    public Dictionary<string, object> PlayerInformation;
    public int PlayerId;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Generate new list
    // Group all function that serve the same algorithm
    public void GetData()
    {
        RankList = FindAnyObjectByType<AccessDatabase>().GetAllRank();
        PlayerId = FindAnyObjectByType<AccessDatabase>().GetCurrentSessionPlayerId();
        PlayerInformation = FindAnyObjectByType<AccessDatabase>().GetPlayerInformationById(PlayerId);
        PlayerName.GetComponent<TextMeshPro>().text = (string)PlayerInformation["Name"];
        PlayerRank.GetComponent<TextMeshPro>().text = "<color="+(string)PlayerInformation["RankColor"]+">" + (string)PlayerInformation["Rank"] + "</color>";
        for (int i = 0; i < RankList.Count; i++)
        {
            GameObject game = Instantiate(Template, Template.transform.position, Quaternion.identity);
            game.name = RankList[i][1];
            game.transform.GetChild(0).GetComponent<TMP_Text>().text = "<color=" + RankList[i][7] + ">" + RankList[i][1] + "</color>";
            game.GetComponent<PersonalAreaItem>().ItemId = RankList[i][0];
            game.transform.SetParent(Content.transform);
            game.SetActive(true);
            if (int.Parse(RankList[i][0]) == (int)PlayerInformation["RankId"])
            {
                ShowRankInfo(RankList[i][0]);
            }
            LockItem(game, RankList[i][0]);
        }
        DateTime currentTime = System.DateTime.Now;
        DateTime time = currentTime.Date + new TimeSpan(10, 30, 0);
        Debug.Log("Current time: " + (currentTime - time));
    }
    #endregion
    #region Show rank's information (Daily income, condition to rank up,...)
    // Group all function that serve the same algorithm
    public void ShowRankInfo(string Id)
    {
        CurrentItem(Id);
        string RankCondition = FindAnyObjectByType<GlobalFunctionController>().ConvertRankUpConditions(RankList[int.Parse(Id) - 1][2], RankList[int.Parse(Id) - 1][3], RankList[int.Parse(Id) - 1][4]);
        RankDesc.GetComponent<TMP_Text>().text =  RankCondition;
        RankSalary.GetComponent<TMP_Text>().text = RankList[int.Parse(Id) - 1][5] + " <sprite index='3'>";
        CurrentSalary.GetComponent<TMP_Text>().text = PlayerInformation["DailyIncome"] + " <sprite index='3'>";
    }
    #endregion
    #region Check current item
    public void CurrentItem(string Id)
    {
        for (int i = 1; i < Content.transform.childCount; i++)
        {
            Content.transform.GetChild(i).GetComponent<Image>().color = Color.white;
        }
        Content.transform.GetChild(int.Parse(Id)).GetComponent<Image>().color = Color.green;
    }
    #endregion
    #region Lock item
    public void LockItem(GameObject game, string Id)
    {
        if (int.Parse(PlayerInformation["RankId"].ToString()) < int.Parse(RankList[int.Parse(Id) - 1][0]))
        {
            game.transform.GetChild(1).gameObject.SetActive(true);
        }
    }
    #endregion
}
