﻿using System.Collections;
using UnityEngine;

public class TDS_ExplosiveThrowable : TDS_Throwable 
{
    /* TDS_ExplosiveThrowable :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	[Throwable that explodes after a delay]
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
	 *	Date :			[20/05/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes :
	 *
	 *	[Initialisation of the class]
	 *  - Initialisation of the Overriden methods of the throwable class
     *  - Initalise the explosion settings
	 *	-----------------------------------
	*/

    #region Events

    #endregion

    #region Fields / Properties
    [Header("Explosion Settings")]
    [SerializeField, Range(1,60)] private float explodingDelay = 5;

    [SerializeField] private Animator animator = null;

    [SerializeField, Range(.5f, 1f)] private float animationSpeedMin = .5f;
    [SerializeField, Range(1,5)] private float animationSpeedMax = 2;

    private Coroutine explosionCoroutine = null; 
	#endregion

	#region Methods

	#region Original Methods
    /// <summary>
    /// Setup the explosion 
    /// Wait a delay method before playing the particles and activate the Hitbox
    /// Then wait and destroy the Throwable
    /// </summary>
    /// <returns></returns>
    private IEnumerator SetupExplosion(Tags _hitableTags)
    {
        float _timer = 0.1f;

        float _animationSpeed = animationSpeedMin; 

        while (_timer < explodingDelay)
        {
            _timer += Time.deltaTime; 
            _animationSpeed = Mathf.MoveTowards(_animationSpeed, animationSpeedMax, (Time.deltaTime * _timer));
            if (animator) animator.SetFloat("speedMultiplier", _animationSpeed); 
            yield return new WaitForEndOfFrame();
        }
        if (isHeld) owner.DropObject();
        TDS_VFXManager.Instance.SpawnEffect(FXType.Kaboom, transform.position);
        hitBox.Activate(attack, owner, _hitableTags.ObjectTags);

        if (TDS_VFXManager.Instance.KaboomFX)
            yield return new WaitForSeconds(TDS_VFXManager.Instance.KaboomFX.main.duration);
        else yield return new WaitForSeconds(1); 
        DestroyThrowableObject(); 
    }
    #endregion

    #region Overriden Methods
    /// <summary>
    /// Desactivate the hitbox and destroy the object
    /// </summary>
    protected override void DestroyThrowableObject()
    {
        hitBox.Desactivate();
        if (isHeld) owner.DropObject(); 
        base.DestroyThrowableObject();
    }

    /// <summary>
    /// The Throwable don't loose durability when it's exploding
    /// </summary>
    protected override void LoseDurability()
    {
        //No durability
    }

    /// <summary>
    /// Modify the Throwable Position as in <see cref="TDS_Throwable.Throw(Vector3, float, int)"/> 
    /// But don't Activate the hitbox
    /// </summary>
    /// <param name="_finalPosition"></param>
    /// <param name="_angle"></param>
    /// <param name="_bonusDamage"></param>
    public override void Throw(Vector3 _finalPosition, float _angle, int _bonusDamage)
    {
        if (!isHeld) return;
        if (hitBox.IsActive)
        {
            hitBox.Desactivate();
        }
        rigidbody.isKinematic = false;
        transform.SetParent(null, true);
        bonusDamage = _bonusDamage;
        rigidbody.velocity = TDS_ThrowUtility.GetProjectileVelocityAsVector3(transform.position, _finalPosition, _angle);
        Tags _hitableTags = owner.HitBox.HittableTags;
        if (owner is TDS_Enemy)
        {
            _hitableTags.AddTag("Enemy");
        }
        isHeld = false;
        if(explosionCoroutine == null)
        {
            explosionCoroutine = StartCoroutine(SetupExplosion(_hitableTags));
        }
    }
    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    protected override void Awake()
    {
        //base.Awake(); 
        if (!hitBox)
        {
            hitBox = GetComponentInChildren<TDS_HitBox>();
        }
    }

    protected override void OnCollisionEnter(Collision other)
    {
        return; 
    }
    #endregion

    #endregion
}
