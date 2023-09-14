using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ArsenalItem : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    public GameObject Arsenal;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    public string Id;
    public string Type;
    private Arsenal ar;
    private Dictionary<string, object> Status;
    private Dictionary<string, object> RankSys;
    private string PriceColor;
    private string RankColor;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        ar = Arsenal.GetComponent<Arsenal>();
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Function group 1
    // Group all function that serve the same algorithm
    private void OnMouseDown()
    {

        if (Type == "Weapon")
        {
            RankSys = FindAnyObjectByType<AccessDatabase>().GetRankById(int.Parse(ar.WeaponList[int.Parse(Id) - 1][8]));
            Status = FindAnyObjectByType<GlobalFunctionController>().ConvertWeaponStatsToDictionary(ar.WeaponList[int.Parse(Id) - 1][4]);
            for (int i = 0; i < ar.ItemStatus.Count; i++)
            {
                Debug.Log("abcd");
                ar.ItemStatus[i].GetComponent<TextMeshPro>().text = (string)Status[ar.ItemStatus[i].name];
            }
            ar.DescContent.GetComponent<TMP_Text>().text = ar.WeaponList[int.Parse(Id) - 1][3];
            // check if enough timeless shard
            if ((int)ar.PlayerInformation["TimelessShard"] < int.Parse(ar.WeaponList[int.Parse(Id) - 1][6]))
            {
                PriceColor = "red";
                ar.CanBuy = false;
            } else
            {
                if ((int)ar.PlayerInformation["TimelessShard"] == 0)
                {
                    ar.CanBuy = false;
                    PriceColor = "red";
                } else
                {
                    PriceColor = "green";
                    ar.CanBuy = true;
                }               
            }
            // check if enough cash
            if ((int)ar.PlayerInformation["Cash"] < int.Parse(ar.WeaponList[int.Parse(Id) - 1][5]))
            {
                PriceColor = "red";
                ar.CanBuy = false;
            }
            else
            {
                PriceColor = "green";
                ar.CanBuy = true;
            }
            //check rank required
            if ((string)ar.PlayerInformation["Rank"] == (string)RankSys["RankName"])
            {
                RankColor = "green";
                ar.CanBuy = true;
            } else
            {
                RankColor = "red";
                ar.CanBuy = true;
            }
            ar.ItemTimelessShard.GetComponentInChildren<TextMeshPro>().text = "<color=" + PriceColor + ">" + ar.WeaponList[int.Parse(Id) - 1][6] +"</color>" ;
            ar.ItemCash.GetComponentInChildren<TextMeshPro>().text = "<color=" + PriceColor + ">" + ar.WeaponList[int.Parse(Id) - 1][5] + "</color>";
            ar.Rank.GetComponentInChildren<TextMeshPro>().text = "<color=" + RankColor + ">Rank Required</color><br><color="+ (string)RankSys["RankTier"] +">" + (string)RankSys["RankName"] + "</color>" ;
        }
    }
    #endregion
    #region Function group ...
    // Group all function that serve the same algorithm
    #endregion
}
