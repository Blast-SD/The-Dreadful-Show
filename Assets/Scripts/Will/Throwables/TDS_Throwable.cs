﻿using UnityEngine;
using Photon;

#pragma warning disable 0414
[RequireComponent(typeof(Rigidbody))]
public class TDS_Throwable : PunBehaviour
{
    /* TDS_Throwable :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	[Throwable object behavior]
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :	12/02/2019
	 *	Author :William COMMINGES
	 *
	 *	Changes :
	 *
	 *	[CHANGES]
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties
    #region ObjectSettings
    [SerializeField, Header("Object settings")]
    protected bool isHeld = false;
    [SerializeField]
    protected new BoxCollider collider = null;
    [SerializeField]
    protected float bouncePower = .5f;
    [SerializeField] private GameObject shadow = null;
    [SerializeField, Range(0, 20)]
    protected int bonusDamage = 0;
    [SerializeField, Range(0, 10)]
    protected int objectDurability = 10;
    public int ObjectDurability
    {
        get { return objectDurability;  }
        set
        {
            if(value > 0)
            {
                objectDurability = value; 
            }
        }
    }
    [SerializeField]
    protected int weight = 2;
    public int Weight { get { return weight; } }
    [SerializeField]
    protected new Rigidbody rigidbody = null;
    #endregion
    #region PlayerSettings
    [SerializeField, Header("Character settings")]
    protected TDS_Character owner = null;
    #endregion
    #region Hitbox
    [SerializeField]
    protected TDS_HitBox hitBox;
    public TDS_HitBox HitBox { get { return hitBox; } }
    [SerializeField]
    protected TDS_Attack attack = null; 
    public AttackEffectType ThrowableAttackType
    {
        get
        {
            if (!attack) return AttackEffectType.None;
            return attack.Effect.EffectType; 
        }
    }
    #endregion
    #endregion

    #region Methods
    #region Original Methods
    /// <summary>
    /// bounce object when it touches a collider
    /// </summary>
    protected void BounceObject()
    {
        rigidbody.velocity *= bouncePower*-1;
    }
    /// <summary>
    /// Destroy the gameObject Throwable if the durability is less or equal to zero 
    /// </summary>
    protected virtual void DestroyThrowableObject()
    {
        if (!PhotonNetwork.isMasterClient) return;
        //PhotonNetwork.Instantiate("Poof", hitBox.Collider.bounds.center, Quaternion.identity, 0);
        TDS_VFXManager.Instance.SpawnEffect(FXType.MagicDisappear, transform.position);
        PhotonNetwork.Destroy(gameObject);
    }
    /// <summary>
    /// Unparent the object from the character who was carring it. 
    /// </summary>
    public void Drop()
    {
        if (!isHeld) return;
        rigidbody.isKinematic = false;
        transform.SetParent(null, true);
        isHeld = false;
        collider.enabled = true;
        SetLayer(LayerMask.NameToLayer("Object"));
    }
    /// <summary> 
    /// Reduces the durability of the object and if the durability is lower or equal to zero called the method that destroys the object. 
    /// </summary> 
    /// <param name="_valueToWithdraw"></param> 
    protected virtual void LoseDurability()
    {
        if (!PhotonNetwork.isMasterClient) return;  
        objectDurability --;
        if (!(objectDurability <= 0)) return;
        DestroyThrowableObject();
    }

    /// <summary> 
    /// Picks up the object and parent it at the corresponding root 
    /// </summary> 
    /// <param name="_carrier"></param> 
    /// <param name="_rootCharacterObject"></param> 
    /// <returns></returns> 
    public bool PickUp(TDS_Character _carrier, Transform _rootCharacterObject)
    {
        if (isHeld) return false;
        if (hitBox.IsActive)
        {
            hitBox.Desactivate();
        }
        if (shadow && !shadow.activeInHierarchy) shadow.SetActive(true);
        rigidbody.isKinematic = true;
        transform.position = _rootCharacterObject.transform.position;
        transform.SetParent(_rootCharacterObject.transform, true);
        transform.rotation = Quaternion.identity;
        isHeld = true;
        owner = _carrier;
        collider.enabled = false;
        SetLayer(_carrier.gameObject.layer);

        return true;
    }

    /// <summary>
    /// Set this object layer.
    /// </summary>
    /// <param name="_layerID">ID of the new object layer.</param>
    protected void SetLayer(int _layerID)
    {
        if (PhotonNetwork.isMasterClient)
        {
            TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, this.GetType(), "SetLayer"), new object[] { _layerID });
        }

        gameObject.layer = _layerID;
    }

    /// <summary> 
    /// Throws the object to a given position by converting the final position to velocity 
    /// </summary> 
    /// <param name="_finalPosition"></param> 
    /// <param name="_angle"></param> 
    /// <param name="_bonusDamage"></param> 
    public virtual void Throw(Vector3 _finalPosition,float _angle, int _bonusDamage)
    {
        if (!isHeld) return;
        if(hitBox.IsActive)
        {
            hitBox.Desactivate();
        }
        rigidbody.isKinematic = false;
        transform.SetParent(null, true);
        bonusDamage = _bonusDamage;
        rigidbody.velocity = TDS_ThrowUtility.GetProjectileVelocityAsVector3(transform.position,_finalPosition,_angle);
        Tags _hitableTags = owner.HitBox.HittableTags; 
        if (owner is TDS_Enemy)
        {
            _hitableTags.AddTag("Enemy");
        }
        hitBox.Activate(attack, owner, _hitableTags.ObjectTags);
        isHeld = false;
        collider.enabled = true;
        SetLayer(LayerMask.NameToLayer("Object"));
    }
    #endregion

    #region Unity Methods
    protected virtual void Awake()
    {
        if (!rigidbody) rigidbody = GetComponentInChildren<Rigidbody>();
        if (!collider) collider = GetComponentInChildren<BoxCollider>();
        if (!hitBox)
        {
            hitBox = GetComponentInChildren<TDS_HitBox>();
        }
        if (!shadow) shadow = transform.GetChild(1).gameObject;
        hitBox.OnTouch += hitBox.Desactivate;
        hitBox.OnTouch += BounceObject;
        hitBox.OnTouch += LoseDurability;
        hitBox.OnTouch += () => owner = null;
    }

    protected virtual void OnCollisionEnter(Collision other)
    {
        if (hitBox.IsActive)
        {
            hitBox.Desactivate();
            LoseDurability();
            owner = null;
        }
    }

    protected virtual void Start ()
    {
        if(!PhotonNetwork.isMasterClient)
        {
            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;
        }
    }

    public override void OnJoinedRoom()
    {
        if(PhotonNetwork.isMasterClient)
        {
            rigidbody.useGravity = true; 
        }
    }

    #endregion
    #endregion
}