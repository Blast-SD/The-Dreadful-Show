﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDS_FatLady : TDS_Player 
{
	/* TDS_FatLady :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	[PURPOSE]
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

	#endregion

	#region Methods

	#region Original Methods

	#endregion

	#region Unity Methods
	// Awake is called when the script instance is being loaded
    protected override void Awake()
    {
        base.Awake();

        // Set player type, just in case
        PlayerType = PlayerType.FatLady;
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
	#endregion

	#endregion
}
