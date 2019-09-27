﻿using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable object used to save and access to all tags of a project
/// for the multi-tags system.
/// </summary>
public class TagsSO : ScriptableObject 
{
    /* TagsSO :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	    Scriptable object used to store all tags of a project, allowing the save & load them dynamically.
	 *
     *	#####################
	 *	####### TO DO #######
	 *	#####################
     *  
     *      Nothing to see here...
     * 
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
     *	Date :			[06 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
     *	    
     *	    • Object now get project tags missing from it and adds its own that
     *	the project doesn't have regularly thanks to custom update in editor.
	 *
	 *	-----------------------------------
     * 
     *	Date :			[03 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
     *	    
     *	    • Now, the tags are fully loaded & set when this scriptable object is being loaded. That's really sweet.
     *	    You can copy this scriptable object from project to project, and all tags will be fully loaded. That's cool.
     *	    Oh, and the tags of the project not yet on this object are automatically added on load. Awesome, isn't it ?
	 *
	 *	-----------------------------------
     * 
	 *	Date :			[21 / 01 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    Creation of the TagsScriptableObject class.
     *	    
     *	    • Base content, with only a list of Tag objects.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties
    /// <summary>
    /// All this project tags.
    /// </summary>
    public Tag[]                    AllTags
    {
        get
        {
            Tag[] _allTags = new Tag[UnityBuiltInTags.ObjectTags.Length + CustomTags.ObjectTags.Length];

            for (int _i = 0; _i < UnityBuiltInTags.ObjectTags.Length; _i++)
            {
                _allTags[_i] = UnityBuiltInTags.ObjectTags[_i];
            }
            for (int _i = 0; _i < CustomTags.ObjectTags.Length; _i++)
            {
                _allTags[_i + UnityBuiltInTags.ObjectTags.Length] = CustomTags.ObjectTags[_i];
            }

            return _allTags;
        }
    }

    /// <summary>
    /// All custom tags of this project.
    /// </summary>
    public Tags                     CustomTags              = new Tags();

    /// <summary>
    /// All Unity built-in tags of this project.
    /// </summary>
    public Tags                     UnityBuiltInTags        = new Tags();


    /// <summary>
    /// Dictionary referencing all scene objects tags with their ID as key and tags as value.
    /// </summary>
    private Dictionary<int, Tags>   objectsTags             = new Dictionary<int, Tags>();
    #endregion

    #region Methods

    #region Original Methods

    #region Unity Editor
    #if UNITY_EDITOR
    /// <summary>
    /// Connect the project tags with this object.
    /// </summary>
    private void Connect()
    {
        // Initializes the project with this object
        UnityEditor.Editor _thisEditor = UnityEditor.Editor.CreateEditor(this);

        if (!_thisEditor) return;

        _thisEditor.GetType().InvokeMember("ConnectProjectTags", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.NonPublic, null, _thisEditor, null);

        // Destroy the created editor
        DestroyImmediate(_thisEditor);
    }
#endif
    #endregion

    #region Runtime
    /// <summary>
    /// Adds a new tag to a GameObject.
    /// </summary>
    /// <param name="_object">GameObject to add tag to.</param>
    /// <param name="_newTag">New Tag to add.</param>
    public void AddTagToObject(GameObject _object, string _newTag)
    {
        if (!TDS_LevelManager.Instance)
        {
            Debug.LogError("There is no LevelManager in the scene (yet) to add tag !");
            return;
        }

        RegisterObject(_object);

        TDS_LevelManager.Instance.ObjectsTags[_object.GetInstanceID()].AddTag(_newTag);
    }

    /// <summary>
    /// Adds a new tag to a GameObject.
    /// </summary>
    /// <param name="_object">GameObject to add tag to.</param>
    /// <param name="_newTag">New Tag to add.</param>
    public void AddTagToObject(GameObject _object, Tag _newTag)
    {
        if (!TDS_LevelManager.Instance)
        {
            Debug.LogError("There is no LevelManager in the scene (yet) to add tag !");
            return;
        }

        RegisterObject(_object);

        TDS_LevelManager.Instance.ObjectsTags[_object.GetInstanceID()].AddTag(_newTag);
    }


    /// <summary>
    /// Get an object tags.
    /// </summary>
    /// <param name="_object">Object to get tags from.</param>
    /// <returns>Returns the tags of the object.</returns>
    public Tag[] GetObjectTags(GameObject _object)
    {
        if (!TDS_LevelManager.Instance)
        {
            return MultiTags.GetTags(_object.tag.Split(MultiTags.TAG_SEPARATOR));
        }

        RegisterObject(_object);

        return TDS_LevelManager.Instance.ObjectsTags[_object.GetInstanceID()].ObjectTags;
    }


    /// <summary>
    /// Registers a GameObject into the field <see cref="objectsTags"/>.
    /// </summary>
    /// <param name="_object">GameObject to register.</param>
    private void RegisterObject(GameObject _object)
    {
        int _id = _object.GetInstanceID();

        if (!TDS_LevelManager.Instance.ObjectsTags.ContainsKey(_id))
        {
            TDS_LevelManager.Instance.ObjectsTags.Add(_id, new Tags(MultiTags.GetTags(_object.tag.Split(MultiTags.TAG_SEPARATOR))));
        }
    }


    /// <summary>
    /// Removes a new tag from a GameObject.
    /// </summary>
    /// <param name="_object">GameObject to remove tag from.</param>
    /// <param name="_tag">Tag to remove.</param>
    public void RemoveTagFromObject(GameObject _object, string _tag)
    {
        if (!TDS_LevelManager.Instance)
        {
            Debug.LogError("There is no LevelManager in the scene (yet) to remove tag !");
            return;
        }

        RegisterObject(_object);

        TDS_LevelManager.Instance.ObjectsTags[_object.GetInstanceID()].RemoveTag(_tag);
    }

    /// <summary>
    /// Removes a new tag from a GameObject.
    /// </summary>
    /// <param name="_object">GameObject to remove tag from.</param>
    /// <param name="_tag">Tag to remove.</param>
    public void RemoveTagFromObject(GameObject _object, Tag _tag)
    {
        if (!TDS_LevelManager.Instance)
        {
            Debug.LogError("There is no LevelManager in the scene (yet) to add tag !");
            return;
        }

        RegisterObject(_object);

        TDS_LevelManager.Instance.ObjectsTags[_object.GetInstanceID()].RemoveTag(_tag);
    }
    #endregion

    #endregion

    #region Unity Methods
    // This function is called when the scriptable object will be destroyed
    private void OnDestroy()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.quitting -= Connect;
        #endif
    }

    // This function is called when the object is loaded
    private void OnEnable()
    {
        #if UNITY_EDITOR
        // In editor, connect the project with this asset when enabled
        if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Connect();
        }

        // Connect on editor quit
        UnityEditor.EditorApplication.quitting -= Connect;
        UnityEditor.EditorApplication.quitting += Connect;
        #endif
    }
    #endregion

    #endregion
}
