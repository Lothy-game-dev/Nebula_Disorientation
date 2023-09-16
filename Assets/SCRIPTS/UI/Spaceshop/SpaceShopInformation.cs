using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

public class SpaceShopInformation : MonoBehaviour
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
        DataDictionary = FindObjectOfType<AccessDatabase>().GetConsumableDataByName(name);
        OutputData = FindObjectOfType<GlobalFunctionController>().ConvertDictionaryDataToOutputCons(DataDictionary);
        for (int i=0;i<ConsumableList.transform.childCount;i++)
        {
            if (ConsumableList.transform.GetChild(i).name.Replace(" ","").Replace("-","").ToLower()
                .Equals(name.Replace(" ","").Replace("-","").ToLower()))
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
        InputField.text = "1";
        MaxStock = int.Parse(OutputData["Stock"]);
        Price = int.Parse(OutputData["Price"]);
        ShowBuySellInfo();
        // Currently Own: WIP
    }

    private void ShowBuySellInfo()
    {
        if (!BuyButton.GetComponent<Collider2D>().enabled)
        {
            BuyButton.GetComponent<Collider2D>().enabled = true;
        }
        BuyButton.transform.GetChild(0).GetComponent<TextMeshPro>().color = Color.white;
        BuyButton.GetComponent<SpriteRenderer>().color = Color.white;
        BuyButton.transform.GetChild(0).GetComponent<TextMeshPro>().text =
            "Buy (" + (Price * int.Parse(InputField.text)).ToString() + " <sprite index='0'>)";
        BuyButton.GetComponent<SpaceShopBuySellButton>().CurrentValue = Price * int.Parse(InputField.text);
        BuyButton.GetComponent<SpaceShopBuySellButton>().ItemName = BasicInfo.transform.GetChild(2).GetComponent<TextMeshPro>().text;
        BuyButton.GetComponent<SpaceShopBuySellButton>().Quantity = int.Parse(InputField.text);
        if (!SellButton.GetComponent<Collider2D>().enabled)
        {
            SellButton.GetComponent<Collider2D>().enabled = true;
        }
        SellButton.transform.GetChild(0).GetComponent<TextMeshPro>().color = Color.white;
        SellButton.GetComponent<SpriteRenderer>().color = Color.green;
        SellButton.transform.GetChild(0).GetComponent<TextMeshPro>().text =
            "Sell (" + (Price * int.Parse(InputField.text) / 2).ToString() + " <sprite index='0'>)";
        SellButton.GetComponent<SpaceShopBuySellButton>().CurrentValue = Price * int.Parse(InputField.text)/2;
        SellButton.GetComponent<SpaceShopBuySellButton>().ItemName = BasicInfo.transform.GetChild(2).GetComponent<TextMeshPro>().text;
        SellButton.GetComponent<SpaceShopBuySellButton>().Quantity = int.Parse(InputField.text);
    }

    private void CheckInput()
    {
        Regex Regex = new Regex(@"^\d+$");
        if (Regex.Match(InputField.text).Success && int.Parse(InputField.text)>=1)
        {
            if (MaxStock == 0)
            {
                ShowBuySellInfo();
            } else
            {
                if (int.Parse(InputField.text)>MaxStock)
                {
                    InputField.text = MaxStock.ToString();
                }
            }
        } else
        {
            InputField.text = "1";
            ShowBuySellInfo();
        }
    }
    #endregion
}
