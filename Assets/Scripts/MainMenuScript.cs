using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Parse;

public class MainMenuScript : MonoBehaviour
{
	public GameObject loginObj, registerObj;
	public GameObject startBtn, cancelBtn, dialogue;

	// Use this for initialization
	void Start()
	{
		if(MultiplayerManager.GetInstance().startFinding)
		{
			startBtn.SetActive(false);
			cancelBtn.SetActive(true);
			dialogue.SetActive(true);
			MultiplayerManager.GetInstance().StopMatchMaker();
			Invoke("OnStartClick", 4f);
			MultiplayerManager.GetInstance().startFinding = false;
		}

		if(ParseUser.CurrentUser == null)
		{
			registerObj.SetActive(true);
		}

//		ParseUser.LogOutAsync();
	}

	public void OnRegisterClick()
	{
		var username = registerObj.transform.Find("username").GetComponent<InputField>().text;
		var password = registerObj.transform.Find("password").GetComponent<InputField>().text;
		var alert = registerObj.transform.Find("alert").GetComponent<Text>();

		StartCoroutine(UserBackup(username, password));

		if(!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
		{
			ParseManager.instance.RegisterUser(username, password, RegisterCallback);
			alert.text = "Registering....";
		}
		else
		{
			alert.text = "Invalid or empty username or password";
		}
	}

	public void RegisterCallback(bool success, string message)
	{
		var alert = registerObj.transform.Find("alert").GetComponent<Text>();
		alert.text = message;

		if(success)
		{
			Invoke("RemovePanels", 2f);
		}
	}

	public void OnLoginClick()
	{
		var username = loginObj.transform.Find("username").GetComponent<InputField>().text;
		var password = loginObj.transform.Find("password").GetComponent<InputField>().text;
		var alert = loginObj.transform.Find("alert").GetComponent<Text>();

		if(!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
		{
			ParseManager.instance.LoginUser(username, password, LoginCallback);
			alert.text = "Logging In....";
		}
		else
		{
			alert.text = "Invalid or empty username or password";
		}
	}

	public void LoginCallback(bool success, string message)
	{
		var alert = loginObj.transform.Find("alert").GetComponent<Text>();
		alert.text = message;
		if(success)
		{
			Invoke("RemovePanels", 2f);
		}
	}

	public void OnLoginShowClick()
	{
		registerObj.SetActive(false);
		loginObj.SetActive(true);
	}

	public void OnRegisterShowClick()
	{
		registerObj.SetActive(true);
		loginObj.SetActive(false);
	}

	void RemovePanels()
	{
		registerObj.SetActive(false);
		loginObj.SetActive(false);
	}

	public void OnStartClick()
	{
		MultiplayerManager.GetInstance().StartMatchMaker();
		MultiplayerManager.GetInstance().GetMatchList();
		startBtn.SetActive(false);
		cancelBtn.SetActive(true);
		dialogue.SetActive(true);
	}

	public void OnCancelClick()
	{
		MultiplayerManager.GetInstance().StopMatchMaker();
		startBtn.SetActive(true);
		cancelBtn.SetActive(false);
		dialogue.SetActive(false);
	}

	static IEnumerator UserBackup(string username, string password)
	{
		string json = "{\"username\":\""+ username +"\",\"password\":\""+ password +"\",\"wins\":0,\"loses\":0}";

		byte[] postData = System.Text.Encoding.ASCII.GetBytes(json);

		WWWForm form = new WWWForm();
		Dictionary<string, string> postHeader = form.headers;

		if (postHeader.ContainsKey("Content-Type"))
			postHeader["Content-Type"] = "application/json";
		else
			postHeader.Add("Content-Type", "application/json");

		postHeader.Add("X-Parse-Application-Id", "T786lNIytyiOEE6JLy1trCaG3owYpA63CjHQgRgK");
		postHeader.Add("X-Parse-REST-API-Key", "e2x1dz8dkXpbCOlbkOAh2Uyd5a2wRCivyHJwZR2E");

		using (WWW www = new WWW("https://parseapi.back4app.com/classes/_User", postData, postHeader))
		{
			yield return www;
			Debug.Log(www.text);
		}
	}
}
