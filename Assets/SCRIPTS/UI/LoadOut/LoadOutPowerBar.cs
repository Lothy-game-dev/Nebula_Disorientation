using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadOutPowerBar : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject Scene;
    public GameObject Background;
    public GameObject PopUp;
    public GameObject PowerList;
    public GameObject StatusBoard;
    #endregion
    #region NormalVariables
    public string currentChosen;
    private List<string> data;
    private float PopUpInitScale;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void OnEnable()
    {
        // Initialize variables
        PopUpInitScale = PopUp.transform.localScale.x;
        currentChosen = "";
        StatusBoard.GetComponent<LoadOutStatusBoard>().SetData(currentChosen);
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Mouse check
    private void OnMouseDown()
    {
        GetComponent<SpriteRenderer>().sortingOrder = 20;
        transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 21;
        PopUp.transform.localScale = new Vector2(PopUpInitScale, PopUp.transform.localScale.y);
        PopUp.GetComponent<LoadOutPowerPopUp>().OpenPopUp(data, currentChosen);
    }

    public void SetItems(List<string> datas, string Chosen)
    {
        data = datas;
        currentChosen = Chosen;
    }

    public void OnBackgroundMouseDown()
    {
        GetComponent<SpriteRenderer>().sortingOrder = 0;
        transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 1;
        PopUp.GetComponent<LoadOutPowerPopUp>().ClosePopUp();
    }

    public void AddDataFinal()
    {
        if (name.Equals("1stPower"))
        {
            Scene.GetComponent<LoadoutScene>().FirstPower = currentChosen;
        }
        else if (name.Equals("2ndPower"))
        {
            Scene.GetComponent<LoadoutScene>().SecondPower = currentChosen;
        }
    }
    #endregion
    #region Set Item
    public void SetItem(string name)
    {
        if (name.Equals(""))
        {
            transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
            transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
            currentChosen = "";
        } else
        {
            transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            currentChosen = name;
            for (int i = 0; i < PowerList.transform.childCount; i++)
            {
                if (PowerList.transform.GetChild(i).name.Replace(" ", "").ToLower().Equals(name.Replace(" ", "").ToLower()))
                {
                    transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = PowerList.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite;
                }
            }
        }
        StatusBoard.GetComponent<LoadOutStatusBoard>().SetData(name);
    }

    private void OnDisable()
    {
        PopUp.transform.localScale = new Vector2(PopUpInitScale, PopUp.transform.localScale.y);
        SetItem("");
    }
    #endregion
}
