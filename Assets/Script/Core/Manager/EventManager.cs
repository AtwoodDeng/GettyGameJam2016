using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class EventManager : MonoBehaviour
{

	public EventManager() { s_Instance = this; }
	public static EventManager Instance { get { return s_Instance; } }
	private static EventManager s_Instance;
	void Awake(){ s_Instance = this; }
	
	private  struct EventClass
	{

		public EventClass(EventDefine name, EventHandler handler , object sender = null)
		{
			m_Name = name; m_Handler = handler; m_sender = sender;
		}
		public EventDefine m_Name;
		public EventHandler m_Handler;
		public object m_sender;
	};

	private  struct EventInvocation
	{

		public EventInvocation(EventDefine name, Message msg = null, object sender = null)
		{
			m_Name = name; m_message = msg; m_sender = sender;
		}
		public EventDefine m_Name;
		public Message m_message;
		public object m_sender; 
	};

	private Dictionary<EventDefine,Dictionary<EventHandler,EventClass>> eventDict 
		= new Dictionary<EventDefine,Dictionary<EventHandler,EventClass>> ();

	public delegate void EventHandler(Message msg);

	public void RegistersEvent(EventDefine eventName , EventHandler handler , object sender = null )
	{
		if ( eventDict.ContainsKey(eventName) )
		{
			Dictionary<EventHandler,EventClass> ecDict;
			eventDict.TryGetValue(eventName, out ecDict);
			if ( ecDict == null )
			{
				Debug.LogError("No event class list found in " + eventName.ToString() +"'s dictionary");
			}else
			{
				if ( ecDict.ContainsKey(handler) )
				{
					Debug.LogError("Mutiply define of handler" + eventName.ToString());
				}else
				{
					ecDict.Add(handler, new EventClass(eventName,handler,sender));
				}
			}
		}
		else
		{
			var ecDict = new Dictionary<EventHandler,EventClass>();
			ecDict.Add(handler, new EventClass(eventName,handler,sender));
			eventDict.Add(eventName, ecDict);
		}
	}

	public void UnregistersEvent(EventDefine eventName , EventHandler handler)
	{
		if ( eventDict.ContainsKey(eventName))
		{
			Dictionary<EventHandler,EventClass> ecDict;
			eventDict.TryGetValue(eventName, out ecDict);
			if ( ecDict == null )
			{
				Debug.LogError("no event class list found in " + eventName.ToString() +"'s list");
			}else
			{
				if ( !ecDict.ContainsKey(handler) )
				{
					Debug.LogError("No define of handler" + eventName.ToString());
				}else
				{
					ecDict.Remove(handler);
				}
			}
		}
	}

	List<EventInvocation> postEventList = new List<EventInvocation>();

	public void PostEvent(EventDefine eventName , Message msg = null , object sender=null)
	{
		if (msg == null )
			msg = new Message();
		postEventList.Add(new EventInvocation(eventName,msg, sender));
	}

	void Update()
	{
		foreach(EventInvocation eventInv in postEventList)
		{
			EventDefine tempEventName = eventInv.m_Name;
			foreach(EventHandler handler in eventDict[tempEventName].Keys)
			{
				EventClass eventClass = eventDict[tempEventName][handler];
				Message msg = eventInv.m_message;
				if ( eventInv.m_sender == null )
					msg.SetSender(eventClass.m_sender);
				else
					msg.SetSender(eventInv.m_sender);
				msg.SetEventName(eventClass.m_Name);
				handler(msg);
			}
		}
		postEventList.Clear();
	}
}

public class Message
{
	public Message(){}
	EventDefine m_eventName;
	object m_sender;
	Dictionary<string,object> m_dict = new Dictionary<string,object>();

	public EventDefine eventName {get{return m_eventName;}}
	public object sender{get{return m_sender;}}

	public void SetEventName(EventDefine name){
		m_eventName = name;
	}
	public void SetSender(object sender){
		m_sender = sender;
	}
	public void AddMessage(string key, object val)
	{
		m_dict.Add(key, val);
	}
	public object GetMessage(string key)
	{
		object res;
		m_dict.TryGetValue(key , out res);
		return res;
	}
}