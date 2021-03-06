﻿using UnityEngine;

public class TDS_LevelBounds : MonoBehaviour 
{
    /* TDS_LevelBounds :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Activate bounds for the player & the camera when entering trigger,
     *	and set this as current bounds in the Level Manager ;
     *	desactivate them with a public method.
	 *
	 *	#####################
	 *	####### TO DO #######
	 *	#####################
	 *
	 *	Hum...
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[01 / 04 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_LevelBounds class.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties
    /// <summary>
    /// Should this object be destroyed after being activated ?
    /// </summary>
    [SerializeField] private bool doDestroyOnActivate = true;

    /// <summary>
    /// Should this object be desactivated after being activated ?
    /// </summary>
    [SerializeField] private bool doDisableOnActivate = false;


    /// <summary>
    /// Bottom bound position.
    /// </summary>
    [SerializeField] private Vector3 bottomBound = Vector3.zero;

    /// <summary>Public accessor for <see cref="bottomBound"/>.</summary>
    public Vector3 BottomBound { get { return bottomBound; } }

    /// <summary>
    /// Left bound position.
    /// </summary>
    [SerializeField] private Vector3 leftBound = Vector3.zero;

    /// <summary>Public accessor for <see cref="leftBound"/>.</summary>
    public Vector3 LeftBound { get { return leftBound; } }

    /// <summary>
    /// Right bound position.
    /// </summary>
    [SerializeField] private Vector3 rightBound = Vector3.zero;

    /// <summary>Public accessor for <see cref="rightBound"/>.</summary>
    public Vector3 RightBound { get { return rightBound; } }

    /// <summary>
    /// Top bound position.
    /// </summary>
    [SerializeField] private Vector3 topBound = Vector3.zero;

    /// <summary>Public accessor for <see cref="topBound"/>.</summary>
    public Vector3 TopBound { get { return topBound; } }


    /// <summary>
    /// Tags detected by the trigger to enable.
    /// </summary>
    [SerializeField] private Tags detectedTags = new Tags(new Tag[] { new Tag("Player") });

    /// <summary>
    /// Activation mode used to trigger these bounds.
    /// </summary>
    [SerializeField] private TriggerActivationMode activationMode = TriggerActivationMode.Enter;
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Activate these bounds.
    /// </summary>
    public void Activate()
    {
        TDS_Camera.Instance.SetBounds(this);
        if (doDestroyOnActivate) Destroy(this);
        else if (doDisableOnActivate) enabled = false;
    }
    #endregion

    #region Unity Methods
    // OnTriggerEnter is called when the GameObject collides with another GameObject
    private void OnTriggerEnter(Collider other)
    {
        if ((activationMode == TriggerActivationMode.Enter) && (other.gameObject.HasTag(detectedTags.ObjectTags))) Activate();
    }

    // OnTriggerExit is called when the Collider other has stopped touching the trigger
    private void OnTriggerExit(Collider other)
    {
        if ((activationMode == TriggerActivationMode.Exit) && (other.gameObject.HasTag(detectedTags.ObjectTags))) Activate();
    }
    #endregion

    #endregion
}
