using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName ="LevelData", menuName = "PunchClub/LevelData")]
public class LevelData : ScriptableObject 
{
	public List<BattleEvent> battleData;
	public GameObject levelPrefab;
	public string levelName;
}

[Serializable]
public class BattleEvent 
{
	public int column;
	public List<EnemyData> enemies;
}

[Serializable]
public class EnemyData 
{
	public EnemyType type;
	public RobotColor color;
	public int row;
	public float offset;
}

public enum EnemyType 
{
	Robot = 0,
	Boss
}