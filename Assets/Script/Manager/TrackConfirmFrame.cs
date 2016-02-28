using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TrackConfirmFrame : MonoBehaviour {

	[SerializeField] Button btn;
	[SerializeField] Image back;
	[SerializeField] Text text;
	[SerializeField] string showText = "You find a NAME";
	// Use this for initialization
	void Awake () {
		btn.gameObject.SetActive(false);
		back.gameObject.SetActive(false);
		text.gameObject.SetActive(false);
	}

	void OnEnable()
	{
		EventManager.Instance.RegistersEvent(EventDefine.TrackFound, OnTrackingFound);
	}

	void OnDisable()
	{
		EventManager.Instance.UnregistersEvent(EventDefine.TrackFound, OnTrackingFound);
	}

	void OnTrackingFound(Message msg)
	{
		string name = (string)msg.GetMessage("name");
		if (LogicManager.Instance.testTrack(name))
			Show(name);	
	}

	void Show(string name)
	{
		text.text = showText.Replace("NAME", name);
		btn.gameObject.SetActive(true);
		back.gameObject.SetActive(true);
		text.gameObject.SetActive(true);

	}

	public void Confirm()
	{
		EventManager.Instance.PostEvent(EventDefine.TRACK_CONFIRM);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
