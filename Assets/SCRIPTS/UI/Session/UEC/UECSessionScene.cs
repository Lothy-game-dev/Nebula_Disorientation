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
    public GameObject ConsumableBox;
    public GameObject ModelBox;
    public GameObject WeaponList;
    public GameObject PowerList;
    public GameObject ConsumableList;
    public GameObject ModelList;
    public GameObject Cash;
    public GameObject TimelessShard;
    public GameObject FuelCell;
    public GameObject CurrentHPSlider;
    public GameObject CurrentHPText;
    public UECSessionShopIcon[] Icons;
    public GameSavedText SaveText;
    #endregion
    #region NormalVariables
    private Dictionary<string, object> SessionData;
    public string ChosenFirstWeapon;
    public string ChosenSecondWeapon;
    public string ChosenFirstPower;
    public string ChosenSecondPower;
    public string ChosenConsumable;
    private GameObject ModelGO;
    private GameObject FirstWeaponGO;
    private GameObject SecondWeaponGO;
    private GameObject FirstPowerGO;
    private GameObject SecondPowerGO;
    private List<GameObject> Consumables;
    private float currentRotateAngle;
    public bool StopMovingIcon;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void OnEnable()
    {
        // Initialize variables
        GenerateData();
        SaveText.gameObject.SetActive(true);
        SaveText.FadingCountDown = 5f;
        SaveText.AlreadySetCountDown = true;
        StopMovingIcon = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        if (ModelGO!=null)
        {
            ModelGO.transform.Rotate(new Vector3(0, 0, -1));
            currentRotateAngle -= 1;
        }
        if (StopMovingIcon)
        {
            foreach (var icon in Icons)
            {
                if (icon.GetComponent<Rigidbody2D>().velocity != new Vector2(0, 0))
                {
                    icon.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                }
            }
        } else
        {
            foreach (var icon in Icons)
            {
                if (icon.alreadySetVeloc)
                {
                    icon.alreadySetVeloc = false;
                }
            }
        }
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
        FuelCell.transform.GetChild(0).GetChild(0).GetComponent<Slider>().value = (int)SessionData["SessionFuelCore"];
        Consumables = new();
        List<string> ListWeapon = FindObjectOfType<AccessDatabase>().GetSessionOwnedWeaponExceptForName(PlayerPrefs.GetInt("PlayerID"), "");
        int inter = 0;
        // First Weapon
        FirstWeaponGO = new();
        ChosenFirstWeapon = (string)SessionData["LeftWeapon"];
        Debug.Log(ChosenFirstWeapon);
        int n = FindObjectOfType<AccessDatabase>().GetSessionCurrentOwnershipWeaponPowerModelByName(PlayerPrefs.GetInt("PlayerID"), ChosenFirstWeapon, "Weapon");
        if (n==0)
        {
            ChosenFirstWeapon = ListWeapon[inter];
            Debug.Log(ChosenFirstWeapon);
            inter++;
            FindObjectOfType<AccessDatabase>().UpdateSessionInfo(PlayerPrefs.GetInt("PlayerID"), "LeftWeapon", ChosenFirstWeapon.Replace(" ",""));
        }
        // Second Weapon
        SecondWeaponGO = new();
        ChosenSecondWeapon = (string)SessionData["RightWeapon"];
        int m = FindObjectOfType<AccessDatabase>().GetSessionCurrentOwnershipWeaponPowerModelByName(PlayerPrefs.GetInt("PlayerID"), ChosenSecondWeapon, "Weapon");
        if (m == 0)
        {
            ChosenSecondWeapon = ListWeapon[inter];
            FindObjectOfType<AccessDatabase>().UpdateSessionInfo(PlayerPrefs.GetInt("PlayerID"), "RightWeapon", ChosenSecondWeapon.Replace(" ", ""));
        } else if (m==1)
        {
            if (ChosenSecondWeapon == ChosenFirstWeapon)
            {
                ChosenSecondWeapon = ListWeapon[inter];
                FindObjectOfType<AccessDatabase>().UpdateSessionInfo(PlayerPrefs.GetInt("PlayerID"), "RightWeapon", ChosenSecondWeapon.Replace(" ", ""));
            }
        }
        Debug.Log(ChosenSecondWeapon);
        // First Weapon Gen
        for (int i=0; i< WeaponList.transform.childCount;i++)
        {
            if (WeaponList.transform.GetChild(i).name.Replace(" ", "").Replace("-", "").ToLower().Equals(ChosenFirstWeapon.Replace(" ", "").Replace("-", "").ToLower()))
            {
                FirstWeaponGO = Instantiate(WeaponList.transform.GetChild(i).gameObject, FirstWeapon.transform.position, Quaternion.identity);
                FirstWeaponGO.transform.SetParent(FirstWeapon.transform);
                FirstWeaponGO.transform.localScale = new Vector3(2, 2, 2);
                Destroy(FirstWeaponGO.transform.GetChild(0).GetComponent<BoxCollider2D>());
                FirstWeaponGO.transform.GetChild(0).GetComponent<LoadOutBox>().enabled = false;
                FirstWeaponGO.SetActive(true);
                FirstWeapon.transform.GetChild(1).GetComponent<UECSessionWeaponBox>().ChosenWeapon = ChosenFirstWeapon;
                FirstWeapon.transform.GetChild(1).GetComponent<UECSessionWeaponBox>().OtherChosen = ChosenSecondWeapon;
                FirstWeapon.transform.GetChild(0).GetComponent<UECSessionStatusBoard>().GenerateData("Weapon", ChosenFirstWeapon);
                break;
            }
        }
        // Second Weapon Gen
        for (int i = 0; i < WeaponList.transform.childCount; i++)
        {
            if (WeaponList.transform.GetChild(i).name.Replace(" ", "").Replace("-", "").ToLower().Equals(ChosenSecondWeapon.Replace(" ", "").Replace("-", "").ToLower()))
            {
                SecondWeaponGO = Instantiate(WeaponList.transform.GetChild(i).gameObject, SecondWeapon.transform.position, Quaternion.identity);
                SecondWeaponGO.transform.SetParent(SecondWeapon.transform);
                SecondWeaponGO.transform.localScale = new Vector3(2, 2, 2);
                Destroy(SecondWeaponGO.transform.GetChild(0).GetComponent<BoxCollider2D>());
                SecondWeaponGO.transform.GetChild(0).GetComponent<LoadOutBox>().enabled = false;
                SecondWeaponGO.SetActive(true);
                SecondWeapon.transform.GetChild(1).GetComponent<UECSessionWeaponBox>().ChosenWeapon = ChosenSecondWeapon;
                SecondWeapon.transform.GetChild(1).GetComponent<UECSessionWeaponBox>().OtherChosen = ChosenFirstWeapon;
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
                ModelGO.transform.Rotate(new Vector3(0, 0, currentRotateAngle));
                ModelGO.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                if (FirstWeaponGO != null) 
                {
                    ModelGO.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = FirstWeaponGO.transform.GetChild(0).GetComponent<LoadOutBox>().IngameSprite;
                }
                if (SecondWeaponGO != null)
                {
                    ModelGO.transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite = SecondWeaponGO.transform.GetChild(0).GetComponent<LoadOutBox>().IngameSprite;
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
        ChosenSecondPower = (string)SessionData["SecondPower"];
        for (int i=0; i< PowerList.transform.childCount;i++)
        {
            if (PowerList.transform.GetChild(i).name.Replace(" ", "").Replace("-", "").ToLower().Equals(ChosenFirstPower.Replace(" ", "").Replace("-", "").ToLower()))
            {
                FirstPowerGO = Instantiate(PowerList.transform.GetChild(i).gameObject, FirstPower.transform.position, Quaternion.identity);
                FirstPowerGO.transform.SetParent(FirstPower.transform);
                FirstPowerGO.transform.localScale = new Vector3(3, 3, 3);
                FirstPowerGO.SetActive(true);
                FirstPowerGO.GetComponent<SpriteRenderer>().sortingOrder = 1;
                FirstPower.transform.GetChild(1).GetComponent<UECSessionPowerBox>().ChosenPower = ChosenFirstPower;
                FirstPower.transform.GetChild(1).GetComponent<UECSessionPowerBox>().OtherChosen = ChosenSecondPower;

                FirstPower.transform.GetChild(0).GetComponent<UECSessionStatusBoard>().GenerateData("Power", ChosenFirstPower);
                break;
            }
        }
        // Second Power
        if (SP==2)
        {
            for (int i = 0; i < PowerList.transform.childCount; i++)
            {
                if (PowerList.transform.GetChild(i).name.Replace(" ", "").Replace("-", "").ToLower().Equals(ChosenSecondPower.Replace(" ", "").Replace("-", "").ToLower()))
                {
                    SecondPowerGO = Instantiate(PowerList.transform.GetChild(i).gameObject, SecondPower.transform.position, Quaternion.identity);
                    SecondPowerGO.transform.SetParent(SecondPower.transform);
                    SecondPowerGO.transform.localScale = new Vector3(3, 3, 3);
                    SecondPowerGO.SetActive(true);
                    SecondPowerGO.GetComponent<SpriteRenderer>().sortingOrder = 1;
                    SecondPower.transform.GetChild(1).GetComponent<UECSessionPowerBox>().ChosenPower = ChosenSecondPower;
                    SecondPower.transform.GetChild(1).GetComponent<UECSessionPowerBox>().OtherChosen = ChosenFirstPower;
                    SecondPower.transform.GetChild(0).GetComponent<UECSessionStatusBoard>().GenerateData("Power", ChosenSecondPower);
                    break;
                }
            }
        } else
        {
            SecondPower.GetComponent<SpriteRenderer>().color = new Color(50 / 255f, 50 / 255f, 50 / 255f);
            SecondPower.transform.GetChild(0).gameObject.SetActive(false);
            Destroy(SecondPower.GetComponent<Collider2D>());
        }
        int SC = int.Parse((string)statsDict["SC"]);
        // Consumables
        ChosenConsumable = (string)SessionData["Consumables"];
        FirstConsumable.transform.GetChild(0).gameObject.SetActive(false);
        SecondConsumable.transform.GetChild(0).gameObject.SetActive(false);
        ThirdConsumable.transform.GetChild(0).gameObject.SetActive(false);
        FourthConsumable.transform.GetChild(0).gameObject.SetActive(false);
        if (ChosenConsumable!="")
        {
            string[] ConsAndStack = ChosenConsumable.Split("|");
            int count = 1;
            foreach (var item in ConsAndStack)
            {
                string name = item.Split("-")[0];
                string stack = item.Split("-")[1];
                Dictionary<string, object> ConssData = FindObjectOfType<AccessDatabase>().GetConsumableDataByName(name);
                for (int i = 0; i < ConsumableList.transform.childCount; i++)
                {
                    if (ConsumableList.transform.GetChild(i).name.Replace(" ", "").Replace("-", "").ToLower().Equals(name.Replace(" ", "").Replace("-", "").ToLower()))
                    {
                        GameObject ChosenPlace;
                        if (count == 1)
                        {
                            ChosenPlace = FirstConsumable;
                        }
                        else if (count == 2)
                        {
                            ChosenPlace = SecondConsumable;
                        }
                        else if (count == 3)
                        {
                            ChosenPlace = ThirdConsumable;
                        }
                        else
                        {
                            ChosenPlace = FourthConsumable;
                        }
                        GameObject Consumable = Instantiate(ConsumableList.transform.GetChild(i).gameObject,
                            ChosenPlace.transform.position, Quaternion.identity);
                        Consumable.transform.SetParent(ChosenPlace.transform);
                        Consumable.transform.localScale = new Vector3(3, 3, 3);
                        ChosenPlace.transform.GetChild(0).GetComponent<TextMeshPro>().text = stack + "/" + (int)ConssData["Stack"];
                        ChosenPlace.transform.GetChild(0).gameObject.SetActive(true);
                        Consumable.SetActive(true);
                        Consumable.GetComponent<SpriteRenderer>().sortingOrder = ConsumableBox.activeSelf ? 201 : 1;
                        Consumables.Add(Consumable);
                        count++;
                        break;
                    }
                }
                if (count > SC)
                {
                    break;
                }
            }
        }
        ConsumableBox.GetComponent<UECSessionConsumableBox>().consStr = ChosenConsumable;
        ConsumableBox.GetComponent<UECSessionConsumableBox>().ConsAvaiCount = SC;
        if (SC==2)
        {
            ThirdConsumable.GetComponent<SpriteRenderer>().color = new Color(50 / 255f, 50 / 255f, 50 / 255f);
            ThirdConsumable.transform.GetChild(0).gameObject.SetActive(false);
            FourthConsumable.GetComponent<SpriteRenderer>().color = new Color(50 / 255f, 50 / 255f, 50 / 255f);
            FourthConsumable.transform.GetChild(0).gameObject.SetActive(false);
        } else if (SC==3)
        {
            FourthConsumable.GetComponent<SpriteRenderer>().color = new Color(50 / 255f, 50 / 255f, 50 / 255f);
            FourthConsumable.transform.GetChild(0).gameObject.SetActive(false);
        }
        // Current HP
        string stats2 = FindObjectOfType<AccessDatabase>().GetFighterStatsByName(Model);
        int maxHP = Mathf.CeilToInt(float.Parse((string)statsDict["HP"]) * CalculateMaxHPScaleLOTW());
        CurrentHPSlider.GetComponent<Slider>().maxValue = maxHP;
        CurrentHPSlider.GetComponent<Slider>().value = (int)SessionData["SessionCurrentHP"];
        CurrentHPText.GetComponent<TextMeshPro>().text = "Current HP (" + Mathf.CeilToInt((int)SessionData["SessionCurrentHP"]/(float)maxHP * 100) + "%)";
    }

    public void ResetEconomyData()
    {
        SessionData = FindObjectOfType<AccessDatabase>().GetSessionInfoByPlayerId(PlayerPrefs.GetInt("PlayerID"));
        // Cash Shard Cell
        Cash.transform.GetChild(1).GetComponent<TextMeshPro>().text = ((int)SessionData["SessionCash"]).ToString();
        TimelessShard.transform.GetChild(1).GetComponent<TextMeshPro>().text = ((int)SessionData["SessionTimelessShard"]).ToString();
        FuelCell.transform.GetChild(0).GetChild(0).GetComponent<Slider>().maxValue = 10;
        FuelCell.transform.GetChild(0).GetChild(0).GetComponent<Slider>().value = (int)SessionData["SessionFuelCore"];
        // Model
        string Model = (string)SessionData["Model"];
        string stats = FindObjectOfType<AccessDatabase>().GetFighterStatsByName(Model);
        Dictionary<string, object> statsDict = FindObjectOfType<GlobalFunctionController>().ConvertModelStatsToDictionary(stats);
        int maxHP = Mathf.CeilToInt(float.Parse((string)statsDict["HP"]) * CalculateMaxHPScaleLOTW());
        CurrentHPSlider.GetComponent<Slider>().maxValue = maxHP;
        CurrentHPSlider.GetComponent<Slider>().value = (int)SessionData["SessionCurrentHP"];
        CurrentHPText.GetComponent<TextMeshPro>().text = "Current HP (" + Mathf.CeilToInt((int)SessionData["SessionCurrentHP"] / (float)maxHP * 100) + "%)";
    }

    public void RemoveData()
    {
        Destroy(ModelGO);
        Destroy(FirstWeaponGO);
        Destroy(SecondWeaponGO);
        Destroy(FirstPowerGO);
        Destroy(SecondPowerGO);
        int n = 0;
        while (n < Consumables.Count)
        {
            if (Consumables[n]!=null)
            {
                GameObject Temp = Consumables[n];
                Consumables.Remove(Temp);
                Destroy(Temp);
            } else
            {
                n++;
            }
        }
    }

    public void RegenerateAllData()
    {
        RemoveData();
        GenerateData();
    }

    public float CalculateMaxHPScaleLOTW()
    {
        float LOTWMaxHPScale = 1;
        List<Dictionary<string, object>> ListLOTWOwned = FindObjectOfType<AccessDatabase>().GetLOTWInfoOwnedByID(PlayerPrefs.GetInt("PlayerID"));
        foreach (var LOTWData in ListLOTWOwned)
        {
            if ((int)LOTWData["Duration"] > 0)
            {
                string str = (string)LOTWData["Effect"];
                int stack = (int)LOTWData["Stack"];
                if (str.Contains("HP-"))
                {
                    LOTWMaxHPScale += float.Parse(str.Replace("HP-", "")) * stack / 100f;
                }
            }
        }
        return LOTWMaxHPScale;
    }
    #endregion
}
