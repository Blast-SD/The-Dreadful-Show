﻿using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

using Object = UnityEngine.Object;

#pragma warning disable 0649
[Serializable]
public class TDS_Event 
{
    /* TDS_Event :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Event object used in an event system, triggering something.
	 *
	 *	#####################
	 *	####### TO DO #######
	 *	#####################
	 *
	 *	    • Implement RPC methods if not local.
     *	    
     *	    • Wait for a specific action
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[11 / 04 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_Event class.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties
    /// <summary>
    /// Name of this event.
    /// </summary>
    public string Name = "New Event";

    /// <summary>
    /// Does this event require a specific player type in game.
    /// </summary>
    [SerializeField] private bool doRequireSpecificPlayerType = false;

    /// <summary>
    /// Indicates if the event is waiting for something or not.
    /// </summary>
    public bool IsWaiting { get; private set; } = true;

    /// <summary>
    /// Type of this event.
    /// </summary>
    [SerializeField] private CustomEventType eventType = CustomEventType.UnityEventLocal;

    /// <summary>Public accessor for <see cref="eventType"/>.</summary>
    public CustomEventType EventType { get { return eventType; } }

    /// <summary>
    /// Delay to wait before starting processing this event.
    /// </summary>
    [SerializeField] private float delay = 0;

    /// <summary>
    /// Speed coefficient applied to the camera movement.
    /// </summary>
    [SerializeField] private float cameraSpeedCoef = 1;

    /// <summary>
    /// Time during which the camera will wait in front of its target before reset.
    /// </summary>
    [SerializeField] private float cameraWaitTime = 1;

    /// <summary>
    /// Prefab to instantiate.
    /// </summary>
    [SerializeField] private GameObject prefab = null;

    /// <summary>
    /// Player type required to execute this action.
    /// If unknown, then no specific player type is required.
    /// </summary>
    [SerializeField] private PlayerType playerType = PlayerType.Unknown;

    /// <summary>
    /// Event string, used for text ID to load a narrator quote / information box text, or as name of a prefab to instantiate with Photon.
    /// </summary>
    [SerializeField] private string eventString = "ID";

    /// <summary>
    /// Transform used for the event.
    /// </summary>
    [SerializeField] private Transform eventTransform = null;

    /// <summary>
    /// Unity event to invoke.
    /// </summary>
    [SerializeField] private UnityEvent unityEvent;

    /// <summary>
    /// Action to wait player to perform.
    /// </summary>
    [SerializeField] private WaitForAction actionType = WaitForAction.UseRabbit;
    #endregion

    #region Methods
    /// <summary>
    /// Check left bound position, and stop waiting if set as expected.
    /// </summary>
    private void CheckBound(float _xMovement)
    {
        if (TDS_Camera.Instance.CurrentBounds.XMin >= eventTransform.position.x) StopWaiting();
    }

    /// <summary>
    /// Stop waiting. Just stop it.
    /// </summary>
    public void StopWaiting() => IsWaiting = false;

    /// <summary>
    /// Triggers this event.
    /// </summary>
    /// <returns></returns>
    public IEnumerator Trigger()
    {
        // If this event requires a specific player type and he's not in the game, just skip this event
        if (doRequireSpecificPlayerType && !TDS_LevelManager.Instance.AllPlayers.Any(p => p.PlayerType == playerType)) yield break;

        // Wait for delay
        yield return new WaitForSeconds(delay);

        switch (eventType)
        {
            // Make a camera movement
            case CustomEventType.CameraMovement:
                yield return TDS_Camera.Instance.LookTarget(eventTransform.position.x, eventTransform.position.y, eventTransform.position.z, cameraWaitTime, cameraSpeedCoef);
                break;

            // Desactivate the current information box
            case CustomEventType.DesactiveInfoBox:
                if (doRequireSpecificPlayerType)
                {
                    TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", TDS_LevelManager.Instance.AllPlayers.ToList().Where(p => p.PlayerType == playerType).Select(p => p.photonView.owner).First(), TDS_RPCManager.GetInfo(TDS_UIManager.Instance.photonView, TDS_UIManager.Instance.GetType(), "DesactivateDialogBox")
                      , new object[] { });
                }
                else
                {
                    TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.All, TDS_RPCManager.GetInfo(TDS_UIManager.Instance.photonView, TDS_UIManager.Instance.GetType(), "DesactivateDialogBox")
                      , new object[] { });
                }
                break;

            // Display a message in an information box
            case CustomEventType.DisplayInfoBox:
                // Activate the info box for the requested player or for everyone
                if (doRequireSpecificPlayerType)
                {
                    TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", TDS_LevelManager.Instance.AllPlayers.ToList().Where(p => p.PlayerType == playerType).Select(p => p.photonView.owner).First(), TDS_RPCManager.GetInfo(TDS_UIManager.Instance.photonView, TDS_UIManager.Instance.GetType(), "ActivateDialogBox")
                      , new object[] { TDS_GameManager.GetDialog(eventString)[0] });
                }
                else
                {
                    TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.All, TDS_RPCManager.GetInfo(TDS_UIManager.Instance.photonView, TDS_UIManager.Instance.GetType(), "ActivateDialogBox")
                      , new object[] { TDS_GameManager.GetDialog(eventString)[0] });
                }
                break;

            // Instantiate a prefab
            case CustomEventType.Instantiate:
                GameObject _object = Object.Instantiate(prefab, eventTransform.position, eventTransform.rotation);
                _object.transform.SetParent(eventTransform, true);
                break;

            // Instantiate with Photon
            case CustomEventType.InstantiatePhoton:
                PhotonNetwork.Instantiate(eventString, eventTransform.position, eventTransform.rotation, 0);
                break;

            // Triggers a particular quote of the Narrator
            case CustomEventType.Narrator:
                TDS_UIManager.Instance.ActivateNarratorBox(TDS_GameManager.GetDialog(eventString).Skip(1).ToArray());
                break;

            // Just invoke a Unity Event, that's it
            case CustomEventType.UnityEventLocal:
                unityEvent.Invoke();
                break;

            // Just invoke a Unity Event, that's it
            case CustomEventType.UnityEventOnline:
                unityEvent.Invoke();
                break;

            // Wait for an action of the local player
            case CustomEventType.WaitForAction:
                switch (actionType)
                {
                    case WaitForAction.UseRabbit:
                        TDS_WhiteRabbit.OnUseRabbit += StopWaiting;
                        TDS_WhiteRabbit.OnLoseRabbit += StopWaiting;
                        break;

                    default:
                        break;
                }

                while (IsWaiting)
                {
                    yield return null;
                }

                switch (actionType)
                {
                    case WaitForAction.UseRabbit:
                        TDS_WhiteRabbit.OnUseRabbit -= StopWaiting;
                        TDS_WhiteRabbit.OnLoseRabbit -= StopWaiting;
                        break;

                    default:
                        break;
                }

                IsWaiting = true;
                break;

            // Wait that everyone is in the zone
            case CustomEventType.WaitForEveryone:
                if ((TDS_LevelManager.Instance.OtherPlayers.Count > 0) && (TDS_Camera.Instance.CurrentBounds.XMin < eventTransform.position.x))
                {
                    TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.All, TDS_RPCManager.GetInfo(TDS_UIManager.Instance.photonView, typeof(TDS_UIManager), "SwitchWaitingPanel"), new object[] { });

                    TDS_Camera.Instance.OnMoveX += CheckBound;

                    yield return new WaitForSeconds(.25f);

                    // Wait patiently
                    while (IsWaiting) yield return null;

                    TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.All, TDS_RPCManager.GetInfo(TDS_UIManager.Instance.photonView, typeof(TDS_UIManager), "SwitchWaitingPanel"), new object[] { });
                }
                break;

            // Nobody here but us chicken
            default:
                break;
        }
        
        yield break;
    }
	#endregion
}
