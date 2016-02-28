using UnityEngine;
using System.Collections;

namespace Vuforia
{
    /// <summary>
    /// A custom handler that implements the ITrackableEventHandler interface.
    /// </summary>
    public class TrackPattern : MonoBehaviour, ITrackableEventHandler
    {
        #region PRIVATE_MEMBER_VARIABLES
 
        private TrackableBehaviour mTrackableBehaviour;
    
        #endregion // PRIVATE_MEMBER_VARIABLES



        #region UNTIY_MONOBEHAVIOUR_METHODS
    
        void Start()
        {
            mTrackableBehaviour = GetComponent<TrackableBehaviour>();
            if (mTrackableBehaviour)
            {
                mTrackableBehaviour.RegisterTrackableEventHandler(this);
            }
        }

        #endregion // UNTIY_MONOBEHAVIOUR_METHODS

        #region PUBLIC_METHODS

        /// <summary>
        /// Implementation of the ITrackableEventHandler function called when the
        /// tracking state changes.
        /// </summary>
         public void OnTrackableStateChanged(
                                        TrackableBehaviour.Status previousStatus,
                                        TrackableBehaviour.Status newStatus)
        {
            if (newStatus == TrackableBehaviour.Status.DETECTED ||
                newStatus == TrackableBehaviour.Status.TRACKED ||
                newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
            {
                OnTrackingFound();
            }
            else
            {
                OnTrackingLost();
            }
        }

        #endregion // PUBLIC_METHODS



        #region PRIVATE_METHODS

        private void OnTrackingFound()
        {	
        	ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
        	foreach(ParticleSystem p in particles)
        	{
        		p.enableEmission = true;
        	}

        	Message msg = new Message();
        	msg.AddMessage("name", gameObject.name);
        	EventManager.Instance.PostEvent(EventDefine.TrackFound, msg );
        }

        private void OnTrackingLost()
        {

        	ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
        	foreach(ParticleSystem p in particles)
        	{
        		p.enableEmission = false;
        	}
        
        	Message msg = new Message();
        	msg.AddMessage("name", gameObject.name);
        	EventManager.Instance.PostEvent(EventDefine.TrackLost, msg );	
        }


        #endregion // PRIVATE_METHODS
    }
}
