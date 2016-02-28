using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LogicManager : MonoBehaviour {

	[SerializeField] GameObject Beginning;
	[SerializeField] GameObject Room;
	[SerializeField] GameObject RoomUI;
	[SerializeField] GameObject Track;
	[SerializeField] GameObject TrackUI;
	[SerializeField] GameObject Secretaire;
	[SerializeField] GameObject End;

	[SerializeField] Camera mainCamera;

	[SerializeField] GameObject room_secre;

	[SerializeField] DialogFrame dialogFrame;

	[SerializeField] List<string> states;
	[SerializeField] State tempState;


	public LogicManager() { s_Instance = this; }
	public static LogicManager Instance { get { return s_Instance; } }
	private static LogicManager s_Instance;

	XMLHelper helper;

	void OnEnable()
	{
		// EventManager.Instance.RegistersEvent(EventDefine.TrackFound, OnTrackingFound);
		EventManager.Instance.RegistersEvent(EventDefine.BEGIN_DIALOG, OnBeginDialog);
		EventManager.Instance.RegistersEvent(EventDefine.END_DIALOG, OnEndDialog);
		EventManager.Instance.RegistersEvent(EventDefine.TRACK_CONFIRM, OnTrackConfirm);
	}

	void OnDisable()
	{
		// EventManager.Instance.UnregistersEvent(EventDefine.TrackFound, OnTrackingFound);
		EventManager.Instance.UnregistersEvent(EventDefine.BEGIN_DIALOG, OnBeginDialog);
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

	void OnBeginDialog(Message msg)
	{

	}


	void Awake()
	{
		helper = new XMLHelper("Data/script");

		Debug.Log(helper.ReadSheet("first").Select("0").Select("Dialog"));

		NextState();
	}

	void Update()
	{
		if (Input.GetKeyUp(KeyCode.C))
		{
			StartCamera();
		}

		if (Input.GetKeyUp(KeyCode.R))
		{
			StartRoom();
		}
	}

	public void StartCamera()
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
	}


	public void OnShowItemDialog(string item)
	{
		DataTable table = helper.ReadSheet(item);
		for(int i = 0; i < table.rows.Count ; ++i)
		{
			dialogFrame.addDialog(table.rows[i].row.Select("Character"),table.rows[i].row.Select("Dialog"));
		}

	}

	public void StartSecretaire()
	{

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
		}else if (name.StartsWith("track"))
		{
			tempState = new TrackState();
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
}
