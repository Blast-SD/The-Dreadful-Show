﻿using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy_Attack", menuName = "Attacks/Enemy Attack", order = 2), Serializable]
public class TDS_EnemyAttack : TDS_Attack 
{
    /* TDS_EnemyAttack :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	- Inherited class from TDS_Attack 
     *	- Contains complementary informations for the enemies
     *	    - float predictedRange
     *	    - float probability
     *	    - int consecutiveUses
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
     *	
     *	Date :			[22/05/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes : - Adding the method Apply Effect
     *	          - Removing the IsDistance boolean
	 *
	 *	-----------------------------------
     *	
	 *	Date :			[06/02/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes : Adding a variable Attack Cooldown
	 *
	 *	-----------------------------------
     *	
	 *	Date :			[24/01/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes : Initialisation of the class
	 *
	 *	- Create all variables : isDistanceRange, predictedRange, probability and consecutive uses
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties
    #region int
    /// <summary>
    /// ID of the animation used to play this attack.
    /// </summary>
    public int AnimationID = 0;

    /// <summary>
    /// Backing field of <see cref="ConsecutiveUses"/>
    /// </summary>
    protected int consecutiveUses = 0;
    /// <summary>
    /// Number of consecutives uses of this attack
    /// Reset to zero when the enemy use another attack
    /// </summary>
    public int ConsecutiveUses
    {
        get
        {
            return consecutiveUses; 
        }
        set
        {
            if (value < 0)
                consecutiveUses = 0;
            else consecutiveUses = value; 
        }
    }
    #endregion

    #region float 
    /// <summary>
    /// Backing field <see cref="Cooldown"/>
    /// </summary>
    [SerializeField] protected float cooldown = 3;
    /// <summary>
    /// Cooldown of the attack.
    /// The enemy has to wait this time (in seconds) before attacking again
    /// </summary>
    public float Cooldown { get { return cooldown; } }

    /// <summary>
    /// Backing field <see cref="MaxRange"/>
    /// </summary>
    [SerializeField] protected float maxRange = 0;
    /// <summary>
    /// Predicted Range of the attack. 
    /// Used in debug and to check if the enemy is in range to cast this attack
    /// </summary>
    public float MaxRange
    {
        get
        {
            return maxRange; 
        }
        set
        {
            if (value < 0) maxRange = 0;
            else maxRange = value; 
        }
    }

    [SerializeField] protected float minRange = 0;
    public float MinRange { get { return minRange; } }

    /// <summary>
    /// Probability to cast this attack over 100.
    /// </summary>
    [SerializeField] protected float probability = 100;
    /// <summary>
    /// Probability to cast this attack divided by the number of consecutive uses +1.
    /// </summary>
    public float Probability
    {
        get
        {
            return probability / (consecutiveUses+1); 
        }
    }
    #endregion

    #region bool
    [SerializeField]protected bool canBreakGuard = false; 
    public bool CanBreakGuard
    {
        get { return canBreakGuard;  }
    }
    #endregion

    #endregion

    #region Methods 
    public virtual void ApplyAttackBehaviour(TDS_Enemy _caster)
    {
        // DO NOT IMPLEMENT BEHAVIOUR HERE
        // CREATE AN INHERITED CLASS TO IMPLEMENT A NEW BEHAVIOUR
    }

    /// <summary>
    /// Attacks the target, by inflicting damages and applying effects.
    /// </summary>
    /// <param name="_attacker">The HitBox attacking the target.</param>
    /// <param name="_target">Target to attack.</param>
    /// <returns>Returns -2 if target didn't take any damages, -1 if the target is dead, 0 if no effect could be applied, and 1 if everything went good.</returns>
    public override int Attack(TDS_HitBox _attacker, TDS_Damageable _target)
    {
        if (canBreakGuard && (_target is TDS_Player _player)) _player.StopParry();
        return base.Attack(_attacker, _target);
    }
    #endregion 
}
