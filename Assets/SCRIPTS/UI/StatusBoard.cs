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
    public TMP_Text HealthText;
    public Slider HPSlider;
    public TMP_Text TemperText;
    public Slider TemperSlider;

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
    private EnemyShared EnemyObject;
    private EnemyShared CloneEnemyObject;
    private Rigidbody2D CloneEnemyRb2D;
    private Collider2D CloneEnemyColl;
    private float ImageInitScaleX;
    private float ImageInitScaleY;
    private bool isEnding;
    private bool startCounting;
    private bool OkToDestroy;
    private bool alreadyDelete;
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
        for (int i=0; i< 8; i++)
        {
            Physics2D.IgnoreLayerCollision(i, gameObject.layer, true);
        }
        alreadyDelete = false;
    }

    // Update is called once per frame
    void Update()
    {
        
        // React When Player Zoom out/ close
        ReactWhenZoom();
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
                    HealthText.gameObject.SetActive(false);
                    HPSlider.gameObject.SetActive(false);
                    TemperText.gameObject.SetActive(false);
                    TemperSlider.gameObject.SetActive(false);
                    alreadyDelete = true;
                }
                StartCoroutine(CloseBoard());
            }
        }
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
            // set Sorting order
            CloneEnemy.GetComponent<SpriteRenderer>().sortingOrder = 100;
            // Set color and transparency
            Color c = CloneEnemy.GetComponent<SpriteRenderer>().color;
            c.a = 0.5f;
            c.r = 1;
            c.g = 1;
            c.b = 1;
            CloneEnemy.GetComponent<SpriteRenderer>().color = c;
            // set Clone Enemy's parent as this board
            CloneEnemy.transform.SetParent(transform);
            // Destroy objects need to be destroyed so it wont interact
            Destroy(CloneEnemy.transform.GetChild(1).gameObject);

            // turn off scripts
            CloneEnemyObject = CloneEnemy.GetComponent<TestDisk>();
            CloneEnemyObject.enabled = false;
            // turn off component
            CloneEnemyRb2D = CloneEnemy.GetComponent<Rigidbody2D>();
            CloneEnemyRb2D.simulated = false;
            CloneEnemyColl = CloneEnemy.GetComponent<Collider2D>();
            CloneEnemyColl.enabled = false;
        }

        // WAit for 1s to show HP and temp bar
        yield return new WaitForSeconds(0.4f);
        EnemyObject = Enemy.GetComponent<EnemyShared>();
        HPSlider.gameObject.SetActive(true);
        HPSlider.maxValue = EnemyObject.MaxHP;
        HPSlider.value = EnemyObject.CurrentHP;

        //Set HP to show how much current HP
        HealthText.gameObject.SetActive(true);
        HealthText.text = Mathf.Round(EnemyObject.CurrentHP) + "/" + EnemyObject.MaxHP;

        //Setting slider base on current temperature
        TemperSlider.gameObject.SetActive(true);
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
        TemperText.text = EnemyObject.currentTemperature + "°C";
        if (Timer==0f)
        {
            Timer = 1000f;
        }
        startCounting = true;
        isShow = true;
    }

    public void UpdateStatus()
    {
        EnemyObject = Enemy.GetComponent<EnemyShared>();
        HPSlider.maxValue = EnemyObject.MaxHP;
        HPSlider.value = EnemyObject.CurrentHP;

        //Set HP to show how much current HP
        HealthText.text = Mathf.Round(EnemyObject.CurrentHP) + "/" + EnemyObject.MaxHP;

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
        TemperText.text = EnemyObject.currentTemperature + "°C";
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

    private void OnMouseOver()
    {
        CheckOnDestroy();
    }
}
