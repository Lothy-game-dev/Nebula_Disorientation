using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Text.RegularExpressions;

public class SpaceShopSessionInformation : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public TMP_InputField InputField;
    public GameObject BasicInfo;
    public GameObject Effect;
    public GameObject CDAndStack;
    public GameObject PurchaseInfo;
    public GameObject BuyButton;
    public GameObject SellButton;
    public GameObject ConsumableList;
    #endregion
    #region NormalVariables
    private Dictionary<string, object> DataDictionary;
    private Dictionary<string, string> OutputData;
    private int Price;
    private string PreviousInput;
    private int MaxStock;
    private string currentItemName;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void OnEnable()
    {
        // Initialize variables
        DataDictionary = new Dictionary<string, object>();
        OutputData = new Dictionary<string, string>();
        PreviousInput = "1";
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        if (InputField.text != PreviousInput)
        {
            CheckInput();
            PreviousInput = InputField.text;
        }
    }
    #endregion
    #region Show Info
    public void ShowInformationOfItem(string name)
    {
        // Get data and show it to the UI
        currentItemName = name;
        DataDictionary = FindObjectOfType<AccessDatabase>().GetConsumableDataByName(name);
        OutputData = FindObjectOfType<GlobalFunctionController>().ConvertDictionaryDataToOutputCons(DataDictionary);
        for (int i = 0; i < ConsumableList.transform.childCount; i++)
        {
            if (ConsumableList.transform.GetChild(i).name.Replace(" ", "").Replace("-", "").ToLower()
                .Equals(name.Replace(" ", "").Replace("-", "").ToLower()))
            {
                BasicInfo.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite
                    = ConsumableList.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite;
                break;
            }
        }
        BasicInfo.transform.GetChild(2).GetComponent<TextMeshPro>().text = OutputData["Name"];
        BasicInfo.transform.GetChild(3).GetComponent<TextMeshPro>().text = OutputData["Rarity"];
        BasicInfo.transform.GetChild(4).GetComponent<TextMeshPro>().text = OutputData["Description"];
        Effect.transform.GetChild(1).GetComponent<TextMeshPro>().text = OutputData["Effect"];
        Effect.transform.GetChild(2).GetComponent<TextMeshPro>().text = OutputData["Duration"];
        CDAndStack.transform.GetChild(1).GetComponent<TextMeshPro>().text = OutputData["Cooldown"];
        CDAndStack.transform.GetChild(2).GetComponent<TextMeshPro>().text = OutputData["Stack"];
        SetInfoForStockAndOwned();
        InputField.text = "1";
        Price = int.Parse(OutputData["Price"]);
        ShowBuySellInfo();
        // Currently Own: WIP
    }

    private void ShowBuySellInfo()
    {
        // Show buy sell money
        if (!BuyButton.GetComponent<Collider2D>().enabled)
        {
            BuyButton.GetComponent<Collider2D>().enabled = true;
        }
        BuyButton.transform.GetChild(0).GetComponent<TextMeshPro>().color = Color.white;
        BuyButton.transform.GetChild(0).GetComponent<TextMeshPro>().text =
            "Buy (" + (Price * int.Parse(InputField.text) * 2).ToString() + " <sprite index='3'>)";
        BuyButton.GetComponent<SpaceShopSessionBuySellButton>().CurrentValue = Price * int.Parse(InputField.text) * 2;
        BuyButton.GetComponent<SpaceShopSessionBuySellButton>().ItemName = BasicInfo.transform.GetChild(2).GetComponent<TextMeshPro>().text;
        BuyButton.GetComponent<SpaceShopSessionBuySellButton>().ItemNameNoColor = (string)DataDictionary["Name"];
        BuyButton.GetComponent<SpaceShopSessionBuySellButton>().Quantity = int.Parse(InputField.text);
        // Fuel cell cant sell
        if (!"fuelcore".Equals(((string)DataDictionary["Name"]).Replace(" ", "").ToLower()))
        {
            SellButton.SetActive(true);
            if (!SellButton.GetComponent<Collider2D>().enabled)
            {
                SellButton.GetComponent<Collider2D>().enabled = true;
            }
            SellButton.transform.GetChild(0).GetComponent<TextMeshPro>().color = Color.white;
            SellButton.GetComponent<SpriteRenderer>().color = Color.green;
            SellButton.transform.GetChild(0).GetComponent<TextMeshPro>().text =
                "Sell (" + (Price * int.Parse(InputField.text) / 2).ToString() + " <sprite index='3'>)";
            SellButton.GetComponent<SpaceShopSessionBuySellButton>().CurrentValue = Price * int.Parse(InputField.text) / 2;
            SellButton.GetComponent<SpaceShopSessionBuySellButton>().ItemName = BasicInfo.transform.GetChild(2).GetComponent<TextMeshPro>().text;
            SellButton.GetComponent<SpaceShopSessionBuySellButton>().ItemNameNoColor = (string)DataDictionary["Name"];
            SellButton.GetComponent<SpaceShopSessionBuySellButton>().Quantity = int.Parse(InputField.text);
        }
        else
        {
            SellButton.SetActive(false);
        }

    }

    public void SetInfoForStockAndOwned()
    {
        if (currentItemName != null)
        {
            if ("fuelcell".Equals(currentItemName.Replace(" ", "").ToLower()))
            {
                PurchaseInfo.transform.GetChild(1).GetComponent<TextMeshPro>().text = "(Currently have: "
            + ((int)FindObjectOfType<AccessDatabase>().GetPlayerInformationById(
            PlayerPrefs.GetInt("PlayerID"))["FuelCell"]).ToString() + " cells)";
            }
            else
            {
                PurchaseInfo.transform.GetChild(1).GetComponent<TextMeshPro>().text = "(Session Owned: "
            + FindObjectOfType<AccessDatabase>().GetSessionCurrentOwnedNumberOfConsumableByName(
            PlayerPrefs.GetInt("PlayerID"), currentItemName).ToString() + " items)";
            }
            MaxStock = FindObjectOfType<AccessDatabase>().GetStocksPerDayOfConsumable((string)DataDictionary["Name"]);
            if (MaxStock == 0)
            {
                if (BuyButton.GetComponent<CursorUnallowed>() == null)
                {
                    BuyButton.AddComponent<CursorUnallowed>();
                    BuyButton.GetComponent<SpriteRenderer>().color = Color.red;
                }
            }
            else
            {
                if (BuyButton.GetComponent<CursorUnallowed>() != null)
                {
                    Destroy(BuyButton.GetComponent<CursorUnallowed>());
                    BuyButton.GetComponent<SpriteRenderer>().color = Color.white;
                }
            }
        }
    }
    private void CheckInput()
    {
        Regex Regex = new Regex(@"^\d+$");
        // Check if input is integer and >=1
        if (Regex.Match(InputField.text).Success && int.Parse(InputField.text) >= 1)
        {
            // check limit 100 per transaction
            if (int.Parse(InputField.text) > 100)
            {
                InputField.text = "100";
            }
            ShowBuySellInfo();
        }
        else
        {
            InputField.text = "1";
            ShowBuySellInfo();
        }
    }
    #endregion
}
