using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadOutConsumables : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject Scene;
    public GameObject PopUp;
    public GameObject[] Consumables;
    public GameObject ConsumeList;
    #endregion
    #region NormalVariables
    private List<GameObject> CurrentItems;
    private List<int> CurrentItemCount;
    private List<int> CurrentItemLimit;
    private Dictionary<string, int> Datas;
    private GameObject AddModel;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void OnEnable()
    {
        // Initialize variables
        CurrentItems = new List<GameObject>(new GameObject[4]);
        CurrentItemCount = new List<int>(new int[4]);
        CurrentItemLimit = new List<int>(new int[4]);
        if (PopUp.activeSelf)
        {
            PopUp.GetComponent<LoadOutConsumablePopUp>().ClosePopup();
        }
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
        GetComponent<Collider2D>().enabled = false;
        for (int i=0;i<Consumables.Length; i++)
        {
            Consumables[i].GetComponent<SpriteRenderer>().sortingOrder = 21;
            Consumables[i].transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 22;
            Consumables[i].transform.GetChild(1).GetComponent<TextMeshPro>().sortingOrder = 23;
        }
        PopUp.GetComponent<LoadOutConsumablePopUp>().OpenPopup(Datas, GetListCurrentChosen());
    }

    public void SetInitData(Dictionary<string, int> data)
    {
        Datas = data;
    }

    private Dictionary<string, int> GetListCurrentChosen()
    {
        // Check the list currently chosen
        Dictionary<string, int> DataTemp = new Dictionary<string, int>();
        for (int i=0;i<CurrentItems.Count;i++)
        {
            if (CurrentItems[i]!=null)
            {
                DataTemp.Add(CurrentItems[i].name.Replace(" ", "").Replace("-", ""), CurrentItemCount[i]);
            }
        }
        return DataTemp;
    }

    public void OnBackgroundMouseDown()
    {
        for (int i = 0; i < Consumables.Length; i++)
        {
            Consumables[i].GetComponent<SpriteRenderer>().sortingOrder = 4;
        }
        PopUp.GetComponent<LoadOutConsumablePopUp>().ClosePopup();
        for (int i=0;i<CurrentItems.Count;i++)
        {
            if (CurrentItems[i] !=null)
            {
                if (Scene.GetComponent<LoadoutScene>().Consumables.ContainsKey(CurrentItems[i].name))
                {
                    Scene.GetComponent<LoadoutScene>().Consumables[CurrentItems[i].name] = CurrentItemCount[i];
                } else
                Scene.GetComponent<LoadoutScene>().Consumables.Add(CurrentItems[i].name, CurrentItemCount[i]);
            }
        }
    }

    public int IncreaseItem(string ItemName)
    {
        // If the item is not chosen before
        if (ItemSelected(ItemName)==0)
        {
            // Check if there is slot for this item
            if (GetAvaiSlotForNewItem(ItemName) != 0)
            {
                // If yes then check if model for this item is avai
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
                    int n = GetAvaiSlotForNewItem(ItemName) -1;
                    // get stack of the item
                    int k = FindObjectOfType<AccessDatabase>().GetStackLimitOfConsumableByName(ItemName);
                    // Add item
                    if (k != -1)
                    {
                        CurrentItems[n] = AddModel;
                        CurrentItemCount[n] = 1;
                        CurrentItemLimit[n] = k;
                        Consumables[n].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite
                        = AddModel.GetComponent<SpriteRenderer>().sprite;
                        Consumables[n].transform.GetChild(2).gameObject.SetActive(false);
                        Consumables[n].transform.GetChild(1).gameObject.SetActive(true);
                        Consumables[n].transform.GetChild(1).GetComponent<TextMeshPro>().text = CurrentItemCount[n] + "/" + CurrentItemLimit[n];
                        return CurrentItemCount[n];
                    } else
                    return -1;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return -1;
            }
        } else if (ItemSelected(ItemName) == -1)
        {
            // If the item has already exist and cannot be added anymore
            FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Scene.transform.position,
                "Cannot bring anymore of this item!", 5f);
            return -1;
        } else
        {
            // If the item has already exist and can still be add
            int n = ItemSelected(ItemName) - 1;
            CurrentItemCount[n]++;
            Consumables[n].transform.GetChild(1).GetComponent<TextMeshPro>().text = CurrentItemCount[n] + "/" + CurrentItemLimit[n];
            return CurrentItemCount[n];
        }
    }

    public int DecreaseItem(string ItemName)
    {
        // Same as increase but reverse
        if (ItemSelectedDecrease(ItemName) == 0)
        {
            FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Scene.transform.position,
                "Error!\nNothing to decrease here!", 5f);
            return -1;
        } else
        {
            int n = ItemSelectedDecrease(ItemName) - 1;
            CurrentItemCount[n]--;
            if (CurrentItemCount[n] == 0)
            {
                CurrentItems[n] = null;
                CurrentItemLimit[n] = 0;
                CurrentItemCount[n] = 0;
                Consumables[n].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite
                        = null;
                Consumables[n].transform.GetChild(2).gameObject.SetActive(true);
                Consumables[n].transform.GetChild(1).gameObject.SetActive(false);
                Consumables[n].transform.GetChild(1).GetComponent<TextMeshPro>().text = "";
            }
            else
            {
                Consumables[n].transform.GetChild(1).GetComponent<TextMeshPro>().text = CurrentItemCount[n] + "/" + CurrentItemLimit[n];
            }
            return CurrentItemCount[n];
        }
    }

    private int GetAvaiSlotForNewItem(string ItemName)
    {
        // Check if there are still slots for new item
        for (int i = 0; i < CurrentItems.Count;i++)
        {
            if (CurrentItems[i] == null)
            {
                return i+1;
            }
            else if (CurrentItems[i] != null && CurrentItemCount[i] < CurrentItemLimit[i]
                && CurrentItems[i].name.Replace(" ", "").ToLower().Equals(ItemName.Replace(" ", "").ToLower()))
            {
                return i+1;
            }
        }
        return 0;
    } 

    private int ItemSelected(string name)
    {
        for (int i = 0; i < CurrentItems.Count; i++)
        {
            if (CurrentItems[i]!=null && CurrentItems[i].name.Replace(" ", "").ToLower().Equals(name.Replace(" ", "").ToLower()))
            {
                if (CurrentItemCount[i] < CurrentItemLimit[i])
                    return i + 1;
                else return -1;
            }
        }
        return 0;
    }

    private int ItemSelectedDecrease(string name)
    {
        for (int i = 0; i < CurrentItems.Count; i++)
        {
            if (CurrentItems[i] != null && CurrentItems[i].name.Replace(" ", "").ToLower().Equals(name.Replace(" ", "").ToLower()))
            {
                if (CurrentItemCount[i] > 0)
                    return i + 1;
                else return -1;
            }
        }
        return 0;
    }

    public void ResetNumberOfConsumable(int n)
    {
        // For case fighters has 2, 3 and 4 slots
        if (n==2)
        {
            if (CurrentItems.Count==3)
            {
                if (Scene.GetComponent<LoadoutScene>().Consumables != null &&
                CurrentItems[2] != null &&
                Scene.GetComponent<LoadoutScene>().Consumables.ContainsKey(CurrentItems[2].name))
                {
                    Scene.GetComponent<LoadoutScene>().Consumables.Remove(CurrentItems[2].name);
                }
                CurrentItems.RemoveAt(2);
                CurrentItemLimit.RemoveAt(2);
                CurrentItemCount.RemoveAt(2);
                Consumables[2].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite
                        = null;
                Consumables[2].transform.GetChild(1).gameObject.SetActive(false);
                Consumables[2].transform.GetChild(2).gameObject.SetActive(false);
                Consumables[2].transform.GetChild(3).gameObject.SetActive(true);
                Consumables[2].transform.GetChild(1).GetComponent<TextMeshPro>().text = "";
                Consumables[2].GetComponent<SpriteRenderer>().color = new Color(125 / 255f, 125 / 255f, 125 / 255f);
            } else if (CurrentItems.Count == 4)
            {
                if (Scene.GetComponent<LoadoutScene>().Consumables != null &&
                CurrentItems[2] != null &&
                Scene.GetComponent<LoadoutScene>().Consumables.ContainsKey(CurrentItems[2].name))
                {
                    Scene.GetComponent<LoadoutScene>().Consumables.Remove(CurrentItems[2].name);
                }
                CurrentItems.RemoveAt(2);
                CurrentItemLimit.RemoveAt(2);
                CurrentItemCount.RemoveAt(2);
                Consumables[2].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite
                        = null;
                Consumables[2].transform.GetChild(1).gameObject.SetActive(false);
                Consumables[2].transform.GetChild(2).gameObject.SetActive(false);
                Consumables[2].transform.GetChild(3).gameObject.SetActive(true);
                Consumables[2].transform.GetChild(1).GetComponent<TextMeshPro>().text = "";
                Consumables[2].GetComponent<SpriteRenderer>().color = new Color(125 / 255f, 125 / 255f, 125 / 255f);
                if (Scene.GetComponent<LoadoutScene>().Consumables != null &&
                CurrentItems[2] != null &&
                Scene.GetComponent<LoadoutScene>().Consumables.ContainsKey(CurrentItems[2].name))
                {
                    Scene.GetComponent<LoadoutScene>().Consumables.Remove(CurrentItems[2].name);
                }
                CurrentItems.RemoveAt(2);
                CurrentItemLimit.RemoveAt(2);
                CurrentItemCount.RemoveAt(2);
                Consumables[3].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite
                        = null;
                Consumables[3].transform.GetChild(1).gameObject.SetActive(false);
                Consumables[3].transform.GetChild(2).gameObject.SetActive(false);
                Consumables[3].transform.GetChild(3).gameObject.SetActive(true);
                Consumables[3].transform.GetChild(1).GetComponent<TextMeshPro>().text = "";
                Consumables[3].GetComponent<SpriteRenderer>().color = new Color(125 / 255f, 125 / 255f, 125 / 255f);
            }
        }
        else if (n==3)
        {
            if (CurrentItems.Count==2)
            {
                CurrentItems.Add(null);
                CurrentItemLimit.Add(1);
                CurrentItemCount.Add(0);
                Consumables[2].GetComponent<SpriteRenderer>().color = Color.white;
                Consumables[2].transform.GetChild(2).gameObject.SetActive(true);
                Consumables[2].transform.GetChild(3).gameObject.SetActive(false);
            } else if (CurrentItems.Count == 4)
            {
                if (Scene.GetComponent<LoadoutScene>().Consumables != null &&
                CurrentItems[3] != null &&
                Scene.GetComponent<LoadoutScene>().Consumables.ContainsKey(CurrentItems[3].name))
                {
                    Scene.GetComponent<LoadoutScene>().Consumables.Remove(CurrentItems[3].name);
                }
                CurrentItems.RemoveAt(3);
                CurrentItemLimit.RemoveAt(3);
                CurrentItemCount.RemoveAt(3);
                Consumables[3].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite
                        = null;
                Consumables[3].transform.GetChild(1).gameObject.SetActive(false);
                Consumables[3].transform.GetChild(2).gameObject.SetActive(false);
                Consumables[3].transform.GetChild(3).gameObject.SetActive(true);
                Consumables[3].transform.GetChild(1).GetComponent<TextMeshPro>().text = "";
                Consumables[3].GetComponent<SpriteRenderer>().color = new Color(125 / 255f, 125 / 255f, 125 / 255f);
            }
        } else if (n==4)
        {
            if (CurrentItems.Count==2)
            {
                CurrentItems.Add(null);
                CurrentItemLimit.Add(1);
                CurrentItemCount.Add(0);
                Consumables[2].GetComponent<SpriteRenderer>().color = Color.white;
                Consumables[2].transform.GetChild(2).gameObject.SetActive(true);
                Consumables[2].transform.GetChild(3).gameObject.SetActive(false);
                CurrentItems.Add(null);
                CurrentItemLimit.Add(1);
                CurrentItemCount.Add(0);
                Consumables[3].GetComponent<SpriteRenderer>().color = Color.white;
                Consumables[3].transform.GetChild(2).gameObject.SetActive(true);
                Consumables[3].transform.GetChild(3).gameObject.SetActive(false);
            } else if (CurrentItems.Count == 3)
            {
                CurrentItems.Add(null);
                CurrentItemLimit.Add(1);
                CurrentItemCount.Add(0);
                Consumables[3].GetComponent<SpriteRenderer>().color = Color.white;
                Consumables[3].transform.GetChild(2).gameObject.SetActive(true);
                Consumables[3].transform.GetChild(3).gameObject.SetActive(false);
            }
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < Consumables.Length; i++)
        {
            Consumables[i].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
            Consumables[i].transform.GetChild(1).gameObject.SetActive(false);
        }
    }
    #endregion
}
