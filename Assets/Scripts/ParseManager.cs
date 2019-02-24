using System.Collections.Generic;
using UnityEngine;
using Parse;
using System;
using System.Collections;
using UnityEngine.UI;


public class ParseManager : MonoBehaviour
{
	public static ParseManager instance = null;

	void Awake()
	{
		if(instance == null)
		{
			instance = this;
//			DontDestroyOnLoad(gameObject);
		}
//		else if(instance != this)
//		{
//			Destroy(gameObject);
//		}
	}

	public void RegisterUser(string username, string password, Action<bool, string> callback)
	{
		var user = new ParseUser()
		{
			Username = username.ToLower(),
			Password = password,
		};

		user["wins"] = 0;
		user["loses"] = 0;

		user.SignUpAsync().ContinueWith(task =>
		{
			if(task.Exception != null)
			{
				Debug.Log("Register Exception : " + task.Exception.InnerExceptions[0].Message);
				callback(false, task.Exception.InnerExceptions[0].Message);
            }
			else
			{
				Debug.Log("User Registeration Successful");
				callback(true, "Successfully Registered");
            }
		});
	}

	public void LoginUser(string username, string password, Action<bool, string> callback)
	{
		ParseUser.LogInAsync(username, password).ContinueWith(task =>
		{
			if(task.Exception != null)
			{
				Debug.Log("Login Exception : " + task.Exception.InnerExceptions[0].Message);
				callback(false, task.Exception.InnerExceptions[0].Message);
			}
			else
			{
				Debug.Log("User Login Successful");
				callback(true, "Successfully Logged In");
			}
		});
	}

	public void IncrementWins()
	{
		ParseUser.CurrentUser.Increment("wins");
		ParseUser.CurrentUser.SaveAsync().ContinueWith(task =>
		{
			if (task.IsFaulted || task.IsCanceled)
			{
				IEnumerable<Exception> exceptions = task.Exception.InnerExceptions;

				foreach (var exception in exceptions)
				{
					Debug.Log("wins increment error : " + exception.Message);
				}
			}
			else
			{
				Debug.Log("wins incremented successfully");
			}
		});
	}

	public void IncrementLoses()
	{
		ParseUser.CurrentUser.Increment("loses");
		ParseUser.CurrentUser.SaveAsync().ContinueWith(task =>
		{
			if (task.IsFaulted || task.IsCanceled)
			{
				IEnumerable<Exception> exceptions = task.Exception.InnerExceptions;

				foreach (var exception in exceptions)
				{
					Debug.Log("loses increment error : " + exception.Message);
				}
			}
			else
			{
				Debug.Log("loses incremented successfully");
			}
		});
	}
}
