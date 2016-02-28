using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

public class LogicManager : MonoBehaviour {

	[SerializeField] GameObject Beginning;
	[SerializeField] GameObject Room;
	[SerializeField] GameObject RoomUI;
	[SerializeField] GameObject Track;
	[SerializeField] GameObject TrackUI;
	[SerializeField] GameObject Secretaire;
	[SerializeField] GameObject End;
	[SerializeField] GameObject Title;

	/////////begin
	[SerializeField] float beginShowTextTime;
	[SerializeField]  Text beginText;
	[SerializeField] string beginSentence =  "Hello? Anyone out there? I need some help! Hello! ";
	[SerializeField] string beginSentence2 =  " I need your help. But let's make sure we're in the right frequency first.";
	[SerializeField] Button beginHellowBtn;
	[SerializeField] Image white;
	[SerializeField] AudioSource lostSignal;
	[SerializeField] Image signal;

	[SerializeField] Image SecretaireOpen;
	[SerializeField] Image SecretaireClose;
	[SerializeField] Image bigWhite;
	[SerializeField] Image bigPicture;
	[SerializeField] Sprite[] BigSprites;

	[SerializeField] Camera mainCamera;

	[SerializeField] GameObject room_secre;

	[SerializeField] DialogFrame dialogFrame;
	[SerializeField] Image character;

	[SerializeField] Image scanBack;
	[SerializeField] Image scanAni;
	[SerializeField] Image Map;

	[SerializeField] Text scanTutorial;
	[SerializeField] Image scanTutorialBack;
	[SerializeField] Text tagTutorial;
	[SerializeField] Image tagTutorialBack;

	[SerializeField] Button[] itemButtons;

	List<string> states = new List<string>();
	[SerializeField] State tempState;


	public LogicManager() { s_Instance = this; }
	public static LogicManager Instance { get { return s_Instance; } }
	private static LogicManager s_Instance;

	XMLHelper helper;

	void OnEnable()
	{
		EventManager.Instance.RegistersEvent(EventDefine.END_DIALOG, OnEndDialog);
		EventManager.Instance.RegistersEvent(EventDefine.TRACK_CONFIRM, OnTrackConfirm);
	}

	void OnDisable()
	{
		EventManager.Instance.UnregistersEvent(EventDefine.END_DIALOG, OnEndDialog);
		EventManager.Instance.UnregistersEvent(EventDefine.TRACK_CONFIRM, OnTrackConfirm);
	}

	public bool testTrack(string _name)
	{
		string pattern_Name = tempState.name.Split('-')[1];
		return _name == pattern_Name;
	}

	void OnEndDialog(Message msg)
	{
		if ( tempState.name.EndsWith("scan"))
		{
			scanBack.gameObject.SetActive(true);
			scanAni.gameObject.SetActive(true);

			scanBack.DOFade(0,0);
			scanAni.DOFade(0,0);
			scanBack.DOFade(0.7f, 1f);
			scanAni.DOFade(1f, 1f);

			scanTutorial.DOFade(1f, 1f);
			scanTutorialBack.DOFade(1f, 1f);
		}

		if (tempState.name.EndsWith("room"))
		{
			tagTutorial.DOFade(1f, 1f);
			tagTutorialBack.DOFade(1f, 1f);
			foreach(Button btn in itemButtons)
			{
				btn.image.raycastTarget = true;
			}
		}




		if (tempState != null)
			tempState.OnDialogEnd();
		bigPicture.DOFade(0, 1f);
		bigWhite.DOFade(0, 1f);

	}

	void OnTrackConfirm(Message msg)
	{
		if (tempState != null)
			tempState.OnTrackConfirm();

		StartRoom();
	}

