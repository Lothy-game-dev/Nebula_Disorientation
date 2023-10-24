using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UECSessionScene : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject FirstWeapon;
    public GameObject SecondWeapon;
    public GameObject FirstPower;
    public GameObject SecondPower;
    public GameObject FirstConsumable;
    public GameObject SecondConsumable;
    public GameObject ThirdConsumable;
    public GameObject FourthConsumable;
    public GameObject ModelBox;
    public GameObject WeaponList;
    public GameObject PowerList;
    public GameObject ConsumableList;
    public GameObject ModelList;
    public GameObject Cash;
    public GameObject TimelessShard;
    public GameObject FuelCell;
    #endregion
    #region NormalVariables
    private Dictionary<string, object> SessionData;
    private string ChosenFirstWeapon;
    private string ChosenSecondWeapon;
    private string ChosenFirstPower;
    private string ChosenSecondPower;
    private string ChosenConsumable;
    private GameObject ModelGO;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        GenerateData();
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Generate Data
    // Group all function that serve the same algorithm
    public void GenerateData()
    {
        SessionData = FindObjectOfType<AccessDatabase>().GetSessionInfoByPlayerId(PlayerPrefs.GetInt("PlayerID"));
        // Cash Shard Cell
        Cash.transform.GetChild(1).GetComponent<TextMeshPro>().text = ((int)SessionData["SessionCash"]).ToString();
        TimelessShard.transform.GetChild(1).GetComponent<TextMeshPro>().text = ((int)SessionData["SessionTimelessShard"]).ToString();
        FuelCell.transform.GetChild(0).GetChild(0).GetComponent<Slider>().maxValue = 10;
        Dictionary<string, object> datas = FindObjectOfType<AccessDatabase>().GetPlayerInformationById(PlayerPrefs.GetInt("PlayerID"));
        FuelCell.transform.GetChild(0).GetChild(0).GetComponent<Slider>().value = (int)datas["FuelCell"];
        // First Weapon
        GameObject FirstWeaponGO = new();
        ChosenFirstWeapon = (string)SessionData["LeftWeapon"];
        for (int i=0; i< WeaponList.transform.childCount;i++)
        {
            if (WeaponList.transform.GetChild(i).name.Replace(" ", "").Replace("-", "").ToLower().Equals(ChosenFirstWeapon.Replace(" ", "").Replace("-", "").ToLower()))
            {
                FirstWeaponGO = Instantiate(WeaponList.transform.GetChild(i).gameObject, FirstWeapon.transform.position, Quaternion.identity);
                FirstWeaponGO.transform.SetParent(FirstWeapon.transform);
                FirstWeaponGO.transform.localScale = new Vector3(2, 2, 2);
                FirstWeaponGO.SetActive(true);
                FirstWeapon.transform.GetChild(0).GetComponent<UECSessionStatusBoard>().GenerateData("Weapon", ChosenFirstWeapon);
                break;
            }
        }
        // Second Weapon
        GameObject SecondWeaponGO = new();
        ChosenSecondWeapon = (string)SessionData["RightWeapon"];
        for (int i = 0; i < WeaponList.transform.childCount; i++)
        {
            if (WeaponList.transform.GetChild(i).name.Replace(" ", "").Replace("-", "").ToLower().Equals(ChosenSecondWeapon.Replace(" ", "").Replace("-", "").ToLower()))
            {
                SecondWeaponGO = Instantiate(WeaponList.transform.GetChild(i).gameObject, SecondWeapon.transform.position, Quaternion.identity);
                SecondWeaponGO.transform.SetParent(SecondWeapon.transform);
                SecondWeaponGO.transform.localScale = new Vector3(2, 2, 2);
                SecondWeaponGO.SetActive(true);
                SecondWeapon.transform.GetChild(0).GetComponent<UECSessionStatusBoard>().GenerateData("Weapon", ChosenSecondWeapon);
                break;
            }
        }
        // Model
        string Model = (string)SessionData["Model"];
        for (int i = 0; i < ModelList.transform.childCount; i++)
        {
            if (ModelList.transform.GetChild(i).name.Replace(" ", "").Replace("-", "").ToLower().Equals(Model.Replace(" ", "").Replace("-", "").ToLower()))
            {
                ModelGO = Instantiate(ModelList.transform.GetChild(i).gameObject, new Vector3(ModelBox.transform.position.x,ModelBox.transform.position.y, transform.position.z), Quaternion.identity);
                ModelGO.transform.GetChild(0).gameObject.SetActive(false);
                ModelGO.SetActive(true);
                ModelGO.transform.SetParent(ModelBox.transform.GetChild(0).GetChild(0));
                ModelGO.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                if (FirstWeaponGO != null) 
                {
                    ModelGO.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = FirstWeaponGO.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite;
                }
                if (SecondWeaponGO != null)
                {
                    ModelGO.transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite = SecondWeaponGO.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite;
                }
                ModelGO.transform.GetChild(0).gameObject.SetActive(true);
                break;
            }
        }
        string stats = FindObjectOfType<AccessDatabase>().GetFighterStatsByName(Model);
        Dictionary<string, object> statsDict = FindObjectOfType<GlobalFunctionController>().ConvertModelStatsToDictionary(stats);
        // power slot
        int SP = int.Parse((string)statsDict["SP"]);
        // First Power
        ChosenFirstPower = (string)SessionData["FirstPower"];
        for (int i=0; i< PowerList.transform.childCount;i++)
        {
            if (PowerList.transform.GetChild(i).name.Replace(" ", "").Replace("-", "").ToLower().Equals(ChosenFirstPower.Replace(" ", "").Replace("-", "").ToLower()))
            {
                GameObject FirstPowerGO = Instantiate(PowerList.transform.GetChild(i).gameObject, FirstPower.transform.position, Quaternion.identity);
                FirstPowerGO.transform.SetParent(FirstPower.transform);
                FirstPowerGO.transform.localScale = new Vector3(3, 3, 3);
                FirstPowerGO.SetActive(true);
                FirstPowerGO.GetComponent<SpriteRenderer>().sortingOrder = 1;
                FirstPower.transform.GetChild(0).GetComponent<UECSessionStatusBoard>().GenerateData("Power", ChosenFirstPower);
                break;
            }
        }
        // Second Power
        if (SP==2)
        {
            ChosenSecondPower = (string)SessionData["SecondPower"];
            for (int i = 0; i < PowerList.transform.childCount; i++)
            {
                if (PowerList.transform.GetChild(i).name.Replace(" ", "").Replace("-", "").ToLower().Equals(ChosenSecondPower.Replace(" ", "").Replace("-", "").ToLower()))
                {
                    GameObject SecondPowerGO = Instantiate(PowerList.transform.GetChild(i).gameObject, SecondPower.transform.position, Quaternion.identity);
                    SecondPowerGO.transform.SetParent(SecondPower.transform);
                    SecondPowerGO.transform.localScale = new Vector3(3, 3, 3);
                    SecondPowerGO.SetActive(true);
                    SecondPowerGO.GetComponent<SpriteRenderer>().sortingOrder = 1;
                    SecondPower.transform.GetChild(0).GetComponent<UECSessionStatusBoard>().GenerateData("Power", ChosenSecondPower);
                    break;
                }
            }
        }
        // Consumables
        ChosenConsumable = (string)SessionData["Consumables"];
        string[] ConsAndStack = ChosenConsumable.Split("|");
        int count = 1;
        foreach (var item in ConsAndStack)
        {
            string name = item.Split("-")[0];
            string stack = item.Split("-")[1];
            for (int i = 0; i < ConsumableList.transform.childCount; i++)
            {
                if (ConsumableList.transform.GetChild(i).name.Replace(" ", "").Replace("-", "").ToLower().Equals(name.Replace(" ", "").Replace("-", "").ToLower()))
                {
                    GameObject ChosenPlace;
                    if (count==1)
                    {
                        ChosenPlace = FirstConsumable;
                    } else if (count==2)
                    {
                        ChosenPlace = SecondConsumable;
                    } else if (count==3)
                    {
                        ChosenPlace = ThirdConsumable;
                    } else
                    {
                        ChosenPlace = FourthConsumable;
                    }
                    GameObject Consumable = Instantiate(ConsumableList.transform.GetChild(i).gameObject, 
                        ChosenPlace.transform.position, Quaternion.identity);
                    Consumable.transform.SetParent(ChosenPlace.transform);
                    Consumable.transform.localScale = new Vector3(3, 3, 3);
                    ChosenPlace.transform.GetChild(0).GetComponent<TextMeshPro>().text = stack;
                    ChosenPlace.transform.GetChild(0).gameObject.SetActive(true);
                    Consumable.SetActive(true);
                    Consumable.GetComponent<SpriteRenderer>().sortingOrder = 1;
                    count++;
                    break;
                }
            }
        }
    }
    #endregion
}
