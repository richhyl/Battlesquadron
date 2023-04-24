//using UnityEngine;
//using UnityEngine.SceneManagement;
//using System.Collections;
//using UnityEngine.UI;

//public class Done_GameController : MonoBehaviour
//{
//    public GameObject[] enemies;
//    public Vector3 spawnValues;
//    public int enemyCount;
//    public float spawnWait;
//    public float startWait;
//    public float waveWait;

//    //public Text scoreText;
//    //public Text restartText;
//    //public Text gameOverText;

//    //private bool gameOver;
//    //private bool restart;
//    private int score;

//    void Start()
//    {
//        //gameOver = false;
//        //restart = false;
//        //restartText.text = "";
//        //gameOverText.text = "";
//        //score = 0;
//        //UpdateScore();
//        StartCoroutine(SpawnWaves());
//    }

//    //void Update()
//    //{
//    //    if (restart)
//    //    {
//    //        if (Input.GetKeyDown(KeyCode.R))
//    //        {
//    //            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
//    //        }
//    //    }
//    //}

//    IEnumerator SpawnWaves()
//    {
//        yield return new WaitForSeconds(startWait);
//        while (true)
//        {
//            for (int i = 0; i < enemyCount; i++)
//            {
//                GameObject enemy = enemies [Random.Range(0, enemies.Length)];
//                Vector3 spawnPosition = new Vector3(Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
//                Quaternion spawnRotation = Quaternion.identity;
//                Instantiate(enemy, spawnPosition, spawnRotation);
//                yield return new WaitForSeconds(spawnWait);
//            }
//            yield return new WaitForSeconds(waveWait);

//            //if (gameOver)
//            //{
//            //    restartText.text = "Press 'R' for Restart";
//            //    restart = true;
//            //    break;
//            //}
//        }
//    }

//    //public void AddScore(int newScoreValue)
//    //{
//    //    score += newScoreValue;
//    //    UpdateScore();
//    //}

//    //void UpdateScore()
//    //{
//    //    scoreText.text = "Score: " + score;
//    //}

//    //public void GameOver()
//    //{
//    //    gameOverText.text = "Game Over!";
//    //    gameOver = true;
//    //}
//}