	void Awake()
	{
		states = new List<string>();
		states.Add("begin");
		states.Add("dialog-room");
		states.Add("delay-60");
		states.Add("dialog-scan");
		states.Add("dialog-scan2");
		states.Add("track-Medusa");
		states.Add("dialog-room2");
		states.Add("track-Lion");
		states.Add("dialog-room3");
		states.Add("sec");
		states.Add("dialog-map");
		states.Add("end");

		helper = new XMLHelper("Data/script");

		StartCoroutine(Begin());
		// NextState();
		// EnterRoom();
	}

	IEnumerator Begin()
	{
		HideAll();
		Title.SetActive(true);
		yield return new WaitForSeconds(5f);
		NextState();

	}

	void Update()
	{
		if (Input.GetKeyUp(KeyCode.C))
		{
			StartScan();
		}

		if (Input.GetKeyUp(KeyCode.R))
		{
			StartRoom();
		}

		if (Input.GetKeyUp(KeyCode.D))
		{
			EventManager.Instance.PostEvent(EventDefine.TRACK_CONFIRM);
		}
	}
	public void StartBegin()
	{
		HideAll();
		Beginning.SetActive(true);
		beginText.DOText( beginSentence , beginShowTextTime).OnComplete( ShowBeginButton);
	}
	public void ShowBeginButton()
	{
		beginHellowBtn.image.DOFade(1f, 1f);
		beginHellowBtn.GetComponentInChildren<Text>().DOFade(1f, 1f);
	}

	public void StartBegin2()
	{
		Sequence seq = DOTween.Sequence();
		beginHellowBtn.gameObject.SetActive(false);
		beginText.text = "";
		seq.Append( beginText.DOText( beginSentence2 , beginShowTextTime))
			.AppendInterval(2f)
			.Append(white.DOFade(1f, 2f).OnComplete(EnterRoom))
			.Append(white.DOFade(0f, 2f));
	}

	public void EnterRoom()
	{
		StartRoom();

		lostSignal.Play();
		lostSignal.DOFade(0, 2f).SetDelay(1f);
		signal.DOFade(1f, 0);
		signal.DOFade(0, 2f).SetDelay(1f).OnComplete(OnSignalOut);

		character.gameObject.SetActive(true);
	}

	public void OnSignalOut()
	{

		NextState();

	}

	public void StartScan()
	{
		Debug.Log("Start Camera");

		scanTutorial.DOFade(0f, 1f);
		scanTutorialBack.DOFade(0f, 1f);

		HideAll();
		Track.SetActive(true);
		TrackUI.SetActive(true);
		mainCamera.enabled = false;
	}

	public void StartRoom()
	{
		HideAll();
		Room.SetActive(true);
		RoomUI.SetActive(true);
		mainCamera.enabled = true;
		dialogFrame.gameObject.SetActive(true);
	}

	public void StartSecretaire()
	{
		HideAll();
		Secretaire.SetActive(true);
		dialogFrame.gameObject.SetActive(true);
		Room.SetActive(true);

		bigWhite.DOFade(1f, 1f);

		Debug.Log("add dialog");
		dialogFrame.addDialog("TRAVELLER","Can you see a map anywhere?","");

	}

	public void OnShowItemDialog(string item)
	{
		int bigIndex = 0 ;
		if (item == "teaset")
			bigIndex = 0;
		if (item == "bronzes")
			bigIndex = 1;
		if (item == "scu1")
			bigIndex = 2;
		if (item == "clock")
			bigIndex = 3;


		tagTutorial.DOFade(0f, 1f);
		tagTutorialBack.DOFade(0f, 1f);

		bigPicture.sprite = BigSprites[bigIndex];
		bigPicture.DOFade(1f, 1f);
		bigWhite.DOFade(0.8f, 1f);

		Debug.Log("ReadItem" + item);
		DataTable table = helper.ReadSheet(item);

		for(int i = 0; i < table.rows.Count ; ++i)
		{
			string dialog2 = "";
			if (table.Contains("Dialog2"))
				dialog2 = table.rows[i].row.Select("Dialog2");
			dialogFrame.addDialog(table.rows[i].row.Select("Character"),table.rows[i].row.Select("Dialog"),dialog2);
		}
	}

