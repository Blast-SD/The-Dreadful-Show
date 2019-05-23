﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDS_ThrowingAttackBehaviour : TDS_EnemyAttack 
{
    /* TDS_ThrowingAttackBehaviour :
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

    #region Events

    #endregion

    #region Fields / Properties
    [SerializeField] private string thrownObjectName = string.Empty;  
    #endregion

    #region Methods

    #region Original Methods
    public override void ApplyAttackBehaviour(TDS_Enemy _caster)
    {
        TDS_Throwable _throwable = PhotonNetwork.Instantiate("ExplosiveGift", _caster.HandsTransform.position, Quaternion.identity, 0).GetComponent<TDS_Throwable>();
        _throwable.PickUp(_caster, _caster.HandsTransform);
        _caster.ThrowObject(_caster.PlayerTarget.transform.position);
    }
    #endregion

    #endregion
}