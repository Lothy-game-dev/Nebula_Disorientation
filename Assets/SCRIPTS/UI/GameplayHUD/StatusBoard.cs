using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusBoard : MonoBehaviour
{
    #region ComponentVariables
    private Animator anim;
    #endregion
    #region InitializeVariables
    // Status related
    public GameController GameController;
    // Zoom out and close related
    public GameObject ZoomOutPosition;
    public GameObject ClosePosition;
    //Status
    public GameObject NameBox;
    public GameObject WeaponBox;
    public GameObject PowerBox;
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
    // Check mouse
    public GameObject Left;
    public GameObject Right;
    public GameObject Top;
    public GameObject Bottom;
    //Image
    public GameObject EnemyImagePosition;

    #endregion
    #region NormalVariables
    public GameObject Enemy;
    // Time Counter
    public float Timer;
    // Is hide or not 
    public bool isShow;
    private float initScale;
    private GameObject CloneEnemy;
    private FighterShared EnemyObject;
    private EnemyShared CloneEnemyObject;
    private Rigidbody2D CloneEnemyRb2D;
    private Collider2D CloneEnemyColl;
    private float ImageInitScaleX;
    private float ImageInitScaleY;
    private bool isEnding;
    private bool startCounting;
    private bool OkToDestroy;
    private bool alreadyDelete;
    private GameObject LastEnemy;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        initScale = transform.localScale.x;
        if (Enemy != null)
        {
            ImageInitScaleX = Enemy.transform.localScale.x * Enemy.GetComponent<EnemyShared>().ScaleOnStatusBoard / initScale;
            ImageInitScaleY = Enemy.transform.localScale.y * Enemy.GetComponent<EnemyShared>().ScaleOnStatusBoard / initScale;
        }
        alreadyDelete = false;
    }

    // Update is called once per frame
    void Update()
    {
        
        // React When Player Zoom out/ close
        ReactWhenZoom();
        if (!isMouseOutsideRange())
        {
            CheckOnDestroy();
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
                    WeaponBox.SetActive(false);
                    PowerBox.SetActive(false);
                    HealthText.gameObject.SetActive(false);
                    HPBar.SetActive(false);
                    BarrierText.gameObject.SetActive(false);
                    BarrierBar.SetActive(false);
                    TemperText.gameObject.SetActive(false);
                    TempBar.SetActive(false);
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
    #endregion
    #region React When Zoom Out/Close
    // React to zoom
    private void ReactWhenZoom()
    {
        if (GameController.IsClose)
        {
            // If close -> All range and scale is half
            transform.position = new Vector3(ClosePosition.transform.position.x, ClosePosition.transform.position.y, transform.position.z);
            transform.localScale = new Vector3(initScale / 2, initScale / 2, initScale / 2);
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
            transform.localScale = new Vector3(initScale, initScale, initScale);
        }
    }
    #endregion
    #region Show Status
    // Start showing enemy
    public void StartShowing(GameObject enemy)
    {
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
        // Wait for animation 1s to show enemy
        yield return new WaitForSeconds(0.6f);
        if (CloneEnemy == null)
        {
            CloneEnemy = Instantiate(Enemy, EnemyImagePosition.transform.position, Quaternion.identity);
            LastEnemy = Enemy;
            // set Sorting order
            CloneEnemy.GetComponent<SpriteRenderer>().sortingOrder = 300;
            // Set color and transparency
            Color c = CloneEnemy.GetComponent<SpriteRenderer>().color;
            c.a = 1f;
            c.r = 1;
            c.g = 1;
            c.b = 1;
            CloneEnemy.GetComponent<SpriteRenderer>().color = c;
            // set Clone Enemy's parent as this board
            CloneEnemy.transform.SetParent(transform);
            // Destroy objects need to be destroyed so it wont interact
            for (int i=0;i<CloneEnemy.transform.childCount;i++)
            {
                Destroy(CloneEnemy.transform.GetChild(i).gameObject);
            }
            // turn off scripts
            CloneEnemyObject = CloneEnemy.GetComponent<EnemyShared>();
            CloneEnemyObject.enabled = false;
            FighterMovement fm = CloneEnemy.GetComponent<FighterMovement>();
            fm.enabled = false;
            // turn off component
            CloneEnemyRb2D = CloneEnemy.GetComponent<Rigidbody2D>();
            CloneEnemyRb2D.simulated = false;
            CloneEnemyColl = CloneEnemy.GetComponent<Collider2D>();
            CloneEnemyColl.enabled = false;
        }
        NameBox.SetActive(true);
        NameBox.transform.GetChild(0).GetComponent<TextMeshPro>().text = EnemyObject.FighterName;
        // WAit time to show HP and temp bar
        yield return new WaitForSeconds(0.4f);
        WeaponBox.SetActive(true);
        PowerBox.SetActive(true);
        /*StatusBox.SetActive(true);*/
        EnemyObject = Enemy.GetComponent<EnemyShared>();
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
        TemperText.text = Mathf.Round(EnemyObject.currentTemperature * 10) / 10 + "°C";
        if (Timer==0f)
        {
            Timer = 1000f;
        }
        startCounting = true;
        isShow = true;
    }

    public void UpdateStatus()
    {
        if (Enemy!= LastEnemy)
        {
            Destroy(CloneEnemy);
            CloneEnemy = Instantiate(Enemy, EnemyImagePosition.transform.position, Quaternion.identity);
            // set Sorting order
            CloneEnemy.GetComponent<SpriteRenderer>().sortingOrder = 300;
            // Set color and transparency
            Color c = CloneEnemy.GetComponent<SpriteRenderer>().color;
            c.a = 1f;
            c.r = 1;
            c.g = 1;
            c.b = 1;
            CloneEnemy.GetComponent<SpriteRenderer>().color = c;
            // set Clone Enemy's parent as this board
            CloneEnemy.transform.SetParent(transform);
            // Destroy objects need to be destroyed so it wont interact
            for (int i = 0; i < CloneEnemy.transform.childCount; i++)
            {
                Destroy(CloneEnemy.transform.GetChild(i).gameObject);
            }
            // turn off scripts
            CloneEnemyObject = CloneEnemy.GetComponent<EnemyShared>();
            CloneEnemyObject.enabled = false;
            FighterMovement fm = CloneEnemy.GetComponent<FighterMovement>();
            fm.enabled = false;
            // turn off component
            CloneEnemyRb2D = CloneEnemy.GetComponent<Rigidbody2D>();
            CloneEnemyRb2D.simulated = false;
            CloneEnemyColl = CloneEnemy.GetComponent<Collider2D>();
            CloneEnemyColl.enabled = false;
        }
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
        } else if (EnemyObject.currentTemperature < 50)
        {
            TemperSlider.fillRect.GetComponentInChildren<Image>().color = Color.Lerp(Color.blue, Color.green, TemperSlider.normalizedValue);
        } else if (EnemyObject.currentTemperature == 50)
        {
            TemperSlider.fillRect.GetComponentInChildren<Image>().color = Color.green;
        }

        //Setting to show current tempurature
        TemperText.text = Mathf.Round(EnemyObject.currentTemperature * 10) / 10 + "°C";
    }

    // Stop showing board
    public void StopShowing()
    {
        gameObject.SetActive(false);
        Destroy(CloneEnemy);
    }

    // Close board
    private IEnumerator CloseBoard()
    {
        isEnding = true;
        OkToDestroy = true;
        anim.SetTrigger("End");
        // Wait for aniamtion
        yield return new WaitForSeconds(1f);
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
        Timer = 5f;
        if (isEnding)
        {
            isEnding = false;
            // Remove trigger end from animator
            anim.ResetTrigger("End");
            // Deny destroy object
            OkToDestroy = false;
            // Stop the coroutine for closing board
            StopCoroutine(CloseBoard());
            // Trigger Start anim again
            anim.SetTrigger("Start");
            // Show stats
            StartShowing(Enemy);
        }
    }
    #endregion
}
