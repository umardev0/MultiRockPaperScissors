using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{

	public static Player localPlayer;

	void Start()
	{
		if(isLocalPlayer)
			localPlayer = this;
	}


	//This will be called from local copy of player on client but the function will
	//run on the server's copy of player
	//all the parameters are passed as it is to server
	[Command]
	public void CmdLockAnswer(int option, bool p1)
	{
		Debug.Log("CmdLockAnswer called");

		GameManager.instance.LockAnswer(option, p1);
	}
}
