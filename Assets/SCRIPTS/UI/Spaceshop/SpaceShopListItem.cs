using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpaceShopListItem : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject Scene;
    public GameObject Template;
    public GameObject ConsumableList;
    public GameObject Top;
    public GameObject Bottom;
    public GameObject FirstPos;
    public GameObject Content;
    public TMP_InputField SearchInput;
    public GameObject Loading;
    public GameObject Scroll;
    public GameObject Information;
    #endregion
    #region NormalVariables
    private List<GameObject> SpriteList;
    public List<GameObject> ItemAfterGen;
    private List<string> ListName;
    private float BoxSize;
    private string TempText;
    private bool ContinueGenerating;
    private float GenerateTimer;
    private bool alreadyGen;
    private bool AlreadyClear;
    private float maximumHeight;
    private float InitPosYContent;
    private bool AlreadyGetPosYContent;
    private GameObject CurrentChosenGO;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void OnEnable()
    {
        // Initialize variables
        SpriteList = new List<GameObject>();
        TempText = "";
        ItemAfterGen = new List<GameObject>();
        AlreadyGetPosYContent = false;
        BoxSize = Mathf.Abs(Top.transform.position.y - Bottom.transform.position.y);
        SetItem(FindObjectOfType<AccessDatabase>().GetSpaceShopItemNameSearchByName(""));
        alreadyGen = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        if (SearchInput.isFocused)
        {
            if (!TempText.Equals(SearchInput.text))
            {
                GenerateTimer = 0.75f;
                Scroll.GetComponent<ScrollRect>().vertical = false;
                AlreadyClear = false;
                alreadyGen = false;
                Loading.SetActive(false);
            }
            TempText = SearchInput.text;
        }
        if (GenerateTimer > 0f)
        {
            GenerateTimer -= Time.deltaTime;
            if (!AlreadyClear)
            {
                AlreadyClear = true;
                RemoveAllItem();
                Loading.SetActive(true);
                Loading.transform.GetChild(0).GetComponent<SpaceShopLoading>().LoadTime = 0.75f;
            }
        }
        else
        {
            if (!alreadyGen)
            {
                Loading.SetActive(false);
                alreadyGen = true;
                if (AlreadyGetPosYContent)
                {
                    Content.transform.position = new Vector3(Content.transform.position.x,
                        InitPosYContent, Content.transform.position.z);
                }
                SetItem(FindObjectOfType<AccessDatabase>().GetSpaceShopItemNameSearchByName(SearchInput.text));
            }
        }
    }
    #endregion
    #region Set Item
    // Same as bars in Load out
    public void SetItem(List<string> Item)
    {
        Scroll.GetComponent<ScrollRect>().vertical = false;
        SpriteList.Clear();
        ListName = Item;
        RemoveAllItem();
        for (int i=0;i<Item.Count;i++)
        {
            for (int j = 0; j < ConsumableList.transform.childCount; j++)
            {
                if (ConsumableList.transform.GetChild(j).name.Replace(" ", "").Replace("-", "").ToLower()
                    .Equals(Item[i].Replace(" ", "").Replace("-", "").ToLower())) 
                {
                    SpriteList.Add(ConsumableList.transform.GetChild(j).gameObject);
                    break;
                }
            }
        }
        ContinueGenerating = true;
        StartCoroutine(GenerateItems());
    }

    // Stop generating and destroy all items
    private void RemoveAllItem()
    {
        ContinueGenerating = false;
        StopCoroutine(GenerateItems());
        int m = 0;
        while (m < ItemAfterGen.Count)
        {
            if (ItemAfterGen[m] != null)
            {
                GameObject temp = ItemAfterGen[m];
                ItemAfterGen.RemoveAt(m);
                Destroy(temp);
            }
            else
            {
                m++;
            }
        }
    }

    private IEnumerator GenerateItems()
    {
        // Generate Items: Same as other bars
        Vector2 pos = FirstPos.transform.position;
        maximumHeight = BoxSize * SpriteList.Count * 0.78f;
        Content.GetComponent<RectTransform>().sizeDelta
            = new Vector2(Content.GetComponent<RectTransform>().sizeDelta.x,
            maximumHeight);
        for (int i=0;i<SpriteList.Count;i++)
        {
            if (ContinueGenerating)
            {
                GameObject go = Instantiate(Template, new Vector3(pos.x, pos.y, Template.transform.position.z), Quaternion.identity);
                ItemAfterGen.Add(go);
                go.transform.SetParent(Content.transform);
                go.transform.GetChild(0).GetComponent<Image>().sprite = SpriteList[i].GetComponent<SpriteRenderer>().sprite;
                go.name = SpriteList[i].name;
                go.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = ListName[i];
                go.transform.localScale = Template.transform.localScale;
                if (i == 0)
                {
                    go.GetComponent<Image>().color = Color.green;
                    ChooseItem(go);
                }
                int n = FindObjectOfType<AccessDatabase>().GetStocksPerDayOfConsumable(go.name);
                if (n!=-1)
                {
                    go.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = n.ToString();
                }
                if (n==0)
                {
                    // Black fade
                    Debug.Log("Black Fade For " + go.name);
                }
                go.SetActive(true);
                pos = new Vector2(pos.x, pos.y - BoxSize);
                yield return new WaitForSeconds(0.05f);
            } else
            {
                break;
            }
        }
        Scroll.GetComponent<ScrollRect>().vertical = true;
        if (!AlreadyGetPosYContent)
        {
            AlreadyGetPosYContent = true;
            InitPosYContent = Content.transform.position.y;
        }
    }

    public void ChooseItem(GameObject go)
    {
        // When choose an item, all other change color to normal, the chosen change to green
        CurrentChosenGO = go;
        if (ItemAfterGen.Contains(go))
        {
            foreach (var item in ItemAfterGen)
            {
                item.GetComponent<Image>().color = new Color(1,1,1,205/255f);
            }
            go.GetComponent<Image>().color = Color.green;
            Information.GetComponent<SpaceShopInformation>().ShowInformationOfItem(go.name);
        } else
        {
            FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Scene.transform.position,
                "Cannot fetch data for this item! Please contact to our email or try again!", 5f);
        }
    }

    public void UpdateItemStocks()
    {
        if (CurrentChosenGO!=null)
        {
            int n = FindObjectOfType<AccessDatabase>().GetStocksPerDayOfConsumable(CurrentChosenGO.name);
            if (n != -1)
            {
                CurrentChosenGO.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = n.ToString();
            }
        }
    }

    // When disabled, remove all items
    private void OnDisable()
    {
        RemoveAllItem();
        if (AlreadyGetPosYContent)
        {
            Content.transform.position = new Vector3(Content.transform.position.x,
                InitPosYContent, Content.transform.position.z);
        }
    }
    #endregion
}
