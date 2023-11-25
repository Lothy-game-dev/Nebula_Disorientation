 using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using UnityEngine;
using UnityEngine.UI;

public class StatusBoard : MonoBehaviour
{
    #region ComponentVariables
    #endregion
    #region InitializeVariables
    // Status related
    public GameController GameController;
    // Zoom out and close related
    public GameObject ZoomOutPosition;
    public GameObject ClosePosition;
    //Status
    public GameObject NameBox;
    public GameObject ItemBox1;
    public GameObject ItemBox2;
    public GameObject StatusBox;
    public TextMeshPro HealthText;
    public Slider HPSlider;
    public TextMeshPro BarrierText;
    public Slider BarrierSlider;
    public TextMeshPro TemperText;
    public Slider TemperSlider;
    public GameObject HPBar;
    public GameObject BarrierBar;
    public GameObject TempBar;
    public GameObject TierText;
    // Check mouse
    public GameObject Left;
    public GameObject Right;
    public GameObject Top;
    public GameObject Bottom;
    //Image
    public GameObject EnemyImagePosition;
    public GameObject WeaponList;
    public GameObject PowerList;
    public Vector2 WeaponScale;
    public Vector2 PowerScale;
    public List<GameObject> WsSsWeaponList;
    #endregion
    #region NormalVariables
    public GameObject Enemy;
    // Time Counter
    public float Timer;
    // Is hide or not 
    public bool isShow;
    private bool AllowUpdate;
    private float initScale;
    private GameObject CloneEnemy;
    private FighterShared EnemyObject;
    private FighterShared CloneEnemyObject;
    private Rigidbody2D CloneEnemyRb2D;
    private Collider2D CloneEnemyColl;
    private float ImageInitScaleX;
    private float ImageInitScaleY;
    private bool isEnding;
    private bool startCounting;
    private bool OkToDestroy;
    private bool alreadyDelete;
    private GameObject LastEnemy;
    public float WeaponPowerSwitchTimer;
    private bool isShowingWeapon;
    private string LeftWeaponName;
    private string RightWeaponName;
    private string FirstPower;
    private string SecondPower;
    public GameObject ModelLeftWeapon;
    public GameObject ModelRightWeapon;
    public GameObject ModelFirstPower;
    public GameObject ModelSecondPower;
    private SpaceStationShared SpaceStation;
    private WSShared Warship;
    private bool isTextTranUp;
    private bool Showing;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        initScale = transform.localScale.x;
/*        if (Enemy.GetComponent<FighterShared>() != null)
        {
            ImageInitScaleX = Enemy.transform.localScale.x * (Enemy.GetComponent<EnemyShared>()!=null? Enemy.GetComponent<EnemyShared>().ScaleOnStatusBoard : Enemy.GetComponent<AlliesShared>().ScaleOnStatusBoard) / initScale;
            ImageInitScaleY = Enemy.transform.localScale.y * (Enemy.GetComponent<EnemyShared>() != null ? Enemy.GetComponent<EnemyShared>().ScaleOnStatusBoard : Enemy.GetComponent<AlliesShared>().ScaleOnStatusBoard) / initScale;
        } else
        {
            if (Enemy.GetComponent<StatusBoard>() != null)
            {

            }
        }*/

