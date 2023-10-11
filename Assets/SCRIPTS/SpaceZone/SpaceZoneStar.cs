using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceZoneStar : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject Explosion;
    public GameObject Range;
    public GameObject Stardust;
    #endregion
    #region NormalVariables
    public float currentHP;
    public float maxHP;
    private bool alreadyDestroy;
    private float RotateSpeed;
    private float delaySpawnStardust;
    private Vector2 MovingVector;
    private float Radius;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        transform.Rotate(new Vector3(0, 0, 2 + RotateSpeed));
        if (currentHP <= 0f)
        {
            if (!alreadyDestroy)
            {
                alreadyDestroy = true;
                StartCoroutine(StarDestroy());
            }
        }
        delaySpawnStardust -= Time.deltaTime;
        if (delaySpawnStardust <= 0f)
        {
            delaySpawnStardust = 0.1f;
            SpawnStardust();
        }
    }
    #endregion
    #region Initialize
    public void InitializeStar(Vector2 EndPos, float Speed)
    {
        MovingVector = EndPos - new Vector2(transform.position.x, transform.position.y);
        Radius = (Range.transform.position - transform.position).magnitude;
        GetComponent<Rigidbody2D>().velocity = MovingVector / MovingVector.magnitude * Speed;
        Destroy(gameObject, 25f);
        maxHP = 30000;
        currentHP = maxHP;
    }
    
    private void SpawnStardust()
    {
        for (int i=0; i<5; i++)
        {
            float angle = Random.Range(1, 90);
            float tempCos = Mathf.Cos(angle * Mathf.Deg2Rad) * Radius;
            float tempSin = Mathf.Sin(angle * Mathf.Deg2Rad) * Radius;
            Vector2 newPos = -MovingVector / MovingVector.magnitude * tempCos + new Vector2(transform.position.x, transform.position.y);
            Vector2 perpen = Vector2.Perpendicular(MovingVector);
            Vector2 finalPos = newPos + perpen / perpen.magnitude * tempSin * (Random.Range(0, 2) - 0.5f) * 2;
            GameObject stard = Instantiate(Stardust, finalPos, Quaternion.identity);
            stard.transform.localScale = stard.transform.localScale * Random.Range(0.2f, 1f);
            stard.SetActive(true);
            Destroy(stard, 1 * (90f - angle)/90f);
        }
    }

    public IEnumerator StarDestroy()
    {
        GetComponent<Collider2D>().enabled = false;
        for (int i=0; i<10; i++)
        {
            RotateSpeed = i;
            GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity * (10-i)/10;
            Color c = GetComponent<SpriteRenderer>().color;
            c.r -= 1 / 10f;
            c.b -= 1 / 10f;
            c.g -= 1 / 10f;
            GetComponent<SpriteRenderer>().color = c;
            for (int k=0; k< i/2 + 1; k++)
            {
                GameObject expl = Instantiate(Explosion, new Vector2(transform.position.x + Random.Range(-Radius, Radius), transform.position.y + Random.Range(-Radius, Radius)), Quaternion.identity);
                expl.SetActive(true);
                Destroy(expl, 0.3f);
            }
            yield return new WaitForSeconds(0.1f);
        }
        Destroy(gameObject);
    }
    #endregion
}
