using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SessionArsenalService : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    public GameObject Fighter;
    public GameObject[] Item;
    public GameObject Effect;
    public GameObject Price;
    public GameObject FighterGrade;
    public GameObject CurrentHPSlider;
    public GameObject CurrentHPText;
    public GameObject ModelList;
    public GameObject ModelBox;
    public GameObject PlayerCash;
    public GameObject OrderButton;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    private Dictionary<string, object> SessionData;
    private GameObject ModelGO;
    private float currentRotateAngle;
    public string grade;
    private Dictionary<string, object> ServiceData;
    private Dictionary<string, string> ServicePrice;
    private string Model;
    private int CurrentHPPercent;
    public bool CanBeRepaired;
    public int MaxHP;
    public int CurrentHP;
    public int SessionID;
    public int PriceToRepair;
    private int PCash;
    private bool isEnoughMoney;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void OnEnable()
    {
        // Initialize variables
        GetData();
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        if (ModelGO != null)
        {
            ModelGO.transform.Rotate(new Vector3(0, 0, -1));
            currentRotateAngle -= 1;
        }
    }
    #endregion
    #region Get data 
    // Group all function that serve the same algorithm
    public void GetData()
    {
        SessionData = FindObjectOfType<AccessDatabase>().GetSessionInfoByPlayerId(PlayerPrefs.GetInt("PlayerID"));
        PlayerCash.GetComponent<TextMeshPro>().text = ((int)SessionData["SessionCash"]).ToString();
        PCash = ((int)SessionData["SessionCash"]);
        SessionID = int.Parse(SessionData["SessionID"].ToString());

        // Model
        Model = (string)SessionData["Model"];
        for (int i = 0; i < ModelList.transform.childCount; i++)
        {
            if (ModelList.transform.GetChild(i).name.Replace(" ", "").Replace("-", "").ToLower().Equals(Model.Replace(" ", "").Replace("-", "").ToLower()))
            {
                ModelGO = Instantiate(ModelList.transform.GetChild(i).gameObject, new Vector3(ModelBox.transform.position.x, ModelBox.transform.position.y, transform.position.z), Quaternion.identity);
                ModelGO.transform.GetChild(0).gameObject.SetActive(false);
                ModelGO.SetActive(true);
                ModelGO.transform.SetParent(ModelBox.transform.GetChild(0).GetChild(0));
                ModelGO.transform.Rotate(new Vector3(0, 0, currentRotateAngle));
                ModelGO.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                ModelGO.transform.GetChild(0).gameObject.SetActive(true);
                break;
            }
        }

        // Fighter Tier
        string tier = FindObjectOfType<AccessDatabase>().GetFighterTierByName(Model);
        switch (tier)
        {
            case "#36b37e": FighterGrade.GetComponent<TextMeshPro>().text = "<color=" + tier + ">Grade III</color>"; grade = "Grade3";  break;
            case "#4c9aff": FighterGrade.GetComponent<TextMeshPro>().text = "<color=" + tier + ">Grade II</color>"; grade = "Grade2"; break;
            case "#ff0d11": FighterGrade.GetComponent<TextMeshPro>().text = "<color=" + tier + ">Grade I</color>"; grade = "Grade1"; break;
        }

        
        SetHealth();
        LockItem();

        //Set data for item
        List<Dictionary<string, object>> data = FindObjectOfType<AccessDatabase>().GetAllArsenalService();
        for (int i = 0; i < data.Count; i++)
        {
            Item[i].GetComponent<SessionArsenalServiceItem>().ItemID = (int)data[i]["ID"];
        }
        ShowInformation(1);


    }
    #endregion
    #region Show Service Information
    // Group all function that serve the same algorithm
    public void ShowInformation(int id)
    {
        CurrentItem(id);
        ServiceData = FindObjectOfType<AccessDatabase>().GetArsenalServiceById(id);
        ServicePrice = FindAnyObjectByType<GlobalFunctionController>().ConvertPrice(ServiceData["Price"].ToString());
        Effect.GetComponent<TextMeshPro>().text = "Repair <color=green>" + ServiceData["Effect"].ToString() + "% </color>Max Health";

        string pricecolor = "green";
        // check if enough money
        if (PCash >= int.Parse(ServicePrice[grade]))
        {
            isEnoughMoney = true;
        } else
        {
            pricecolor = "red";
            isEnoughMoney = false;
        }
        Price.GetComponent<TextMeshPro>().text = "<color="+ pricecolor + ">" +ServicePrice[grade] + "</color>";
        PriceToRepair = int.Parse(ServicePrice[grade]);
        //Order button
        OrderButton.transform.GetChild(0).GetComponent<TextMeshPro>().text = "Order " + ServiceData["Name"].ToString();
        OrderButton.GetComponent<SessionArsenalServiceButton>().HealPercent = int.Parse(ServiceData["Effect"].ToString());
        OrderButton.GetComponent<SessionArsenalServiceButton>().CanBeRepaired = Item[id - 1].GetComponent<SessionArsenalServiceItem>().CanBeRepaired;
        OrderButton.GetComponent<SessionArsenalServiceButton>().isEnoughMoney = isEnoughMoney;    
        if (!Item[id - 1].GetComponent<SessionArsenalServiceItem>().CanBeRepaired)
        {
            if (OrderButton.GetComponent<CursorUnallowed>() == null)
            {
                OrderButton.AddComponent<CursorUnallowed>();
            }
        } else
        {
            if (OrderButton.GetComponent<CursorUnallowed>() != null)
            {
                Destroy(OrderButton.GetComponent<CursorUnallowed>());
            }
        }
    }
    #endregion
    #region Show current item
    public void CurrentItem(int id)
    {
        for (int i = 0; i < Item.Length; i++)
        {
            Item[i].GetComponent<SpriteRenderer>().color = Color.white;
            Item[i].transform.GetChild(1).gameObject.SetActive(true);
        }
        Item[id - 1].GetComponent<SpriteRenderer>().color = Color.green;
        Item[id - 1].transform.GetChild(1).gameObject.SetActive(false);
    }
    #endregion
    #region Set Health
    public void SetHealth()
    {
        string stats = FindObjectOfType<AccessDatabase>().GetFighterStatsByName(Model);
        Dictionary<string, object> statsDict = FindObjectOfType<GlobalFunctionController>().ConvertModelStatsToDictionary(stats);
        MaxHP = Mathf.CeilToInt(float.Parse((string)statsDict["HP"]) * FindAnyObjectByType<UECSessionScene>().CalculateMaxHPScaleLOTW());
        CurrentHPSlider.GetComponent<Slider>().maxValue = MaxHP;
        CurrentHPSlider.GetComponent<Slider>().value = (int)SessionData["SessionCurrentHP"];
        CurrentHPText.GetComponent<TextMeshPro>().text = "Current HP (" + Mathf.CeilToInt((int)SessionData["SessionCurrentHP"] / (float)MaxHP * 100) + "%)";
        CurrentHPPercent = Mathf.CeilToInt((int)SessionData["SessionCurrentHP"] / (float)MaxHP * 100) ;
        CurrentHP = (int)SessionData["SessionCurrentHP"];
    }
    #endregion
    #region Lock Item
    public void LockItem()
    {
        //Lock if current hp that unmet the requirement
        for (int i = 0; i < Item.Length; i++)
        {
            Item[i].transform.GetChild(1).gameObject.SetActive(true);
            Item[i].GetComponent<SessionArsenalServiceItem>().CanBeRepaired = false;           
            if (i == 0)
            {
                if (100 - CurrentHPPercent <= 25 && 100 - CurrentHPPercent > 0)
                {
                    Item[i].transform.GetChild(1).gameObject.SetActive(false);
                    Item[i].GetComponent<SessionArsenalServiceItem>().CanBeRepaired = true;
                }
            }
            else
            {
                if (i == 1)
                {
                    if (100 - CurrentHPPercent <= 50 && 100 - CurrentHPPercent > 25)
                    {
                        Item[i].transform.GetChild(1).gameObject.SetActive(false);
                        Item[i - 1].transform.GetChild(1).gameObject.SetActive(false);
                        Item[i].GetComponent<SessionArsenalServiceItem>().CanBeRepaired = true;
                        Item[i - 1].GetComponent<SessionArsenalServiceItem>().CanBeRepaired = true;
                    }
                }
                else
                {
                    if (i == 2)
                    {
                        if (100 - CurrentHPPercent < 100 && 100 - CurrentHPPercent > 50)
                        {
                            Debug.Log("aaa");
                            Item[i].transform.GetChild(1).gameObject.SetActive(false);
                            Item[i - 1].transform.GetChild(1).gameObject.SetActive(false);
                            Item[i - 2].transform.GetChild(1).gameObject.SetActive(false);
                            Item[i].GetComponent<SessionArsenalServiceItem>().CanBeRepaired = true;
                            Item[i - 1].GetComponent<SessionArsenalServiceItem>().CanBeRepaired = true;
                            Item[i - 2].GetComponent<SessionArsenalServiceItem>().CanBeRepaired = true;
                        }
                    }
                }
            }          
        }
    }
    #endregion
    #region Reset after repair
    public void ResetAfterRepair()
    {
        SessionData = FindObjectOfType<AccessDatabase>().GetSessionInfoByPlayerId(PlayerPrefs.GetInt("PlayerID"));
        PlayerCash.GetComponent<TextMeshPro>().text = ((int)SessionData["SessionCash"]).ToString();
        SetHealth();
        ShowInformation(1);
        LockItem();
    }
    #endregion
    #region Reset data after exiting
    private void OnDisable()
    {
        Destroy(ModelGO);
    }
    #endregion
}
