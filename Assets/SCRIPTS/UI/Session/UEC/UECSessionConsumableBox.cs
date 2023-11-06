using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UECSessionConsumableBox : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject Scene;
    public GameObject ConsumableList;
    public GameObject[] Consumables;
    public GameObject ConsMain;
    public GameObject Template;
    public GameObject Content;
    public GameObject ScrollView;
    public GameObject Background;
    public GameObject Left;
    public GameObject Right;
    public GameObject Top;
    public GameObject Bottom;
    public GameObject Item;
    public GameObject[] DisableColliders;
    #endregion
    #region NormalVariables
    public List<string> CurrentCons;
    public List<int> CurrentConsCount;
    public List<int> CurrentConsMax;
    public List<GameObject> ListConsumables;
    public string consStr;
    public int ConsAvaiCount;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void OnEnable()
    {
        // Initialize variables
        StartCoroutine(StartAnimation());
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        if (!CheckIfMouseOutsideScrollRange())
        {
            Background.GetComponent<UECSessionBlackBackground>().DisableCollider *= 0;
        }
    }
    #endregion
    #region Function group 1
    private bool CheckIfMouseOutsideScrollRange()
    {
        if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x > Right.transform.position.x ||
            Camera.main.ScreenToWorldPoint(Input.mousePosition).x < Left.transform.position.x ||
            Camera.main.ScreenToWorldPoint(Input.mousePosition).y > Top.transform.position.y ||
            Camera.main.ScreenToWorldPoint(Input.mousePosition).y < Bottom.transform.position.y)
            return true;
        return false;
    }
    private IEnumerator StartAnimation()
    {
        foreach (var col in DisableColliders)
        {
            if (col.GetComponent<Collider2D>() != null)
                col.GetComponent<Collider2D>().enabled = false;
        }
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
        transform.GetChild(3).gameObject.SetActive(false);
        Color c = GetComponent<SpriteRenderer>().color;
        c.a = 0;
        GetComponent<SpriteRenderer>().color = c;
        Color c2 = transform.GetChild(4).GetComponent<SpriteRenderer>().color;
        c2.a = 0;
        transform.GetChild(4).GetComponent<SpriteRenderer>().color = c2;
        for (int i = 0; i < 10; i++)
        {
            Color c1 = GetComponent<SpriteRenderer>().color;
            c1.a += 1 / 10f;
            GetComponent<SpriteRenderer>().color = c1;
            Color c3 = transform.GetChild(4).GetComponent<SpriteRenderer>().color;
            c3.a += (156 / 255f) / 10f;
            transform.GetChild(4).GetComponent<SpriteRenderer>().color = c3;
            yield return new WaitForSeconds(0.5f / 10f);
        }
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(true);
        transform.GetChild(2).gameObject.SetActive(true);
        transform.GetChild(3).gameObject.SetActive(true);
        GenerateData(consStr);
    }

    public void GenerateData(string consStr)
    {
        CurrentCons = new();
        CurrentConsCount = new();
        ConvertConsumableString(consStr);
        GenerateItem();
    }

    private void GenerateItem()
    {
        ListConsumables = new();
        ScrollView.GetComponent<ScrollRect>().horizontal = false;
        Dictionary<string, int> ListOwnCons = FindObjectOfType<AccessDatabase>().GetSessionOwnedConsumables(PlayerPrefs.GetInt("PlayerID"));
        foreach (var item in ListOwnCons)
        {
            for (int i = 0; i < ConsumableList.transform.childCount; i++)
            {
                if (ConsumableList.transform.GetChild(i).name.Replace("-", "").Replace(" ", "").ToLower().Equals(item.Key.Replace("-", "").Replace(" ", "").ToLower()))
                {
                    GameObject Consumable = Instantiate(Template, Template.transform.position, Quaternion.identity);
                    Consumable.transform.GetChild(0).GetComponent<Image>().sprite = ConsumableList.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite;
                    Consumable.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = item.Value.ToString();
                    Consumable.name = item.Key;
                    if (CurrentCons.Contains(item.Key))
                    {
                        int n = CurrentCons.IndexOf(item.Key);
                        Consumable.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = CurrentConsCount[n].ToString();
                        Consumable.transform.GetChild(2).gameObject.SetActive(true);
                        Consumable.GetComponent<Image>().color = Color.green;
                        Consumable.transform.GetChild(3).gameObject.SetActive(true);
                    }
                    Consumable.transform.SetParent(Content.transform);
                    Consumable.transform.localScale = Template.transform.localScale;
                    Consumable.SetActive(true);
                    ListConsumables.Add(Consumable);
                    break;
                }
            }
        }
        ScrollView.GetComponent<ScrollRect>().horizontal = true;
    }

    public string AddItem(GameObject go)
    {
        ShowDataOfItem(go);
        if (CurrentCons.Contains(go.name))
        {
            int n = CurrentCons.IndexOf(go.name);
            if (CurrentConsCount[n] < CurrentConsMax[n])
            {
                CurrentConsCount[n]++;
                ReloadData();
                return "Success";
            } else
            {
                return "Fail";
            }
        } else
        {
            if (CurrentCons.Count < ConsAvaiCount)
            {
                CurrentCons.Add(go.name);
                CurrentConsCount.Add(1);
                Dictionary<string, object> ConssData = FindObjectOfType<AccessDatabase>().GetConsumableDataByName(go.name);
                CurrentConsMax.Add((int)ConssData["Stack"]);
                ReloadData();
                return "Success";
            }
            else
            {
                return "Fail";
            }
        }
    }

    public string ReduceItem(GameObject go)
    {
        ShowDataOfItem(go);
        if (CurrentCons.Contains(go.name))
        {
            int n = CurrentCons.IndexOf(go.name);
            if (CurrentConsCount[n] >= 1)
            {
                CurrentConsCount[n]--;
                if (CurrentConsCount[n]==0)
                {
                    CurrentCons.Remove(go.name);
                    CurrentConsCount.RemoveAt(n);
                    CurrentConsMax.RemoveAt(n);
                }
                ReloadData();
                return "Success";
            }
            else
            {
                return "Fail";
            }
        }
        else
        {
            return "Fail";
        }
    }

    public void ShowDataOfItem(GameObject go)
    {
        Item.SetActive(true);
        Dictionary<string, object> DataDict = FindObjectOfType<AccessDatabase>().GetConsumableDataByName(go.name);
        for (int i=0;i<ConsumableList.transform.childCount;i++)
        {
            if (ConsumableList.transform.GetChild(i).name.Replace(" ","").Replace("-","").ToLower().Equals(go.name.Replace(" ", "").Replace("-", "").ToLower()))
            {
                Item.GetComponent<SpriteRenderer>().sprite = ConsumableList.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite;
                break;
            }
        }
        Item.transform.GetChild(0).GetComponent<TextMeshPro>().text = "<color=" + (string)DataDict["Color"] + ">" + (string)DataDict["Name"] + "</color>";
        Item.transform.GetChild(1).GetComponent<TextMeshPro>().text = (string)DataDict["Description"];
        Item.transform.GetChild(2).GetComponent<TextMeshPro>().text = FindObjectOfType<GlobalFunctionController>().ConvertEffectAndDurationOfConsumables((string)DataDict["Effect"], (int)DataDict["Duration"]);
        Item.transform.GetChild(3).GetComponent<TextMeshPro>().text = "Max Stack: " + (int)DataDict["Stack"];
    }
    public void ReloadData()
    {
        // Save Data
        FindObjectOfType<AccessDatabase>().UpdateSessionInfo(PlayerPrefs.GetInt("PlayerID"), "Consumable", ConvertToConsumableString());
        // Regenerate
        Scene.GetComponent<UECSessionScene>().RegenerateAllData();
    }

    public void BackgroundMouseDown()
    {
        // Save Data
        FindObjectOfType<AccessDatabase>().UpdateSessionInfo(PlayerPrefs.GetInt("PlayerID"), "Consumable", ConvertToConsumableString());
        foreach (var item in Consumables)
        {
            item.GetComponent<SpriteRenderer>().sortingOrder = 0;
            item.transform.GetChild(0).GetComponent<TextMeshPro>().sortingOrder = 2;
            if (item.transform.childCount > 1)
            item.transform.GetChild(1).GetComponent<SpriteRenderer>().sortingOrder = 1;
        }
        ConsMain.GetComponent<Collider2D>().enabled = true;
        gameObject.SetActive(false);
        // Regenerate
        Scene.GetComponent<UECSessionScene>().RegenerateAllData();
    }

    private void OnDisable()
    {
        foreach (var col in DisableColliders)
        {
            if (col.GetComponent<Collider2D>() != null)
                col.GetComponent<Collider2D>().enabled = true;
        }
        Item.SetActive(false);
        int n = 0;
        while (n<ListConsumables.Count)
        {
            if (ListConsumables[n]!=null)
            {
                GameObject temp = ListConsumables[n];
                ListConsumables.Remove(temp);
                Destroy(temp);
            } else
            {
                n++;
            }
        }
    }

    private void ConvertConsumableString(string str)
    {
        if (str.Length>0)
        {
            string[] ConsData = str.Split("|");
            foreach (var item in ConsData)
            {
                CurrentCons.Add(item.Split("-")[0]);
                CurrentConsCount.Add(int.Parse(item.Split("-")[1]));
                Dictionary<string, object> ConssData = FindObjectOfType<AccessDatabase>().GetConsumableDataByName(item.Split("-")[0]);
                CurrentConsMax.Add((int)ConssData["Stack"]);
            }
        }
    }

    private string ConvertToConsumableString()
    {
        string final = "";
        for (int i =0;i<CurrentCons.Count;i++)
        {
            final += CurrentCons[i] + "-" + CurrentConsCount[i] + "|";
        }
        if (final.Length>0)
        final = final.Substring(0, final.Length - 1);
        Debug.Log(final);
        return final;
    }
    #endregion
}
