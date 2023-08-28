using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusBoard : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Render related
    public GameController GameController;
    // Zoom out and close related
    public GameObject ZoomOutPosition;
    public GameObject ClosePosition;
    // Time Counter
    public float Timer;
    // Is hide or not 
    public bool isShow;
    //Status
    public TMP_Text HealthText;
    public Slider HPSlider;
    public TMP_Text TemperText;
    public Slider TemperSlider;

    //Image
    public GameObject EnemyImagePosition;
    public float AvatarHeight;
    public float AvatarWidth;

    #endregion
    #region NormalVariables
    public GameObject Enemy;
    private float initScale;
    private GameObject CloneEnemy;
    private TestDisk EnemyObject;
    private TestDisk CloneEnemyObject;
    private Rigidbody2D CloneEnemyRb2D;
    private Collider2D CloneEnemyColl;
    private float ImageInitScaleX;
    private float ImageInitScaleY;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        initScale = transform.localScale.x;
        if (Enemy != null)
        {
            ImageInitScaleX = Enemy.transform.localScale.x * 3 / transform.localScale.x;
            ImageInitScaleY = Enemy.transform.localScale.y * 3 / transform.localScale.y;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        // React When Player Zoom out/ close
        ReactWhenZoom();
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
            if (CloneEnemy!=null)
            {
                CloneEnemy.transform.localScale = new Vector2(ImageInitScaleX, ImageInitScaleY);
            }
        }
        else
        {
            // If zoom out -> All range and scale is back to normal
            transform.position = new Vector3(ZoomOutPosition.transform.position.x, ZoomOutPosition.transform.position.y, transform.position.z);
            transform.localScale = new Vector3(initScale, initScale, initScale);
            if (CloneEnemy != null)
            {
                CloneEnemy.transform.localScale = new Vector2(ImageInitScaleX, ImageInitScaleY);
            }
        }
    }
    #endregion
    #region Show Status

    public void ShowStatus()
    {
        if (gameObject.activeSelf)
        {
            if (CloneEnemy == null)
            {
                CloneEnemy = Instantiate(Enemy, EnemyImagePosition.transform.position, Quaternion.identity);
                CloneEnemy.GetComponent<SpriteRenderer>().sortingOrder = 100;
                Color c = CloneEnemy.GetComponent<SpriteRenderer>().color;
                c.a = 0.5f;
                CloneEnemy.GetComponent<SpriteRenderer>().color = c;
                CloneEnemy.transform.SetParent(transform);
                // turn off scripts
                CloneEnemyObject = CloneEnemy.GetComponent<TestDisk>();
                CloneEnemyObject.enabled = false;
                // turn off component
                CloneEnemyRb2D = CloneEnemy.GetComponent<Rigidbody2D>();
                CloneEnemyRb2D.simulated = false;
                CloneEnemyColl = CloneEnemy.GetComponent<Collider2D>();
                CloneEnemyColl.enabled = false;
            }
            EnemyObject = Enemy.GetComponent<TestDisk>();

            //Setting slider base on current HP
            HPSlider.gameObject.SetActive(true);
            HPSlider.maxValue = EnemyObject.MaxHP;
            HPSlider.value = EnemyObject.CurrentHP;

            //Set HP to show how much current HP
            HealthText.gameObject.SetActive(true);
            HealthText.text = EnemyObject.CurrentHP + "/" + EnemyObject.MaxHP;

            //Setting slider base on current temperature
            TemperSlider.gameObject.SetActive(true);
            TemperSlider.maxValue = 100;
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
            TemperText.gameObject.SetActive(true);
            TemperText.text = EnemyObject.currentTemperature + "°C";

            

        }

        
    }
    
    public void DeleteClone(bool option)
    {
        if (option)
        {
            Destroy(CloneEnemy);
        }
    }

    #endregion
}
