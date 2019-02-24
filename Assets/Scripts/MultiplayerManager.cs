using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.SceneManagement;

public class MultiplayerManager : NetworkLobbyManager
{
	private static MultiplayerManager instance;

	[HideInInspector]
	public int playersConnected = 0;
	[HideInInspector]
	public bool isServer = false;
	[HideInInspector]
	public bool startFinding = false;

	public static MultiplayerManager GetInstance()
	{
		return instance;
	}

	void Awake()
	{
		if(instance != null && instance != this)
		{
			Destroy(gameObject);
		}
		else
		{
			instance = this;
		}
	}

	public override void OnClientConnect(NetworkConnection conn)
	{
		base.OnClientConnect(conn);
		Debug.Log("Client is connected");
	}

	//Called on clients when disconnected from a server
	public override void OnClientDisconnect(NetworkConnection conn)
	{
		base.OnClientDisconnect(conn);
		Debug.Log("Server is disconnected");

		playersConnected = 0;
		StopMatchMaker();
		LoadMenu();
	}

	//Called on the server when a client disconnects.
	public override void OnServerDisconnect (NetworkConnection conn)
	{
		base.OnServerDisconnect (conn);
		Debug.Log("Client is disconnected");
		DestroyMatch();
	}

	public override void OnDestroyMatch (bool success, string extendedInfo)
	{
		base.OnDestroyMatch (success, extendedInfo);
		if(success)
		{
			playersConnected = 1;
			StopMatchMaker();
			Invoke("LoadMenu", 2f);
		}
		else
			DestroyMatch();
	}

	void LoadMenu()
	{
		startFinding = true;

		var lobbyPlayers = Transform.FindObjectsOfType<NetworkLobbyPlayer>();
		for(int i=0; i<lobbyPlayers.Length; i++)
		{
			DestroyImmediate(lobbyPlayers[i].gameObject);
		}
		SceneManager.LoadScene("MainMenuScene");
	}

	public override void OnStartHost()
	{
		base.OnStartHost();
		Debug.Log("Host is made");
	}

	//Called on the server when a new client connects.
	public override void OnServerConnect(NetworkConnection connection)
	{
		Debug.Log("New player connected");
		Debug.Log("Connections : " + Network.connections.Length.ToString() + " , Players : " + playersConnected.ToString() + " , NumPlayers : " + numPlayers.ToString());
		playersConnected++;
		if(Network.connections.Length == 2 || playersConnected == 2 || numPlayers == 2)
		{
			playersConnected = 0;
			Invoke("StartGame", 2f);
		}
	}

	void StartGame()
	{
		ServerChangeScene(playScene);
	}

	public void DestroyMatch()
	{
		matchMaker.DestroyMatch(matchInfo.networkId, 0, OnDestroyMatch); 
	}

	public void GetMatchList()
	{
		matchMaker.ListMatches(0, 6, "", true, 0, 0, OnMatchList);
	}

	//Umar::
	public override void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
	{
		base.OnMatchList(success, extendedInfo, matches);
		if(success)
		{
			Debug.Log("List fetched: Success");

			if (matches.Count == 0)//if no match found then create new one
			{
				matchMaker.CreateMatch("", 2, true, "", "", "", 0, 0, OnMatchCreate);
			}
			else//if found then simply join first one
			{
				Debug.Log("Network Id : " + matches[0].networkId.ToString());
				matchMaker.JoinMatch(matches[0].networkId, "", "", "", 0, 0, OnMatchJoined);
			}
		}
		else
		{
			Debug.Log("List fetched: Failure");
		}
	}

	//Umar::
	public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
	{
		base.OnMatchCreate (success,extendedInfo,matchInfo);

		if(success)
		{
			isServer = true;
			Debug.Log("MatchCreate : Success");
		}
		else
		{
			Debug.Log("MatchCreate: Failure");
		}
	}

	public override void OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo)
	{
		base.OnMatchJoined(success,extendedInfo,matchInfo);
		Debug.Log("Match is joined");
	}

	void OnAplicationQuit()
	{
		if(isServer)
			DestroyMatch();
	}
}
