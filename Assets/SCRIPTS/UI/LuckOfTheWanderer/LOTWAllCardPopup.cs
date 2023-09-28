using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LOTWAllCardPopup : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject TierCard;
    public GameObject Icon;
    public GameObject Left;
    public GameObject Right;
    public GameObject UpFirstPos;
    public GameObject DownFirstPos;
    public GameObject TemplateCard;
    public GameObject Content;
    public GameObject ScrollRect;
    #endregion
    #region NormalVariables
    private List<Dictionary<string, object>> ListDataAllCard;
    private List<GameObject> CardList;
    private float CardWidth;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Awake()
    {
        // Initialize variables
        ListDataAllCard = new List<Dictionary<string, object>>();
        GetAllCardsData();
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Get All Cards Data
    private void GetAllCardsData()
    {
        List<int> list = FindObjectOfType<AccessDatabase>().GetListIDAllLOTW(0);
        foreach (var n in list)
        {
            ListDataAllCard.Add(FindObjectOfType<AccessDatabase>().GetLOTWInfoByID(n));
        }
    }

    public void Open()
    {
        CardWidth = Mathf.Abs(Left.transform.position.x - Right.transform.position.x);
        ScrollRect.GetComponent<ScrollRect>().horizontal = false;
        CardList = new List<GameObject>();
        StartCoroutine(GenerateAllCards());
    }

    public void Close()
    {
        Content.GetComponent<RectTransform>().sizeDelta =
            new Vector2(0,
            Content.GetComponent<RectTransform>().sizeDelta.y);
        int n = 0;
        while (n<CardList.Count)
        {
            if (CardList[n]!=null)
            {
                GameObject temp = CardList[n];
                CardList.RemoveAt(n);
                Destroy(temp);
            } else
            {
                n++;
            }
        }
        gameObject.SetActive(false);
    }

    private IEnumerator GenerateAllCards()
    {
        Content.GetComponent<RectTransform>().sizeDelta = 
            new Vector2(25.7249f,
            Content.GetComponent<RectTransform>().sizeDelta.y);
        Vector2 UpPos = new Vector2(UpFirstPos.transform.position.x, UpFirstPos.transform.position.y);
        Vector2 DownPos = new Vector2(DownFirstPos.transform.position.x, DownFirstPos.transform.position.y);
        bool isUp = true;
        for (int i=0;i<ListDataAllCard.Count;i++)
        {
            Dictionary<string, object> dataDict = ListDataAllCard[i];
            if (isUp)
            {
                isUp = false;
                GameObject Card = Instantiate(TemplateCard, new Vector3(UpPos.x, UpPos.y, TemplateCard.transform.position.z), Quaternion.identity);
                // Info
                Card.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = (string)dataDict["Name"];
                Color TierColor;
                ColorUtility.TryParseHtmlString((string)dataDict["Color"], out TierColor);
                Card.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = TierColor;
                Card.GetComponent<Image>().sprite = TierCard.transform.GetChild(
                    3 - (int)dataDict["Tier"]).GetComponent<SpriteRenderer>().sprite;
                Card.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = FindObjectOfType<GlobalFunctionController>().ConvertEffectStringToText((string)dataDict["Effect"]);
                if ((string)dataDict["Type"] == "OFF")
                {
                    Card.transform.GetChild(0).GetComponent<Image>().sprite = Icon.transform.GetChild(0).GetChild(3 - (int)dataDict["Tier"]).GetComponent<SpriteRenderer>().sprite;
                }
                else if ((string)dataDict["Type"] == "DEF")
                {
                    Card.transform.GetChild(0).GetComponent<Image>().sprite = Icon.transform.GetChild(1).GetChild(3 - (int)dataDict["Tier"]).GetComponent<SpriteRenderer>().sprite;
                }
                else if ((string)dataDict["Type"] == "SPE")
                {
                    Card.transform.GetChild(0).GetComponent<Image>().sprite = Icon.transform.GetChild(2).GetChild(3 - (int)dataDict["Tier"]).GetComponent<SpriteRenderer>().sprite;
                }
                if ((int)dataDict["Duration"]>0)
                {
                    Card.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "Duration:\n" + (int)dataDict["Duration"] + " Stages";
                } else if ((int)dataDict["Duration"] == -1)
                {
                    Card.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "Duration:\nInfinite";
                } else
                {
                    Card.transform.GetChild(3).gameObject.SetActive(false);
                }
                UpPos = new Vector2(UpPos.x + CardWidth, UpPos.y);
                Card.transform.SetParent(Content.transform);
                Card.transform.localScale = TemplateCard.transform.localScale;
                Card.SetActive(true);
                CardList.Add(Card);
            } else
            {
                isUp = true; 
                GameObject Card = Instantiate(TemplateCard, new Vector3(DownPos.x, DownPos.y, TemplateCard.transform.position.z), Quaternion.identity);
                // Info
                Card.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = (string)dataDict["Name"];
                Color TierColor;
                ColorUtility.TryParseHtmlString((string)dataDict["Color"], out TierColor);
                Card.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = TierColor;
                Card.GetComponent<Image>().sprite = TierCard.transform.GetChild(
                    3 - (int)dataDict["Tier"]).GetComponent<SpriteRenderer>().sprite;
                Card.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = FindObjectOfType<GlobalFunctionController>().ConvertEffectStringToText((string)dataDict["Effect"]);
                if ((string)dataDict["Type"] == "OFF")
                {
                    Card.transform.GetChild(0).GetComponent<Image>().sprite = Icon.transform.GetChild(0).GetChild(3 - (int)dataDict["Tier"]).GetComponent<SpriteRenderer>().sprite;
                }
                else if ((string)dataDict["Type"] == "DEF")
                {
                    Card.transform.GetChild(0).GetComponent<Image>().sprite = Icon.transform.GetChild(1).GetChild(3 - (int)dataDict["Tier"]).GetComponent<SpriteRenderer>().sprite;
                }
                else if ((string)dataDict["Type"] == "SPE")
                {
                    Card.transform.GetChild(0).GetComponent<Image>().sprite = Icon.transform.GetChild(2).GetChild(3 - (int)dataDict["Tier"]).GetComponent<SpriteRenderer>().sprite;
                }
                if ((int)dataDict["Duration"] > 0)
                {
                    Card.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "Duration:\n" + (int)dataDict["Duration"] + " Stages";
                }
                else if ((int)dataDict["Duration"] == -1)
                {
                    Card.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "Duration:\nInfinitite";
                }
                else
                {
                    Card.transform.GetChild(3).gameObject.SetActive(false);
                }
                DownPos = new Vector2(DownPos.x + CardWidth, DownPos.y);
                Card.transform.SetParent(Content.transform);
                Card.transform.localScale = TemplateCard.transform.localScale;
                Card.SetActive(true);
                CardList.Add(Card);
            }
            yield return new WaitForSeconds(0.01f);
        }
        ScrollRect.GetComponent<ScrollRect>().horizontal = true;
    }
    #endregion
}
