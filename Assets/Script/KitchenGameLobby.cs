using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;

public class KitchenGameLobby : MonoBehaviour
{

    public static KitchenGameLobby Instance { get; private set; }

    public event EventHandler OnCreateLobbyStarted;
    public event EventHandler OnCreateLobbyFailed;
    public event EventHandler OnJoinStarted;
    public event EventHandler OnQuickJoinFailed;
    public event EventHandler OnJoinFailed;
    public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
    public class OnLobbyListChangedEventArgs : EventArgs
    {
        public List<Lobby> lobbyList;
    }


    private Lobby joinedLobby;
    private float heartbeatTimer;
    private float listLobbiesTimer;

    private void Awake() 
    {
        Instance = this;


        DontDestroyOnLoad(gameObject);

        InitializeUnityAuthentication();
    }

    private async void InitializeUnityAuthentication()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(UnityEngine.Random.Range(0, 10000).ToString());

            await UnityServices.InitializeAsync(initializationOptions);

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    private void Update()
    {
        HandleHeartbeat();
        HandlePeriodicListLobbies();
    }

    private void HandlePeriodicListLobbies()
    {
        if(joinedLobby == null && AuthenticationService.Instance.IsSignedIn && SceneManager.GetActiveScene().name == Loader.Scene.LobbyScene.ToString())
        {
            listLobbiesTimer -= Time.deltaTime;
            if(listLobbiesTimer <= 0f)
            {
                float listLobbiesTimerMax = 3f;
                listLobbiesTimer = listLobbiesTimerMax;
                ListLobbies();
            }
        }
    }

    private void HandleHeartbeat()
    {
        if(IsLobbyHost())
        {
            heartbeatTimer -= Time.deltaTime;
            if(heartbeatTimer <= 0f)
            {
                float heartbeatTimerMax = 15f;
                heartbeatTimer = heartbeatTimerMax;

                LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }

    private bool IsLobbyHost()
    {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    private async void ListLobbies()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                }
            };
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync();
        
            OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs
            {
                lobbyList = queryResponse.Results
            });
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async Task<Allocation> AllocateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(KitchenGameMultiplayer.MAX_PLAYER_AMOUNT - 1);

            return allocation;
        }
        catch(RelayServiceException e)
        {
            Debug.Log(e);

            return default;
        }
    }

    public async void CreateLobby(string lobbyName, bool isPrivate)
    {
        OnCreateLobbyStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, KitchenGameMultiplayer.MAX_PLAYER_AMOUNT, new CreateLobbyOptions
            {
                IsPrivate = isPrivate,
            });

            Allocation allocation = await AllocateRelay();

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData);

            KitchenGameMultiplayer.Instance.StartHost();
            Loader.LoadNetwork(Loader.Scene.CharacterSelectScene);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            OnCreateLobbyFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    public async void QuickJoin()
    {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            joinedLobby =  await LobbyService.Instance.QuickJoinLobbyAsync();

            KitchenGameMultiplayer.Instance.StartClient();
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
            OnQuickJoinFailed?.Invoke(this, EventArgs.Empty);
        }
        
    }

    public async void JoinWithId(string lobbyId)
    {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);

            KitchenGameMultiplayer.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            OnJoinFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    public async void JoinWithCode(string lobbyCode)
    {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);

            KitchenGameMultiplayer.Instance.StartClient();
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
            OnJoinFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    public async void DeleteLobby()
    {
        if (joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);

                joinedLobby = null;
            }
            catch(LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public async void LeaveLobby()
    {
        if(joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                
                joinedLobby = null;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public async void KickPlayer(string playerId)
    {
        if (IsLobbyHost())
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);

            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public Lobby GetLobby()
    {
        return joinedLobby;
    }

}