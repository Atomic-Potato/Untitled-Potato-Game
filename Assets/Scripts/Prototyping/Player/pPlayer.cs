﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Note:
// This script execution is ran before the default execution order
public class pPlayer : MonoBehaviour
{
    #region INSPECTOR VARIABLES
    [Range(0, 999)]
    [SerializeField] int hitPoints = 4;
    [Range(0f, 10f)]
    [SerializeField] float recoveryTime = 2f;
    #endregion

    #region STATIC & PUBLIC VARIABLES
    static pPlayer _instance;
    public static pPlayer Instance => _instance;
    public GameObject Object => gameObject;
    public int HitPoints => hitPoints;
    public Action<int> Damage;
    #endregion

    #region PRIVATE VARIABLES
    int _initialHitPoints;
    bool _isInRecovery;
    Coroutine _recoverCache;
    #endregion

    void Awake() 
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;    
        }

        _initialHitPoints = hitPoints;

        Damage = ApplyDamage;
    }


    void ApplyDamage(int damagePoints)
    {
        if (_isInRecovery)
        {
            return;
        }

        hitPoints -= damagePoints;

        if (hitPoints <= 0)
        {
            Kill();
        }

        Recover();

        void Recover()
        {
            if (_recoverCache == null)
            {
                _recoverCache = StartCoroutine(ExecuteRecover());
            }

            IEnumerator ExecuteRecover()
            {
                _isInRecovery = true;
                yield return new WaitForSeconds(recoveryTime);
                _isInRecovery = false;
                _recoverCache = null;
            }
        }
    }

    public void Kill()
    {
        Object.SetActive(false);
        Respawn();
    }

    public void Respawn()
    {
        ReloadScene();

        void ReloadScene()
        {
           Scene scene = SceneManager.GetActiveScene();
           SceneManager.LoadScene(scene.name); 
        }
    }
}
