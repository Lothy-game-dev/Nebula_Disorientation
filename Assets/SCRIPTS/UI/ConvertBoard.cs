using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ConvertBoard : MonoBehaviour
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
        Text.text = "Convert " + From + " to " + To + "?";
    }

    public void Convert()
    {
        if (int.Parse(TextOutput.text)>0)
        {
            ItemLeft.StartAnimation();
        } else
        {
            FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(transform.position,
            "Cannot convert, please re-enter the amount of the pre-converted item", 5f);
        }
    }

    public void ConvertDone()
    {
        // Insert
        FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(transform.position,
            "Convert successfully!!!\nYou received " + TextOutput.text + " " + To + "!", 5f);
        Destroy(gameObject.transform.parent.gameObject);
    }
    #endregion
}
