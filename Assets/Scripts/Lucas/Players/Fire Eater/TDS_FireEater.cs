﻿using System.Collections;
using UnityEngine;

public class TDS_FireEater : TDS_Player 
{
    /* TDS_FireEater :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Gameplay class manipulating the Fire Eater player.
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[21 / 05 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_FireEater class.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties

    #region Variables
    /// <summary>Backing field for <see cref="IsDrunk"/>.</summary>
    [SerializeField] private bool isDrunk = false;

    /// <summary>
    /// Indicates if the Fire Eater is currently drunk.
    /// </summary>
    public bool IsDrunk
    {
        get { return isDrunk; }
        set
        {
            if (value) SetFireEaterAnim(FireEaterAnimState.Drunk);
            else SetFireEaterAnim(FireEaterAnimState.Sober);

            isDrunk = value;
        }
    }

    /// <summary>Backing field for <see cref="DrunkSpeedCoef"/>.</summary>
    [SerializeField] private float drunkSpeedCoef = .8f;

    /// <summary>
    /// Coefficient applied to speed when drunk.
    /// </summary>
    public float DrunkSpeedCoef
    {
        get { return drunkSpeedCoef; }
        set
        {
            if (value < 0) value = 0;
            drunkSpeedCoef = value;

            if (isDrunk) speedCoef = value;
        }
    }

    /// <summary>Backing field for <see cref="SoberUpTime"/>.</summary>
    [SerializeField] private float soberUpTime = 10;

    /// <summary>
    /// Time it takes to the Fire Eater to sober up.
    /// </summary>
    public float SoberUpTime
    {
        get { return soberUpTime; }
        set
        {
            if (value < 0) value = 0;
            soberUpTime = value;
        }
    }

    /// <summary>Backing field for <see cref="DrunkJumpForce"/>.</summary>
    [SerializeField] private int drunkJumpForce = 200;

    /// <summary>
    /// Force to apply when performing a jump when drunk.
    /// </summary>
    public int DrunkJumpForce
    {
        get { return drunkJumpForce; }
        set
        {
            if (value < 0) value = 0;
            drunkJumpForce = value;
        }
    }
    #endregion

    #region Memory
    /// <summary>
    /// Timer used to make the Fire Eater sober up, in the GetDrunkCoroutine method.
    /// </summary>
    [SerializeField] private float soberUpTimer = 0;
    #endregion

    #endregion

    #region Methods

    #region Original Methods

    #region Drunk
    /// <summary>
    /// Method to call when crashing on ground to prevent the character from moving.
    /// </summary>
    public void CrashOnGround() => IsParalyzed = true;

    /// <summary>
    /// Starts the coroutine to make the Fire Eater get drunk.
    /// </summary>
    public void GetDrunk() => StartCoroutine(GetDrunkCotourine());

    /// <summary>
    /// Call this when the character is getting up from crashing on ground.
    /// </summary>
    public void GetUpFromCrash() => IsParalyzed = false;

    /// <summary>
    /// Makes the Fire Eater get drunk, and sober him up after a certain time.
    /// </summary>
    /// <returns></returns>
    private IEnumerator GetDrunkCotourine()
    {
        IsDrunk = true;
        speedCoef = drunkSpeedCoef;

        soberUpTimer = soberUpTime;

        while (soberUpTimer > 0)
        {
            yield return null;
            soberUpTimer -= Time.deltaTime;
        }

        speedCoef = 1;
        IsDrunk = false;
    }

    /// <summary>
    /// Call this before starting to get up from ground after a drunken dodge to correctly set the character position.
    /// </summary>
    /// <param name="_xMovement">Movement on the X axis to teleport the character from.</param>
    public void TeleportAfterDrunkenDodge(float _xMovement)
    {
        transform.position += Vector3.right * isFacingRight.ToSign() * _xMovement;
    }
    #endregion

    #region Actions
    /// <summary>
    /// Performs the catch attack of this player.
    /// </summary>
    public override void Catch()
    {
        if (isDrunk) return;

        base.Catch();
    }

    /// <summary>
    /// Make the player dodge.
    /// </summary>
    public override void StartDodge()
    {
        if (isDrunk && !isGrounded) return;

        base.StartDodge();
    }
    #endregion

    #region Attacks
    /// <summary>
    /// Performs the Super attack if the gauge is filled enough.
    /// </summary>
    public override void SuperAttack()
    {
        if (isDrunk) return;

        base.SuperAttack();
    }
    #endregion

    #region Movements
    /// <summary>
    /// Starts a brand new jump !
    /// </summary>
    public override void StartJump()
    {
        if (isDrunk)
        {
            // Adds a different force when drunk
            rigidbody.AddForce(Vector3.up * drunkJumpForce);
            return;
        }

        base.StartJump();
    }
    #endregion

    #region Animations
    /// <summary>
    /// Set an animation state of the Fire Eater, used in the animator.
    /// </summary>
    /// <param name="_state">State to set in animation.</param>
    public void SetFireEaterAnim(FireEaterAnimState _state)
    {
        switch (_state)
        {
            case FireEaterAnimState.Sober:
                animator.SetBool("IsDrunk", false);
                break;

            case FireEaterAnimState.Drunk:
                animator.SetBool("IsDrunk", true);
                break;

            default:
                break;
        }
    }
    #endregion

    #region Interactions
    /// <summary>
    /// Interacts with the nearest available object in range.
    /// </summary>
    /// <returns>Returns true if interacted with something. False if nothing was found.</returns>
    public override bool Interact()
    {
        if (isDrunk) return false;

        return base.Interact();
    }
    #endregion

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

        // Set player type, just in case
        PlayerType = PlayerType.FireEater;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
    #endregion

    #endregion
}
