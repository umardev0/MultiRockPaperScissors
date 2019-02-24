using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
	public static GameManager instance = null;

	public Button[] myBtns;
	public Button[] oppBtns;
	public Text result;
	public Text timer;

	private bool p1Flag = false;
	private int responseRecieved = 0;
	private int[] responses;
	private int timeLeft = 0;
	private int timeTotal = 15;

	void Awake()
	{
		if(instance == null)
		{
			instance = this;
		}
		else if(instance != this)
		{
			Destroy(gameObject);
		}
	}

	// Use this for initialization
	void Start()
	{
		if(isServer)
		{
			p1Flag = true;
			StartP1Turn();
		}
		else
		{
			p1Flag = false;
			result.text = "Opponent's turn";
		}

		responses = new int[2];
	}

	void StartP1Turn()
	{
		foreach(Button btn in myBtns)
		{
			btn.interactable = true;
		}
		result.text = "Your turn";
		timeLeft = timeTotal;
		timer.text = timeLeft.ToString();
		InvokeRepeating("ReduceTime", 1f, 1f);
	}

	[ClientRpc]
	public void RpcStartP2Turn()
	{
		if(p1Flag)
			return;
			
		foreach(Button btn in myBtns)
		{
			btn.interactable = true;
		}
		result.text = "Your turn";
		timeLeft = timeTotal;
		timer.text = timeLeft.ToString();
		InvokeRepeating("ReduceTime", 1f, 1f);
	}

	void ReduceTime()
	{
		timeLeft--;
		timer.text = timeLeft.ToString();
		if(timeLeft == 0)
		{
			AutoTurn();
		}
	}

	void AutoTurn()
	{
		OnBtnClick(Random.Range(0,3));
	}

	public void OnBtnClick(int option)
	{
		CancelInvoke();
		foreach(Button btn in myBtns)
		{
			btn.interactable = false;
		}

		Player.localPlayer.CmdLockAnswer(option, p1Flag);
		result.text = "Opponent's turn";
		timer.text = "";
	}
		
	public void LockAnswer(int option, bool p1)
	{
		Debug.Log("LockAnswer called");

		if(p1)
			responses[0] = option;
		else
			responses[1] = option;
		
		responseRecieved++;
		if(responseRecieved == 2)
		{
			responseRecieved = 0;
			RpcShowResult(responses);
		}
		else
		{
			RpcStartP2Turn();
		}
	}

	[ClientRpc]
	public void RpcShowResult(int[] options)
	{
		Debug.Log("RpcShowResult called");

		int won = -1;

		//0 = rock, 1 = paper, 2 = scissors
		//options[0] = server's/p1's choice
		//options[1] = client's/p2's choice

		if(options[0] == 0 && options[1] == 1)
		{
			won = 1;
		}
		else if(options[0] == 0 && options[1] == 2)
		{
			won = 0;
		}
		else if(options[0] == 1 && options[1] == 0)
		{
			won = 0;
		}
		else if(options[0] == 1 && options[1] == 2)
		{
			won = 1;
		}
		else if(options[0] == 2 && options[1] == 0)
		{
			won = 1;
		}
		else if(options[0] == 2 && options[1] == 1)
		{
			won = 0;
		}

//		result.gameObject.SetActive(true);

		if(won == -1)
		{
			result.text = "Draw";
		}
		else if(won == 0)
		{
			if(p1Flag)
			{
				result.text = "You Won";
				ChangeMyColor(options[0], true);
				ChangeOppColor(options[1], false);
				ParseManager.instance.IncrementWins();
			}
			else
			{
				result.text = "You Lost";
				ChangeMyColor(options[1], false);
				ChangeOppColor(options[0], true);
				ParseManager.instance.IncrementLoses();
			}
		}
		else if(won == 1)
		{
			if(p1Flag)
			{
				result.text = "You Lost";
				ChangeMyColor(options[0], false);
				ChangeOppColor(options[1], true);
				ParseManager.instance.IncrementLoses();
			}
			else
			{
				result.text = "You Won";
				ChangeMyColor(options[1], true);
				ChangeOppColor(options[0], false);
				ParseManager.instance.IncrementWins();
			}
		}

		Invoke("Reset", 5f);
	}

	void ChangeMyColor(int option, bool won)
	{
		if(won)
			myBtns[option].image.color = Color.green;
		else
			myBtns[option].image.color = Color.red;
	}

	void ChangeOppColor(int option, bool won)
	{
		if(won)
			oppBtns[option].image.color = Color.green;
		else
			oppBtns[option].image.color = Color.red;
	}

	void Reset()
	{
		for(int i=0; i<3; i++)
		{
			myBtns[i].image.color = Color.white;
			oppBtns[i].image.color = Color.white;
		}
//		result.gameObject.SetActive(false);
		Start();
	}
}
