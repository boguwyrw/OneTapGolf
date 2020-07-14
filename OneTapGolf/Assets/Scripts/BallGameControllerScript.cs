using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallGameControllerScript : MonoBehaviour
{

    [SerializeField]
    private Transform hole;
    [SerializeField]
    private Transform flag;
    [SerializeField]
    private Transform parabolaPoint;
    [SerializeField]
    private Text pointsText;

    private float maxDistance;
    private float cameraDistance;
    private float holePositionX;
    private float flagPositionX;

    private int points = 0;
    private int currentParabolaPointPosition;
    private int nextParabolaPointPosition;
    private int force;
    private Rigidbody2D rb2D;

    private void Awake ()
    {
        cameraDistance = Vector3.Distance(transform.position , Camera.main.transform.position);
        Vector2 topCorner = Camera.main.ViewportToWorldPoint(new Vector3(1.0f, 1.0f, cameraDistance));
        maxDistance = topCorner.x - 1.0f;

        ChangingHolePosition();

        parabolaPoint.position = transform.position;

        pointsText.transform.position = new Vector3(0.9f * Screen.width, 0.85f * Screen.height, 0.0f);
        ShowingPoints();

        currentParabolaPointPosition = (int)parabolaPoint.transform.localPosition.x;
        nextParabolaPointPosition = currentParabolaPointPosition + 1;
        force = 0;
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void Update ()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            ChangingHolePosition();
        }

        ShowingPoints();
        ParabolaImpactForce();

        currentParabolaPointPosition = (int)parabolaPoint.transform.localPosition.x;
        Debug.Log("currentParabolaPointPosition: " + currentParabolaPointPosition);
        Debug.Log("nextParabolaPointPosition: " + nextParabolaPointPosition);
        if (currentParabolaPointPosition == nextParabolaPointPosition)
        {
            force++;
            nextParabolaPointPosition = currentParabolaPointPosition + 1;
        }
        Debug.Log(force);
        /*
        if (Input.GetKeyUp(KeyCode.Space))
        {
            transform.localPosition = new Vector2(parabolaPoint.position.x, parabolaPoint.position.y);
        }
        */
        if (Input.GetKeyUp(KeyCode.Space))
        {
            rb2D.AddForce(transform.up * force, ForceMode2D.Impulse);
            rb2D.AddForce(transform.right * force, ForceMode2D.Impulse);
        }
    }
    /*
    void FixedUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            rb2D.AddForce(transform.up * Mathf.Abs(currentParabolaPointPosition), ForceMode2D.Impulse);
            rb2D.AddForce(transform.right * Mathf.Abs(currentParabolaPointPosition), ForceMode2D.Impulse);
        }
    }
    */
    private void ChangingHolePosition()
    {
        holePositionX = Random.Range(0, maxDistance);
        flagPositionX = holePositionX;
        hole.position = new Vector2(holePositionX, hole.transform.position.y);
        flag.position = new Vector2(flagPositionX, flag.transform.position.y);
    }

    private void ShowingPoints()
    {
        pointsText.text = points.ToString();
    }

    private void ParabolaImpactForce()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            parabolaPoint.transform.Translate(Vector2.right * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Hole"))
        {
            points++;
        }
    }

}
