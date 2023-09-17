using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadOutConsumablePopUp : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject Scene;
    public GameObject ConsumeMain;
    public GameObject Top;
    public GameObject Bottom;
    public GameObject Left;
    public GameObject Right;
    public GameObject Content;
    public GameObject ScrollRect;
    public GameObject ConsumeList;
    public GameObject FirstPos;
    public GameObject BoxLeft;
    public GameObject BoxRight;
    public GameObject Template;
    public GameObject[] DisableColliders;
    public GameObject[] Lines;
    public GameObject Background;
    public GameObject ClickText;
    public GameObject Details;
    #endregion
    #region NormalVariables
    public GameObject CurrentClickItem;

    private List<GameObject> Models;
    private List<GameObject> IconAfterGen;
    private List<GameObject> ChosenGO;
    private List<int> ListIconCount;
    private List<int> ListIconSelectedCount;
    private List<string> ChosenName;
    private float currentTransparency;
    private float BoxSize;
    private float maximumWidth;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        if (!isMouseOutsideRange())
        {
            Background.GetComponent<LoadOutBarBackground>().DisableCollider *= 0;
        }
    }
    #endregion
    #region Mouse Check
    private bool isMouseOutsideRange()
    {
        if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x > Right.transform.position.x ||
            Camera.main.ScreenToWorldPoint(Input.mousePosition).x < Left.transform.position.x ||
            Camera.main.ScreenToWorldPoint(Input.mousePosition).y > Top.transform.position.y ||
            Camera.main.ScreenToWorldPoint(Input.mousePosition).y < Bottom.transform.position.y)
            return true;
        return false;
    }
    #endregion
    #region Set Data
    public void OpenPopup(Dictionary<string, int> ListConsumes, Dictionary<string, int> ChosenConsumes)
    {
        // Set trans
        Color c = GetComponent<SpriteRenderer>().color;
        currentTransparency = 180 / 255f;
        c.a = 0f;
        GetComponent<SpriteRenderer>().color = c;
        // Calculate each item size
        BoxSize = Mathf.Abs(BoxLeft.transform.position.x - BoxRight.transform.position.x);
        // Reset data
        Models = new List<GameObject>();
        IconAfterGen = new List<GameObject>();
        ChosenGO = new List<GameObject>();
        ChosenName = new List<string>();
        // Disable scroll
        ScrollRect.GetComponent<ScrollRect>().horizontal = false;
        // Disable text
        ClickText.SetActive(false);
        Details.SetActive(false);
        Details.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
        Details.transform.GetChild(1).GetComponent<TextMeshPro>().text = "";
        Details.transform.GetChild(2).GetComponent<TextMeshPro>().text = "";
        foreach (var col in DisableColliders)
        {
            if (col.GetComponent<Collider2D>()!=null)
            {
                col.GetComponent<Collider2D>().enabled = false;
            }
        }
        foreach (var line in Lines)
        {
            line.SetActive(false);
        }
        gameObject.SetActive(true);
        StartCoroutine(StartAnimation(ListConsumes, ChosenConsumes));
    }

    private IEnumerator StartAnimation(Dictionary<string, int> ListConsumes, Dictionary<string, int> ChosenConsumes)
    {
        for (int i=0;i<20;i++)
        {
            Color c = GetComponent<SpriteRenderer>().color;
            c.a += currentTransparency / 20f;
            GetComponent<SpriteRenderer>().color = c;
            yield return new WaitForSeconds(0.02f);
        }
        foreach (var line in Lines)
        {
            line.SetActive(true);
        }
        Background.SetActive(true);
        SetData(ListConsumes, ChosenConsumes);
    }

    public void ClosePopup()
    {
        int i = 0;
        while (i<IconAfterGen.Count)
        {
            if (IconAfterGen[i]!=null)
            {
                GameObject temp = IconAfterGen[i];
                IconAfterGen.RemoveAt(i);
                Destroy(temp);
            } else
            {
                i++;
            }
        }
        // Reset size of content
        Content.GetComponent<RectTransform>().sizeDelta
            = new Vector2(0, 0);
        foreach (var line in Lines)
        {
            line.SetActive(false);
        }
        if (ClickText.activeSelf)
        {
            ClickText.SetActive(false);
        }
        Background.SetActive(false);
        Details.SetActive(false);
        StartCoroutine(CloseAnimation());
    }

    private IEnumerator CloseAnimation()
    {
        for (int i = 0; i < 20; i++)
        {
            Color c = GetComponent<SpriteRenderer>().color;
            c.a -= currentTransparency / 20f;
            GetComponent<SpriteRenderer>().color = c;
            yield return new WaitForSeconds(0.02f);
        }
        foreach (var col in DisableColliders)
        {
            if (col.GetComponent<Collider2D>() != null)
            {
                col.GetComponent<Collider2D>().enabled = true;
            }
        }
        gameObject.SetActive(false);
    }

    private void SetData(Dictionary<string, int> DictConsumes, Dictionary<string, int> DictChosenConsumes)
    {
        List<string> ListConsumes = new List<string>(DictConsumes.Keys);
        ListIconCount = new List<int>(DictConsumes.Values);
        List<string> ChosenConsumes = new List<string>(DictChosenConsumes.Keys);
        ListIconSelectedCount = new List<int>(DictChosenConsumes.Values);
        // Same check model as other bars
        foreach (string cons in ChosenConsumes)
        {
            if (!ListConsumes.Contains(cons))
            {
                FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Scene.transform.position,
                    "Can not fetch data for " + cons + ".\nPlease try again!", 5f);
                return;
            }
        }
        for (int i = 0; i < ListConsumes.Count; i++)
        {
            for (int j = 0; j < ConsumeList.transform.childCount; j++)
            {
                if (ConsumeList.transform.GetChild(j).name.Replace(" ", "").ToLower()
                    .Equals(ListConsumes[i].Replace(" ", "").ToLower()))
                {
                    Models.Add(ConsumeList.transform.GetChild(j).gameObject);
                    break;
                }
            }
        }
        if (ChosenConsumes.Count>0)
        {
            ChosenName = ChosenConsumes;
        }
        StartCoroutine(GenerateItems());
    }

    private IEnumerator GenerateItems()
    {
        // Same generate item as other bars
        Vector2 pos = FirstPos.transform.position;
        maximumWidth = BoxSize * (Models.Count) * 0.9f - ScrollRect.GetComponent<RectTransform>().sizeDelta.x;
        if (maximumWidth > 0f)
        {
            Content.GetComponent<RectTransform>().sizeDelta
                = new Vector2(maximumWidth,
                Content.GetComponent<RectTransform>().sizeDelta.y);
        }
        else
        {
            Content.GetComponent<RectTransform>().sizeDelta
                = new Vector2(Content.GetComponent<RectTransform>().sizeDelta.x,
                Content.GetComponent<RectTransform>().sizeDelta.y);
        }
        for (int i=0;i<Models.Count;i++)
        {
            GameObject go = Instantiate(Template, new Vector3(pos.x, pos.y, Template.transform.position.z)
                , Quaternion.identity);
            go.transform.SetParent(Content.transform);
            go.transform.GetChild(0).GetComponent<Image>().sprite = Models[i].GetComponent<SpriteRenderer>().sprite;
            go.transform.localScale = Template.transform.localScale;
            go.name = Models[i].name;
            go.GetComponent<LoadOutConsumableBox>().SetStackText(ListIconCount[i]);
            IconAfterGen.Add(go);
            for (int j=0;j<ChosenName.Count;j++)
            {
                if (go.name.Replace(" ", "").ToLower().Equals(ChosenName[j].Replace(" ","").ToLower())) {
                    go.transform.GetComponent<Image>().color = Color.green;
                    ChosenGO.Add(go);
                    go.GetComponent<LoadOutConsumableBox>().SetChosenAmount(ListIconSelectedCount[j]);
                    break;
                }
            }
            go.SetActive(true);
            pos = new Vector2(pos.x + BoxSize, pos.y);
            yield return new WaitForSeconds(0.05f);
        }
        ScrollRect.GetComponent<ScrollRect>().horizontal = true;
        if (Details.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite == null)
        ClickText.SetActive(true);
    }

    public void ShowClickItem(GameObject item)
    {
        if (ClickText.activeSelf) ClickText.SetActive(false);
        CurrentClickItem = item;
        // Set Data To Detail
        if (!Details.activeSelf) Details.SetActive(true);
        Details.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = item.transform.GetChild(0).GetComponent<Image>().sprite;
        Dictionary<string, object> data = FindObjectOfType<AccessDatabase>().GetConsumableDataByName(item.name);
        Details.transform.GetChild(1).GetComponent<TextMeshPro>().text = "<color=" + (string)data["Color"]
            + "><b>" + (string)data["Name"] + "</b></color>";
        Details.transform.GetChild(2).GetComponent<TextMeshPro>().text = (string)data["Description"] + "\n"
            + FindObjectOfType<GlobalFunctionController>().ConvertEffectAndDurationOfConsumables((string)data["Effect"], (int)data["Duration"]) + "\n"
            + "Max Stacks: " + (int)data["Stack"] +" per session.";
    }
    public void CheckIncreaseClickItem(GameObject item)
    {
        // Set Item to main
        int k = ConsumeMain.GetComponent<LoadOutConsumables>().IncreaseItem(item.name);
        if (k > 0)
        {
            if (!ChosenGO.Contains(item)) ChosenGO.Add(item);
            if (!ChosenName.Contains(item.name)) ChosenName.Add(item.name);
            item.GetComponent<LoadOutConsumableBox>().SetChosenAmount(k);
        } else
        {
            // You cannot add this item
        }
        // Change item UI
        foreach (var icons in IconAfterGen)
        {
            icons.GetComponent<Image>().color = Color.white;
        }
        foreach (var go in ChosenGO)
        {
            go.GetComponent<Image>().color = Color.green;
        }
    }

    public void CheckDecreaseClickItem(GameObject item)
    {
        // Set Item to main
        int k = ConsumeMain.GetComponent<LoadOutConsumables>().DecreaseItem(item.name);
        if (k > 0)
        {
            if (!ChosenGO.Contains(item)) ChosenGO.Add(item);
            if (!ChosenName.Contains(item.name)) ChosenName.Add(item.name);
            item.GetComponent<LoadOutConsumableBox>().SetChosenAmount(k);
        } else
        {
            if (ChosenGO.Contains(item)) ChosenGO.Remove(item);
            if (ChosenName.Contains(item.name)) ChosenName.Remove(item.name);
            item.GetComponent<LoadOutConsumableBox>().SetChosenAmount(k);
        }
        // Change item UI
        foreach (var icons in IconAfterGen)
        {
            icons.GetComponent<Image>().color = Color.white;
        }
        foreach (var go in ChosenGO)
        {
            go.GetComponent<Image>().color = Color.green;
        }
    }
    #endregion
}
