using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    private Text startGameText;
    [SerializeField]
    private Text pointsText;
    [SerializeField]
    private GameObject gameOverObject;
    [SerializeField]
    private Text gameOverText;
    [SerializeField]
    private Text scoreText;
    [SerializeField]
    private Text bestScoreText;
    [SerializeField]
    private Button restartButton;

    private float maxDistanceX;
    private float cameraDistance;
    private float holePositionX;
    private float flagPositionX;
    private int points = 0;
    private int currentParabolaPointPosition;
    private int nextParabolaPointPosition;
    private int force;
    private Rigidbody2D rb2D;
    private Vector2 ballStartPosition;
    private Vector2 parabolaPointStartPosition;
    private float parabolaPointSpeed;
    private bool isGameStart;

    public static int bestScore = 0;

    private void Awake ()
    {
        cameraDistance = Vector3.Distance(transform.position , Camera.main.transform.position);
        Vector2 topCorner = Camera.main.ViewportToWorldPoint(new Vector3(1.0f, 1.0f, cameraDistance));
        maxDistanceX = topCorner.x;

        ChangingHolePosition();

        parabolaPoint.position = transform.position;

        startGameText.transform.position = new Vector3(0.5f * Screen.width, 0.65f * Screen.height, 0.0f);
        pointsText.transform.position = new Vector3(0.9f * Screen.width, 0.85f * Screen.height, 0.0f);

        gameOverText.transform.position = new Vector3(0.5f * Screen.width, 0.7f * Screen.height, 0.0f);
        scoreText.transform.position = new Vector3(0.3f * Screen.width, 0.5f * Screen.height, 0.0f);
        bestScoreText.transform.position = new Vector3(0.7f * Screen.width, 0.5f * Screen.height, 0.0f);
        restartButton.transform.position = new Vector3(0.5f * Screen.width, 0.3f * Screen.height, 0.0f);

        ShowingPoints();

        RestartParabolaPointPosition();

        force = 0;
        rb2D = GetComponent<Rigidbody2D>();

        ballStartPosition = new Vector2(transform.position.x, transform.position.y);
        parabolaPointStartPosition = new Vector2(parabolaPoint.transform.position.x, parabolaPoint.transform.position.y);
        parabolaPointSpeed = 1;
        isGameStart = false;
        gameOverObject.SetActive(false);
    }

    private void Update ()
    {
        ShowingPoints();
        ParabolaImpactForce();

        currentParabolaPointPosition = (int)parabolaPoint.transform.localPosition.x;

        if (currentParabolaPointPosition == nextParabolaPointPosition)
        {
            force++;
            nextParabolaPointPosition = currentParabolaPointPosition + 1;
        }
        
        if (Input.GetKeyUp(KeyCode.Space))
        {
            BallIsFly();
        }

        if (bestScore < points)
        {
            bestScore = points;
        }
    }

    private void ChangingHolePosition()
    {
        holePositionX = Random.Range(0, maxDistanceX - 1.0f);
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
            startGameText.gameObject.SetActive(false);
            rb2D.constraints = RigidbodyConstraints2D.None;
            parabolaPoint.transform.Translate(Vector2.right * parabolaPointSpeed * Time.deltaTime);
            if (parabolaPoint.transform.localPosition.x >= maxDistanceX)
            {
                parabolaPoint.transform.Translate(Vector2.zero);
                BallIsFly();
            }
            isGameStart = true;
        }
    }

    private void RestartParabolaPointPosition()
    {
        currentParabolaPointPosition = (int)parabolaPoint.transform.localPosition.x;
        nextParabolaPointPosition = currentParabolaPointPosition + 1;
    }

    private void BallIsFly()
    {
        Vector2 direction = new Vector2(1, 1);
        rb2D.AddForce(direction * Mathf.Sqrt(Mathf.Abs(Physics.gravity.y) * force / 2), ForceMode2D.Impulse);
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if(collision.gameObject.CompareTag("Hole"))
        {
            isGameStart = false;
            points += 1;
            ChangingHolePosition();
            transform.position = ballStartPosition;
            rb2D.constraints = RigidbodyConstraints2D.FreezeAll;
            parabolaPoint.transform.position = parabolaPointStartPosition;
            force = 0;
            RestartParabolaPointPosition();
            parabolaPointSpeed++;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (isGameStart)
            {
                scoreText.text = "SCORE: " + points.ToString();
                bestScoreText.text = "BEST: " + bestScore.ToString();
                gameOverObject.SetActive(true);
                rb2D.constraints = RigidbodyConstraints2D.FreezeAll;
            }
        }
    }

    public void RestartGameButton()
    {
        SceneManager.LoadScene("GameScene");
    }

}
