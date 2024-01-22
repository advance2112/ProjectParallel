using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Advance.ProjectParallel
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager instance;
        
        public List<GameObject> floorsList;
        public int currentLevel;

        private int maxLevel = 10;
        private bool gameOver;

        private List<Enemy> currentEnemies;
        public List<List<GameObject>> floors;
        public GameObject youWinText;

        public Enemy s1Prefab;
        public Enemy s2Prefab;
        public Enemy s3Prefab;
        public Enemy k1Prefab;
        public Enemy k2Prefab;
        public Enemy k3Prefab;
        public Enemy f1Prefab;
        public Enemy f2Prefab;
        public Enemy t1Prefab;
        public Enemy t2Prefab;
        public Enemy t3Prefab;
        private Dictionary<string, Enemy> enemyNames;


        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            enemyNames = new Dictionary<string, Enemy>
            {
                { "s1", s1Prefab },
                { "s2", s2Prefab },
                { "s3", s3Prefab },
                { "k1", k1Prefab },
                { "k2", k2Prefab },
                { "k3", k3Prefab },
                { "f1", f1Prefab },
                { "f2", f2Prefab },
                { "t1", t1Prefab },
                { "t2", t2Prefab },
                { "t3", t3Prefab }
            };

            floors = new List<List<GameObject>>(6);
            for (int i = 0; i < 6; i++)
            {
                floors.Add(new List<GameObject>(6));
                for (int j = 0; j < 6; j++)
                {
                    floors[i].Add(floorsList[i*6 + j]);
                }
            }
        }

        private void LoadNextLevel()
        {
            currentLevel++;
            if (currentLevel > maxLevel)
            {
                gameOver = true;
                youWinText.SetActive(true);
                return;
            }
            LoadLevel(currentLevel);
        }

        private void LoadLevel(int level)
        {
            LoadLevel(GetLevelArray(level));
        }

        private string[][] GetLevelArray(int level)
        {
            List<string> data = GetFileData("Level" + level + ".csv");

            string[][] levelData = new string[data.Count][];
            for (int i = 0; i < data.Count; i++)
            {
                levelData[i] = data[i].Split(",");
            }

            return levelData;
        }
        
        /**
         * @Author Advance
         * From Gwent Clone
         */
        private List<string> GetFileData(string fileName)
        {
            StreamReader reader = new StreamReader(fileName);
            List<string> data = new List<string>();
            while (reader.Peek() != -1)
            {
                data.Add(reader.ReadLine());
            }
            reader.Close();
            return new List<string>(data);
        }

        private void LoadLevel(string[][] enemies)
        {
            ResetCurrentEnemies();
            for (int i = 0; i < enemies.Length; i++)
            {
                for (int j = 0; j < enemies[i].Length; j++)
                {
                    Summon(floors[i][j], enemies[i][j]);
                }
            }
        }

        private void ResetCurrentEnemies()
        {
            if (currentEnemies != null)
            {
                foreach (Enemy currentEnemy in currentEnemies)
                {
                    currentEnemy.Kill();
                }
            }
            currentEnemies = new List<Enemy>();
        }

        private void Summon(GameObject floor, string prefabName)
        {
            Enemy enemyPrefab = GetEnemyPrefab(prefabName);
            if (enemyPrefab == null)
            {
                return;
            }
            currentEnemies.Add(Instantiate(enemyPrefab, floor.transform.position, Quaternion.identity));
        }

        public Enemy GetEnemyPrefab(string prefabName)
        {
            return enemyNames.GetValueOrDefault(prefabName);
        }

        private void Update()
        {
            if (DoLoadNextLevel())
            {
                LoadNextLevel();
            }
        }

        private bool DoLoadNextLevel()
        {
            return !gameOver && (currentEnemies == null || currentEnemies.All(currentEnemy => currentEnemy == null || currentEnemy.IsDead()));
        }
    }
}