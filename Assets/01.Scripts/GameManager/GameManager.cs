﻿using System.Collections;
using System.Collections.Generic;
using System.Resources;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : DontDestroySingleton<GameManager>
{
    [HideInInspector]
    public TouchManager touchManager;
    
    [HideInInspector]
    public WidgetViewer widgetViewer;

    [Header("Setting")]
    [SerializeField]
    private InGameResources _inGameResources;
    public InGameResources inGameResources {
        get => _inGameResources;
        set => _inGameResources = value;
    }
    
    [SerializeField]
    private MapFile _songData;
    public MapFile songData {
        get => _songData;
        set => _songData = value;
    }
    
    [SerializeField]
    private SongItemInformation _selectSongItem;
    public SongItemInformation selectSongItem {
        get => _selectSongItem;
        set => _selectSongItem = value;
    }
    
    private GameResult _gameResult;
    public GameResult gameResult {
        get => _gameResult;
        set => _gameResult = value;
    }
    
    private PlayerSetting playerSetting;
    public PlayerSetting PlayerSetting => playerSetting;
    
    private bool someUIInteraction;
    public bool SomeUIInteraction {get => someUIInteraction; set => someUIInteraction = value;}

    private ServerConnectorController serverConnector;
    public ServerConnectorController ServerConnector => serverConnector;

    private string serverUrl;

    public string ServerUrl => $"http://{serverUrl}";

    private void Awake(){
        if (instance != this && instance != null) {
            Destroy(gameObject);
        }
        
        touchManager = gameObject.GetComponent<TouchManager>();
        widgetViewer = gameObject.GetComponent<WidgetViewer>();
        serverConnector = gameObject.GetComponent<ServerConnectorController>();

        serverUrl = serverConnector.ServerUrl;
        
        playerSetting = Resources.Load<PlayerSetting>("Player Setting/PlayerSetting");
        playerSetting.AchieveRequireData.Initialize();

    }

    private void Start() {
        LoginSetting().Start(this);
    }
    
    private IEnumerator LoginSetting() {
        if (bool.Parse(PlayerPrefs.GetString("SIGNUP","false")) == false) {
            bool signUpSuccess = false;
            
            var randomValue = Random.Range(0, 10000);
            serverConnector.SignUp($"guest_{randomValue}", randomValue.ToString(), $"{randomValue}번 띠용이", () => signUpSuccess = true);
            
            yield return new WaitUntil( () => signUpSuccess );
            "Sign Up Success".Log();
            playerSetting.GuestNumber = randomValue;
            playerSetting.userName = $"{randomValue}번 띠용이";
        }

        bool loginSuccess = false;
        var guestNumber = playerSetting.GuestNumber;
        
        serverConnector.Login($"guest_{guestNumber}", guestNumber.ToString(), new UserData(), () => loginSuccess = true);
        yield return new WaitUntil(() => loginSuccess);        
        "Login Success".Log();
    }
}
