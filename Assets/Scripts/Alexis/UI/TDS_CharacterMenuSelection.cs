﻿using System.Linq; 
using UnityEngine;

#pragma warning disable 0649

public class TDS_CharacterMenuSelection : MonoBehaviour
{
    /* TDS_CharacterMenuSelection :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	[PURPOSE]
	 *
	 *	#####################
	 *	####### TO DO #######
	 *	#####################
	 *
	 *	[TO DO]
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[DATE]
	 *	Author :		[NAME]
	 *
	 *	Changes :
	 *
	 *	[CHANGES]
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties
    [Header("Character Selection Elements")]
    [SerializeField] private TDS_CharacterSelectionElement[] characterSelectionElements = new TDS_CharacterSelectionElement[] { };
    private TDS_CharacterSelectionElement localElement = null;
    public TDS_CharacterSelectionElement LocalElement
    {
        get { return localElement;  }
    }
    #endregion

    #region Methods

    #region Online
    /// <summary>
    /// Add a new player within the CharacterSelectionElements
    /// If the added player is the local player, set the element as the local element
    /// </summary>
    /// <param name="_newPlayer">Id of the added player</param>
    public void AddNewPhotonPlayer(PhotonPlayer _newPlayer)
    {
        characterSelectionElements.Where(e => e.PhotonPlayer == null).FirstOrDefault()?.SetPhotonPlayer(_newPlayer);
        if (_newPlayer.ID == PhotonNetwork.player.ID)
        {
            localElement = characterSelectionElements.Where(e => e.PhotonPlayer == _newPlayer).First();
            localElement.SetPlayerLocal();
            return; 
        }
    }

    /// <summary>
    /// Remove the player from the character selection Elements
    /// If the player is'nt connected anymore, clear all the character selection elements
    /// </summary>
    /// <param name="_removedPlayer"></param>
    public void RemovePhotonPlayer(PhotonPlayer _removedPlayer)
    {
        if (PhotonNetwork.room == null || !PhotonNetwork.connected || PhotonNetwork.player == null)
        {
            ClearOnlineMenu(); 
            return; 
        }
        if (_removedPlayer == PhotonNetwork.player) return;
        TDS_CharacterSelectionElement _cleanedElement = characterSelectionElements.Where(e => e.PhotonPlayer == _removedPlayer).FirstOrDefault();
        if (_cleanedElement)
        {
            _cleanedElement.DisconnectPlayer();
        }
        
    }

    /// <summary>
    /// Clear all the character Selection elements 
    /// </summary>
    public void ClearOnlineMenu()
    {
        characterSelectionElements.ToList().ForEach(e => e.DisconnectPlayer()); 
    }

    /// <summary>
    /// Lock a particulary element 
    /// </summary>
    /// <param name="_playerID">Id of the player to lock</param>
    /// <param name="_playerIsLocked">Does the element has to be locked or unlocked</param>
    public void LockOnlinePlayer(int _playerID, bool _playerIsLocked)
    {
        // SET THE TOGGLE
        if (PhotonNetwork.player.ID == _playerID)
        {
            localElement.IsLocked = _playerIsLocked;
            return;
        }
        characterSelectionElements.Where(e => e.PhotonPlayer.ID == _playerID).First().LockElement(_playerIsLocked);
    }

    /// <summary>
    /// Make the elements selectable or not if a player select it
    /// If the local can't be selected, display the next selectable element
    /// </summary>
    /// <param name="_previousType">Previous Type of the player</param>
    /// <param name="_newType">new Type of the player</param>
    public void UpdateOnlineSelection(PlayerType _newType, int _playerID)
    {
        characterSelectionElements.ToList().ForEach(e => e.CharacterSelectionImages.Where(i => i.CharacterType == _newType).ToList().ForEach(i => i.CanBeSelected = false));
        TDS_CharacterSelectionElement _element = characterSelectionElements.Where(e => e.PhotonPlayer.ID == _playerID).FirstOrDefault(); 
        if(_element)
        {
            _element.DisplayImageOfType(_newType); 
        }
        if (!TDS_GameManager.LocalIsReady && !localElement.CurrentSelection.CanBeSelected) localElement.DisplayNextImage(); 
    }

    /// <summary>
    /// Unlock the character when a player leave the game
    /// </summary>
    /// <param name="_playerType">Player type to unlock</param>
    public void UnlockCharacterOnline(PlayerType _playerType)
    {
        characterSelectionElements.ToList().ForEach(e => e.CharacterSelectionImages.Where(i => i.CharacterType == _playerType).ToList().ForEach(i => i.CanBeSelected = true));
    }

    /// <summary>
    /// Unlock the character when a player leave the game
    /// </summary>
    /// <param name="_playerType">Player type to unlock</param>
    public void UnlockCharacterOnline(int _playerType)
    {
        characterSelectionElements.ToList().ForEach(e => e.CharacterSelectionImages.Where(i => i.CharacterType == (PlayerType)_playerType).ToList().ForEach(i => i.CanBeSelected = true));
    }

    /// <summary>
    /// Update the selection element relative to a player. 
    /// Used online
    /// </summary>
    /// <param name="_player"></param>
    /// <param name="_newIndex"></param>
    public void UpdateMenuOnline(int _playerID, int _newIndex)
    {
        characterSelectionElements.Where(e => e.PhotonPlayer.ID == _playerID).FirstOrDefault().DisplayImageAtIndex(_newIndex); 
    }

    /// <summary>
    /// Update the selection element relative to a player. 
    /// Used online
    /// </summary>
    /// <param name="_player"></param>
    /// <param name="_newIndex"></param>
    public void UpdateMenuOnline(int _playerID, PlayerType _playerType)
    {
        characterSelectionElements.Where(e => e.PhotonPlayer.ID == _playerID).FirstOrDefault().DisplayImageOfType(_playerType);
    }

    #endregion

    #region Local 
    public void AddNewPlayer()
    {
        TDS_CharacterSelectionElement _elem = characterSelectionElements.Where(e => !e.IsUsedLocally).FirstOrDefault();
        if (!_elem) return;
        _elem.SetPlayerLocalID(); 
    }
    #endregion 

    #endregion

}
