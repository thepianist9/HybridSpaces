using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using HybridSpaces;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;


    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.buildIndex == 1) //game scene
        { 
           uint id  = GetComponent<VoiceChatManager>().UId;
           GameObject Instance = PhotonNetwork.Instantiate( Path.Combine("Photonprefabs", "PlayerManager"), Vector3.zero,
                Quaternion.identity);
           Instance.GetComponent<PlayerManager>().CreateController(id);
        }
    }
    
  
    
}
