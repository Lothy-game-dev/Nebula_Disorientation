using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadOutBar : MonoBehaviour
{
    #region ComponentVariables
    private Animator anim;
    #endregion
    #region InitializeVariables
    public GameObject Scene;
    public GameObject FighterDemo;
    public string Type;
    public GameObject[] DisableColliderList;
    public GameObject StatusBoard;
    public GameObject Background;
    public GameObject ListSprite;
    public GameObject Contents;
    public GameObject CurrentItemPos;
    public GameObject ScrollRect;
    public float LeftShredViewport;
    public float RightShredViewport;
    public GameObject ViewPort;
    public GameObject LeftCheckPos;
    public GameObject RightCheckPos;
    public GameObject ScrollLimitTop;
    public GameObject ScrollLimitBottom;
    public GameObject ScrollLimitLeft;
    public GameObject ScrollLimitRight;
    public string WeaponPosition;
    #endregion
    #region NormalVariables
    public GameObject CurrentItem;
    private int CurrentIndex;
    private List<string> ItemsFromStart;
    private string ChosenItemStart;
    public List<GameObject> ListItem;
    private List<string> ListName;
    private float DistanceBetweenEach;
    private List<GameObject> AfterGenerateList;
    private GameObject AfterGenerateCurrentItem;
    private bool AllowScroll;
    private float CurrentRightOfScroll;
    private bool isScrollingLeft;
    private int IsLeft;
    private GameObject TempForFourItem;
    private string CurrentItemName;
    private string CurrentItemNameFull;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void OnEnable()
    {
        // Initialize variables
        anim = GetComponent<Animator>();
        // Distance between each weapon
        DistanceBetweenEach = ListSprite.transform.GetChild(0).GetChild(0).GetChild(1).position.x
            - ListSprite.transform.GetChild(0).GetChild(0).GetChild(2).position.x;
        // Set view port to the box before animation
        ViewPort.GetComponent<RectTransform>().offsetMin = new Vector2(LeftShredViewport,0);
        ViewPort.GetComponent<RectTransform>().offsetMax = new Vector2(-RightShredViewport,0);
        Contents.GetComponent<RectTransform>().offsetMin = new Vector2(- LeftShredViewport, Contents.GetComponent<RectTransform>().offsetMin.y);
        Contents.GetComponent<RectTransform>().offsetMax = new Vector2(RightShredViewport, Contents.GetComponent<RectTransform>().offsetMax.y);
        ListItem = new List<GameObject>();
        AllowScroll = false;
        // Preset data
        AfterGenerateList = new List<GameObject>();
        List<string> ListWeapon = FindObjectOfType<AccessDatabase>().GetAllOwnedWeapon(FindObjectOfType<UECMainMenuController>().PlayerId);
        Debug.Log(FindObjectOfType<UECMainMenuController>().PlayerId);
        if (ListWeapon.Count>0)
        {
            SetItem(
            ListWeapon,
            ListWeapon[0],true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        // If mouse out side of scroll range: stop scroll movement
        // If mouse in side of scroll range: disable collider for background
        if (CheckIfMouseOutsideScrollRange())
        {
            if (AfterGenerateList.Count > 4)
            {
                ScrollRect.GetComponent<ScrollRect>().StopMovement();
                if (ScrollRect.GetComponent<ScrollRect>().enabled)
                {
                    ScrollRect.GetComponent<ScrollRect>().enabled = false;
                }
            }
        } else
        {
            if (!ScrollRect.GetComponent<ScrollRect>().enabled)
            {
                ScrollRect.GetComponent<ScrollRect>().enabled = true;
            }
            Background.GetComponent<LoadOutBarBackground>().DisableCollider *= 0;
        }
        // If scroll is allowed, check scroll
        if (AllowScroll)
        {
            CheckScroll();
        }
    }
    #endregion
    #region MouseCheck
    // Check on mouse down
    private void OnMouseDown()
    {
        SetItem(
            FindObjectOfType<AccessDatabase>().GetOwnedWeaponExceptForName(FindObjectOfType<UECMainMenuController>().PlayerId,
            WeaponPosition == "Left" ? Scene.GetComponent<LoadoutScene>().RightWeapon : Scene.GetComponent<LoadoutScene>().LeftWeapon),
            CurrentItemName==null || CurrentItemName == "" ? FindObjectOfType<AccessDatabase>().GetOwnedWeaponExceptForName(FindObjectOfType<UECMainMenuController>().PlayerId,
            WeaponPosition == "Left" ? Scene.GetComponent<LoadoutScene>().RightWeapon : Scene.GetComponent<LoadoutScene>().LeftWeapon)[0] : CurrentItemName, false);
        // Animation
        anim.ResetTrigger("End");
        anim.SetTrigger("Start");
        // Set colliders of both weapons disabled = false
        GetComponent<Collider2D>().enabled = false;
        foreach (var go in DisableColliderList)
        {
            go.GetComponent<Collider2D>().enabled = false;
        }
        // Set sorting order to check
        GetComponent<SpriteRenderer>().sortingOrder = 50;
        // Set Fighter Demo To Focus On Weapon
        FighterDemo.GetComponent<LoadOutFighterDemo>().FocusOnWeapon(Type == "LeftWeapon");
        // Open background
        Background.SetActive(true);
        // disable click on bg to exit 
        Background.GetComponent<Collider2D>().enabled = false;
        Background.GetComponent<LoadOutBarBackground>().enabled = false;
        // Set background sorting order
        Background.GetComponent<SpriteRenderer>().sortingOrder = 49;
        // Set viewport to after animation
        ViewPort.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
        ViewPort.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
        Contents.GetComponent<RectTransform>().offsetMin = new Vector2(Contents.GetComponent<RectTransform>().offsetMin.x + LeftShredViewport, Contents.GetComponent<RectTransform>().offsetMin.y);
        Contents.GetComponent<RectTransform>().offsetMax = new Vector2(Contents.GetComponent<RectTransform>().offsetMax.x - RightShredViewport, Contents.GetComponent<RectTransform>().offsetMax.y);
        // Show item
        StartCoroutine(ShowItem());
    }

    // Check if mouse outside of range
    private bool CheckIfMouseOutsideScrollRange()
    {
        if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x > ScrollLimitRight.transform.position.x ||
            Camera.main.ScreenToWorldPoint(Input.mousePosition).x < ScrollLimitLeft.transform.position.x ||
            Camera.main.ScreenToWorldPoint(Input.mousePosition).y > ScrollLimitTop.transform.position.y ||
            Camera.main.ScreenToWorldPoint(Input.mousePosition).y < ScrollLimitBottom.transform.position.y)
            return true;
        return false;
    }

    // When mouse down on background, it will call to this function
    public void BackgroundMouseDown()
    {
        // Set animation
        anim.SetTrigger("End");
        anim.ResetTrigger("Start");
        // Reset data
        /*ListItem = new List<GameObject>();*/
        AllowScroll = false;
        // Disable scroll
        ScrollRect.GetComponent<ScrollRect>().horizontal = false;
        // Stop focus on weapon on fighter demo
        FighterDemo.GetComponent<LoadOutFighterDemo>().StopFocusOnWeapon(Type == "LeftWeapon");
        // Set sorting order to start
        GetComponent<SpriteRenderer>().sortingOrder = 5;
        // Play animation for close
        StartCoroutine(CloseAnimation());
    }

    private IEnumerator CloseAnimation()
    {
        // Destroy and remove all objects in the list
        int i = 0;
        while (i<AfterGenerateList.Count)
        {
            if (AfterGenerateList[i] != CurrentItem)
            {
                GameObject temp = AfterGenerateList[i];
                AfterGenerateList.RemoveAt(i);
                Destroy(temp);
            } else
            {
                i++;
            }
        }
        // Set view port to before animation
        ViewPort.GetComponent<RectTransform>().offsetMin = new Vector2(LeftShredViewport, 0);
        ViewPort.GetComponent<RectTransform>().offsetMax = new Vector2(- RightShredViewport, 0);
        Contents.GetComponent<RectTransform>().offsetMin = new Vector2(- LeftShredViewport, Contents.GetComponent<RectTransform>().offsetMin.y);
        Contents.GetComponent<RectTransform>().offsetMax = new Vector2(RightShredViewport, Contents.GetComponent<RectTransform>().offsetMax.y);
        // Wait for animation
        yield return new WaitForSeconds(0.5f);
        // Close background
        Background.SetActive(false);
        // Destroy and remove current item
        GameObject temp2 = CurrentItem;
        CurrentItemName = CurrentItem.name.Replace("(Clone)", "").Replace(" ", "");
        Destroy(temp2,0.5f);
        AfterGenerateList.RemoveAt(0);
        // Set item back from start
        SetItem(ItemsFromStart, CurrentItemName, true);
        yield return new WaitForSeconds(1f);
        // Set collider to enabled
        GetComponent<Collider2D>().enabled = true;
        foreach (var go in DisableColliderList)
        {
            go.GetComponent<Collider2D>().enabled = true;
        }
    }

    #endregion
    #region Check Scroll
    // Check if scroll is left or right
    private void CheckScroll()
    {
        // if right transform of recttransform > right transform the frame before -> is right
        // else is left
        float check = Contents.GetComponent<RectTransform>().offsetMax.x;
        if (check > CurrentRightOfScroll)
        {
            isScrollingLeft = true;
        } else if (check < CurrentRightOfScroll)
        {
            isScrollingLeft = false;
        }
        // set current right for next frame
        CurrentRightOfScroll = check;
        // Check object movement to generate 
        CheckMoveObject();
    }
    private void CheckMoveObject()
    {
        // Pre check only. <=3 cannot scroll, ==4 will not allow scroll outside of limit
        if (AfterGenerateList.Count>4)
        {
            // If count => 4 and is scrolling to the left: If the first item reach left check position
            // Change the left item to the last item of the list
            if (isScrollingLeft)
            {
                if (AfterGenerateList[0].transform.position.x >= LeftCheckPos.transform.position.x)
                {
                    float PosX = AfterGenerateList[0].transform.position.x - DistanceBetweenEach;
                    GameObject go = Instantiate(AfterGenerateList[AfterGenerateList.Count - 1],
                        new Vector3(PosX, AfterGenerateList[AfterGenerateList.Count - 1].transform.position.y,
                        AfterGenerateList[AfterGenerateList.Count - 1].transform.position.z), Quaternion.identity);
                    go.transform.localScale = new Vector2(0.75f, 0.75f);
                    go.transform.SetParent(Contents.transform);
                    List<GameObject> TempList = new List<GameObject>();
                    TempList.Add(go);
                    GameObject temp = AfterGenerateList[AfterGenerateList.Count - 1];
                    if (CurrentItem==temp)
                    {
                        CurrentItem = go;
                    }
                    AfterGenerateList.RemoveAt(AfterGenerateList.Count - 1);
                    Destroy(temp);
                    TempList.AddRange(AfterGenerateList);
                    AfterGenerateList = TempList;
                }
            }
            // If count > 4 and is scrolling to the right: If the last item reach right check position
            // Change the last item to the first item of the list
            else
            {
                if (AfterGenerateList[AfterGenerateList.Count - 1].transform.position.x <= RightCheckPos.transform.position.x)
                {
                    float PosX = AfterGenerateList[AfterGenerateList.Count - 1].transform.position.x + DistanceBetweenEach;
                    GameObject go = Instantiate(AfterGenerateList[0],
                        new Vector3(PosX, AfterGenerateList[0].transform.position.y,
                        AfterGenerateList[0].transform.position.z), Quaternion.identity);
                    go.transform.localScale = new Vector2(0.75f, 0.75f);
                    go.transform.SetParent(Contents.transform);
                    GameObject temp = AfterGenerateList[0];
                    if (CurrentItem == temp)
                    {
                        CurrentItem = go;
                    }
                    AfterGenerateList.RemoveAt(0);
                    Destroy(temp);
                    AfterGenerateList.Add(go);
                }
            }
        } 
    }
    #endregion
    #region Check Item
    private IEnumerator ShowItem()
    {
        // Change the current item to green
        AfterGenerateCurrentItem.transform.GetChild(0).GetComponent<Image>().color = Color.green;
        // Set sorting order of the gameobject
        transform.GetChild(1).GetComponent<Canvas>().sortingOrder = 52;
        // Allow box to detect mouse
        AfterGenerateCurrentItem.transform.GetChild(0).GetComponent<LoadOutBox>().detectMouse = true;
        CurrentItem = AfterGenerateCurrentItem;
        // Wait for animation
        yield return new WaitForSeconds(0.5f);
        // Set active true for all items in list
        for (int i = 0; i < AfterGenerateList.Count; i++)
        {
            AfterGenerateList[i].SetActive(true);
            AfterGenerateList[i].transform.GetChild(0).GetComponent<LoadOutBox>().detectMouse = true;
        }
        yield return new WaitForSeconds(0.5f);
        // enable click in bg to exit
        Background.GetComponent<Collider2D>().enabled = true;
        Background.GetComponent<LoadOutBarBackground>().enabled = true;
    }
    public void SetItem(List<string> Items, string cItem, bool ShowStats)
    {
        cItem = cItem.Replace(" ", "");
        if (!ListReplaceSpace(Items).Contains(cItem))
        {
            FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(transform.parent.position,
                "Unable to fetch owned weapons.\nPlease try again!", 5f);
            return;
        }
        // Set items for next time click usage
        ItemsFromStart = ListReplaceSpace(Items);
        ListItem = new List<GameObject>();
        ListName = new List<string>();
        AllowScroll = false;
        // Preset data
        AfterGenerateList = new List<GameObject>();
        // Get all game objects to the list
        for (int i = 0; i < Items.Count; i++)
        {
            int j = 0;
            while (j < ListSprite.transform.childCount)
            {
                if (ListSprite.transform.GetChild(j).name.ToLower().Equals(Items[i].Replace(" ", "").ToLower()))
                {
                    ListItem.Add(ListSprite.transform.GetChild(j).gameObject);
                    ListName.Add(Items[i]);
                    break;
                }
                else
                {
                    j++;
                }
            }
        }
        // Current selected object
        for (int i = 0; i<ListItem.Count;i++)
        {
            if (ListItem[i].name.ToLower().Equals(cItem.ToLower()))
            {
                CurrentItem = ListItem[i];
                CurrentIndex = i;
                break;
            }
        }
        // Update current item to scene controller
        if (Type == "LeftWeapon")
        {
            Scene.GetComponent<LoadoutScene>().LeftWeapon = cItem;
            FighterDemo.GetComponent<LoadOutFighterDemo>().SetWeapon(true, cItem);
        }
        else if (Type == "RightWeapon")
        {
            Scene.GetComponent<LoadoutScene>().RightWeapon = cItem;
            FighterDemo.GetComponent<LoadOutFighterDemo>().SetWeapon(false, cItem);
        }
        if (ShowStats)
        {
            // Set item to status board
            StatusBoard.GetComponent<LoadOutStatusBoard>().SetData(cItem);
        }
        GenerateItems();
    }
    
    public void SetWeaponToFighterDemo(string item)
    {
        FindObjectOfType<LoadOutFighterDemo>().SetWeapon(Type=="LeftWeapon", item);
    }

    // Generate Items 
    private void GenerateItems()
    {
        // Clone the current item and set to the position
        GameObject Item = Instantiate(CurrentItem, new Vector3(CurrentItemPos.transform.position.x,
            CurrentItemPos.transform.position.y, CurrentItem.transform.position.z), Quaternion.identity);
        Item.SetActive(true);
        Item.transform.SetParent(Contents.transform);
        Item.transform.GetChild(0).GetComponent<LoadOutBox>().bar = gameObject;
        Item.transform.GetChild(0).GetComponent<LoadOutBox>().background = Background;
        Item.transform.GetChild(0).GetComponent<LoadOutBox>().WeaponName = CurrentItem.name.Replace("(Clone)", "");
        Item.transform.GetChild(0).GetComponent<LoadOutBox>().detectMouse = false;
        transform.GetChild(1).GetComponent<Canvas>().sortingOrder = 10;
        AfterGenerateCurrentItem = Item;
        // If list item <= 3 items: no need to scroll, just clone the rest to the bar
        if (ListItem.Count <= 3)
        {
            ScrollRect.GetComponent<ScrollRect>().horizontal = false;
            AllowScroll = false;
            AfterGenerateList.Add(Item);
            float currentPosX = Item.transform.position.x;
            for (int i = 0; i < ListItem.Count; i++)
            {
                if (i == CurrentIndex) continue;
                GameObject It = ListItem[i];
                currentPosX += DistanceBetweenEach;
                Vector3 position = new Vector3(
                    currentPosX,
                    CurrentItemPos.transform.position.y,
                    It.transform.position.z);
                GameObject ItemTemp = Instantiate(It, position, Quaternion.identity);
                AfterGenerateList.Add(ItemTemp);
                ItemTemp.transform.SetParent(Contents.transform);
                ItemTemp.transform.GetChild(0).GetComponent<LoadOutBox>().bar = gameObject;
                ItemTemp.transform.GetChild(0).GetComponent<LoadOutBox>().background = Background;
                ItemTemp.transform.GetChild(0).GetComponent<LoadOutBox>().WeaponName = ListName[i];
            }
        } 
        // If list >= 4 items: allow scroll.
        else
        {
            ScrollRect.GetComponent<ScrollRect>().horizontal = true;
            AllowScroll = true;
            if (ListItem.Count==4)
            {
                ScrollRect.GetComponent<ScrollRect>().movementType = UnityEngine.UI.ScrollRect.MovementType.Elastic;
            }
            // set current right to check scroll direction
            CurrentRightOfScroll = Contents.GetComponent<RectTransform>().offsetMax.x;
            // Case current item is not the first item of the list:
            // Generate the list: 0 [1] 2 3 ... n
            if (CurrentIndex > 0)
            {
                float currentPosX = Item.transform.position.x - DistanceBetweenEach;
                GameObject It2 = ListItem[CurrentIndex - 1];
                Vector3 position2 = new Vector3(
                    currentPosX,
                    CurrentItemPos.transform.position.y,
                    It2.transform.position.z);
                GameObject ItemTemp2 = Instantiate(It2, position2, Quaternion.identity);
                AfterGenerateList.Add(ItemTemp2);
                ItemTemp2.transform.SetParent(Contents.transform);
                ItemTemp2.transform.GetChild(0).GetComponent<LoadOutBox>().bar = gameObject;
                ItemTemp2.transform.GetChild(0).GetComponent<LoadOutBox>().background = Background;
                ItemTemp2.transform.GetChild(0).GetComponent<LoadOutBox>().WeaponName = ListName[CurrentIndex - 1];
                AfterGenerateList.Add(Item);
                currentPosX = Item.transform.position.x;
                for (int i = CurrentIndex + 1; i < ListItem.Count; i++)
                {
                    GameObject It = ListItem[i];
                    currentPosX += DistanceBetweenEach;
                    Vector3 position = new Vector3(
                        currentPosX,
                        CurrentItemPos.transform.position.y,
                        It.transform.position.z);
                    GameObject ItemTemp = Instantiate(It, position, Quaternion.identity);
                    AfterGenerateList.Add(ItemTemp);
                    ItemTemp.transform.SetParent(Contents.transform);
                    ItemTemp.transform.GetChild(0).GetComponent<LoadOutBox>().bar = gameObject;
                    ItemTemp.transform.GetChild(0).GetComponent<LoadOutBox>().background = Background;
                    ItemTemp.transform.GetChild(0).GetComponent<LoadOutBox>().WeaponName = ListName[i];
                }
                for (int i = 0; i < CurrentIndex - 1; i++)
                {
                    GameObject It = ListItem[i];
                    currentPosX += DistanceBetweenEach;
                    Vector3 position = new Vector3(
                        currentPosX,
                        CurrentItemPos.transform.position.y,
                        It.transform.position.z);
                    GameObject ItemTemp = Instantiate(It, position, Quaternion.identity);
                    AfterGenerateList.Add(ItemTemp);
                    ItemTemp.transform.SetParent(Contents.transform);
                    ItemTemp.transform.GetChild(0).GetComponent<LoadOutBox>().bar = gameObject;
                    ItemTemp.transform.GetChild(0).GetComponent<LoadOutBox>().background = Background;
                    ItemTemp.transform.GetChild(0).GetComponent<LoadOutBox>().WeaponName = ListName[i];
                }
            }
            // Case current item is the first item of the list:
            // Generate the list: n [0] 1 2 ... n-1
            else
            {
                float currentPosX = Item.transform.position.x - DistanceBetweenEach;
                GameObject It2 = ListItem[ListItem.Count - 1];
                Vector3 position2 = new Vector3(
                    currentPosX,
                    CurrentItemPos.transform.position.y,
                    It2.transform.position.z);
                GameObject ItemTemp2 = Instantiate(It2, position2, Quaternion.identity);
                AfterGenerateList.Add(ItemTemp2); 
                ItemTemp2.transform.SetParent(Contents.transform);
                ItemTemp2.transform.GetChild(0).GetComponent<LoadOutBox>().bar = gameObject;
                ItemTemp2.transform.GetChild(0).GetComponent<LoadOutBox>().background = Background;
                ItemTemp2.transform.GetChild(0).GetComponent<LoadOutBox>().WeaponName = ListName[ListItem.Count - 1];
                AfterGenerateList.Add(Item);
                currentPosX = Item.transform.position.x;
                for (int i = 1; i < ListItem.Count - 1; i++)
                {
                    GameObject It = ListItem[i];
                    currentPosX += DistanceBetweenEach;
                    Vector3 position = new Vector3(
                        currentPosX,
                        CurrentItemPos.transform.position.y,
                        It.transform.position.z);
                    GameObject ItemTemp = Instantiate(It, position, Quaternion.identity);
                    AfterGenerateList.Add(ItemTemp);
                    ItemTemp.transform.SetParent(Contents.transform);
                    ItemTemp.transform.GetChild(0).GetComponent<LoadOutBox>().bar = gameObject;
                    ItemTemp.transform.GetChild(0).GetComponent<LoadOutBox>().background = Background;
                    ItemTemp.transform.GetChild(0).GetComponent<LoadOutBox>().WeaponName = ListName[i];
                }
                
            }
        }
    }
    #endregion
    #region List Modify
    private List<string> ListReplaceSpace(List<string> inList)
    {
        for (int i = 0; i < inList.Count; i++)
        {
            inList[i] = inList[i].Replace(" ", "");
        }
        return inList;
    }

    private void OnDisable()
    {
        int i = 0;
        while (i < AfterGenerateList.Count)
        {
            GameObject temp = AfterGenerateList[i];
            AfterGenerateList.RemoveAt(i);
            Destroy(temp);
        }
        if (!GetComponent<Collider2D>().enabled)
        {
            GetComponent<Collider2D>().enabled = true;
        }
    }
    #endregion
}
