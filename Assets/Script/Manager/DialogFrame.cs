using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

public class DialogFrame : MonoBehaviour {
[System.SerializableAttribute]
	struct DialogPair
	{
		public DialogPair(string c , string w ) {character = c; words = w;}
		public string character;
		public string words;
	}
	[SerializeField] List<DialogPair> dialogList = new List<DialogPair>();

	[SerializeField] bool isShowing = false;

	public Image back;
	public Text text;
	public Outline textOutline;

	public float fadeTime = 1f;
	[SerializeField] public string dialogName;

	void Awake()
	{
		dialogList = new List<DialogPair>();
	}

	[SerializeField]KeyCode continueKey;



	public void addDialog(string character , string words)
	{
		// Debug.Log(gameObject.name + " add dialog " + character + " : " + words + " isShowing " + isShowing.ToString());
		dialogList.Add(new DialogPair(character,words));
	}

	public void showDialog(string character , string words)
	{
		if (textOutline != null)
		{
		textOutline.enabled = false;
		if (words.StartsWith("[HL]"))
		{
			textOutline.enabled = true;
			words = words.Remove(0, 4);
		}
		}
		Debug.Log(gameObject.name + " show dialog " + character + " : " + words + " isShowing " + isShowing.ToString());
		

		text.text = character + ":\r\n" + words;


		Message msg = new Message();
		msg.AddMessage("dialog", dialogName);
		EventManager.Instance.PostEvent(EventDefine.BEGIN_DIALOG,msg);

	}

	void Update()
	{
		if ( !isShowing && dialogList.Count > 0 )
		{
			BeginShow();
		}
		if ( Input.GetKeyDown(continueKey))
		{
			TryShowNext();
		}
		
	}

	public void OnClick()
	{
		TryShowNext();
	}

	public void TryShowNext()
	{
		if ( isShowing )
		{
			if ( !ShowNext())
			{
				EndShow();
			}
		}
	}

	bool ShowNext()
	{
		if (dialogList.Count > 0 )
		{
			showDialog(dialogList[0].character,dialogList[0].words);
			dialogList.RemoveAt(0);
			return true;
		}
		return false;
	}

	public void BeginShow()
	{
		// if (dialogList.Count <= 0 )
		// 	return;
		Debug.Log("Begin Show totally " + dialogList.Count.ToString() );
		isShowing = true;

		back.DOFade(1.0f, fadeTime);
		text.DOFade(1.0f, fadeTime);

		ShowNext();
	}

	void EndShow()
	{
		isShowing = false;
		Debug.Log("End Show");

		Message msg = new Message();
		msg.AddMessage("dialog", dialogName);
		EventManager.Instance.PostEvent(EventDefine.END_DIALOG,msg);

		back.DOFade(0f, fadeTime);
		text.DOFade(0f, fadeTime);
	}
}
