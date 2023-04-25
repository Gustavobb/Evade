// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class GuardianManager : MonoBehaviour
// {
//     [SerializeField] public int MAX_GUARDIANS = 3;
//     [SerializeField] public int _guardianCount = 1;
//     private int _guardiansActive = 0;

//     private static GuardianManager _instance;
//     public static GuardianManager Instance
//     {
//         get
//         {
//             if (_instance == null) _instance = FindObjectOfType<GuardianManager>();
//             return _instance;
//         }
//     }

//     private List<Enemy> _guardians = new List<Enemy>();

//     private void Start()
//     {
//         _guardiansActive = _guardianCount;
//         for (int i = 0; i < transform.childCount; i++)
//         {
//             _guardians.Add(transform.GetChild(i).gameObject.GetComponent<Enemy>());
//             if (i >= _guardianCount) _guardians[i].SetActive(false);
//         }
//     }

//     private void Update()
//     {
//         HandleSlowDown();
//     }

//     private void HandleSlowDown()
//     {
//         if (!GameManager.Instance.OnGame || PowerUpManager.Instance.OnPowerUpMenu || Pause.Paused) return;
//         if (Input.GetMouseButtonDown(0))
//             UseClock();
//     }

//     public void UseClock()
//     {
//         if (Player.Instance.IsInvincible) return;
//         if (_guardiansActive == 0) return;

//         _guardiansActive --;
//         _guardians[_guardiansActive].SetActive(false);
//         _audioSource.Play();
        
//         StartCoroutine(SlowDownCoroutine());
//     }

//     public void WaveReset()
//     {
//         _guardiansActive = _guardianCount;
//         for (int i = 0; i < _guardiansActive; i++)
//             _guardians[i].SetActive(true);
//     }

//     private IEnumerator SlowDownCoroutine()
//     {
//         _enemyData.MultiplySpeed(_timeScale);
//         WaveManager.Instance.SetTimeScale(_timeScale);
//         GameManager.Instance.AnimateMaterial("_GreyScale", 0f, 1f, .3f);
//         GameManager.Instance.AnimateMaterial("_ColorDecay", 1f, .2f, .3f);
//         AudioHelper.Instance.SmoothAudio(GameManager.Instance._AudioSource, _timeScale, .4f, false, false);
        
//         float timer = 0f;
//         while (timer < _duration)
//         {
//             GameManager.Instance.SlowDown();
//             if (PowerUpManager.Instance.OnPowerUpMenu || !GameManager.Instance.OnGame) break;
//             if (Pause.Paused) timer -= Time.deltaTime;
//             timer += Time.deltaTime;

//             yield return null;
//         }
        
//         WaveManager.Instance.SetTimeScale(1f);
//         _enemyData.MultiplySpeed(1f);
//         GameManager.Instance.StopSlowDown();
//         GameManager.Instance.AnimateMaterial("_GreyScale", 1f, 0f, .3f);
//         GameManager.Instance.AnimateMaterial("_ColorDecay", .2f, 1f, .3f);
//         AudioHelper.Instance.SmoothAudio(GameManager.Instance._AudioSource, 1, .4f, false, false);
//     }

//     public void ResetClock(){
//         _guardianCount = 1;
//         WaveReset();
//     }
// }
