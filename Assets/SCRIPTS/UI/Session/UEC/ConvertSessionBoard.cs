using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ConvertSessionBoard : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public ConvertItemBox ItemLeft;
    public ConvertItemBox ItemRight;
    public TMP_InputField TextInput;
    public TextMeshPro TextOutput;
    public TextMeshPro Text;
    #endregion
    #region NormalVariables
    private float ConvertRate;
    private string From;
    private string To;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variable
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        if (TextInput.isFocused)
        {
            if (TextInput.text!="")
            {
                if (int.Parse(TextInput.text)>100000)
                {
                    TextInput.text = "100000";
                }
                TextOutput.text = ((int)(int.Parse(TextInput.text) * ConvertRate)).ToString();
            } else
            {
                TextOutput.text = "0";
            }
        }
    }
    #endregion
    #region Set Convert Item
    public void SetConvertItem(string ConvertFrom, string ConvertTo, float Rate)
    {
        From = ConvertFrom;
        ItemLeft.ShowItem(ConvertFrom);
        To = ConvertTo;
        ItemRight.ShowItem(ConvertTo);
        ConvertRate = Rate;
        Text.text = "Convert " + FindObjectOfType<GlobalFunctionController>().ConvertToIcon(From) + " to " + FindObjectOfType<GlobalFunctionController>().ConvertToIcon(To) + "?";
    }

    public void Convert()
    {
        int id = PlayerPrefs.GetInt("PlayerID");
        Dictionary<string, object> SessionData = FindObjectOfType<AccessDatabase>().GetSessionInfoByPlayerId(PlayerPrefs.GetInt("PlayerID"));
        string check = "Success";
        if (int.Parse(TextInput.text) > (int)SessionData["SessionTimelessShard"])
        {
            check = "Not Enough";
        }
        if ("Fail".Equals(check))
        {
            FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(transform.position,
            "Fail to fetch data.\nPlease try again or contact our email!", 5f);
            ItemLeft.ShowItem(From);
        }
        else if ("No Exist".Equals(check))
        {
            FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(transform.position,
            "Pilot does not exist!\n Please log-in again.", 5f);
            ItemLeft.ShowItem(From);
        }
        else if ("Over Limit".Equals(check))
        {
            FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(transform.position,
            FindObjectOfType<GlobalFunctionController>().ConvertToIcon(To) + "'s quantity after converted is over its limit!\n Please try again.", 5f);
            TextInput.text = "0";
            TextOutput.text = "0";
            ItemLeft.ShowItem(From);
        }
        else if ("Not Enough".Equals(check))
        {
            FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(transform.position,
            "Your " + FindObjectOfType<GlobalFunctionController>().ConvertToIcon(From) + " is not enough for this conversion!\n Please try again.", 5f);
            TextInput.text = "0";
            TextOutput.text = "0";
            ItemLeft.ShowItem(From);
        }
        else if ("Success".Equals(check))
        {
            if (int.Parse(TextOutput.text) > 0)
            {
                ItemLeft.StartAnimation();
            }
            else
            {
                FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(transform.position,
                "Cannot convert, please re-enter the amount of the pre-converted item", 5f);
                TextInput.text = "0";
                TextOutput.text = "0";
            }
        }
    }

    public void ConvertDone()
    {
        int id = PlayerPrefs.GetInt("PlayerID");
        string check = "Success";
        check = FindObjectOfType<AccessDatabase>().UpdateSessionCashAndShardByPlayerID(id, false, 0, int.Parse(TextInput.text));
        if (check=="Success")
            check = FindObjectOfType<AccessDatabase>().UpdateSessionCashAndShardByPlayerID(id, true, int.Parse(TextOutput.text), 0);
        if ("Fail".Equals(check))
        {
            FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(transform.position,
            "Convert failed.\nPlease try again or contact our email!", 5f);
            ItemLeft.ShowItem(From);
        }
        else if ("No Exist".Equals(check))
        {
            FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(transform.position,
            "Pilot does not exist!\n Please try again.", 5f);
            ItemLeft.ShowItem(From);
        }
        else if ("Success".Equals(check))
        {
            FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(transform.position,
            "Converted successfully!!!\nYou received " + TextOutput.text + " " + FindObjectOfType<GlobalFunctionController>().ConvertToIcon(To) + "!", 5f);
            FindObjectOfType<UECSessionScene>().ResetEconomyData();
            Destroy(gameObject.transform.parent.gameObject);
        }
    }
    #endregion
}
