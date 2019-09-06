﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random; 

public class TDS_BrutalSiamese : TDS_Enemy 
{
    /* TDS_BrutalSiamese :
     *
     *	#####################
     *	###### PURPOSE ######
     *	#####################
     *
     *	[Behaviour of the brutal siamese]
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
     *	Date :			[24/05/2019]
     *	Author :		[THIEBAUT Alexis]
     *
     *	Changes :
     *
     *	[Initialisation of the class]
     *  
     *	-----------------------------------
    */

    #region Events

    #endregion

    #region Fields / Properties

    #endregion

    #region Methods

    #region Original Methods
    private IEnumerator WaitSecondsBeforeAttacking(float _waitingSeconds = 2f)
    {
        IsPacific = true;
        yield return new WaitForSeconds(_waitingSeconds);
        IsPacific = false;
    }
    #endregion

    #region Overriden Methods
    protected override Vector3 GetAttackingPosition(out bool _hasToWander)
    {
        _hasToWander = false; 
        return base.GetAttackingPosition();
    }

    protected override void Die()
    {
        base.Die();
        if (Area) Area.RemoveEnemy(this);
    }
    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    protected override void Awake()
    {
        base.Awake();
    }

	// Use this for initialization
    protected override void Start()
    {
        base.Start();
        canThrow = false;

        StartCoroutine(WaitSecondsBeforeAttacking());
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update(); 
	}
	#endregion

	#endregion
}
