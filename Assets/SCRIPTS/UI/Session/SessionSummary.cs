using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SessionSummary : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    public GameObject GameStatus;
    public GameObject ModelList;
    public GameObject WeaponList;
    public GameObject PowerList;
    public GameObject SpaceZoneNo;
    public GameObject EnemyDestroyed;
    public GameObject DamageDealt;
    public GameObject Shard;
    public GameObject Cash;
    public GameObject FuelEnergy;
    public GameObject Power1Template;
    public GameObject Power2Template;
    public GameObject Weapon1Template;
    public GameObject Weapon2Template;
    public GameObject ModelTemplate;
    public GameObject BlackFade;
    public GameObject ConsInfo;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    private AccessDatabase ad;
    private Dictionary<string, object> Data;
    public bool isFailed;
    public int ShardAmount;
    public int CashAmount;
    public bool ShardCollected;
    public bool CashCollected;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        Camera.main.transparencySortAxis = new Vector3(0, 0, 1);
        ad = FindAnyObjectByType<AccessDatabase>();
        SetData();
        GenerateBlackFadeOpen(transform.position, 3f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Set data
    // Group all function that serve the same algorithm
    public void SetData()
    {
        Data = ad.GetSessionInfoByPlayerId(PlayerPrefs.GetInt("PlayerID"));
        isFailed = (PlayerPrefs.GetString("isFailed") == "T");
        PlayerPrefs.SetString("isFailed", "");

        int SZno = (isFailed ? int.Parse(Data["CurrentStage"].ToString()) - 1 : int.Parse(Data["CurrentStage"].ToString()));
        SpaceZoneNo.transform.GetChild(0).GetComponent<TextMeshPro>().text = SpaceZoneNo.transform.GetChild(0).GetComponent<TextMeshPro>().text.Replace("?", SZno.ToString());
        EnemyDestroyed.transform.GetChild(0).GetComponent<TextMeshPro>().text = "Enemy Destroyed: " + Data["EnemyDestroyed"].ToString();
        DamageDealt.transform.GetChild(0).GetComponent<TextMeshPro>().text = "Damage Dealt: " + Data["DamageDealt"].ToString();
        FuelEnergy.transform.GetChild(0).GetComponent<TextMeshPro>().text = "Fuel Energy: " + Data["SessionFuelEnergy"].ToString();

        //Economy
        ShardAmount = (isFailed ? int.Parse(Data["SessionTimelessShard"].ToString()) / 2 : int.Parse(Data["SessionTimelessShard"].ToString()));
        CashAmount = (isFailed ? int.Parse(Data["SessionCash"].ToString()) / 2 : int.Parse(Data["SessionCash"].ToString()));
        Shard.transform.GetChild(0).GetComponent<TextMeshPro>().text = (isFailed ? ShardAmount + "<color=red> (-" + ShardAmount + ") </color>" : ShardAmount) + " <sprite index='0'> ";
        Cash.transform.GetChild(0).GetComponent<TextMeshPro>().text = (isFailed ? CashAmount + "<color=red> (-" + CashAmount + ") </color>" : CashAmount) + " <sprite index='3'> "; ;      
        if (!isFailed)
        {
            string consString = "";
            Dictionary<string, int> Data = FindObjectOfType<AccessDatabase>().GetSessionOwnedConsumables(PlayerPrefs.GetInt("PlayerID"));
            bool show = false;
            foreach (var item in Data)
            {
                show = true;
                consString += (string)FindObjectOfType<AccessDatabase>().GetConsumableDataByName(item.Key)["Name"] + " x " + item.Value + "\n";
            }
            if (show)
            {
                ConsInfo.GetComponent<SessionSummaryConsBoard>().SetData(consString);
                ConsInfo.SetActive(true);
            }
        }

        GameStatus.transform.GetChild(0).GetComponent<TextMeshPro>().text = (isFailed ? "Mission Failed!" : "Successfully Retreated.");

        //Get model
        for (int i = 0; i < ModelList.transform.childCount; i++)
        {
            if (ModelList.transform.GetChild(i).name == Data["Model"].ToString())
            {
                ModelTemplate.GetComponent<Image>().sprite = ModelList.transform.GetChild(i).GetComponent<Image>().sprite;
            }
        }

        //Get Weapon
        for (int i = 0; i < WeaponList.transform.childCount; i++)
        {
            if (WeaponList.transform.GetChild(i).name.Replace(" ", "") == Data["LeftWeapon"].ToString())
            {
                Weapon1Template.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = WeaponList.transform.GetChild(i).GetChild(0).GetChild(0).GetComponent<Image>().sprite;
                Weapon1Template.transform.GetChild(1).GetComponent<TextMeshPro>().text = WeaponList.transform.GetChild(i).name;
            }
            if (WeaponList.transform.GetChild(i).name.Replace(" ", "") == Data["RightWeapon"].ToString())
            {
                Weapon2Template.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = WeaponList.transform.GetChild(i).GetChild(0).GetChild(0).GetComponent<Image>().sprite;
                Weapon2Template.transform.GetChild(1).GetComponent<TextMeshPro>().text = WeaponList.transform.GetChild(i).name;
            }
        }

        //Get Power
        for (int i = 0; i < PowerList.transform.childCount; i++)
        {
            if (PowerList.transform.GetChild(i).name == Data["FirstPower"].ToString())
            {
                Power1Template.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = PowerList.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite;               
            }
            if (PowerList.transform.GetChild(i).name == Data["SecondPower"].ToString())
            {
                Power2Template.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = PowerList.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite;
            }
        }

        //To check shard/cash can be collected
        ShardCollected = false;
        CashCollected = false;

       
    }
    #endregion
    #region Black Fade
    // Group all function that serve the same algorithm
    public void GenerateBlackFadeOpen(Vector2 pos, float delay, float duration)
    {
        GameObject bf = Instantiate(BlackFade, new Vector3(pos.x, pos.y, BlackFade.transform.position.z), Quaternion.identity);
        Color c = bf.GetComponent<SpriteRenderer>().color;
        c.a = 1;
        bf.GetComponent<SpriteRenderer>().color = c;
        bf.transform.SetParent(transform);
        bf.SetActive(true);
        StartCoroutine(BlackFadeOpen(bf, delay, duration));
    }

    private IEnumerator BlackFadeOpen(GameObject Fade, float delay, float duration)
    {
        yield return new WaitForSeconds(delay);
        //Check rank up
        FindAnyObjectByType<RankController>().CheckToRankUp();
        for (int i = 0; i < 50; i++)
        {
            Color c = Fade.GetComponent<SpriteRenderer>().color;
            c.a -= 1 / 50f;
            Fade.GetComponent<SpriteRenderer>().color = c;
            yield return new WaitForSeconds(duration / 50f);
        }
        Destroy(Fade);
    }
    #endregion
}
