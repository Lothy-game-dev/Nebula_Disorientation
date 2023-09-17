using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadOutPowerPopUp : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject Scene;
    public GameObject bar;
    public List<GameObject> DisabledCollider;
    public GameObject PowerList;
    public GameObject Content;
    public GameObject Left;
    public GameObject Right;
    public GameObject FirstGenPos;
    public GameObject Template;
    public GameObject Background;
    public GameObject PopUpTopBorder;
    public GameObject PopUpBottomBorder;
    public GameObject PopUpLeftBorder;
    public GameObject PopUpRightBorder;
    public GameObject ScrollRect;
    #endregion
    #region NormalVariables
    public GameObject CurrentChosenPower;
    public string currentChosenName;
    private float maximumWidth;
    public List<GameObject> ListPowerIcon;
    private List<GameObject> PowerIconAfterGenerate;
    private float BoxWidth;
    private float InitScaleX;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void OnEnable()
    {
        // Initialize variables
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        if (!isMouseOutsideOfRange())
        {
            Background.GetComponent<LoadOutBarBackground>().DisableCollider *= 0;
        }
    }
    #endregion
    #region Animation
    public void OpenPopUp(List<string> ListPower, string chosenPower)
    {
        // Open popup
        // Calculate Width of each box
        BoxWidth = Mathf.Abs(Left.transform.position.x - Right.transform.position.x);
        // Clear old list power
        if (ListPowerIcon!=null)
        {
            ListPowerIcon.Clear();
        } else
        ListPowerIcon = new List<GameObject>();
        if (PowerIconAfterGenerate != null)
        {
            PowerIconAfterGenerate.Clear();
        }
        else PowerIconAfterGenerate = new List<GameObject>();
        // Set Transparency at start
        Color c = GetComponent<SpriteRenderer>().color;
        c.a = 0;
        GetComponent<SpriteRenderer>().color = c;
        // Disable collider in the bakcground
        foreach (var col in DisabledCollider)
        {
            if (col.GetComponent<Collider2D>()!=null)
            {
                col.GetComponent<Collider2D>().enabled = false;
            }
        }
        InitScaleX = transform.localScale.x;
        transform.localScale = new Vector2(InitScaleX / 20f, transform.localScale.y);
        gameObject.SetActive(true);
        StartCoroutine(StartAnimation(ListPower, chosenPower));
    }

    private IEnumerator StartAnimation(List<string> ListPower, string chosenPower)
    {
        // Increase collider and scale by time
        for (int i=0;i<19;i++)
        {
            Color c = GetComponent<SpriteRenderer>().color;
            c.a += (120/255f) / 19;
            GetComponent<SpriteRenderer>().color = c;
            transform.localScale = new Vector2(transform.localScale.x + InitScaleX / 20f, transform.localScale.y);
            yield return new WaitForSeconds(0.025f);
        }
        Background.SetActive(true);
        Background.GetComponent<SpriteRenderer>().sortingOrder = 18;
        // Disable scroll until data are loaded
        ScrollRect.GetComponent<ScrollRect>().horizontal = false;
        SetData(ListPower, chosenPower);
    }

    public void ClosePopUp()
    {
        // Remove all items
        int i = 0;
        while (i<PowerIconAfterGenerate.Count)
        {
            if (PowerIconAfterGenerate[i]!=null)
            {
                GameObject temp = PowerIconAfterGenerate[i];
                PowerIconAfterGenerate.RemoveAt(i);
                Destroy(temp);
            } else
            {
                i++;
            }
        }
        // Set content size to 0
        Content.GetComponent<RectTransform>().sizeDelta
            = new Vector2(0,0);
        StartCoroutine(CloseAnimation());
    }

    private IEnumerator CloseAnimation()
    {
        for (int i = 0; i < 19; i++)
        {
            Color c = GetComponent<SpriteRenderer>().color;
            c.a -= (120 / 255f) / 19;
            GetComponent<SpriteRenderer>().color = c;
            transform.localScale = new Vector2(transform.localScale.x - InitScaleX / 20f, transform.localScale.y);
            yield return new WaitForSeconds(0.025f);
        }
        Background.SetActive(false);
        foreach (var col in DisabledCollider)
        {
            if (col.GetComponent<Collider2D>() != null)
            {
                col.GetComponent<Collider2D>().enabled = true;
            }
        }
        bar.GetComponent<LoadOutPowerBar>().SetItem(currentChosenName);
        bar.GetComponent<LoadOutPowerBar>().AddDataFinal();
        gameObject.SetActive(false);
    }
    #endregion
    #region Set Data
    public void SetData(List<string> ListPower, string chosenPower)
    {
        // Destroy all datas
        if (chosenPower!="")
        {
            if (!ListPower.Contains(chosenPower))
            {
                FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Scene.transform.position,
               "Cannot fetch data for this power!\nPlease try again!", 5f);
                return;
            }
        }
        // Check if list data has sprite in list sprite
        for (int i=0;i<ListPower.Count;i++)
        {
            for (int j=0;j<PowerList.transform.childCount;j++)
            {
                if (PowerList.transform.GetChild(j).name.Replace(" ","").ToLower()
                    .Equals(ListPower[i].Replace(" ","").ToLower()))
                {
                    ListPowerIcon.Add(PowerList.transform.GetChild(j).gameObject);
                }
            }
        }
        if (chosenPower!="")
        {
            for (int i = 0; i < ListPowerIcon.Count; i++)
            {
                if (ListPowerIcon[i].name.Replace(" ", "").ToLower()
                    .Equals(chosenPower.Replace(" ", "").ToLower()))
                {
                    CurrentChosenPower = ListPowerIcon[i];
                    currentChosenName = ListPowerIcon[i].name;
                }
            }
        } else
        {
            CurrentChosenPower = null;
            currentChosenName = "";
        }
        StartCoroutine(GenerateIcons());
    }

    private IEnumerator GenerateIcons()
    {
        Vector2 pos = FirstGenPos.transform.position;
        maximumWidth = BoxWidth * (ListPowerIcon.Count) * 0.9f - ScrollRect.GetComponent<RectTransform>().sizeDelta.x;
        if (maximumWidth>0f)
        {
            Content.GetComponent<RectTransform>().sizeDelta
                = new Vector2(maximumWidth,
                Content.GetComponent<RectTransform>().sizeDelta.y);
        } else
        {
            Content.GetComponent<RectTransform>().sizeDelta
                = new Vector2(Content.GetComponent<RectTransform>().sizeDelta.x,
                Content.GetComponent<RectTransform>().sizeDelta.y);
        }
        for (int i=0;i<ListPowerIcon.Count;i++)
        {
            GameObject go = Instantiate(Template,
                new Vector3(pos.x, pos.y, Template.transform.position.z), Quaternion.identity);
            go.transform.SetParent(Content.transform);
            go.transform.GetChild(0).GetComponent<Image>().sprite = ListPowerIcon[i].GetComponent<SpriteRenderer>().sprite;
            go.transform.localScale = Template.transform.localScale;
            go.name = ListPowerIcon[i].name;
            if (go.name.Replace(" ", "").ToLower().Equals(currentChosenName.Replace(" ", "").ToLower()))
            {
                go.GetComponent<Image>().color = Color.green;
                if (go.GetComponent<LoadOutPowerPopUpBox>() != null)
                {
                    go.GetComponent<LoadOutPowerPopUpBox>().isSelected = true;
                }
            }
            go.SetActive(true);
            PowerIconAfterGenerate.Add(go);
            pos = new Vector2(pos.x + BoxWidth, pos.y);
            yield return new WaitForSeconds(0.1f);
        }
        ScrollRect.GetComponent<ScrollRect>().horizontal = true;
    }
    #endregion
    #region Check Mouse
    private bool isMouseOutsideOfRange()
    {
        if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x > PopUpRightBorder.transform.position.x ||
            Camera.main.ScreenToWorldPoint(Input.mousePosition).x < PopUpLeftBorder.transform.position.x ||
            Camera.main.ScreenToWorldPoint(Input.mousePosition).y > PopUpTopBorder.transform.position.y ||
            Camera.main.ScreenToWorldPoint(Input.mousePosition).y < PopUpBottomBorder.transform.position.y)
            return true;
        return false;
    }

    public void ChooseItem(GameObject item)
    {
        CurrentChosenPower = item;
        currentChosenName = item.name;
        foreach (var go in PowerIconAfterGenerate)
        {
            go.GetComponent<Image>().color = Color.white;
        }
        CurrentChosenPower.GetComponent<Image>().color = Color.green;
    }

    public void RemoveItem()
    {
        CurrentChosenPower = null;
        currentChosenName = "";
        foreach (var go in PowerIconAfterGenerate)
        {
            go.GetComponent<Image>().color = Color.white;
        }
    }
    #endregion
}
