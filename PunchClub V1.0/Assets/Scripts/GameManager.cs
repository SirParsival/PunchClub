using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public LifeBar enemyLifeBar;

    public Hero actor;
    public bool cameraFollows = true;
    public CameraBounds cameraBounds;

    public LevelData currentLevelData;
    private BattleEvent currentBattleEvent;
    private int nextEventIndex;
    public bool hasRemainingEvents;

    public List<GameObject> activeEnemies;
    public Transform[] spawnPositions;

    public GameObject currentLevelBackground;

    public GameObject robotPrefab;

    public Transform walkInStartTarget;
    public Transform walkInTarget;

    public Transform walkOutTarget;

    public LevelData[] levels;
    public static int CurrentLevel = 0;

    void Start()
    {
        nextEventIndex = 0;
        StartCoroutine(LoadLevelData(levels[CurrentLevel]));
        cameraBounds.SetXPosition(cameraBounds.minVisibleX);
    }
    
    void Update() 
    {
        if (currentBattleEvent == null && hasRemainingEvents)
        {
            if (Mathf.Abs(currentLevelData.battleData[nextEventIndex].column - 
                          cameraBounds.activeCamera.transform.position.x) < 0.2f)
            {
                PlayBattleEvent(currentLevelData.battleData[nextEventIndex]);
            }
        }
        if (currentBattleEvent != null)
        {
            if (Robot.TotalEnemies == 0)
            {
                CompleteCurrentEvent();
            }
        }
        if (cameraFollows)
        {
            cameraBounds.SetXPosition(actor.transform.position.x);
        }
    }

    private GameObject SpawnEnemy(EnemyData data)
    {
        GameObject enemyObj = Instantiate(robotPrefab);
        Vector3 position = spawnPositions[data.row].position;
        position.x = cameraBounds.activeCamera.transform.position.x + (data.offset * (cameraBounds.cameraHalfWidth + 1));
        enemyObj.transform.position = position;
        if (data.type == EnemyType.Robot)
        {
            enemyObj.GetComponent<Robot>().SetColor(data.color);
        }
        enemyObj.GetComponent<Enemy>().RegisterEnemy();
        return enemyObj;
    }

    private void PlayBattleEvent(BattleEvent battleEventData)
    {
        currentBattleEvent = battleEventData;
        nextEventIndex++;

        cameraFollows = false;
        cameraBounds.SetXPosition(battleEventData.column);
        foreach (GameObject enemy in activeEnemies)
        {
            Destroy(enemy);
        }
        activeEnemies.Clear();
        Enemy.TotalEnemies = 0;
        foreach (EnemyData enemyData in currentBattleEvent.enemies)
        {
            activeEnemies.Add(SpawnEnemy(enemyData));
        }
    }

    private void CompleteCurrentEvent()
    {
        currentBattleEvent = null;
        cameraFollows = true;
        cameraBounds.CalculateOffset(actor.transform.position.x);
        hasRemainingEvents = currentLevelData.battleData.Count > nextEventIndex;

        enemyLifeBar.EnableLifeBar(false);

        if (!hasRemainingEvents)
        {
            StartCoroutine(HeroWalkout());
        }
    }

    private IEnumerator LoadLevelData(LevelData data)
    {
        cameraFollows = false;
        currentLevelData = data;

        hasRemainingEvents = currentLevelData.battleData.Count > 0;
        activeEnemies = new List<GameObject>();

        yield return null;
        cameraBounds.SetXPosition(cameraBounds.minVisibleX);

        if (currentLevelBackground != null)
        {
            Destroy(currentLevelBackground);
        }

        currentLevelBackground = Instantiate(currentLevelData.levelPrefab);

        cameraBounds.EnableBounds(false);
        actor.transform.position = walkInStartTarget.transform.position;

        yield return new WaitForSeconds(0.1f);

        actor.UseAutopilot(true);
        actor.AnimateTo(walkInTarget.transform.position, false, DidFinishIntro);

        cameraFollows = true;
    }

    private void DidFinishIntro()
    {
        actor.UseAutopilot(false);
        actor.controllable = true;
        cameraBounds.EnableBounds(true);
    }

    private IEnumerator HeroWalkout()
    {
        cameraBounds.EnableBounds(false);
        cameraFollows = false;
        actor.UseAutopilot(true);
        actor.controllable = false;
        actor.AnimateTo(walkOutTarget.transform.position, true, DidFinishWalkout);
        yield return null;
    }
    
    private void DidFinishWalkout()
    {
        CurrentLevel++;
        if (CurrentLevel >= levels.Length)
        {
            Debug.Log("Game Completed!");
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            StartCoroutine(AnimateNextLevel());
        }

        cameraBounds.EnableBounds(true);
        cameraFollows = false;
        actor.UseAutopilot(false);
        actor.controllable = false;
    }

    private IEnumerator AnimateNextLevel()
    {
        yield return null;
        SceneManager.LoadScene("Game");
    }
}
