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
    private Transform parabolaPoint; // end point of the parabola (ball landing point)
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
    [SerializeField]
    private Button exitButton;

    private float maxDistanceX; // maximum screen width
    private float maxDistanceY; // maximum screen height
    private float cameraDistance;
    private float holePositionX;
    private float flagPositionX;
    private int points = 0;
    private Rigidbody2D rb2D;
    private Vector2 ballStartPosition;
    private Vector2 parabolaPointStartPosition;
    private float parabolaPointSpeed; // speed of end point of the parabola (speed of ball landing point)
    private bool isGameStart;
    private float ballForce; // distance between start point and end point of parabola
    private bool ballFly;

    public static int bestScore = 0;

    // Ball Trajectory
    [SerializeField]
    private GameObject pointsParent; // every parabola point will be created as a child of this object
    [SerializeField]
    private GameObject pointPrefab; // object will be used as parabola points (child of pointsParent)

    private int numberOfPoints; // number of points in parabola
    private Transform[] pathPointList;
    private float spacePoints; // distance between parabola points
    private float timeStamp; // time using in parabola equation
    private Vector2 pointPosition; // current point position in parabola
    private Vector2 direction;

    private void Awake ()
    {
        // calculation maximum screen size
        cameraDistance = Vector3.Distance(transform.position , Camera.main.transform.position);
        Vector2 topCorner = Camera.main.ViewportToWorldPoint(new Vector3(1.0f, 1.0f, cameraDistance));
        maxDistanceX = topCorner.x;
        maxDistanceY = topCorner.y;

        ChangingHolePosition();

        parabolaPoint.position = transform.position;

        startGameText.transform.position = new Vector3(0.5f * Screen.width, 0.65f * Screen.height, 0.0f);
        pointsText.transform.position = new Vector3(0.9f * Screen.width, 0.85f * Screen.height, 0.0f);

        gameOverText.transform.position = new Vector3(0.5f * Screen.width, 0.7f * Screen.height, 0.0f);
        scoreText.transform.position = new Vector3(0.3f * Screen.width, 0.5f * Screen.height, 0.0f);
        bestScoreText.transform.position = new Vector3(0.7f * Screen.width, 0.5f * Screen.height, 0.0f);
        restartButton.transform.position = new Vector3(0.5f * Screen.width, 0.3f * Screen.height, 0.0f);

        exitButton.transform.position = new Vector3(0.05f * Screen.width, 0.925f * Screen.height, 0.0f);

        ShowingPoints();

        rb2D = GetComponent<Rigidbody2D>();

        ballStartPosition = new Vector2(transform.position.x, transform.position.y);
        parabolaPointStartPosition = new Vector2(parabolaPoint.transform.position.x, parabolaPoint.transform.position.y);
        parabolaPointSpeed = 1;
        isGameStart = false;
        gameOverObject.SetActive(false);
        ballForce = 0.0f;
        ballFly = false;

        // Ball Trajectory
        numberOfPoints = 40;
        spacePoints = 0.05f;
    }

    private void Start()
    {
        PreparePathPoint();
        direction = new Vector2(1, 1);
        pointsParent.SetActive(false);
    }

    private void Update ()
    {
        ShowingPoints();
        ParabolaImpactForce();
       
        if (Input.GetKeyUp(KeyCode.Space) && parabolaPoint.transform.localPosition.x < maxDistanceX)
        {
            if (!ballFly)
            {
                BallIsFly();
                parabolaPoint.gameObject.SetActive(false);
                ballFly = true;
            }
            pointsParent.SetActive(false);
        }

        if (bestScore < points)
        {
            bestScore = points;
        }

        if (parabolaPoint.transform.localPosition.x >= maxDistanceX && !ballFly)
        {
            // Distance used in ParabolaImpactForce()
            ballForce = Vector2.Distance(parabolaPointStartPosition, new Vector2(maxDistanceX, parabolaPoint.transform.position.y));
            BallIsFly();
            parabolaPoint.gameObject.SetActive(false);
            parabolaPoint.transform.Translate(Vector2.zero);
            ballFly = true;
            pointsParent.SetActive(false);
        }
        else if (!ballFly)
        {
            // Distance used in ParabolaImpactForce()
            ballForce = Vector2.Distance(parabolaPointStartPosition, new Vector2(parabolaPoint.transform.position.x, parabolaPoint.transform.position.y));
        }

        if (transform.position.y > maxDistanceY)
        {
            GameOver();
        }

        
    }

    private void ChangingHolePosition()
    {
        holePositionX = Random.Range(0, maxDistanceX - 1.0f);
        flagPositionX = holePositionX;
        hole.position = new Vector2(holePositionX, hole.transform.position.y);
        flag.position = new Vector2(flagPositionX, flag.transform.position.y);
        parabolaPoint.gameObject.SetActive(true);
    }

    private void ShowingPoints()
    {
        pointsText.text = points.ToString();
    }

    // Method of creating parabola in current time
    private void ParabolaImpactForce()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            startGameText.gameObject.SetActive(false);
            if (!ballFly)
            {
                pointsParent.SetActive(true);
                parabolaPoint.transform.Translate(Vector2.right * parabolaPointSpeed * Time.deltaTime);
            }
            rb2D.constraints = RigidbodyConstraints2D.None;
            CreatingParabola(transform.position, (direction * Mathf.Sqrt(Mathf.Abs(Physics.gravity.y) * ballForce / 2)));
            isGameStart = true;
        }
    }

    // Adding force to the ball and creating ball movement (fly)
    private void BallIsFly()
    {
        rb2D.AddForce(direction * Mathf.Sqrt(Mathf.Abs(Physics.gravity.y) * ballForce / 2), ForceMode2D.Impulse);
    }

    private void GameOver()
    {
        scoreText.text = "SCORE: " + points.ToString();
        bestScoreText.text = "BEST: " + bestScore.ToString();
        gameOverObject.SetActive(true);
        rb2D.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    // Ball Trajectory

    private void PreparePathPoint()
    {
        pathPointList = new Transform[numberOfPoints];
        for (int i = 0; i < numberOfPoints; i++)
        {
            pathPointList[i] = Instantiate(pointPrefab.transform);
            pathPointList[i].parent = pointsParent.transform;
        }
    }

    private void CreatingParabola(Vector2 ballPosition, Vector2 addedForce)
    {
        if (!ballFly)
        {
            timeStamp = spacePoints;
            for (int i = 0; i < numberOfPoints; i++)
            {
                pointPosition.x = ballPosition.x + addedForce.x * timeStamp;
                pointPosition.y = (ballPosition.y + addedForce.y * timeStamp) - (Physics2D.gravity.magnitude * timeStamp * timeStamp) / 2.0f;

                pathPointList[i].position = pointPosition;
                timeStamp += spacePoints;
            }
        }  
    }

    // Triggers and Collisions
    // ball hit hole
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
            parabolaPointSpeed++;
            ballFly = false;
        }
    }
    // ball misses hole
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (isGameStart)
            {
                GameOver();
            }
        }
    }

    // Methods for buttons

    public void RestartGameButton()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}