	public void OnMap()
	{
		NextState();
	}

	public void StartEnd()
	{
		HideAll();
		End.SetActive(true);
	}

	public void HideAll()
	{
		Title.SetActive(false);
		Beginning.SetActive(false);
		Room.SetActive(false);
		RoomUI.SetActive(false);
		Track.SetActive(false);
		TrackUI.SetActive(false);
		Secretaire.SetActive(false);
		End.SetActive(false);
		dialogFrame.gameObject.SetActive(false);
		signal.DOFade(0, 0);
	}

	public void NextState()
	{
		if (states.Count <= 0 )
			return;
		string name = states[0];
		states.RemoveAt(0);

		if ( tempState != null )
		{
			tempState.EndState();
		}

		if (name.StartsWith("dialog"))
		{
			string sheet = name.Split('-')[1];
			DataTable table = helper.ReadSheet(sheet);
			Debug.Log("dialog size" + table.rows.Count);
			for(int i = 0; i < table.rows.Count ; ++i)
			{
				string dialog2 = "";
				if (table.Contains("Dialog2"))
					dialog2 = table.rows[i].row.Select("Dialog2");
				dialogFrame.addDialog(table.rows[i].row.Select("Character"),table.rows[i].row.Select("Dialog"),dialog2);
			}

			tempState = new DialogState();
			tempState.Init(name);


			if (name.EndsWith("room2"))
			{
				SecretaireClose.gameObject.SetActive(true);
			}else if (name.EndsWith("room3"))
			{
				SecretaireClose.gameObject.SetActive(false);
				SecretaireOpen.gameObject.SetActive(true);
			}

			if (name.EndsWith("map"))
			{
				bigWhite.DOFade(0, 1f);
				Map.gameObject.SetActive(true);

			}

		}else if (name.StartsWith("track"))
		{
			tempState = new TrackState();
			tempState.Init(name);
		}else if (name.StartsWith("begin"))
		{
			tempState = new BeginState();
			tempState.Init(name);
		}else if (name.StartsWith("sec"))
		{
			tempState = new SecState();
			tempState.Init(name);	
		}else if (name.StartsWith("end"))
		{
			tempState = new EndState();
			tempState.Init(name);	
		}else if (name.StartsWith("delay"))
		{
			tempState = new DelayState();
			tempState.Init(name);
			float delay = float.Parse(name.Split('-')[1]);
			StartCoroutine(DelayNextState(delay));
		}


	}

	IEnumerator DelayNextState(float delay)
	{
		yield return new WaitForSeconds(delay);
		while(dialogFrame.GetIsShowing())
		{
			yield return null;
		}
		yield return new WaitForSeconds(0.1f);
		NextState();

	}
}

[System.SerializableAttribute]
class State
{
	public string name;
	static public int number = 0;
	virtual public void Init(string _name)
	{
		name = _name;
		number ++;
	}
	virtual public void Update(){}
	virtual public void OnDialogEnd(){}
	virtual public void OnTrackConfirm(){}
	virtual public void EndState(){}
}

class DialogState: State
{
	override public void Init(string _name)
	{
		base.Init(_name);
		LogicManager.Instance.StartRoom();

	}

	override public void OnDialogEnd()
	{
		LogicManager.Instance.NextState();
	}
}

class TrackState: State
{
	override public void Init(string _name)
	{
		base.Init(_name);
	}
	override public void OnTrackConfirm()
	{
		LogicManager.Instance.NextState();
	}
}

class SecState: State
{
	override public void Init(string _name)
	{
		base.Init(_name);
	}
}

class BeginState: State
{
	override public void Init(string _name)
	{
		base.Init(_name);
		LogicManager.Instance.StartBegin();
	}
}

class EndState: State
{
	override public void Init(string _name)
	{
		base.Init(_name);
		LogicManager.Instance.StartEnd();
	}
}

class DelayState: State
{
}