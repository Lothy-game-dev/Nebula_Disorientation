using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LOTWAllOwnedPopup : MonoBehaviour
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
    public GameObject FirstPos;
    public GameObject TemplateCard;
    public GameObject Content;
    public GameObject ScrollRect;
    public GameObject NoCardText;
    #endregion
    #region NormalVariables
    private List<Dictionary<string, object>> ListDataAllCard;
    private List<GameObject> CardList;
    private float CardWidth;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void OnEnable()
    {
        // Initialize variables
        ListDataAllCard = new List<Dictionary<string, object>>();
        NoCardText.SetActive(false);
        GetAllCardsData();
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Set Data
    // Group all function that serve the same algorithm
    public void GetAllCardsData()
    {
        ListDataAllCard = FindObjectOfType<AccessDatabase>().GetLOTWInfoOwnedByID(PlayerPrefs.GetInt("PlayerID"));
    }

    public void Open()
    {
        CardWidth = Mathf.Abs(Left.transform.position.x - Right.transform.position.x);
        ScrollRect.GetComponent<ScrollRect>().vertical = false;
        CardList = new List<GameObject>();
        StartCoroutine(GenerateAllCards());
    }

    public void Close()
    {
        int n = 0;
        while (n < CardList.Count)
        {
            if (CardList[n] != null)
            {
                GameObject temp = CardList[n];
                CardList.RemoveAt(n);
                Destroy(temp);
            }
            else
            {
                n++;
            }
        }
        gameObject.SetActive(false);
    }

    private IEnumerator GenerateAllCards()
    {
        if (ListDataAllCard.Count==0)
        {
            NoCardText.SetActive(true);
        }
        Vector2 Pos = new Vector2(FirstPos.transform.position.x, FirstPos.transform.position.y);
        for (int i = 0; i < ListDataAllCard.Count; i++)
        {
            Dictionary<string, object> dataDict = ListDataAllCard[i];
            if ((int)dataDict["ID"]==34)
            {
                continue;
            }
            GameObject Card = Instantiate(TemplateCard, new Vector3(Pos.x, Pos.y, TemplateCard.transform.position.z), Quaternion.identity);
            // Info
            Card.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = (string)dataDict["Name"];
            Color TierColor;
            ColorUtility.TryParseHtmlString((string)dataDict["Color"], out TierColor);
            Card.transform.GetChild(0).GetComponent<Image>().color = TierColor;
            Color c = Card.transform.GetChild(0).GetComponent<Image>().color;
            c.a = 30 / 255f;
            Card.transform.GetChild(0).GetComponent<Image>().color = c;
            Card.transform.GetChild(0).GetChild(1).GetComponent<Image>().color = TierColor;
            Card.transform.GetChild(0).GetChild(2).GetComponent<Image>().color = TierColor;
            Card.transform.GetChild(0).GetChild(3).GetComponent<Image>().color = TierColor;
            Card.transform.GetChild(0).GetChild(4).GetComponent<Image>().color = TierColor;
            Card.transform.GetChild(0).GetChild(5).GetComponent<TextMeshProUGUI>().text =
                FindObjectOfType<GlobalFunctionController>().ConvertEffectStringToText((string)dataDict["Effect"]);
            Card.transform.GetChild(0).GetChild(5).GetComponent<TextMeshProUGUI>().color = TierColor;
            Card.transform.GetChild(1).GetComponent<Image>().color = TierColor;
            Color c2 = Card.transform.GetChild(1).GetComponent<Image>().color;
            c2.a = 30 / 255f;
            Card.transform.GetChild(1).GetComponent<Image>().color = c2;
            Card.transform.GetChild(1).GetChild(1).GetComponent<Image>().color = TierColor;
            Card.transform.GetChild(1).GetChild(2).GetComponent<Image>().color = TierColor;
            Card.transform.GetChild(1).GetChild(3).GetComponent<Image>().color = TierColor;
            Card.transform.GetChild(1).GetChild(4).GetComponent<Image>().color = TierColor;
            Card.transform.GetChild(2).GetComponent<Image>().color = TierColor;
            Color c3 = Card.transform.GetChild(2).GetComponent<Image>().color;
            c3.a = 30 / 255f;
            Card.transform.GetChild(2).GetComponent<Image>().color = c3;
            Card.transform.GetChild(2).GetChild(1).GetComponent<Image>().color = TierColor;
            Card.transform.GetChild(2).GetChild(2).GetComponent<Image>().color = TierColor;
            Card.transform.GetChild(2).GetChild(3).GetComponent<Image>().color = TierColor;
            Card.transform.GetChild(2).GetChild(4).GetComponent<Image>().color = TierColor;
            if ((int)dataDict["Duration"] > 0 && (int)dataDict["Duration"] < 1000)
            {
                Card.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = ((int)dataDict["Duration"]).ToString();
            }
            else if ((int)dataDict["Duration"] == 1000)
            {
                Card.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Infinite";
            }
            else
            {
                Card.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "-";
            }
            if ((string)dataDict["Stackable"]=="N")
            {
                Card.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "-";
            } else
            {
                Card.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = ((int)dataDict["Stack"]).ToString();
            }
            Card.transform.SetParent(Content.transform);
            Card.transform.localScale = TemplateCard.transform.localScale;
            Card.SetActive(true);
            CardList.Add(Card);
            yield return new WaitForSeconds(0.01f);
        }
        ScrollRect.GetComponent<ScrollRect>().vertical = true;
    }
    #endregion
}
