﻿public class TDS_Punk : TDS_Minion 
{
    /* TDS_Punk :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Specific Behaviour of the Punk
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
     *  Date :          [22/05/2019]
     *	Author:         [THIEBAUT Alexis]
     *	
     *	[Refactoring of the class and its inheritances]
     *	    - Removing all methods that can be virtual in the TDS_EnemyClass
     *	    
     *	-----------------------------------
     *	
     *  Date :			[13/02/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes :
	 *
	 *	[Setting the Punk class as a inherited class from TDS_Enemy]
     *	    - Implementing the classes Behaviour and StartAttack
     *	    - Creating a array of enemy attack to stock all the punk attacks
     *	    - Creating a Method to get the best attack to cast for a given distance
	 *
	 *	-----------------------------------
     * 
	 *	Date :			[22/01/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes :
	 *
	 *	[Initialisation of the Punk Class]
     *	    
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties

    #endregion

    #region Methods

    #region Original Methods
    #endregion

    #region Overriden Methods
    /// <summary>
    /// Set its animation state to its taunt -> It will call the behaviour method
    /// </summary>
    public override void ActivateEnemy(bool _hasToTaunt = false)
    {
        if (_hasToTaunt)
            SetAnimationState((int)EnemyAnimationState.Taunt);
        else base.ActivateEnemy(_hasToTaunt); 
    }
    #endregion

    #region Unity Methods
    #endregion

    #endregion
}
