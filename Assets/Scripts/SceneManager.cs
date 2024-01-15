using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance;

    public Player Player;
    [HideInInspector] public List<Enemie> Enemies;

    public GameObject Lose;
    public GameObject Win;

    [SerializeField] private Text showWaves;
    [SerializeField] private Text wavesLeft;
    
    [SerializeField] private LevelConfig Config;
    private int _currWave;
    private int _waveNumber;

    private void Awake()
    {
        _currWave = 0;
        _waveNumber = 5;
        SingletonPattern();
    }

    private void Start()
    {
        SpawnWave();
    }

    private void Update()
    {
        if (!GameObject.FindGameObjectWithTag("Enemy"))
        {
            SpawnWave();
        }
    }

    public void AddEnemie(Enemie enemie)
    {
        Enemies.Add(enemie);
    }

    public void RemoveEnemie(Enemie enemie)
    {
        Enemies.Remove(enemie);
    }

    public void GameOver()
    {
        Lose.SetActive(true);
    }

    private void SpawnWave()
    {
        if (_currWave >= Config.Waves.Length)
        {
            Win.SetActive(true);
            return;
        }

        var wave = Config.Waves[_currWave];
        foreach (var character in wave.Characters)
        {
            Vector3 pos = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
            Instantiate(character, pos, Quaternion.identity);
        }
        
        _currWave++;
        wavesLeft.text = "Waves left " + (_waveNumber - _currWave);
        showWaves.text = "Wave " + _currWave;

    }

    public void Reset()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    private void SingletonPattern()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

}
