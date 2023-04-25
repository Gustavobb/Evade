using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardianManager : MonoBehaviour
{
    [SerializeField] public int MAX_GUARDIANS = 3;
    [SerializeField] public int _guardianCount = 0;
    private int _guardiansActive = 0;

    private static GuardianManager _instance;
    public static GuardianManager Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<GuardianManager>();
            return _instance;
        }
    }

    private List<Guardian> _guardians = new List<Guardian>();
    public GuardianData guardianData;

    private void Start()
    {
        _guardiansActive = _guardianCount;
        for (int i = 0; i < transform.childCount; i++)
        {
            _guardians.Add(transform.GetChild(i).gameObject.GetComponent<Guardian>());
            if (i >= _guardianCount) _guardians[i].gameObject.SetActive(false);
        }
    }

    public void WaveReset()
    {
        _guardiansActive = _guardianCount;
        for (int i = 0; i < _guardians.Count; i++)
        {
            if (i >= _guardianCount) 
            {
                _guardians[i].gameObject.SetActive(false);
                continue;
            }
            
            _guardians[i].gameObject.SetActive(true);
        }
    }

    public void AddGuardian()
    {
        if (_guardianCount <= MAX_GUARDIANS)
        {
            _guardianCount ++;
            _guardiansActive ++;
        }
    }

    public void ResetGuardians()
    {
        _guardianCount = 0;
        WaveReset();
    }
}
