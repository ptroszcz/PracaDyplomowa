using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public float turnDelay = 10.0f;

    public MapManager mapScript;

    public DungeonManager dungeonScript;

    public PlayerStats playerStats;

    [HideInInspector]
    public PlayerStats basicStats;

    public Item[] itemList;

    public Item[] dungeonItemList;

    [Range(0, 1)]
    public int level = 0;

    public Enemy[] enemiesList;

    public Enemy[] dungeonEnemiesList;

    public Enemy guardian;

    public Enemy finalBoss;

    private List<Enemy> enemiesOnBoard;

    private List<Enemy> enemyToMove;

    [HideInInspector]
    public bool isPlayerTurn = true;

    [HideInInspector]
    public bool enemyTurn = false;

    private bool enemyMoving = false;

    private bool isGameOver = false;

    private bool isFirstSceneLoad = true;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy (gameObject);
        }
        DontDestroyOnLoad (gameObject);
        enemiesOnBoard = new List<Enemy>();
        enemyToMove = new List<Enemy>();
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
        mapScript = GetComponent<MapManager>();
        dungeonScript = GetComponent<DungeonManager>();
        InitGame();
    }

    void InitGame()
    {
        enemiesOnBoard.Clear();
        if (level == 0)
        {
            mapScript.SetupScene();
        }
        else
        {
            dungeonScript.SetupScene();
        }
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode sceneMode)
    {
        FightManager.instance.FindUIFields();
        if (isFirstSceneLoad)
        {
            playerStats.copyTo (basicStats);
            isGameOver = false;
            isFirstSceneLoad = false;
        }
        else if (isGameOver)
        {
            isGameOver = false;
            level = 0;
            InitGame();
        }
        else
        {
            level = 1;
            InitGame();
        }
    }

    public void GameOver()
    {
        isGameOver = true;
        //enabled = false;
    }

    public bool checkIfIsGameOver()
    {
        return isGameOver;
    }

    public void AddEnemyToList(Enemy enemy)
    {
        enemiesOnBoard.Add (enemy);
    }

    public void RemoveEnemyFromList(Enemy enemy)
    {
        enemiesOnBoard.Remove (enemy);
    }

    public void upgradeEnemies()
    {
        foreach (Enemy e in enemiesOnBoard)
        {
            e.stats.power++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!enemyTurn || isPlayerTurn || enemyMoving)
        {
            return;
        }
        enemyToMove.Clear();
        foreach (Enemy e in enemiesOnBoard)
        {
            if (e.CheckIfMove())
            {
                enemyToMove.Add (e);
            }
        }
        StartCoroutine(MoveEnemies());
    }

    IEnumerator MoveEnemies()
    {
        enemyMoving = true;
        yield return new WaitForSeconds(turnDelay);
        if (enemyToMove.Count == 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }

        foreach (Enemy e in enemyToMove)
        {
            e.startMoving();
            while (e.IsDuringMovement())
            {
                e.EnemyMove();
                yield return new WaitForSeconds(e.moveTime * 1.5f);
            }
            while (FightManager.instance.isDuringFight())
            {
                yield return new WaitForSeconds(1.0f);
            }
            yield return new WaitForSeconds(turnDelay);
        }
        enemyMoving = false;
        enemyTurn = false;
    }
}
