﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

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

    #region Events
    /// <summary>
    /// Event called when triggering the mini game.
    /// </summary>
    public event Action OnTriggerMiniGame = null;
    #endregion

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

    /// <summary>Backing field for <see cref="IsInMiniGame"/>.</summary>
    [SerializeField] private bool isInMiniGame = false;

    /// <summary>
    /// Indicates if the Fire Eater is currently in the mini game.
    /// </summary>
    public bool IsInMiniGame
    {
        get { return isInMiniGame; }
        set
        {
            isInMiniGame = value;
            if (photonView.isMine) animator.SetBool("IsInMiniGame", value);
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

    /// <summary>
    /// Distance to move the Fire Eater on x before getting up on a drunken dodge.
    /// </summary>
    [SerializeField] private float xMovementAfterDrunkenDodge = 1.5f;

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

    /// <summary>
    /// Anchor used for the mini game sprites.
    /// </summary>
    [SerializeField] private Transform miniGameAnchor = null;
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
    /// Starts the coroutine to make the Fire Eater get drunk.
    /// </summary>
    public void GetDrunk() => StartCoroutine(GetDrunkCotourine());

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
    public void MoveAfterDrunkenDodge()
    {
        MoveTo(transform.position + (Vector3.right * isFacingRight.ToSign() * xMovementAfterDrunkenDodge));
    }
    #endregion

    #region MiniGame
    /// <summary>
    /// Make the mini game fun !
    /// </summary>
    /// <returns></returns>
    private IEnumerator MiniGame()
    {
        OnTriggerMiniGame = () => SetFireEaterAnim(FireEaterAnimState.ReadyForSpit);

        animator.SetFloat("MiniGameSpeed", Random.Range(.35f, 1f));

        while (isInMiniGame)
        {
            yield return null;

            if (Input.GetButtonDown(LightAttackButton) || Input.GetButtonDown(HeavyAttackButton))
            {
                OnTriggerMiniGame?.Invoke();
                SetFireEaterAnim(FireEaterAnimState.Fire);
                break;
            }   
        }
    }

    /// <summary>
    /// Set the Fire Eater mini game state.
    /// </summary>
    /// <param name="_state"></param>
    public void SetMiniGameState(int _state)
    {
        if (!photonView.isMine) return;

        switch (_state)
        {
            case 0:
                StartCoroutine(MiniGame());
                break;

            case 1:
                OnTriggerMiniGame = null;
                break;

            case 2:
                OnTriggerMiniGame = () => SetFireEaterAnim(FireEaterAnimState.ReadyForDrunk);
                OnTriggerMiniGame += GetDrunk;
                break;

            default:
                TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.All, TDS_RPCManager.GetInfo(photonView, GetType(), "ExitMiniGame"), new object[] { });

                break;
        }
    }

    /// <summary>
    /// Exit the mini game.
    /// </summary>
    public void ExitMiniGame()
    {
        IsInMiniGame = false;
        animator.SetInteger("FireID", -999999);
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
        if (isDrunk && (!isGrounded || isJumping)) return;
        base.StartDodge();
    }
    #endregion

    #region Attacks
    /// <summary>
    /// Ends definitively the current attack and enables back the capacity to attack once more.
    /// </summary>
    protected override void EndAttack()
    {
        base.EndAttack();

        if (isDrunk)
        {
            ComboCurrent = new List<bool>();
            CancelInvoke("ResetCombo");
        }
    }

    /// <summary>
    /// Make the Fire Eater puke.
    /// </summary>
    public void Puke()
    {
        IsAttacking = true;
        animator.SetTrigger("Puke");
    }

    /// <summary>
    /// Spit a fire ball in front of the Fire Eater.
    /// </summary>
    /// <param name="_isUltra">Set 0 for small fire ball, anything else for ultra.
    /// 'Just cause animation don't take a boolean... What a waste.</param>
    public void SpitFireBall(int _isUltra)
    {
        GameObject _fireBall = PhotonNetwork.Instantiate("FireBall", transform.position + (transform.right * .03f) + (Vector3.up * 1.35f), transform.rotation, 0);

        _fireBall.GetComponentInChildren<TDS_HitBox>().Activate(attacks[_isUltra == 0 ? 12 : 13], this);
        if (_isUltra != 0) _fireBall.transform.localScale *= 1.15f;
    }

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
    /// Flips this character to have they looking at the opposite side.
    /// </summary>
    public override void Flip()
    {
        base.Flip();

        miniGameAnchor.Rotate(Vector3.up, 180);
    }

    /// <summary>
    /// Starts a brand new jump !
    /// </summary>
    public override void StartJump()
    {
        if (isDrunk)
        {
            if (isDodging) return;

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
        if (photonView.isMine)
        {
            TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, GetType(), "SetFireEaterAnim"), new object[] { (int)_state });
        }

        switch (_state)
        {
            case FireEaterAnimState.Sober:
                animator.SetBool("IsDrunk", false);
                break;

            case FireEaterAnimState.Drunk:
                animator.SetBool("IsDrunk", true);
                break;

            case FireEaterAnimState.ReadyForSpit:
                animator.SetInteger("FireID", 0);
                break;

            case FireEaterAnimState.ReadyForDrunk:
                animator.SetInteger("FireID", 999999);
                break;

            case FireEaterAnimState.Fire:
                animator.SetTrigger("Fire");
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Set an animation state of the Fire Eater, used in the animator.
    /// </summary>
    /// <param name="_state">State to set in animation.</param>
    public void SetFireEaterAnim(int _state)
    {
        SetFireEaterAnim((FireEaterAnimState)_state);
    }
    #endregion

    #region Interactions
    /// <summary>
    /// Interacts with the nearest available object in range.
    /// </summary>
    /// <returns>Returns true if interacted with something. False if nothing was found.</returns>
    public override bool Interact()
    {
        if (isDrunk)
        {
            Puke();
            return false;
        }

        return base.Interact();
    }
    #endregion

    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    protected override void Awake()
    {
        base.Awake();

        // Set player type, just in case
        PlayerType = PlayerType.FireEater;
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        // Freeze the Fire Eater when drunk & touching the ground while not dodging
        OnGetOnGround += () => { if (isDrunk && !isDodging) FreezePlayer(); };
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
    #endregion

    #endregion
}
