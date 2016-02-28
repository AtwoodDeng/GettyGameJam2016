using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

public class DialogFrame : MonoBehaviour {
[System.SerializableAttribute]
	struct DialogPair
	{
		public DialogPair(string c , string w , string w2) {character = c; words = w;words2=w2;}
		public string character;
		public string words;
		public string words2;
	}
	[SerializeField] List<DialogPair> dialogList = new List<DialogPair>();

	[SerializeField] bool isShowing = false;

	public Image back;
	public Text text;
	public Outline textOutline;

	public Button Choice1Btn;
	public Text Cohice1Text;
	public Button Choice2Btn;
	public Text Cohice2Text;

	public float fadeTime = 1f;
	[SerializeField] public string dialogName;

	enum State
	{
		Normal,
		Choice,
	}
	[SerializeField] State state;

	public bool GetIsShowing()
	{
		return isShowing;
	}

	void Awake()
	{
		dialogList = new List<DialogPair>();
	}

	[SerializeField]KeyCode continueKey;



	public void addDialog(string character , string words, string words2)
	{
		// Debug.Log(gameObject.name + " add dialog " + character + " : " + words + " isShowing " + isShowing.ToString());
		dialogList.Add(new DialogPair(character,words,words2));
	}

	public void showDialog(string character , string words )
	{
		state = State.Normal;
		// if (back.color.a < 1f)
		{
			back.DOFade(1.0f, fadeTime);
			text.DOFade(1.0f, fadeTime);
			back.raycastTarget = true;
		}
		// if (Choice1Btn.image.color.a == 1f)
		{
			Choice1Btn.image.DOFade(0, fadeTime);
			Cohice1Text.DOFade(0, fadeTime);
			Choice2Btn.image.DOFade(0, fadeTime);
			Cohice2Text.DOFade(0, fadeTime);
			Choice1Btn.image.raycastTarget = false;
			Choice2Btn.image.raycastTarget = false;
		}
		if (textOutline != null)
		{
			textOutline.enabled = false;
			if (words.StartsWith("[HL]"))
			{
				textOutline.enabled = true;
				words = words.Remove(0, 4);
			}
		}

		text.text = words;
		// Debug.Log(gameObject.name + " show dialog " + character + " : " + words + " isShowing " + isShowing.ToString());
		

		// text.text = character + ":\r\n" + words;


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
		if (state == State.Normal)
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

	void ShowChoices(string character ,string words,string words2)
	{
		state = State.Choice;
		// if (back.color.a == 1f)
		{
			back.DOFade(0f, fadeTime);
			text.DOFade(0f, fadeTime);
			back.raycastTarget = false;
		}
		// if (Choice1Btn.image.color.a < 1f)
		{
			Choice1Btn.image.DOFade(1f, fadeTime);
			Cohice1Text.DOFade(1f, fadeTime);
			Choice1Btn.image.raycastTarget = true;
		}
		if (words2 != "" && words2 != null)
		{
			Choice2Btn.image.DOFade(1f, fadeTime);
			Cohice2Text.DOFade(1f, fadeTime);
			Choice2Btn.image.raycastTarget = true;
		}else{
			Choice2Btn.image.DOFade(0, fadeTime);
			Cohice2Text.DOFade(0, fadeTime);
			Choice2Btn.image.raycastTarget = false;

		}

		Cohice1Text.text = words;
		Cohice2Text.text = words2;

	}

	public void Choose1()
	{
		if (state == State.Choice)
		{
			if(dialogList.Count >= 4 && Cohice2Text.text.Length >= 1 )
			{
				dialogList.RemoveAt(2);
				dialogList.RemoveAt(2);
			}
			ShowNext();
		}
	}	

	public void Choose2()
	{
		if (state == State.Choice)
		{
			if(dialogList.Count >= 2 )
			{
				dialogList.RemoveAt(0);
				dialogList.RemoveAt(0);
			}
			ShowNext();
		}

	}

	bool ShowNext()
	{
		if (dialogList.Count > 0 )
		{
			string character = dialogList[0].character;
			string words1 = dialogList[0].words;
			string words2 = dialogList[0].words2;
			if (character.Equals("TRAVELLER"))
				showDialog(character,words1);
			else if (character.Equals("YOU"))
				ShowChoices(character, words1 , words2);

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
