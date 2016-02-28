using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PlaySound : MonoBehaviour {


	void OnEnable()
	{
		EventManager.Instance.RegistersEvent(EventDefine.END_DIALOG, OnEndDialog);
	}

	void OnDisable()
	{
		EventManager.Instance.UnregistersEvent(EventDefine.END_DIALOG, OnEndDialog);
	}


	void OnEndDialog(Message msg)
	{
		sound.DOFade(0, 1f);
	}

	[SerializeField] AudioSource sound;
	public void Play()
	{
		sound.Play();
	}
}
