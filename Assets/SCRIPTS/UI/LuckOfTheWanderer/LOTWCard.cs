using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LOTWCard : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject Scene;
    public GameObject CardFontList;
    public GameObject CardIconList;
    public GameObject CardBack;
    public GameObject IconPos;
    public GameObject Effect;
    public GameObject Name;
    public GameObject RedEffect;
    public GameObject BlueEffect;
    public GameObject Duration;
    #endregion
    #region NormalVariables
    public List<GameObject> OtherCards;
    public GameObject ToPos;
    public GameObject CenterPos;
    public bool alreadyShowCard;
    public bool NoRed;

    private int CardTier;
    private int CardId;
    private string CardType;
    private Dictionary<string, object> DataDictionary;
    private float InitScaleX;
    private float InitScaleY;
    private float StopTimer;
    private GameObject Destination;
    private GameObject StartMovingPos;
    private GameObject Icon;
    private Color TierColor;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        InitScaleX = transform.localScale.x;
        InitScaleY = transform.localScale.y;
        StartCoroutine(MoveToPos());
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        if (StopTimer<=0f)
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        } else
        {
            CheckMoveToPos(Destination, StartMovingPos);
            StopTimer -= Time.deltaTime;
        }
        if (BlueEffect.activeSelf)
        {
            if (BlueEffect.transform.GetChild(0).GetChild(1).GetComponent<Image>().fillAmount < 1)
            {
                BlueEffect.transform.GetChild(0).GetChild(1).GetComponent<Image>().fillAmount += Time.deltaTime;
            } else
            {
                if (BlueEffect.transform.GetChild(0).GetChild(1).GetComponent<Image>().color.a > 0)
                {
                    Color c = BlueEffect.transform.GetChild(0).GetChild(1).GetComponent<Image>().color;
                    c.a -= 100 / 255f * Time.deltaTime;
                    BlueEffect.transform.GetChild(0).GetChild(1).GetComponent<Image>().color = c;
                } else
                {
                    BlueEffect.transform.GetChild(0).GetChild(1).GetComponent<Image>().fillAmount = 0;
                    Color c = BlueEffect.transform.GetChild(0).GetChild(1).GetComponent<Image>().color;
                    c.a = 100 / 255f;
                    BlueEffect.transform.GetChild(0).GetChild(1).GetComponent<Image>().color = c;
                }
            }

        }
    }
    #endregion
    #region SetData
    public void SetData(int CardID)
    {
        CardId = CardID;
        OtherCards = new List<GameObject>();
        DataDictionary = FindObjectOfType<AccessDatabase>().GetLOTWInfoByID(CardID);
        Name.GetComponent<TextMeshPro>().text = (string)DataDictionary["Name"];
        CardTier = (int)DataDictionary["Tier"];
        Effect.GetComponent<TextMeshPro>().text = FindObjectOfType<GlobalFunctionController>().ConvertEffectStringToText((string)DataDictionary["Effect"]);
        ColorUtility.TryParseHtmlString((string)DataDictionary["Color"], out TierColor);
        Name.GetComponent<TextMeshPro>().color = TierColor;
        GetComponent<SpriteRenderer>().sprite = CardFontList.transform.GetChild(
            3-(int)DataDictionary["Tier"]).GetComponent<SpriteRenderer>().sprite;
        if ((string)DataDictionary["Type"]=="OFF")
        {
            GameObject icon = CardIconList.transform.GetChild(0).GetChild(3 - (int)DataDictionary["Tier"]).gameObject;
            GameObject IconIns = Instantiate(icon, IconPos.transform.position, Quaternion.identity);
            IconIns.transform.SetParent(transform);
            Icon = IconIns;
            IconIns.SetActive(true);
        } 
        else if ((string)DataDictionary["Type"] == "DEF")
        {
            GameObject icon = CardIconList.transform.GetChild(1).GetChild(3 - (int)DataDictionary["Tier"]).gameObject;
            GameObject IconIns = Instantiate(icon, IconPos.transform.position, Quaternion.identity);
            IconIns.transform.SetParent(transform);
            Icon = IconIns;
            IconIns.SetActive(true);
        }
        else if ((string)DataDictionary["Type"] == "SPE")
        {
            GameObject icon = CardIconList.transform.GetChild(2).GetChild(3 - (int)DataDictionary["Tier"]).gameObject;
            GameObject IconIns = Instantiate(icon, IconPos.transform.position, Quaternion.identity);
            IconIns.transform.SetParent(transform);
            Icon = IconIns;
            IconIns.SetActive(true);
        }
        if ((int)DataDictionary["Duration"] > 0 && (int)DataDictionary["Duration"] < 1000)
        {
            Duration.GetComponent<TextMeshPro>().text = "Duration:\n" + (int)DataDictionary["Duration"] + " Stages";
        }
        else if ((int)DataDictionary["Duration"] == 1000)
        {
            Duration.GetComponent<TextMeshPro>().text = "Duration:\nInfinite";
        }
        else
        {
            Duration.SetActive(false);
        }
    }
    #endregion
    #region Mouse Check
    private void OnMouseDown()
    {
        Selected();
    }


    public void PickCard()
    {
        if (!alreadyShowCard)
        {
            alreadyShowCard = true;
            StartCoroutine(ShowCard());
        }
        else
        {
            foreach (var card in OtherCards)
            {
                card.GetComponent<Collider2D>().enabled = false;
            }
            StartCoroutine(SelectMovement());
        }
    }
    private IEnumerator ShowCard()
    {
        GetComponent<Collider2D>().enabled = false;
        foreach (var card in OtherCards)
        {
            card.GetComponent<Collider2D>().enabled = false;
            card.GetComponent<LOTWCard>().alreadyShowCard = true;
        }
        ShowEffect();
        for (int i=0;i<50;i++)
        {
            CardBack.GetComponent<Image>().fillAmount -= 1 / 50f;
            yield return new WaitForSeconds(1 / 50f);
        }
        foreach (var card in OtherCards)
        {
            card.GetComponent<LOTWCard>().ShowEffect();
        }
        for (int i = 0; i < 50; i++)
        {
            foreach (var card in OtherCards)
            {
                card.GetComponent<LOTWCard>().CardBack.GetComponent<Image>().fillAmount -= 1 / 50f;
            }
            yield return new WaitForSeconds(1 / 50f);
        }
        StartCoroutine(SelectMovement());
    }

    private void Selected()
    {
        GetComponent<Collider2D>().enabled = false;
        transform.localScale = new Vector3(InitScaleX * 1.1f, InitScaleY * 1.1f, transform.localScale.z);
        foreach (var card in OtherCards)
        {
            card.GetComponent<Collider2D>().enabled = true;
            card.transform.localScale = new Vector3(InitScaleX * 0.9f, InitScaleY * 0.9f, transform.localScale.z);
        }
        Scene.GetComponent<LOTWScene>().CardSelected(CardId, gameObject);
    }

    private IEnumerator SelectMovement()
    {
        GetComponent<Rigidbody2D>().velocity = (CenterPos.transform.position - transform.position) * 1.4f;
        Destination = CenterPos;
        StartMovingPos = gameObject;
        StopTimer = 1 / 1.4f;
        List<float> ScaleX = new List<float>();
        List<float> ScaleY = new List<float>();
        foreach (var card in OtherCards)
        {
            card.GetComponent<LOTWCard>().HideEffect();
            ScaleX.Add(card.transform.localScale.x);
            ScaleY.Add(card.transform.localScale.y);
        }
        for (int i = 0; i < 50; i++)
        {
            for (int k=0;k<OtherCards.Count;k++)
            {
                GameObject card = OtherCards[k];
                card.transform.localScale = new Vector2(card.transform.localScale.x - ScaleX[k] / 50,
                    card.transform.localScale.y - ScaleY[k] / 50);
            }
            yield return new WaitForSeconds(0.2f / 50f);
        }
        foreach (var card in OtherCards)
        {
            Destroy(card);
        }
        yield return new WaitForSeconds(0.5f);
        IconEffect();
    }

    private IEnumerator MoveToPos()
    {
        GetComponent<Rigidbody2D>().velocity = (ToPos.transform.position - transform.position)*1.4f;
        Destination = ToPos;
        StartMovingPos = gameObject;
        StopTimer = 1/1.4f;
        if (NoRed)
        {
            alreadyShowCard = true;
            yield return new WaitForSeconds(0.75f);
            for (int i = 0; i < 50; i++)
            {
                CardBack.GetComponent<Image>().fillAmount -= 1 / 50f;
                yield return new WaitForSeconds(0.5f / 50f);
            }
            ShowEffect();
        } else
        {
            yield return new WaitForSeconds(1f);
        }
        GetComponent<Collider2D>().enabled = true;
    }

    public void Regenerate()
    {
        GetComponent<Collider2D>().enabled = false;
        if (BlueEffect.activeSelf)
        {
            BlueEffect.SetActive(false);
        }
        if (RedEffect.activeSelf)
        {
            RedEffect.SetActive(false);
        }
        StartCoroutine(BackToCenter());
    }

    private IEnumerator BackToCenter()
    {
        transform.localScale = new Vector3(InitScaleX, InitScaleY, transform.localScale.z);
        for (int i = 0; i < 40; i++)
        {
            CardBack.GetComponent<Image>().fillAmount += 1 / 50f;
            yield return new WaitForSeconds(0.3f / 50f);
        }
        GetComponent<Rigidbody2D>().velocity = (CenterPos.transform.position - transform.position)*4f;
        Destination = CenterPos;
        StartMovingPos = gameObject;
        StopTimer = 1f/4f;
        for (int i = 0; i < 10; i++)
        {
            CardBack.GetComponent<Image>().fillAmount += 1 / 50f;
            yield return new WaitForSeconds(1f/ 50f);
        }
        yield return new WaitForSeconds(0.1f);
        StopTimer = 1f;
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 20f);
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    private void CheckMoveToPos(GameObject Destination, GameObject StartMovingPos)
    {
        if (Destination.transform.position.x > StartMovingPos.transform.position.x)
        {
            if (transform.position.x >= Destination.transform.position.x - 0.15f)
            {
                StopTimer = 0f;
                GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0f);
                transform.position = Destination.transform.position;
            }
        } else if (Destination.transform.position.x < StartMovingPos.transform.position.x)
        {
            if (transform.position.x <= Destination.transform.position.x + 0.15f)
            {
                StopTimer = 0f;
                GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0f);
                transform.position = Destination.transform.position;
            }
        }
    }

    public void ShowEffect()
    {
        if (CardTier == 1)
        {
            RedEffect.SetActive(true);
        }
        else if (CardTier == 2)
        {
            BlueEffect.SetActive(true);
        }
    }

    public void HideEffect()
    {
        if (CardTier == 1)
        {
            RedEffect.SetActive(false);
        }
        else if (CardTier == 2)
        {
            BlueEffect.SetActive(false);
        }
    }

    public void IconEffect()
    {
        StartCoroutine(IconEffectAnim(Instantiate(Icon,Icon.transform.position,Quaternion.identity)));
    }

    private IEnumerator IconEffectAnim(GameObject go)
    {
        float scaleX = go.transform.localScale.x;
        float scaleY = go.transform.localScale.y;
        go.GetComponent<SpriteRenderer>().color = TierColor;
        for (int i=0;i<=30;i++)
        {
            go.transform.localScale = new Vector2(go.transform.localScale.x + scaleX / 5,
                go.transform.localScale.y + scaleY / 5);
            Color c = go.GetComponent<SpriteRenderer>().color;
            c.a -= 1 / 30f;
            go.GetComponent<SpriteRenderer>().color = c;
            yield return new WaitForSeconds(0.5f / 30f);
        }
        Destroy(go);
    }
    #endregion
}
