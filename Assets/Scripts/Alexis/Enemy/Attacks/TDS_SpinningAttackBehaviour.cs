﻿        using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDS_SpinningAttackBehaviour : TDS_EnemyAttack
{
    /* TDS_SpinningAttackBehaviour :
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
    /// <summary>
    /// Movement speed of the Siamese during the third attack
    /// </summary>
    [SerializeField, Range(2, 10)] private float spinningSpeed = 8;
    /// <summary>
    /// Index of the spinning position
    /// </summary>
    private int spinningIndex = 0;
    /// <summary>
    /// Positions within the bounds of the camera
    /// </summary>
    private Vector3[] spinningPositions = null;

    private TDS_Enemy caster = null; 
    #endregion

    #region Methods

    #region Original Methods

    public override void ApplyAttackBehaviour(TDS_Enemy _caster)
    {
        if (spinningPositions == null)
        {
            TDS_Bounds _bounds = TDS_Camera.Instance.CurrentBounds;
            spinningPositions = new Vector3[5] {new Vector3(_bounds.XMin + 2, 0, _bounds.ZMax + 2 ),
                                            new Vector3(_bounds.XMin + 2, 0, _bounds.ZMin - 2 ),
                                            new Vector3(_bounds.XMax - 2, 0, _bounds.ZMax + 2 ),
                                            new Vector3(_bounds.XMax - 2, 0, _bounds.ZMin - 2 ),
                                            new Vector3((_bounds.XMin + _bounds.XMax) / 2, 0, (_bounds.ZMin + _bounds.ZMax) / 2)};
        }
        caster = _caster;
        caster.Agent.Speed = spinningSpeed;
        caster.Agent.OnDestinationReached += GoNextSpinningPosition;
        GoNextSpinningPosition();
    }


    /// <summary>
    /// Set the new destination as the next position in the spinningPositions array
    /// If there is no more positions in the array, reset all settings and call the Method StopSpinning
    /// </summary>
    private void GoNextSpinningPosition()
    {
        if (spinningIndex > 4)
        {
            caster.Agent.OnDestinationReached -= GoNextSpinningPosition;
            spinningIndex = 0;
            StopSpinning();
            //casterAgent.Speed = SpeedInitial;
            return;
        }
        caster.Agent.SetDestination(spinningPositions[spinningIndex]);
        spinningIndex++;
    }


    /// <summary>
    /// Stop the spinning Animation
    /// -> Stop the attack in the animation
    /// </summary>
    private void StopSpinning()
    {
        caster.SetAnimationTrigger("StopSpinning"); 
    }
    #endregion

    #endregion
}