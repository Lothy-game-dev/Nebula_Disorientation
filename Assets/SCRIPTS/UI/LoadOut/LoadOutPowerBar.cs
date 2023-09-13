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
    public GameObject Background;
    public GameObject PopUp;
    public GameObject PowerList;
    public GameObject StatusBoard;
    #endregion
    #region NormalVariables
    public string currentChosen;
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
        // Retrieve data
        //Test
        List<string> data = new List<string>();
        data.Add("SituationalBarrier");
        data.Add("ShortLaserBeam");
        data.Add("SuperiorRocketBurstDevice");
        data.Add("InstantWormhole");
        data.Add("FortifiedBarrier");
        data.Add("HeavyBarrier");
        data.Add("EnhancedRocketBurstDevice");
        PopUp.transform.localScale = new Vector2(PopUpInitScale, PopUp.transform.localScale.y);
        PopUp.GetComponent<LoadOutPowerPopUp>().OpenPopUp(data, currentChosen);
    }

    public void OnBackgroundMouseDown()
    {
        GetComponent<SpriteRenderer>().sortingOrder = 0;
        transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 1;
        PopUp.GetComponent<LoadOutPowerPopUp>().ClosePopUp();
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
    #endregion
}
