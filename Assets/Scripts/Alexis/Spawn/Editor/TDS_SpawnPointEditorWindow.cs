﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UtilsLibrary.GUILibrary; 



public class TDS_SpawnPointEditorWindow : EditorWindow
{

    /* TDS_SpawnPointEditorWindow :
     *
     *	#####################
     *	###### PURPOSE ######
     *	#####################
     *
     *	Display editing settings for a selected Spawn Point
     *
     *	#####################
     *	### MODIFICATIONS ###
     *	#####################
     *
     *	Date :			[12/02/2019]
     *	Author :		[THIEBAUT Alexis]
     *
     *	Changes :
     *
     *	[Initialisation de la class TDS_SpawnPointEditorWindow]
     *	    - Création des variable property, point and scroll view
     *	    - Création de la méthode Init
     *	    - Implémentation de la méthode OnGUI pour afficher les settings
     *
     *	-----------------------------------
    */

    /// <summary>SerializedProperty for <see cref="TDS_SpawnerArea.spawnPoints[i]"/> of type <see cref="TDS_SpawnPoint"/>.</summary>
    private SerializedProperty property;

    /// <summary>Edited Spawn Point.</summary>
    private TDS_SpawnPoint point;

    /// <summary>Scroll view position of the window.</summary>
    private Vector2 scrollView = Vector2.zero;

    /// <summary>
    /// Init the window settings
    /// set the property and the point of the window
    /// </summary>
    /// <param name="_property">property</param>
    /// <param name="_point">spawn point</param>
    public void Init(SerializedProperty _property, TDS_SpawnPoint _point)
    {
        property = _property;
        point = _point; 
    }
    
    /// <summary>
    /// Called to display settings
    /// </summary>
    private void OnGUI()
    {
        // Close if the property or the serialized Object is null
        if (property == null || property.serializedObject == null)
        {
            Close();
            return;
        }
        //Begin the scroll viex
        scrollView = EditorGUILayout.BeginScrollView(scrollView, true, true);
        //Display properties and settings of the spawn point
        EditorGUILayout.PropertyField(property);
        // Aplly changes on the SerializedObject
        property.serializedObject.ApplyModifiedProperties(); 
        GUILayout.Space(5);
        // Create two ObjectFields to add normal and random spawning informations
        TDS_Enemy _e = null;
        _e = EditorGUILayout.ObjectField("Add Enemy", _e, typeof(TDS_Enemy), false) as TDS_Enemy;
        if (_e != null && !point.ExistsEnemy(_e))
        {
            point.AddSpawningInformations(_e, false);
        }
        _e = null;
        _e = EditorGUILayout.ObjectField("Add Random Enemy", _e, typeof(TDS_Enemy), false) as TDS_Enemy;
        if (_e != null && !point.ExistsRandomEnemy(_e))
        {
            point.AddSpawningInformations(_e, true);
        }
        EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// When the selection change, close the window
    /// </summary>
    private void OnSelectionChange()
    {
        Close(); 
    }
}