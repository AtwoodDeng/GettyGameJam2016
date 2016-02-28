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

	/////////begin
	[SerializeField] float beginShowTextTime;
	[SerializeField]  Text beginText;
	[SerializeField] string beginSentence =  "Hello? Anyone out there? I need some help! Hello! ";
	[SerializeField] string beginSentence2 =  " I need your help. But let's make sure we're in the right frequency first.";
	[SerializeField] Button beginHellowBtn;
	[SerializeField] Image white;

	[SerializeField] Image SecretaireOpen;
	[SerializeField] Image SecretaireClose;

	[SerializeField] Camera mainCamera;

	[SerializeField] GameObject room_secre;

	[SerializeField] DialogFrame dialogFrame;
	[SerializeField] Image character;

	[SerializeField] List<string> states;
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
		if (tempState != null)
			tempState.OnDialogEnd();
	}

	void OnTrackConfirm(Message msg)
	{
		if (tempState != null)
			tempState.OnTrackConfirm();

		StartRoom();
	}

	void Awake()
	{
		helper = new XMLHelper("Data/script");

		NextState();
		EnterRoom();
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
		NextState();

		character.gameObject.SetActive(true);
	}

	public void StartScan()
	{
		Debug.Log("Start Camera");
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

	}

	public void OnShowItemDialog(string item)
	{
		DataTable table = helper.ReadSheet(item);
		for(int i = 0; i < table.rows.Count ; ++i)
		{
			dialogFrame.addDialog(table.rows[i].row.Select("Character"),table.rows[i].row.Select("Dialog"));
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
		Beginning.SetActive(false);
		Room.SetActive(false);
		RoomUI.SetActive(false);
		Track.SetActive(false);
		TrackUI.SetActive(false);
		Secretaire.SetActive(false);
		End.SetActive(false);
		dialogFrame.gameObject.SetActive(false);
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
			for(int i = 0; i < table.rows.Count ; ++i)
			{
				dialogFrame.addDialog(table.rows[i].row.Select("Character"),table.rows[i].row.Select("Dialog"));
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
		}
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
