using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Stack : MonoBehaviour
{
    private ColorPicker colorPicker;
    private GameState gameState;
    [SerializeField] private Loader loader;
    [SerializeField] private GameObject MenuPanel;
    [SerializeField] private GameObject GamePanel;
    [SerializeField] private PlatformBehaviour platformPrefab;
    [SerializeField] private Text currentScoreTxt;
    private List<PlatformBehaviour> cashedPlatforms = new List<PlatformBehaviour>();
    private int currentScore, maxScore;
    private const string maxScoreKey = "maxScoreKey";


    //Displacement Variables
    private Vector3 nextPosition, cashedPos, startPos;
    private float timeLerped = 0.0f;
    private float timeToLerp = 0.45f;

    //Platform Variables
    [SerializeField] private Material baseMat;
    [SerializeField] private Vector2[] spawnPoints = new Vector2[2];
    private Vector3 alignementPos;
    private bool isMovingX;
    private PlatformBehaviour lastPlatform;

    //Cut variables
    private const float FAIL_TO_MERGE = 0.1f;
    private const float MAX_BOUNDS = 8;
    private const float BOUNDS_INCREMENT = 0.25f;
    private float combo = 0;
    private Vector2 currentBounds;

    private void Awake()
    {
        currentBounds = new Vector2(MAX_BOUNDS, MAX_BOUNDS);
        gameState = new GameState();
        colorPicker = new ColorPicker();
    }
    void Start()
    {
        gameState.OnLost.AddListener(OnGameLost);
        gameState.InGame.AddListener(OnGameStarted);
        startPos = cashedPos = nextPosition = this.transform.position;
    }

    void Update()
    {
        if (gameState.State == State.InGame)
            DropTower();
    }

    public void MakeMove()
    {
        if (gameState.State == State.InGame)
        {
            if (PlaceTile())
            {
                GenerateTile();
                CountDesiredPos();
            }
            else
            {
                gameState.OnLost.Invoke();
            }
        }
    }

    private void GenerateTile()
    {
        var position = GenerateSpawnPoint();
        var platform = Instantiate(platformPrefab, this.transform.position, Quaternion.identity);
        platform.transform.localScale = new Vector3(currentBounds.x, 1, currentBounds.y);
        platform.transform.parent = this.transform;
        if (isMovingX)
            platform.SetConfig(gameState, new Vector3(-position.x, currentScore, alignementPos.z), isMovingX);
        else
            platform.SetConfig(gameState, new Vector3(alignementPos.x, currentScore, -position.y), isMovingX);
        var mat = platform.GetComponent<MeshRenderer>().material = Instantiate(baseMat);
        mat.color = colorPicker.GradientColor();
        lastPlatform = platform;
        cashedPlatforms.Add(platform);
    }
    private bool PlaceTile()
    {
        if (isMovingX)
        {
            float deltaX = alignementPos.x - lastPlatform.transform.position.x;
            if (Mathf.Abs(deltaX) > FAIL_TO_MERGE)
            {
                combo = 0;
                currentBounds.x -= Mathf.Abs(deltaX);
                if (currentBounds.x <= 0)
                {
                    return false;
                }
                float middle = alignementPos.x + lastPlatform.transform.localPosition.x / 2;
                lastPlatform.transform.localScale = new Vector3(currentBounds.x, 1, currentBounds.y);
                CreateCube(
                    new Vector3((lastPlatform.transform.position.x > 0) ?
                        lastPlatform.transform.position.x + (lastPlatform.transform.localScale.x / 2) :
                        lastPlatform.transform.position.x - (lastPlatform.transform.localScale.x / 2),
                        lastPlatform.transform.position.y,
                        lastPlatform.transform.position.z),
                    new Vector3(Mathf.Abs(deltaX), 1, lastPlatform.transform.localScale.z),
                    lastPlatform.GetComponent<MeshRenderer>()
                    );
                lastPlatform.transform.localPosition = new Vector3(middle - (alignementPos.x / 2), currentScore - 1, alignementPos.z);
            }
            else
            {
                if (combo >= 2)
                {
                    currentBounds.x += BOUNDS_INCREMENT;
                    if (currentBounds.x > MAX_BOUNDS)
                        currentBounds.x = MAX_BOUNDS;
                    float middle = alignementPos.x + lastPlatform.transform.localPosition.x / 2;
                    lastPlatform.transform.localScale = new Vector3(currentBounds.x, 1, currentBounds.y);
                    lastPlatform.transform.localPosition = new Vector3(middle - (alignementPos.x / 2), currentScore - 1, alignementPos.z);
                }
                combo++;
                lastPlatform.transform.localPosition = new Vector3(alignementPos.x, currentScore - 1, alignementPos.z);
            }
        }
        else
        {
            float deltaZ = alignementPos.z - lastPlatform.transform.position.z;
            if (Mathf.Abs(deltaZ) > FAIL_TO_MERGE)
            {
                combo = 0;
                currentBounds.y -= Mathf.Abs(deltaZ);
                if (currentBounds.y <= 0)
                {
                    return false;
                }
                float middle = alignementPos.z + lastPlatform.transform.localPosition.z / 2;
                lastPlatform.transform.localScale = new Vector3(currentBounds.x, 1, currentBounds.y);
                CreateCube(
                    new Vector3(
                        lastPlatform.transform.position.x,
                        lastPlatform.transform.position.y,
                        (lastPlatform.transform.position.z > 0) ?
                        lastPlatform.transform.position.z + (lastPlatform.transform.localScale.z / 2) :
                        lastPlatform.transform.position.z - (lastPlatform.transform.localScale.z / 2)),
                    new Vector3(lastPlatform.transform.localScale.x, 1, Mathf.Abs(deltaZ)),
                    lastPlatform.GetComponent<MeshRenderer>()
                );
                lastPlatform.transform.localPosition = new Vector3(alignementPos.x, currentScore - 1, middle - (alignementPos.z / 2));
            }
            else
            {
                if (combo >= 2)
                {
                    currentBounds.y += BOUNDS_INCREMENT;
                    if (currentBounds.y > MAX_BOUNDS)
                        currentBounds.y = MAX_BOUNDS;
                    float middle = (alignementPos.z + lastPlatform.transform.localPosition.z) / 2;
                    lastPlatform.transform.localScale = new Vector3(currentBounds.x, 1, currentBounds.y);
                    lastPlatform.transform.localPosition = new Vector3(alignementPos.x, currentScore - 1, middle - (alignementPos.z / 2));
                }
                combo++;
                lastPlatform.transform.localPosition = new Vector3(alignementPos.x, currentScore - 1, alignementPos.z);
            }

        }
        alignementPos = lastPlatform.Place();
        return true;
    }
    private Vector2 GenerateSpawnPoint()
    {
        var point = spawnPoints[UnityEngine.Random.Range(0, 2)];
        if (point == spawnPoints[0])
            isMovingX = true;
        else
            isMovingX = false;
        var multiplier = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;
        point *= multiplier;
        return point;
    }
    private void DropTower()
    {
        timeLerped += Time.deltaTime;
        this.transform.position = Vector3.Lerp(cashedPos, nextPosition, timeLerped / timeToLerp);
    }

    private void CountDesiredPos()
    {
        timeLerped = 0f;
        currentScore++;
        if (currentScore > maxScore)
            PlayerPrefs.SetInt(maxScoreKey, currentScore);
        currentScoreTxt.text = currentScore.ToString();
        cashedPos = this.transform.localPosition;
        nextPosition = startPos + Vector3.down * currentScore;
    }

    public void StartGame()
    {
        gameState.InGame.Invoke();
    }

    private void OnGameLost()
    {
        GamePanel.SetActive(false);
        loader.RefreshScore();
        MenuPanel.SetActive(true);
        foreach (var item in cashedPlatforms)
        {
            Destroy(item.gameObject);
        }
        cashedPlatforms.Clear();
        this.transform.position = Vector3.zero;
        currentBounds = new Vector2(8, 8);
        alignementPos = Vector3.zero;
    }
    private void OnGameStarted()
    {
        maxScore = PlayerPrefs.HasKey(maxScoreKey) ? PlayerPrefs.GetInt(maxScoreKey) : 0;
        currentScore = 0;
        currentScoreTxt.text = "0";
        GenerateTile();
        CountDesiredPos();
    }
    private void CreateCube(Vector3 pos, Vector3 scale, MeshRenderer meshRenderer) 
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.localScale = scale;
        go.transform.localPosition = pos;
        var mat = go.GetComponent<MeshRenderer>().material = Instantiate(baseMat);
        mat.color = meshRenderer.material.color;
        go.AddComponent<Rigidbody>();
        StartCoroutine(DestroyCube(go));
    }
    IEnumerator DestroyCube(GameObject go) 
    {
        yield return new WaitForSeconds(5f);
        Destroy(go);
    }
}