        alreadyDelete = false;
    }

    private void OnEnable()
    {
        if (Showing)
        {
            StartCoroutine(DelayShow());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMouseOutsideRange())
        {
            WeaponPowerSwitchTimer -= Time.deltaTime;
            CheckOnDestroy();
            TextTransUp();
        } else
        {
            WeaponPowerSwitchTimer = 2;
            TextTransDown();
        }
        if (WeaponPowerSwitchTimer<=0f)
        {
            SwitchWeaponPowerInfo();
            WeaponPowerSwitchTimer = 2;
        }
        if (isShow)
        {
            UpdateStatus();
        }
        if (startCounting)
        {
            if (Timer > 0f)
            {
                Timer -= Time.deltaTime;
            }
            else
            {
                isShow = false;
                if (CloneEnemy != null)
                {
                    Destroy(CloneEnemy);
                    NameBox.SetActive(false);
                    ItemBox1.SetActive(false);
                    ItemBox2.SetActive(false);
                    HealthText.gameObject.SetActive(false);
                    HPBar.SetActive(false);
                    BarrierText.gameObject.SetActive(false);
                    BarrierBar.SetActive(false);
                    TemperText.gameObject.SetActive(false);
                    TempBar.SetActive(false);
                    TierText.SetActive(false);
                    alreadyDelete = true;
                }
                StartCoroutine(CloseBoard());
            }
        }
    }
    #endregion
    #region Mouse Over Check
    private bool isMouseOutsideRange()
    {
        if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x > Right.transform.position.x ||
            Camera.main.ScreenToWorldPoint(Input.mousePosition).x < Left.transform.position.x ||
            Camera.main.ScreenToWorldPoint(Input.mousePosition).y > Top.transform.position.y ||
            Camera.main.ScreenToWorldPoint(Input.mousePosition).y < Bottom.transform.position.y)
            return true;
        return false;
    }

    private void TextTransUp()
    {
        Color cHealth = HealthText.color;
        cHealth.a = 1f;
        HealthText.color = cHealth; 
        Color cBarrier = BarrierText.color;
        cBarrier.a = 1f;
        BarrierText.color = cBarrier;
        Color cTemp = TemperText.color;
        cTemp.a = 1f;
        TemperText.color = cTemp;
        Color cTier = TierText.GetComponent<TextMeshPro>().color;
        cTier.a = 1f;
        TierText.GetComponent<TextMeshPro>().color = cTier;
        Color cHPBar = HPBar.transform.GetChild(2).GetComponent<TextMeshPro>().color;
        cHPBar.a = 1f;
        HPBar.transform.GetChild(2).GetComponent<TextMeshPro>().color = cHPBar;
        Color cBRBar = BarrierBar.transform.GetChild(2).GetComponent<TextMeshPro>().color;
        cBRBar.a = 1f;
        BarrierBar.transform.GetChild(2).GetComponent<TextMeshPro>().color = cBRBar;
        Color cTempBar = TempBar.transform.GetChild(2).GetComponent<TextMeshPro>().color;
        cTempBar.a = 1f;
        TempBar.transform.GetChild(2).GetComponent<TextMeshPro>().color = cTempBar;
        Color cName = NameBox.transform.GetChild(0).GetComponent<TextMeshPro>().color;
        cName.a = 1f;
        NameBox.transform.GetChild(0).GetComponent<TextMeshPro>().color = cName;
    }

    private void TextTransDown()
    {
        Color cHealth = HealthText.color;
        cHealth.a = 0.5f;
        HealthText.color = cHealth;
        Color cBarrier = BarrierText.color;
        cBarrier.a = 0.5f;
        BarrierText.color = cBarrier;
        Color cTemp = TemperText.color;
        cTemp.a = 0.5f;
        TemperText.color = cTemp;
        Color cTier = TierText.GetComponent<TextMeshPro>().color;
        cTier.a = 0.5f;
        TierText.GetComponent<TextMeshPro>().color = cTier;
        Color cHPBar = HPBar.transform.GetChild(2).GetComponent<TextMeshPro>().color;
        cHPBar.a = 0.5f;
        HPBar.transform.GetChild(2).GetComponent<TextMeshPro>().color = cHPBar;
        Color cBRBar = BarrierBar.transform.GetChild(2).GetComponent<TextMeshPro>().color;
        cBRBar.a = 0.5f;
        BarrierBar.transform.GetChild(2).GetComponent<TextMeshPro>().color = cBRBar;
        Color cTempBar = TempBar.transform.GetChild(2).GetComponent<TextMeshPro>().color;
        cTempBar.a = 0.5f;
        TempBar.transform.GetChild(2).GetComponent<TextMeshPro>().color = cTempBar;
        Color cName = NameBox.transform.GetChild(0).GetComponent<TextMeshPro>().color;
        cName.a = 0.5f;
        NameBox.transform.GetChild(0).GetComponent<TextMeshPro>().color = cName;
    }
    #endregion
    #region React When Zoom Out/Close
    // React to zoom
    private void ReactWhenZoom()
    {
        if (GameController.IsClose)
        {
            // If close -> All range and scale is half
            transform.position = new Vector3(ClosePosition.transform.position.x, ClosePosition.transform.position.y, transform.position.z);
            transform.localScale = new Vector3(initScale, initScale, initScale);
            if (CloneEnemy != null)
            {
                CloneEnemy.transform.localScale = new Vector2(ImageInitScaleX, ImageInitScaleY);
            }
        }
        else
        {
            // If zoom out -> All range and scale is back to normal
            transform.position = new Vector3(ZoomOutPosition.transform.position.x, ZoomOutPosition.transform.position.y, transform.position.z);
            if (CloneEnemy != null)
            {
                CloneEnemy.transform.localScale = new Vector2(ImageInitScaleX, ImageInitScaleY);
            }
            transform.localScale = new Vector3(initScale*2, initScale*2, initScale*2);
        }
    }
    #endregion
    #region Show Status
    // Start showing enemy
    public void StartShowing(GameObject enemy)
    {
        WeaponPowerSwitchTimer = 2f;
        Enemy = enemy;
        isShow = false;
        startCounting = false;
        // if not active -> active and start animation
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            // Start animation
            StartCoroutine(DelayShow());
        } 
        // if already active: case when the stats and image still on: reset timer and variables
        // case when stats and image already destroy during closing: re-show them
        else
        {
            if (alreadyDelete)
            {
                alreadyDelete = false;
                StartCoroutine(DelayShow());
            } else
            {
                if (Timer == 0f)
                {
                    // Make the board will not close until other function to set its timer
                    Timer = 1000f;
                }
                startCounting = true;
                isShow = true;
            }
        }
    }

    private IEnumerator DelayShow()
    {
        Showing = true;
        LastEnemy = Enemy;
        ItemBox1.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
        ItemBox2.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
        // Wait for animation 1s to show enemy
        yield return new WaitForSeconds(0);
        if (CloneEnemy == null)
        {
            try
            {
                CloneEnemy = Instantiate(Enemy, EnemyImagePosition.transform.position, Quaternion.identity);
                CloneEnemy.SetActive(false);
            } catch (System.NullReferenceException)
            {
                CloseBoard();
            }
            
            // set Sorting order
            CloneEnemy.GetComponent<SpriteRenderer>().sortingOrder = 200;
            // Set color and transparency
            Color c = CloneEnemy.GetComponent<SpriteRenderer>().color;
            c.a = 0.5f;
            c.r = 1;
            c.g = 1;
            c.b = 1;
            CloneEnemy.GetComponent<SpriteRenderer>().color = c;
            if (CloneEnemy.GetComponent<AlliesShared>()!=null)
            {
                if (CloneEnemy.GetComponent<AlliesShared>().Class == "A")
                {
                    TierText.GetComponent<TextMeshPro>().text = "Class A";
                    TierText.GetComponent<TextMeshPro>().color = new Color(0, 1, 0, 127 / 255f);
                } else if (CloneEnemy.GetComponent<AlliesShared>().Class == "B")
                {
                    TierText.GetComponent<TextMeshPro>().text = "Class B";
                    TierText.GetComponent<TextMeshPro>().color = new Color(0, 0, 1, 127 / 255f);
                } else
                {
                    TierText.GetComponent<TextMeshPro>().text = "Class C";
                    TierText.GetComponent<TextMeshPro>().color = new Color(1, 0, 0, 127 / 255f);
                }
            } else if (CloneEnemy.GetComponent<EnemyShared>() != null)
            {
                if (CloneEnemy.GetComponent<EnemyShared>().Tier == 3)
                {
                    TierText.GetComponent<TextMeshPro>().text = "Class A";
                    TierText.GetComponent<TextMeshPro>().color = new Color(0, 1, 0, 127 / 255f);
                }
                else if (CloneEnemy.GetComponent<EnemyShared>().Tier == 2)
                {
                    TierText.GetComponent<TextMeshPro>().text = "Class B";
                    TierText.GetComponent<TextMeshPro>().color = new Color(0, 0, 1, 127 / 255f);
                }
                else
                {
                    TierText.GetComponent<TextMeshPro>().text = "Class C";
                    TierText.GetComponent<TextMeshPro>().color = new Color(1, 0, 0, 127 / 255f);
                }
            }

            if (CloneEnemy.GetComponent<WSShared>() != null)
            {
                if (CloneEnemy.GetComponent<WSShared>().Tier == "Tier I")
                {
                    TierText.GetComponent<TextMeshPro>().text = "Tier I";
                    TierText.GetComponent<TextMeshPro>().color = new Color(0, 1, 0, 127 / 255f);
                }
                else if (CloneEnemy.GetComponent<WSShared>().Tier == "Tier II")
                {
                    TierText.GetComponent<TextMeshPro>().text = "Tier II";
                    TierText.GetComponent<TextMeshPro>().color = new Color(0, 0, 1, 127 / 255f);
                }
            }

            if (CloneEnemy.GetComponent<SpaceStationShared>() != null)
            {
                TierText.GetComponent<TextMeshPro>().text = "Tier I";
                TierText.GetComponent<TextMeshPro>().color = Color.red;
            }
            TierText.SetActive(true);
            // set Clone Enemy's parent as this board
            CloneEnemy.transform.SetParent(transform);
            if (CloneEnemy.GetComponent<WSShared>() != null || CloneEnemy.GetComponent<SpaceStationShared>() != null)
            {
                CloneEnemy.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            } else
            {
                CloneEnemy.transform.localScale = new Vector3(1, 1, 1);
            }
            // Destroy objects need to be destroyed so it wont interact
            for (int i=0;i<CloneEnemy.transform.childCount;i++)
            {
                Destroy(CloneEnemy.transform.GetChild(i).gameObject);
            }
            // turn off scripts
            if (CloneEnemy.GetComponent<EnemyShared>() != null || CloneEnemy.GetComponent<EnemyShared>() != null || CloneEnemy.GetComponent<AlliesShared>() != null)
            {
                CloneEnemyObject = (CloneEnemy.GetComponent<EnemyShared>() != null ? CloneEnemy.GetComponent<EnemyShared>() : CloneEnemy.GetComponent<AlliesShared>());
                CloneEnemyObject.enabled = false;
                Destroy(CloneEnemy.GetComponent<DecisionRequester>());
                if (CloneEnemy.GetComponent<AlliesFighterMLAgent>()!=null)
                {
                    Destroy(CloneEnemy.GetComponent<AlliesFighterMLAgent>());
                } else if (CloneEnemy.GetComponent<EnemyFighterMLAgent>() != null)
                {
                    Destroy(CloneEnemy.GetComponent<EnemyFighterMLAgent>());
                }
                Destroy(CloneEnemy.GetComponent<BehaviorParameters>());
                Destroy(CloneEnemy.GetComponent<SoundController>());
                Destroy(CloneEnemy.GetComponent<AudioSource>());
                FighterMovement fm = CloneEnemy.GetComponent<FighterMovement>();
                fm.enabled = false;
                // turn off component
                CloneEnemyRb2D = CloneEnemy.GetComponent<Rigidbody2D>();
                CloneEnemyRb2D.simulated = false;
                CloneEnemyColl = CloneEnemy.GetComponent<Collider2D>();
                CloneEnemyColl.enabled = false;
            }

            // turn off scripts
            if (CloneEnemy.GetComponent<SpaceStationShared>() != null)
            {
                SpaceStation = CloneEnemy.GetComponent<SpaceStationShared>();
                SpaceStation.enabled = false;
                // turn off component
                CloneEnemyColl = SpaceStation.GetComponent<Collider2D>();
                CloneEnemyColl.enabled = false;
                Destroy(CloneEnemy.GetComponent<AudioSource>());
            }

            // turn off scripts
            if (CloneEnemy.GetComponent<WSShared>() != null)
            {
                Warship = CloneEnemy.GetComponent<WSShared>();
                Warship.enabled = false;
                // turn off component
                CloneEnemyColl = Warship.GetComponent<PolygonCollider2D>();
                CloneEnemyColl.enabled = false;

                WSMovement move = CloneEnemy.GetComponent<WSMovement>();
                move.enabled = false;

                Destroy(CloneEnemy.GetComponent<AudioSource>());
            }

            NameBox.SetActive(true);
            if (EnemyObject != null)
            {
                if (EnemyObject.GetComponent<EnemyShared>() != null)
                {
                    NameBox.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 127 / 255f);
                    NameBox.transform.GetChild(0).GetComponent<TextMeshPro>().color = new Color(1, 1, 1, 127 / 255f);
                }
                else
                {
                    NameBox.GetComponent<SpriteRenderer>().color = new Color(153 / 255f, 173 / 255f, 212 / 255f, 127 / 255f);
                    NameBox.transform.GetChild(0).GetComponent<TextMeshPro>().color = new Color(1, 1, 1, 127 / 255f);
                }
                NameBox.transform.GetChild(0).GetComponent<TextMeshPro>().text = EnemyObject.FighterName;
            }
            if (Warship != null)
            {
                if (Warship.IsEnemy)
                {
                    NameBox.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 127 / 255f);
                    NameBox.transform.GetChild(0).GetComponent<TextMeshPro>().color = new Color(1, 1, 1, 127 / 255f);
                }
                else
                {
                    NameBox.GetComponent<SpriteRenderer>().color = new Color(153 / 255f, 173 / 255f, 212 / 255f, 127 / 255f);
                    NameBox.transform.GetChild(0).GetComponent<TextMeshPro>().color = new Color(1, 1, 1, 127 / 255f);
                }
                NameBox.transform.GetChild(0).GetComponent<TextMeshPro>().text = Warship.WarshipName;
            }

            if (SpaceStation != null)
            {
                if (SpaceStation.isEnemy)
                {
                    NameBox.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 127 / 255f);
                    NameBox.transform.GetChild(0).GetComponent<TextMeshPro>().color = new Color(1, 1, 1, 127 / 255f);
                }
                else
                {
                    NameBox.GetComponent<SpriteRenderer>().color = new Color(153 / 255f, 173 / 255f, 212 / 255f, 127 / 255f);
                    NameBox.transform.GetChild(0).GetComponent<TextMeshPro>().color = new Color(1, 1, 1, 127 / 255f);
                }
                NameBox.transform.GetChild(0).GetComponent<TextMeshPro>().text = SpaceStation.SpaceStationName;
            }

        }
        
        if (CloneEnemy.GetComponent<EnemyShared>() != null || CloneEnemy.GetComponent<EnemyShared>() != null || CloneEnemy.GetComponent<AlliesShared>() != null)
        {
            LeftWeaponName = Enemy.GetComponent<EnemyShared>() != null ? Enemy.GetComponent<EnemyShared>().weaponName1 : Enemy.GetComponent<AlliesShared>().weaponName1;
            RightWeaponName = Enemy.GetComponent<EnemyShared>() != null ? Enemy.GetComponent<EnemyShared>().weaponName2 : Enemy.GetComponent<AlliesShared>().weaponName2;
            FirstPower = Enemy.GetComponent<EnemyShared>() != null ? Enemy.GetComponent<EnemyShared>().Power1 : Enemy.GetComponent<AlliesShared>().Power1;
            SecondPower = Enemy.GetComponent<EnemyShared>() != null ? Enemy.GetComponent<EnemyShared>().Power2 : Enemy.GetComponent<AlliesShared>().Power2;
            ModelLeftWeapon = null;
            ModelRightWeapon = null;
            ModelFirstPower = null;
            ModelSecondPower = null;
            bool alreadyLeft = false;
            bool alreadyRight = false;
            for (int i=0;i<WeaponList.transform.childCount;i++)
            {
                if (alreadyLeft && alreadyRight)
                {
                    break;
                }
                if (WeaponList.transform.GetChild(i).name.Replace(" ","").Replace("-","").ToLower().Equals(LeftWeaponName.Replace(" ","").Replace("-","").ToLower()))
                {
                    alreadyLeft = true;
                    ModelLeftWeapon = WeaponList.transform.GetChild(i).gameObject;
                }
                if (WeaponList.transform.GetChild(i).name.Replace(" ", "").Replace("-", "").ToLower().Equals(RightWeaponName.Replace(" ", "").Replace("-", "").ToLower()))
                {
                    alreadyRight = true;
                    ModelRightWeapon = WeaponList.transform.GetChild(i).gameObject;
                }
            }
            bool alreadyFirst = false;
            bool alreadySecond = false;
            for (int i = 0; i < PowerList.transform.childCount; i++)
            {
                if (alreadyFirst && alreadySecond)
                {
                    break;
                }
                if (FirstPower != null && FirstPower != "")
                {
                    if (PowerList.transform.GetChild(i).name.Replace(" ", "").Replace("-", "").ToLower().Equals(FirstPower.Replace(" ", "").Replace("-", "").ToLower()))
                    {
                        alreadyFirst = true;
                        ModelFirstPower = PowerList.transform.GetChild(i).gameObject;
                    }
                } else
                {
                    alreadyFirst = true;
                }
                if (SecondPower != null && SecondPower != "")
                {
                    if (PowerList.transform.GetChild(i).name.Replace(" ", "").Replace("-", "").ToLower().Equals(SecondPower.Replace(" ", "").Replace("-", "").ToLower()))
                    {
                        alreadySecond = true;
                        ModelSecondPower = PowerList.transform.GetChild(i).gameObject;
                    }
                }
                else
                {
                    alreadySecond = true;
                }
            }
            isShowingWeapon = true;
            if (ModelLeftWeapon != null)
            {
                ItemBox1.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite
                = Enemy.GetComponent<EnemyShared>() != null ? ModelLeftWeapon.GetComponent<Weapons>().ZatIconSprite : ModelLeftWeapon.GetComponent<Weapons>().IconSprite;
                ItemBox1.transform.GetChild(0).localScale = WeaponScale;
                ItemBox1.GetComponent<HUDCreateInfoBoard>().Text[0] = FindObjectOfType<AccessDatabase>().GetItemRealName(LeftWeaponName, "Weapon");
            }
            else
            {
                ItemBox1.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite
                = null;
                ItemBox1.GetComponent<HUDCreateInfoBoard>().Text[0] = "";
            }
            if (ModelRightWeapon != null)
            {
                ItemBox2.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite
                = Enemy.GetComponent<EnemyShared>() != null ? ModelRightWeapon.GetComponent<Weapons>().ZatIconSprite : ModelRightWeapon.GetComponent<Weapons>().IconSprite;
                ItemBox2.transform.GetChild(0).localScale = WeaponScale;
                ItemBox2.GetComponent<HUDCreateInfoBoard>().Text[0] = FindObjectOfType<AccessDatabase>().GetItemRealName(RightWeaponName, "Weapon");
            }
            else
            {
                ItemBox2.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite
                = null;
                ItemBox2.GetComponent<HUDCreateInfoBoard>().Text[0] = "";
            }
        } else
        {
            if (CloneEnemy.GetComponent<WSShared>() != null)
            {
                LeftWeaponName = Enemy.GetComponent<WSShared>().MainWeapon[0];
                RightWeaponName = Enemy.GetComponent<WSShared>().MainWeapon.Count > 1 ? Enemy.GetComponent<WSShared>().MainWeapon[1] : "";
    
                bool alreadyLeft = false;
                bool alreadyRight = false;
                for (int i = 0; i < WsSsWeaponList.Count; i++)
                {
                    if (alreadyLeft && alreadyRight)
                    {
                        break;
                    }
                    if (WsSsWeaponList[i].name.Replace(" ", "").ToLower().Contains(LeftWeaponName.Replace(" ", "").ToLower()))
                    {                       
                        alreadyLeft = true;
                        ModelLeftWeapon = WsSsWeaponList[i];                     
                    }
                    if (RightWeaponName != "")
                    {
                        if (WsSsWeaponList[i].name.Replace(" ", "").ToLower().Contains(RightWeaponName.Replace(" ", "").ToLower()))
                        {
                            alreadyRight = true;
                            ModelRightWeapon = WsSsWeaponList[i];                          
                        }
                    } else
                    {
                        ModelRightWeapon = null;
                    }
                }

                if (ModelLeftWeapon != null)
                {
                    ItemBox1.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite
                    = ModelLeftWeapon.GetComponent<SpriteRenderer>().sprite;
                    ItemBox1.transform.GetChild(0).localScale = WeaponScale / 2;
                    ItemBox1.GetComponent<HUDCreateInfoBoard>().Text[0] = Enemy.GetComponent<WSShared>().MainWeapon[0];
                }
                else
                {
                    ItemBox1.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite
                    = null;
                    ItemBox1.GetComponent<HUDCreateInfoBoard>().Text[0] = "";
                }

                if (ModelRightWeapon != null)
                {
                    ItemBox2.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite
                    = ModelRightWeapon.GetComponent<SpriteRenderer>().sprite;
                    ItemBox2.transform.GetChild(0).localScale = WeaponScale / 2;
                    ItemBox2.GetComponent<HUDCreateInfoBoard>().Text[0] = Enemy.GetComponent<WSShared>().MainWeapon[1];
                }
                else
                {
                    ItemBox2.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite
                    = null;
                    ItemBox2.GetComponent<HUDCreateInfoBoard>().Text[0] = "";
                }
            } else
            {
                if (CloneEnemy.GetComponent<SpaceStationShared>() != null)
                {
                    LeftWeaponName = Enemy.GetComponent<SpaceStationShared>().MainWeapon[0];          
                    for (int i = 0; i < WsSsWeaponList.Count; i++)
                    {                      
                        if (WsSsWeaponList[i].name.Replace(" ", "").ToLower().Contains(LeftWeaponName.Replace(" ", "").ToLower()))
                        {                    
                            ModelLeftWeapon = WsSsWeaponList[i];
                        }                       
                    }

                    if (ModelLeftWeapon != null)
                    {
                        ItemBox1.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite
                        = ModelLeftWeapon.GetComponent<SpriteRenderer>().sprite;
                        ItemBox1.transform.GetChild(0).localScale = WeaponScale / 2;
                        ItemBox1.GetComponent<HUDCreateInfoBoard>().Text[0] = Enemy.GetComponent<SpaceStationShared>().MainWeapon[0];
                    }
                    else
                    {
                        ItemBox1.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite
                        = null;
                        ItemBox1.GetComponent<HUDCreateInfoBoard>().Text[0] = "";
                    }

                    if (ModelRightWeapon != null)
                    {
                        ItemBox2.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite
                        = ModelRightWeapon.GetComponent<SpriteRenderer>().sprite;
                        ItemBox2.transform.GetChild(0).localScale = WeaponScale / 2;
                        ItemBox2.GetComponent<HUDCreateInfoBoard>().Text[0] = Enemy.GetComponent<SpaceStationShared>().MainWeapon[0];
                    }
                    else
                    {
                        ItemBox2.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite
                        = null;
                        ItemBox2.GetComponent<HUDCreateInfoBoard>().Text[0] = "";
                    }

                }
            }
        }

        // WAit time to show HP and temp bar
        yield return new WaitForSeconds(0.4f);
        CloneEnemy.SetActive(true);
        ItemBox1.SetActive(true);
        ItemBox2.SetActive(true);
        /*StatusBox.SetActive(true);*/
        if (CloneEnemy.GetComponent<EnemyShared>() != null || CloneEnemy.GetComponent<EnemyShared>() != null || CloneEnemy.GetComponent<AlliesShared>() != null)
        {
            EnemyObject = (Enemy.GetComponent<EnemyShared>() != null ? Enemy.GetComponent<EnemyShared>() : Enemy.GetComponent<AlliesShared>());
            HPBar.SetActive(true);
            HPSlider.maxValue = EnemyObject.MaxHP;
            HPSlider.value = EnemyObject.CurrentHP;

            //Set HP to show how much current HP
            HealthText.gameObject.SetActive(true);
            HealthText.text = Mathf.Round(EnemyObject.CurrentHP) + "/" + EnemyObject.MaxHP;

            // Barrier undone
            BarrierText.gameObject.SetActive(true);
            BarrierBar.SetActive(true);
            BarrierSlider.maxValue = EnemyObject.MaxBarrier;
            BarrierSlider.value = EnemyObject.CurrentBarrier;

            BarrierText.text = Mathf.Round(EnemyObject.CurrentBarrier) + "/" + EnemyObject.MaxBarrier;

            //Setting slider base on current temperature
            TempBar.SetActive(true);
            TemperSlider.maxValue = 100;
            TemperSlider.value = EnemyObject.currentTemperature;

            //if temp > 50, the slider is red else blue
            if (EnemyObject.currentTemperature > 50)
            {
                TemperSlider.fillRect.GetComponentInChildren<Image>().color = Color.Lerp(Color.green, Color.red, TemperSlider.normalizedValue);
            }
            else if (EnemyObject.currentTemperature < 50)
            {
                TemperSlider.fillRect.GetComponentInChildren<Image>().color = Color.Lerp(Color.blue, Color.green, TemperSlider.normalizedValue);
            }
            else if (EnemyObject.currentTemperature == 50)
            {
                TemperSlider.fillRect.GetComponentInChildren<Image>().color = Color.green;
            }

            //Setting to show current tempurature
            TemperText.gameObject.SetActive(true);
            TemperText.text = Mathf.Round(EnemyObject.currentTemperature * 10) / 10 + "\u00B0C";
        } else
        {
            if (CloneEnemy.GetComponent<WSShared>() != null)
            {
                Warship = CloneEnemy.GetComponent<WSShared>();
                HPBar.SetActive(true);
                HPSlider.maxValue = Warship.MaxHP;
                HPSlider.value = Warship.CurrentHP;

                //Set HP to show how much current HP
                HealthText.gameObject.SetActive(true);
                HealthText.text = Mathf.Round(Warship.CurrentHP) + "/" + Warship.MaxHP;

                // Barrier undone
                BarrierText.gameObject.SetActive(true);
                BarrierBar.SetActive(true);
                BarrierSlider.maxValue = Warship.MaxBarrier;
                BarrierSlider.value = Warship.CurrentBarrier;

                BarrierText.text = Mathf.Round(Warship.CurrentBarrier) + "/" + Warship.MaxBarrier;

                //Setting slider base on current temperature
                TempBar.SetActive(true);
                TemperSlider.maxValue = 100;
                TemperSlider.value = 50;
                TemperSlider.fillRect.GetComponentInChildren<Image>().color = Color.green;
                

                //Setting to show current tempurature
                TemperText.gameObject.SetActive(true);
                TemperText.text = "-";
            } else
            {
                if (CloneEnemy.GetComponent<SpaceStationShared>() != null)
                {
                    SpaceStation = CloneEnemy.GetComponent<SpaceStationShared>();
                    HPBar.SetActive(true);
                    HPSlider.maxValue = SpaceStation.MaxHP;
                    HPSlider.value = SpaceStation.CurrentHP;

                    //Set HP to show how much current HP
                    HealthText.gameObject.SetActive(true);
                    HealthText.text = Mathf.Round(SpaceStation.CurrentHP) + "/" + SpaceStation.MaxHP;

                    // Barrier undone
                    BarrierText.gameObject.SetActive(true);
                    BarrierBar.SetActive(true);
                    BarrierSlider.maxValue = SpaceStation.MaxBarrier;
                    BarrierSlider.value = SpaceStation.CurrentBarrier;

                    BarrierText.text = Mathf.Round(SpaceStation.CurrentBarrier) + "/" + SpaceStation.MaxBarrier;

                    //Setting slider base on current temperature
                    TempBar.SetActive(true);
                    TemperSlider.maxValue = 100;
                    TemperSlider.value = 50;
                    TemperSlider.fillRect.GetComponentInChildren<Image>().color = Color.green;


                    //Setting to show current tempurature
                    TemperText.gameObject.SetActive(true);
                    TemperText.text = "-";
                }
            }
        }
        if (Timer==0f)
        {
            Timer = 1000f;
        }
        startCounting = true;
        isShow = true;
        Showing = false;
    }

    public void UpdateStatus()
    {
        if (!NameBox.activeSelf)
            NameBox.SetActive(true);
        if (!ItemBox1.activeSelf)
            ItemBox1.SetActive(true);
        if (!ItemBox2.activeSelf)
            ItemBox2.SetActive(true);
        if (!HealthText.gameObject.activeSelf)
            HealthText.gameObject.SetActive(true);
        if (!HPBar.activeSelf)
            HPBar.SetActive(true);
        if (!BarrierText.gameObject.activeSelf)
            BarrierText.gameObject.SetActive(true);
        if (!BarrierBar.activeSelf)
            BarrierBar.SetActive(true);
        if (!TemperText.gameObject.activeSelf)
            TemperText.gameObject.SetActive(true);
        if (!TempBar.activeSelf)
            TempBar.SetActive(true);
        if (!TierText.activeSelf)
            TierText.SetActive(true);
        if (Enemy != LastEnemy)
        {
            ItemBox1.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
            ItemBox2.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
            Destroy(CloneEnemy);
            CloneEnemy = Instantiate(Enemy, EnemyImagePosition.transform.position, Quaternion.identity);
            CloneEnemy.SetActive(false);
            // set Sorting order
            LastEnemy = Enemy;
            CloneEnemy.GetComponent<SpriteRenderer>().sortingOrder = 200;
            // Set color and transparency
            Color c = CloneEnemy.GetComponent<SpriteRenderer>().color;
            c.a = 0.5f;
            c.r = 1;
            c.g = 1;
            c.b = 1;
            CloneEnemy.GetComponent<SpriteRenderer>().color = c;
            if (CloneEnemy.GetComponent<AlliesShared>() != null)
            {
                if (CloneEnemy.GetComponent<AlliesShared>().Class == "A")
                {
                    TierText.GetComponent<TextMeshPro>().text = "Class A";
                    TierText.GetComponent<TextMeshPro>().color = new Color(0, 1, 0, 127 / 255f);
                }
                else if (CloneEnemy.GetComponent<AlliesShared>().Class == "B")
                {
                    TierText.GetComponent<TextMeshPro>().text = "Class B";
                    TierText.GetComponent<TextMeshPro>().color = new Color(0, 0, 1, 127 / 255f);
                }
                else
                {
                    TierText.GetComponent<TextMeshPro>().text = "Class C";
                    TierText.GetComponent<TextMeshPro>().color = new Color(1, 0, 0, 127 / 255f);
                }
            }
            else if (CloneEnemy.GetComponent<EnemyShared>() != null)
            {
                if (CloneEnemy.GetComponent<EnemyShared>().Tier == 3)
                {
                    TierText.GetComponent<TextMeshPro>().text = "Class A";
                    TierText.GetComponent<TextMeshPro>().color = new Color(0, 1, 0, 127 / 255f);
                }
                else if (CloneEnemy.GetComponent<EnemyShared>().Tier == 2)
                {
                    TierText.GetComponent<TextMeshPro>().text = "Class B";
                    TierText.GetComponent<TextMeshPro>().color = new Color(0, 0, 1, 127 / 255f);
                }
                else
                {
                    TierText.GetComponent<TextMeshPro>().text = "Class C";
                    TierText.GetComponent<TextMeshPro>().color = new Color(1, 0, 0, 127 / 255f);
                }
            }

            if (CloneEnemy.GetComponent<WSShared>() != null)
            {
                if (CloneEnemy.GetComponent<WSShared>().Tier == "Tier I")
                {
                    TierText.GetComponent<TextMeshPro>().text = "Tier I";
                    TierText.GetComponent<TextMeshPro>().color = new Color(1, 0, 0, 127 / 255f);
                }
                else if (CloneEnemy.GetComponent<WSShared>().Tier == "Tier II")
                {
                    TierText.GetComponent<TextMeshPro>().text = "Tier II";
                    TierText.GetComponent<TextMeshPro>().color = new Color(0, 0, 1, 127 / 255f);
                }
            }

            if (CloneEnemy.GetComponent<SpaceStationShared>() != null)
            {
                TierText.GetComponent<TextMeshPro>().text = "Tier I";
                TierText.GetComponent<TextMeshPro>().color = new Color(0, 1, 0, 127 / 255f);               
            }
            TierText.SetActive(true);
            // set Clone Enemy's parent as this board
            CloneEnemy.transform.SetParent(transform);
            if (CloneEnemy.GetComponent<WSShared>() != null || CloneEnemy.GetComponent<SpaceStationShared>() != null)
            {
                CloneEnemy.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            }
            else
            {
                CloneEnemy.transform.localScale = new Vector3(1, 1, 1);
            }
            // Destroy objects need to be destroyed so it wont interact
            for (int i = 0; i < CloneEnemy.transform.childCount; i++)
            {
                Destroy(CloneEnemy.transform.GetChild(i).gameObject);
            }
            if (CloneEnemy.GetComponent<EnemyShared>() != null || CloneEnemy.GetComponent<EnemyShared>() != null || CloneEnemy.GetComponent<AlliesShared>() != null)
            {
                // turn off scripts
                CloneEnemyObject = (CloneEnemy.GetComponent<EnemyShared>() != null ? CloneEnemy.GetComponent<EnemyShared>() : CloneEnemy.GetComponent<AlliesShared>());
                CloneEnemyObject.enabled = false;
                Destroy(CloneEnemy.GetComponent<DecisionRequester>());
                if (CloneEnemy.GetComponent<AlliesFighterMLAgent>() != null)
                {
                    Destroy(CloneEnemy.GetComponent<AlliesFighterMLAgent>());
                }
                else if (CloneEnemy.GetComponent<EnemyFighterMLAgent>() != null)
                {
                    Destroy(CloneEnemy.GetComponent<EnemyFighterMLAgent>());
                }
                Destroy(CloneEnemy.GetComponent<BehaviorParameters>());
                Destroy(CloneEnemy.GetComponent<SoundController>());
                Destroy(CloneEnemy.GetComponent<AudioSource>());
                FighterMovement fm = CloneEnemy.GetComponent<FighterMovement>();
                fm.enabled = false;
                // turn off component
                CloneEnemyRb2D = CloneEnemy.GetComponent<Rigidbody2D>();
                CloneEnemyRb2D.simulated = false;
                CloneEnemyColl = CloneEnemy.GetComponent<Collider2D>();
                CloneEnemyColl.enabled = false;
                LeftWeaponName = Enemy.GetComponent<EnemyShared>() != null ? Enemy.GetComponent<EnemyShared>().weaponName1 : Enemy.GetComponent<AlliesShared>().weaponName1;
                RightWeaponName = Enemy.GetComponent<EnemyShared>() != null ? Enemy.GetComponent<EnemyShared>().weaponName2 : Enemy.GetComponent<AlliesShared>().weaponName2;
                FirstPower = Enemy.GetComponent<EnemyShared>() != null ? Enemy.GetComponent<EnemyShared>().Power1 : Enemy.GetComponent<AlliesShared>().Power1;
                SecondPower = Enemy.GetComponent<EnemyShared>() != null ? Enemy.GetComponent<EnemyShared>().Power2 : Enemy.GetComponent<AlliesShared>().Power2;
                ModelLeftWeapon = null;
                ModelRightWeapon = null;
                ModelFirstPower = null;
                ModelSecondPower = null;
                bool alreadyLeft = false;
                bool alreadyRight = false;
                if (LeftWeaponName == null || LeftWeaponName == "Transport" || LeftWeaponName== "SuicideBombing")
                {
                    alreadyLeft = true;
                    ModelLeftWeapon = null;
                }
                if (LeftWeaponName == null || RightWeaponName == "Transport" || RightWeaponName == "SuicideBombing")
                {
                    alreadyRight = true;
                    ModelRightWeapon = null;
                }
                for (int i = 0; i < WeaponList.transform.childCount; i++)
                {
                    if (alreadyLeft && alreadyRight)
                    {
                        break;
                    }
                    if (WeaponList.transform.GetChild(i).name.Replace(" ", "").Replace("-", "").ToLower().Equals(LeftWeaponName.Replace(" ", "").Replace("-", "").ToLower()))
                    {
                        alreadyLeft = true;
                        ModelLeftWeapon = WeaponList.transform.GetChild(i).gameObject;
                    }
                    if (WeaponList.transform.GetChild(i).name.Replace(" ", "").Replace("-", "").ToLower().Equals(RightWeaponName.Replace(" ", "").Replace("-", "").ToLower()))
                    {
                        alreadyRight = true;
                        ModelRightWeapon = WeaponList.transform.GetChild(i).gameObject;
                    }
                }

                bool alreadyFirst = false;
                bool alreadySecond = false;
                if (FirstPower == null || FirstPower == "")
                {
                    alreadyFirst = true;
                    ModelFirstPower = null;
                }
                if (SecondPower == null || SecondPower == "")
                {
                    alreadySecond = true;
                    ModelSecondPower = null;
                }
                for (int i = 0; i < PowerList.transform.childCount; i++)
                {
                    if (alreadyFirst && alreadySecond)
                    {
                        break;
                    }
                    if (FirstPower != null && FirstPower != "")
                    {
                        if (PowerList.transform.GetChild(i).name.Replace(" ", "").Replace("-", "").ToLower().Equals(FirstPower.Replace(" ", "").Replace("-", "").ToLower()))
                        {
                            alreadyFirst = true;
                            ModelFirstPower = PowerList.transform.GetChild(i).gameObject;
                        }
                    }
                    else
                    {
                        alreadyFirst = true;
                    }
                    if (SecondPower != null && SecondPower != "")
                    {
                        if (PowerList.transform.GetChild(i).name.Replace(" ", "").Replace("-", "").ToLower().Equals(SecondPower.Replace(" ", "").Replace("-", "").ToLower()))
                        {
                            alreadySecond = true;
                            ModelSecondPower = PowerList.transform.GetChild(i).gameObject;
                        }
                    }
                    else
                    {
                        alreadySecond = true;
                    }
                }
                if (isShowingWeapon)
                {
                    if (ModelLeftWeapon!=null)
                    {
                        ItemBox1.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite
                        = Enemy.GetComponent<EnemyShared>() != null ? ModelLeftWeapon.GetComponent<Weapons>().ZatIconSprite : ModelLeftWeapon.GetComponent<Weapons>().IconSprite;
                        ItemBox1.transform.GetChild(0).localScale = WeaponScale;
                        ItemBox1.GetComponent<HUDCreateInfoBoard>().Text[0] = FindObjectOfType<AccessDatabase>().GetItemRealName(LeftWeaponName, "Weapon");
                    } else
                    {
                        ItemBox1.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite
                        = null;
                        ItemBox1.GetComponent<HUDCreateInfoBoard>().Text[0] = "";
                    }
                    if (ModelRightWeapon!=null)
                    {
                        ItemBox2.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite
                        = Enemy.GetComponent<EnemyShared>() != null ? ModelRightWeapon.GetComponent<Weapons>().ZatIconSprite : ModelRightWeapon.GetComponent<Weapons>().IconSprite;
                        ItemBox2.transform.GetChild(0).localScale = WeaponScale;
                        ItemBox2.GetComponent<HUDCreateInfoBoard>().Text[0] = FindObjectOfType<AccessDatabase>().GetItemRealName(RightWeaponName, "Weapon");
                    } else
                    {
                        ItemBox2.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite
                        = null;
                        ItemBox2.GetComponent<HUDCreateInfoBoard>().Text[0] = "";
                    }
                } else
                {
                    if (ModelFirstPower!=null)
                    {
                        ItemBox1.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite
                        = ModelFirstPower.GetComponent<SpriteRenderer>().sprite;
                        ItemBox1.transform.GetChild(0).localScale = PowerScale;
                        ItemBox1.GetComponent<HUDCreateInfoBoard>().Text[0] = FindObjectOfType<AccessDatabase>().GetItemRealName(FirstPower, "Power");
                    } else
                    {
                        ItemBox1.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite
                        = null;
                        ItemBox1.GetComponent<HUDCreateInfoBoard>().Text[0] = "";
                    }
                    if (ModelSecondPower!=null)
                    {
                        ItemBox2.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite
                        = ModelSecondPower.GetComponent<SpriteRenderer>().sprite;
                        ItemBox2.transform.GetChild(0).localScale = PowerScale;
                        ItemBox2.GetComponent<HUDCreateInfoBoard>().Text[0] = FindObjectOfType<AccessDatabase>().GetItemRealName(SecondPower, "Power");
                    } else
                    {
                        ItemBox2.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite
                        = null;
                        ItemBox2.GetComponent<HUDCreateInfoBoard>().Text[0] = "";
                    }
                }
                EnemyObject = Enemy.GetComponent<FighterShared>();
                if (EnemyObject.GetComponent<EnemyShared>() != null)
                {
                    NameBox.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 127 / 255f);
                    NameBox.transform.GetChild(0).GetComponent<TextMeshPro>().color = new Color(1, 1, 1, 127 / 255f);
                }
                else
                {
                    NameBox.GetComponent<SpriteRenderer>().color = new Color(153 / 255f, 173 / 255f, 212 / 255f, 127 / 255f);
                    NameBox.transform.GetChild(0).GetComponent<TextMeshPro>().color = new Color(1, 1, 1, 127 / 255f);
                }


            } else
            {
                if (Enemy.GetComponent<WSShared>() != null)
                {
                    // turn off scripts
                    if (CloneEnemy.GetComponent<WSShared>() != null)
                    {
                        Warship = CloneEnemy.GetComponent<WSShared>();
                        Warship.enabled = false;
                        // turn off component
                        CloneEnemyColl = Warship.GetComponent<PolygonCollider2D>();
                        CloneEnemyColl.enabled = false;

                        WSMovement move = CloneEnemy.GetComponent<WSMovement>();
                        move.enabled = false;

                        Destroy(CloneEnemy.GetComponent<AudioSource>());
                    }

                    Warship = Enemy.GetComponent<WSShared>();
                    if (Warship != null)
                    {
                        if (Warship.IsEnemy)
                        {
                            NameBox.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 127 / 255f);
                            NameBox.transform.GetChild(0).GetComponent<TextMeshPro>().color = new Color(1, 1, 1, 127 / 255f);
                        }
                        else
                        {
                            NameBox.GetComponent<SpriteRenderer>().color = new Color(153 / 255f, 173 / 255f, 212 / 255f, 127 / 255f);
                            NameBox.transform.GetChild(0).GetComponent<TextMeshPro>().color = new Color(1, 1, 1, 127 / 255f);
                        }
                        NameBox.transform.GetChild(0).GetComponent<TextMeshPro>().text = Warship.WarshipName;
                    }
                    LeftWeaponName = Enemy.GetComponent<WSShared>().MainWeapon[0];
                    RightWeaponName = Enemy.GetComponent<WSShared>().MainWeapon.Count > 1 ? Enemy.GetComponent<WSShared>().MainWeapon[1] : "";

                    bool alreadyLeft = false;
                    bool alreadyRight = false;
                    for (int i = 0; i < WsSsWeaponList.Count; i++)
                    {
                        if (alreadyLeft && alreadyRight)
                        {
                            break;
                        }
                        if (WsSsWeaponList[i].name.Replace(" ", "").ToLower().Contains(LeftWeaponName.Replace(" ", "").ToLower()))
                        {
                            alreadyLeft = true;
                            ModelLeftWeapon = WsSsWeaponList[i];
                        }
                        if (RightWeaponName != "")
                        {
                            if (WsSsWeaponList[i].name.Replace(" ", "").ToLower().Contains(RightWeaponName.Replace(" ", "").ToLower()))
                            {
                                alreadyRight = true;
                                ModelRightWeapon = WsSsWeaponList[i];
                            }
                        } else
                        {
                            ModelRightWeapon = null;
                        }
                    }

                    if (ModelLeftWeapon != null)
                    {
                        ItemBox1.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite
                        = ModelLeftWeapon.GetComponent<SpriteRenderer>().sprite;
                        ItemBox1.transform.GetChild(0).localScale = WeaponScale / 2;
                        ItemBox1.GetComponent<HUDCreateInfoBoard>().Text[0] = Enemy.GetComponent<WSShared>().MainWeapon[0];
                    }
                    else
                    {
                        ItemBox1.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite
                        = null;
                        ItemBox1.GetComponent<HUDCreateInfoBoard>().Text[0] = "";
                    }

                    if (ModelRightWeapon != null)
                    {
                        ItemBox2.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite
                        = ModelRightWeapon.GetComponent<SpriteRenderer>().sprite;
                        ItemBox2.transform.GetChild(0).localScale = WeaponScale / 2;
                        ItemBox2.GetComponent<HUDCreateInfoBoard>().Text[0] = Enemy.GetComponent<WSShared>().MainWeapon[1];
                    }
                    else
                    {
                        ItemBox2.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite
                        = null;
                        ItemBox2.GetComponent<HUDCreateInfoBoard>().Text[0] = "";
                    }

                } else
                {
                    if (Enemy.GetComponent<SpaceStationShared>() != null)
                    {
                        SpaceStation = CloneEnemy.GetComponent<SpaceStationShared>();
                        SpaceStation.enabled = false;
                        // turn off component
                        CloneEnemyColl = SpaceStation.GetComponent<PolygonCollider2D>();
                        CloneEnemyColl.enabled = false;
                        Destroy(CloneEnemy.GetComponent<AudioSource>());

                        SpaceStation = Enemy.GetComponent<SpaceStationShared>();
                        if (SpaceStation != null)
                        {
                            if (SpaceStation.isEnemy)
                            {
                                NameBox.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 127 / 255f);
                                NameBox.transform.GetChild(0).GetComponent<TextMeshPro>().color = new Color(1, 1, 1, 127 / 255f);
                            }
                            else
                            {
                                NameBox.GetComponent<SpriteRenderer>().color = new Color(153 / 255f, 173 / 255f, 212 / 255f, 127 / 255f);
                                NameBox.transform.GetChild(0).GetComponent<TextMeshPro>().color = new Color(1, 1, 1, 127 / 255f);
                            }
                            NameBox.transform.GetChild(0).GetComponent<TextMeshPro>().text = SpaceStation.SpaceStationName;

                            LeftWeaponName = Enemy.GetComponent<SpaceStationShared>().MainWeapon[0];
                            for (int i = 0; i < WsSsWeaponList.Count; i++)
                            {
                                if (WsSsWeaponList[i].name.Replace(" ", "").ToLower().Contains(LeftWeaponName.Replace(" ", "").ToLower()))
                                {
                                    ModelLeftWeapon = WsSsWeaponList[i];
                                }
                            }

                            if (ModelLeftWeapon != null)
                            {
                                ItemBox1.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite
                                = ModelLeftWeapon.GetComponent<SpriteRenderer>().sprite;
                                ItemBox1.transform.GetChild(0).localScale = WeaponScale / 2;
                                ItemBox1.GetComponent<HUDCreateInfoBoard>().Text[0] = Enemy.GetComponent<SpaceStationShared>().MainWeapon[0];
                            }
                            else
                            {
                                ItemBox1.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite
                                = null;
                                ItemBox1.GetComponent<HUDCreateInfoBoard>().Text[0] = "";
                            }

                                           
                            ItemBox2.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite
                            = null;
                            ItemBox2.GetComponent<HUDCreateInfoBoard>().Text[0] = "";
                            
                        }
                    }
                }
            }
            CloneEnemy.SetActive(true);
        }
        if (Enemy==null)
        {
            CloseBoard();
        } else
        {
            if (Enemy.GetComponent<FighterShared>() != null)
            {
                EnemyObject = Enemy.GetComponent<FighterShared>();
                NameBox.transform.GetChild(0).GetComponent<TextMeshPro>().text = EnemyObject.FighterName;
                HPSlider.maxValue = EnemyObject.MaxHP;
                HPSlider.value = EnemyObject.CurrentHP;

                //Set HP to show how much current HP
                HealthText.text = Mathf.Round(EnemyObject.CurrentHP) + "/" + EnemyObject.MaxHP;

                // Barrier
                BarrierSlider.maxValue = EnemyObject.MaxBarrier;
                BarrierSlider.value = EnemyObject.CurrentBarrier;

                BarrierText.text = Mathf.Round(EnemyObject.CurrentBarrier) + "/" + EnemyObject.MaxBarrier;

                //Setting slider base on current temperature
                TemperSlider.value = EnemyObject.currentTemperature;

                //if temp > 50, the slider is red else blue
                if (EnemyObject.currentTemperature > 50)
                {
                    TemperSlider.fillRect.GetComponentInChildren<Image>().color = Color.Lerp(Color.green, Color.red, TemperSlider.normalizedValue);
                }
                else if (EnemyObject.currentTemperature < 50)
                {
                    TemperSlider.fillRect.GetComponentInChildren<Image>().color = Color.Lerp(Color.blue, Color.green, TemperSlider.normalizedValue);
                }
                else if (EnemyObject.currentTemperature == 50)
                {
                    TemperSlider.fillRect.GetComponentInChildren<Image>().color = Color.green;
                }

                //Setting to show current tempurature
                TemperText.text = Mathf.Round(EnemyObject.currentTemperature * 10) / 10 + "\u00B0C";
            } else
            {
                if (Enemy.GetComponent<WSShared>() != null)
                {
                    Warship = Enemy.GetComponent<WSShared>();
                    HPBar.SetActive(true);
                    HPSlider.maxValue = Warship.MaxHP;
                    HPSlider.value = Warship.CurrentHP;

                    //Set HP to show how much current HP
                    HealthText.gameObject.SetActive(true);
                    HealthText.text = Mathf.Round(Warship.CurrentHP) + "/" + Warship.MaxHP;

                    // Barrier undone
                    BarrierText.gameObject.SetActive(true);
                    BarrierBar.SetActive(true);
                    BarrierSlider.maxValue = Warship.MaxBarrier;
                    BarrierSlider.value = Warship.CurrentBarrier;

                    BarrierText.text = Mathf.Round(Warship.CurrentBarrier) + "/" + Warship.MaxBarrier;

                    //Setting slider base on current temperature
                    TempBar.SetActive(true);
                    TemperSlider.maxValue = 100;
                    TemperSlider.value = 50;
                    TemperSlider.fillRect.GetComponentInChildren<Image>().color = Color.green;


                    //Setting to show current tempurature
                    TemperText.gameObject.SetActive(true);
                    TemperText.text = "-";
                } else
                {
                    if (Enemy.GetComponent<SpaceStationShared>() != null)
                    {
                        SpaceStation = Enemy.GetComponent<SpaceStationShared>();
                        HPBar.SetActive(true);
                        HPSlider.maxValue = SpaceStation.MaxHP;
                        HPSlider.value = SpaceStation.CurrentHP;

                        //Set HP to show how much current HP
                        HealthText.gameObject.SetActive(true);
                        HealthText.text = Mathf.Round(SpaceStation.CurrentHP) + "/" + SpaceStation.MaxHP;

                        // Barrier undone
                        BarrierText.gameObject.SetActive(true);
                        BarrierBar.SetActive(true);
                        BarrierSlider.maxValue = SpaceStation.MaxBarrier;
                        BarrierSlider.value = SpaceStation.CurrentBarrier;

                        BarrierText.text = Mathf.Round(SpaceStation.CurrentBarrier) + "/" + SpaceStation.MaxBarrier;

                        //Setting slider base on current temperature
                        TempBar.SetActive(true);
                        TemperSlider.maxValue = 100;
                        TemperSlider.value = 50;
                        TemperSlider.fillRect.GetComponentInChildren<Image>().color = Color.green;


                        //Setting to show current tempurature
                        TemperText.gameObject.SetActive(true);
                        TemperText.text = "-";
                    }
                }
            }
            

        }
    }

    // Stop showing board
    public void StopShowing(GameObject go)
    {
        if (LastEnemy==go)
        {
            gameObject.SetActive(false);
            Destroy(CloneEnemy);
        }
    }

    // Close board
    private IEnumerator CloseBoard()
    {
        isEnding = true;
        OkToDestroy = true;
        // Wait for aniamtion
        yield return new WaitForSeconds(0.1f);
        isEnding = false;
        if (OkToDestroy)
        {
            gameObject.SetActive(false);
        }
    }

    // Check if re-open board on closing phase
    public void CheckOnDestroy()
    {
        // Set Timer to close to 5s
        Timer = 3f;
        if (isEnding)
        {
            isEnding = false;
            // Deny destroy object
            OkToDestroy = false;
            // Stop the coroutine for closing board
            StopCoroutine(CloseBoard());
            // Show stats
            StartShowing(Enemy);
        }
    }

    private void SwitchWeaponPowerInfo()
    {
        if (Enemy.GetComponent<FighterShared>() != null)
        {
            ItemBox1.GetComponent<Collider2D>().enabled = false;
            ItemBox2.GetComponent<Collider2D>().enabled = false;
            if (isShowingWeapon)
            {
                isShowingWeapon = false;
                if (ModelFirstPower!=null)
                {
                    ItemBox1.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite
                        = ModelFirstPower.GetComponent<SpriteRenderer>().sprite;
                    ItemBox1.transform.GetChild(0).localScale = PowerScale;
                    ItemBox1.GetComponent<HUDCreateInfoBoard>().Text[0] = FindObjectOfType<AccessDatabase>().GetItemRealName(FirstPower, "Power");
                } else
                {
                    ItemBox1.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite
                        = null;
                    ItemBox1.GetComponent<HUDCreateInfoBoard>().Text[0] = "";
                }
                if (ModelSecondPower!=null)
                {
                    ItemBox2.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite
                        = ModelSecondPower.GetComponent<SpriteRenderer>().sprite;
                    ItemBox2.transform.GetChild(0).localScale = PowerScale;
                    ItemBox2.GetComponent<HUDCreateInfoBoard>().Text[0] = FindObjectOfType<AccessDatabase>().GetItemRealName(SecondPower, "Power");
                } else
                {
                    ItemBox2.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite
                        = null;
                    ItemBox2.GetComponent<HUDCreateInfoBoard>().Text[0] = "";
                }
            } else
            {
                isShowingWeapon = true;
                if (ModelLeftWeapon != null)
                {
                    ItemBox1.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite
                    = Enemy.GetComponent<EnemyShared>() != null ? ModelLeftWeapon.GetComponent<Weapons>().ZatIconSprite : ModelLeftWeapon.GetComponent<Weapons>().IconSprite;
                    ItemBox1.transform.GetChild(0).localScale = WeaponScale;
                    ItemBox1.GetComponent<HUDCreateInfoBoard>().Text[0] = FindObjectOfType<AccessDatabase>().GetItemRealName(LeftWeaponName, "Weapon");
                }
                else
                {
                    ItemBox1.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite
                    = null;
                    ItemBox1.GetComponent<HUDCreateInfoBoard>().Text[0] = "";
                }
                if (ModelRightWeapon != null)
                {
                    ItemBox2.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite
                    = Enemy.GetComponent<EnemyShared>() != null ? ModelRightWeapon.GetComponent<Weapons>().ZatIconSprite : ModelRightWeapon.GetComponent<Weapons>().IconSprite;
                    ItemBox2.transform.GetChild(0).localScale = WeaponScale;
                    ItemBox2.GetComponent<HUDCreateInfoBoard>().Text[0] = FindObjectOfType<AccessDatabase>().GetItemRealName(RightWeaponName, "Weapon");
                }
                else
                {
                    ItemBox2.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite
                    = null;
                    ItemBox2.GetComponent<HUDCreateInfoBoard>().Text[0] = "";
                }
            }
            StartCoroutine(ResetCollider());

        }
    }

    private IEnumerator ResetCollider()
    {
        yield return new WaitForSeconds(0.1f);
        ItemBox1.GetComponent<Collider2D>().enabled = true;
        ItemBox2.GetComponent<Collider2D>().enabled = true;
    }
    #endregion
}
