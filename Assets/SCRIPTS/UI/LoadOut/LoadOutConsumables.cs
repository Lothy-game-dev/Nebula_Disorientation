using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadOutConsumables : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject PopUp;
    public GameObject Consume1;
    public GameObject Consume2;
    public GameObject Consume3;
    public GameObject Consume4;
    public GameObject ConsumeList;
    #endregion
    #region NormalVariables
    public GameObject CurrentItem1;
    public GameObject CurrentItem2;
    public GameObject CurrentItem3;
    public GameObject CurrentItem4;
    private int CurrentExistItem;
    private GameObject AddModel;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void OnEnable()
    {
        // Initialize variables
        CurrentExistItem = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Mouse Check
    private void OnMouseDown()
    {
        Consume1.GetComponent<SpriteRenderer>().sortingOrder = 21;
        Consume2.GetComponent<SpriteRenderer>().sortingOrder = 21;
        Consume3.GetComponent<SpriteRenderer>().sortingOrder = 21;
        Consume4.GetComponent<SpriteRenderer>().sortingOrder = 21;
        List<string> Datas = new List<string>();
        Datas.Add("WingShield");
        Datas.Add("EngineBooster");
        Datas.Add("AutoRepairModule");
        Datas.Add("ReflectiveWingShield");
        Datas.Add("NanoReflectiveCoat");
        PopUp.GetComponent<LoadOutConsumablePopUp>().OpenPopup(Datas, GetListCurrentChosen());
    }

    private List<string> GetListCurrentChosen()
    {
        List<string> Datas = new List<string>();
        if (CurrentItem1!=null)
        {
            Datas.Add(CurrentItem1.name.Replace(" ",""));
        } 
        if (CurrentItem2 != null)
        {
            Datas.Add(CurrentItem2.name.Replace(" ", ""));
        }
        if (CurrentItem3 != null)
        {
            Datas.Add(CurrentItem3.name.Replace(" ", ""));
        }
        if (CurrentItem4 != null)
        {
            Datas.Add(CurrentItem4.name.Replace(" ", ""));
        }
        return Datas;
    }

    public void OnBackgroundMouseDown()
    {
        Consume1.GetComponent<SpriteRenderer>().sortingOrder = 4;
        Consume2.GetComponent<SpriteRenderer>().sortingOrder = 4;
        Consume3.GetComponent<SpriteRenderer>().sortingOrder = 4;
        Consume4.GetComponent<SpriteRenderer>().sortingOrder = 4;
        PopUp.GetComponent<LoadOutConsumablePopUp>().ClosePopup();
    }

    public bool SetItem(string ItemName)
    {
        if (ItemAlreadySelect(ItemName)==0)
        {
            if (CurrentExistItem < 4)
            {
                if (GetNextAvaiSlot() != 0)
                {
                    for (int i = 0; i < ConsumeList.transform.childCount; i++)
                    {
                        if (ConsumeList.transform.GetChild(i).name.Replace(" ", "").ToLower()
                            .Equals(ItemName.Replace(" ", "").ToLower()))
                        {
                            AddModel = ConsumeList.transform.GetChild(i).gameObject;
                            break;
                        }
                    }
                    if (AddModel != null)
                    {
                        int n = GetNextAvaiSlot();
                        if (n == 1)
                        {
                            CurrentItem1 = AddModel;
                            Consume1.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite
                                = AddModel.GetComponent<SpriteRenderer>().sprite;
                            return true;
                        }
                        else if (n == 2)
                        {
                            CurrentItem2 = AddModel;
                            Consume2.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite
                                = AddModel.GetComponent<SpriteRenderer>().sprite;
                            return true;
                        }
                        else if (n == 3)
                        {
                            CurrentItem3 = AddModel;
                            Consume3.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite
                                = AddModel.GetComponent<SpriteRenderer>().sprite;
                            return true;
                        }
                        else if (n == 4)
                        {
                            CurrentItem4 = AddModel;
                            Consume4.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite
                                = AddModel.GetComponent<SpriteRenderer>().sprite;
                            return true;
                        }
                        return false;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        } else
        {
            int n = ItemAlreadySelect(ItemName);
            if (n == 1)
            {
                CurrentItem1 = null;
                Consume1.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
                return false;
            }
            else if (n == 2)
            {
                CurrentItem2 = null;
                Consume2.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
                return false;
            }
            else if (n == 3)
            {
                CurrentItem3 = null;
                Consume3.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
                return false;
            }
            else if (n == 4)
            {
                CurrentItem4 = null;
                Consume4.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
                return false;
            }
            else return false;
        }
    }

    private int GetNextAvaiSlot()
    {
        if (Consume1.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite == null)
        {
            return 1;
        }
        else if (Consume2.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite == null)
        {
            return 2;
        }
        else if (Consume3.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite == null)
        {
            return 3;
        }
        else if (Consume4.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite == null)
        {
            return 4;
        }
        else return 0;
    } 

    private int ItemAlreadySelect(string name)
    {
        if (CurrentItem1!=null && CurrentItem1.name.Replace(" ", "").ToLower().Equals(name.Replace(" ", "").ToLower())) {
            return 1;
        } 
        else if (CurrentItem2 != null && CurrentItem2.name.Replace(" ", "").ToLower().Equals(name.Replace(" ", "").ToLower())) {
            return 2;
        }
        else if (CurrentItem3 != null && CurrentItem3.name.Replace(" ", "").ToLower().Equals(name.Replace(" ", "").ToLower())) {
            return 3;
        }
        else if (CurrentItem4 != null && CurrentItem4.name.Replace(" ", "").ToLower().Equals(name.Replace(" ", "").ToLower())) {
            return 4;
        }
        else return 0;
    }
    #endregion
}
