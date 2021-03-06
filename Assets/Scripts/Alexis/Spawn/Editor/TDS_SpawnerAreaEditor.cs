﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; 
using UnityEditor;
using UtilsLibrary.GUILibrary; 

[CustomEditor(typeof(TDS_SpawnerArea))]
public class TDS_SpawnerAreaEditor : Editor 
{
    /* TDS_SpawnerAreaEditor :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	[Editor class of the TDS_SpawnerArea class]
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *	 
	 *	Date :			[03/04/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes :
	 *
	 *	[Adding Editor for the Unity Events]
	 *
	 *	-----------------------------------
     *	
	 *	Date :			[11/02/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes :
	 *
	 *	[Initialisation of the TDS_SpawnerArea Editor class]
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties
    #region FoldOut
    /// <summary>Backing field for <see cref="AreSpawnerAreaComponentsUnfolded"/></summary>
    private bool areSpawnerAreaComponentsUnfolded = false;
    /// <summary>
    /// Are the components of the Character class unfolded for editor ?
    /// </summary>
    public bool AreSpawnerAreaComponentsUnfolded
    {
        get { return areSpawnerAreaComponentsUnfolded; }
        set
        {
            areSpawnerAreaComponentsUnfolded = value;

            // Saves this value
            EditorPrefs.SetBool("areSpawnerAreaComponentsUnfolded", value);
        }
    }

    /// <summary>Backing field for <see cref="AreSpawnerAreaSettingsUnfolded"/></summary>
    private bool areSpawnerAreaSettingsUnfolded = false;
    /// <summary>
    /// Are the components of the Character class unfolded for editor ?
    /// </summary>
    public bool AreSpawnerAreaSettingsUnfolded
    {
        get { return areSpawnerAreaSettingsUnfolded; }
        set
        {
            areSpawnerAreaSettingsUnfolded = value;

            // Saves this value
            EditorPrefs.SetBool("areSpawnerAreaSettingsUnfolded", value);
        }
    }

    /// <summary>Backing field for <see cref="AreSpawerAreaEventsUnfolded"/></summary>
    private bool areSpawerAreaEventsUnfolded = false;
    /// <summary>
    /// Are the components of the Character class unfolded for editor ?
    /// </summary>
    public bool AreSpawerAreaEventsUnfolded
    {
        get { return areSpawerAreaEventsUnfolded; }
        set
        {
            areSpawerAreaEventsUnfolded = value;

            // Saves this value
            EditorPrefs.SetBool("areSpawerAreaEventsUnfolded", value);
        }
    }



    private bool areWavesUnfolded = false; 

    public bool AreWavesUnfolded
    {
        get { return areWavesUnfolded; }
        set
        {
            areWavesUnfolded = value;

            // Saves this value
            EditorPrefs.SetBool("areWavesUnfolded", value);
        }
    }
    #endregion

    #region Variables
    /// <summary>SerializedProperty for <see cref="TDS_SpawnerArea.isReady"/> of type <see cref="bool"/>.</summary>
    private SerializedProperty isReady = null;
    /// <summary>SerializedProperty for <see cref="TDS_SpawnerArea.isActivated"/> of type <see cref="bool"/>.</summary>
    private SerializedProperty isActivated = null;

    /// <summary>SerializedProperty for <see cref="TDS_SpawnerArea.isLooping"/> of type <see cref="bool"/>.</summary>
    private SerializedProperty isLooping = null;
    /// <summary>SerializedProperty for <see cref="TDS_SpawnerArea.isActivatedByEvent"/> of type <see cref="bool"/>.</summary>
    private SerializedProperty isActivatedByEvent = null;

    /// <summary>SerializedProperty for <see cref="TDS_SpawnerArea.loopInterval"/> of type <see cref="int"/>.</summary>
    private SerializedProperty loopInterval = null;

    /// <summary>SerializedProperty for <see cref="TDS_SpawnerArea.waves"/> of type <see cref="List{TDS_Wave}"/>.</summary>
    private SerializedProperty waves = null;

    /// <summary>SerializedProperty for <see cref="TDS_SpawnerArea.spawnedEnemies"/> of type <see cref="List{TDS_Enemy}"/>.</summary>
    private SerializedProperty spawnedEnemies = null;
    /// <summary>SerializedProperty for <see cref="TDS_SpawnerArea.areaThrowables"/> of type <see cref="List{TDS_Throwable}"/>.</summary>
    private SerializedProperty areaThrowables = null;

    /// <summary>SerializedProperty for <see cref="TDS_SpawnerArea.OnAreaActivated"/> of type <see cref="UnityEvent"/>.</summary>
    private SerializedProperty eventOnAreaActivated = null;
    /// <summary>SerializedProperty for <see cref="TDS_SpawnerArea.OnAreaDesactivated"/> of type <see cref="UnityEvent"/>.</summary>
    private SerializedProperty eventOnAreaDesactivated = null;
    /// <summary>SerializedProperty for <see cref="TDS_SpawnerArea.OnNextWave"/> of type <see cref="UnityEvent"/>.</summary>
    private SerializedProperty eventOnNextWave = null;
    /// <summary>SerializedProperty for <see cref="TDS_SpawnerArea.OnStartFight"/> of type <see cref="UnityEvent"/>.</summary>
    private SerializedProperty eventOnStartFight = null;
    #endregion

    #endregion

    #region Methods

    #region Original Methods
    private void DrawEvents()
    {
        Color _originalColor = GUI.backgroundColor;
        GUI.backgroundColor = new Color(.8f,.8f,.8f); 

        TDS_EditorUtility.PropertyField("On Spawn Area Activated", "Called when a player activate the trigger", eventOnAreaActivated);
        TDS_EditorUtility.PropertyField("On Fight Start", "Called when the fight is starting", eventOnStartFight);
        TDS_EditorUtility.PropertyField("On Spawn Area Desactivated", "Called when all waves are completed", eventOnAreaDesactivated);
        TDS_EditorUtility.PropertyField("On Next Wave", "Called when a wave is completed", eventOnNextWave);

        GUI.backgroundColor = _originalColor;
    }

    /// <summary>
    /// Sraw spawner Area Editor
    /// </summary>
    void DrawSpawnerAreaEditor()
    {
        // Make a space at the beginning of the editor
        GUILayout.Space(10);
        Color _originalColor = GUI.backgroundColor;

        GUI.backgroundColor = TDS_EditorUtility.BoxDarkColor;

        EditorGUILayout.BeginVertical("HelpBox");
        
        TDS_EditorUtility.RadioToggle("Is Ready", "", isReady);
        TDS_EditorUtility.RadioToggle("Is Activated", "", isActivated);

        EditorGUILayout.EndVertical();

        GUILayout.Space(10);

        EditorGUILayout.BeginVertical("HelpBox");

        // Button to show or not the Character class settings
        if (TDS_EditorUtility.Button("Settings", "Wrap / unwrap settings", TDS_EditorUtility.HeaderStyle)) AreSpawnerAreaSettingsUnfolded = !areSpawnerAreaSettingsUnfolded;

        // If unfolded, draws the custom editor for the sttings
        if (areSpawnerAreaSettingsUnfolded)
        {
            DrawSettings();
        }

        EditorGUILayout.EndVertical();
        GUILayout.Space(15);


        GUI.backgroundColor = TDS_EditorUtility.BoxDarkColor;
        EditorGUILayout.BeginVertical("HelpBox");

        // Button to show or not the Character class components
        if (TDS_EditorUtility.Button("Events", "Wrap / Unwrap Events settings", TDS_EditorUtility.HeaderStyle)) AreSpawerAreaEventsUnfolded = !areSpawerAreaEventsUnfolded;

        // If unfolded, draws the custom editor for the Components & References
        if (areSpawerAreaEventsUnfolded)
        {
            DrawEvents();
        }

        EditorGUILayout.EndVertical();
        GUILayout.Space(15);

        DrawSpawnedEnemies(); 

        // Applies all modified properties on the SerializedObjects
        serializedObject.ApplyModifiedProperties();

        GUI.backgroundColor = _originalColor;
    }

    /// <summary>
    /// Display settings
    /// </summary>
    void DrawSettings()
    {
        // Draw a header for the Spawner Area global settings
        EditorGUILayout.LabelField("Global settings", TDS_EditorUtility.HeaderStyle);
        TDS_EditorUtility.Toggle("is Looping", "Is the area start again when all the waves are cleared.", isLooping);
        if (isLooping.boolValue)
        {
            TDS_EditorUtility.IntSlider("Loop Interval", "Interval at which the area is reactivated (in seconds)", loopInterval, 0, 60);
        }
        TDS_EditorUtility.Toggle("Is Activated by event", "Does the area start by event or by trigger.", isActivatedByEvent);
        GUILayout.Space(10);


        EditorGUILayout.BeginHorizontal();
        if (TDS_EditorUtility.Button("WAVES", "Wrap / Unwrap Waves", TDS_EditorUtility.HeaderStyle)) AreWavesUnfolded = !areWavesUnfolded;

        GUITools.ActionButton("Add Wave", waves.InsertArrayElementAtIndex, waves.arraySize, Color.white, Color.white);
        EditorGUILayout.EndHorizontal();  
        if(areWavesUnfolded)
        {
            for (int i = 0; i < waves.arraySize; i++)
            {
                GUILayout.Space(10);
                DrawWave(i);
            }
        }
    }

    /// <summary>
    /// Draw the wave settings
    /// </summary>
    /// <param name="_index">indew of the wave in the array</param>
    void DrawWave(int _index)
    {
        Color _originalColor = GUI.backgroundColor; 
        GUI.backgroundColor = Color.white;
        //Get the serialized Property 
        SerializedProperty _prop = serializedObject.FindProperty($"{waves.name}").GetArrayElementAtIndex(_index);
        float _size = 50; 
        if(_prop.FindPropertyRelative("isWaveFoldOut").boolValue)
        {
            _size += (_prop.FindPropertyRelative("spawnPoints").arraySize + 1) * 25; 
        }
        // Get the last rect drawn
        Rect _lastRect = GUILayoutUtility.GetLastRect();
        EditorGUILayout.BeginVertical("HelpBox");
        // Set a new based on the last rect position
        Rect _r = new Rect(_lastRect.position.x, _lastRect.position.y + 4, _lastRect.width, _size);
        // Reserve the rect
        _r = GUILayoutUtility.GetRect(_r.width, _r.height, GUIStyle.none);
        GUIContent _content = new GUIContent($"Wave n°{_index}");
        // Draw the Wave Editor
        EditorGUI.PropertyField(_r, _prop, _content);
        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("▲") && _index > 0)
        {
            waves.MoveArrayElement(_index, _index - 1); 
        }
        if (GUILayout.Button("▼") && _index < serializedObject.FindProperty($"{waves.name}").arraySize - 1 )
        {
            waves.MoveArrayElement(_index, _index + 1);
        }
        EditorGUILayout.EndHorizontal(); 
        // Draw a button to destroy the wave
        GUITools.ActionButton("Delete Wave", waves.DeleteArrayElementAtIndex, _index, Color.red, Color.black);
        EditorGUILayout.EndVertical();
        GUI.backgroundColor = _originalColor;

    }

    void DrawSpawnedEnemies()
    {
        EditorGUILayout.BeginVertical("Helpbox");
        EditorGUILayout.LabelField("Spawned Elements", TDS_EditorUtility.HeaderStyle);
        GUILayout.Space(5);
        TDS_EditorUtility.PropertyField("Spawned Enemies", "Enemies already on the scene in editor", spawnedEnemies);
        GUILayout.Space(3);
        TDS_EditorUtility.PropertyField("Linked Throwables", "Throwables linked to this spawn area", areaThrowables);
        EditorGUILayout.EndVertical();
        GUILayout.Space(15);
    }

    #endregion

    #region Unity Methods
    private void OnEnable()
    {
        //Get the serialized properties from the serializedObject

        isReady = serializedObject.FindProperty("isReady");
        isActivated = serializedObject.FindProperty("isActivated");
        isLooping = serializedObject.FindProperty("isLooping");
        isActivatedByEvent = serializedObject.FindProperty("isActivatedByEvent");
        loopInterval = serializedObject.FindProperty("loopInterval");
        waves = serializedObject.FindProperty("waves");
        eventOnAreaActivated = serializedObject.FindProperty("OnAreaActivated");
        eventOnAreaDesactivated = serializedObject.FindProperty("OnAreaDesactivated");
        eventOnNextWave = serializedObject.FindProperty("OnNextWave");
        eventOnStartFight = serializedObject.FindProperty("OnStartFight");
        spawnedEnemies = serializedObject.FindProperty("spawnedEnemies");
        areaThrowables = serializedObject.FindProperty("areaThrowables");

        //Load the editor folded and unfolded values of this class
        areSpawnerAreaComponentsUnfolded = EditorPrefs.GetBool("areSpawnerAreaComponentsUnfolded", areSpawnerAreaComponentsUnfolded);
        areSpawnerAreaSettingsUnfolded = EditorPrefs.GetBool("areSpawnerAreaSettingsUnfolded", areSpawnerAreaSettingsUnfolded);
    }

    public override void OnInspectorGUI()
    {
        DrawSpawnerAreaEditor(); 
    }

    private void OnSceneGUI()
    {
        for (int i = 0; i < waves.arraySize; i++)
        {
            SerializedProperty _wave = waves.GetArrayElementAtIndex(i); 
            if(_wave.FindPropertyRelative("isWaveFoldOut").boolValue)
            {
                GUI.color = _wave.FindPropertyRelative("debugColor").colorValue;
                Handles.color = GUI.color; 
                for (int j = 0; j < _wave.FindPropertyRelative("spawnPoints").arraySize; j++)
                {
                    SerializedProperty _p = _wave.FindPropertyRelative($"spawnPoints").GetArrayElementAtIndex(j);
                    Handles.Label(_p.FindPropertyRelative("spawnPosition").vector3Value + Vector3.up, $"Spawn Point n°{j}");
                    Handles.DrawWireDisc(_p.FindPropertyRelative("spawnPosition").vector3Value, Vector3.up, _p.FindPropertyRelative("spawnRange").floatValue);
                    _p.FindPropertyRelative("spawnPosition").vector3Value = Handles.PositionHandle(_p.FindPropertyRelative("spawnPosition").vector3Value, Quaternion.identity);
                }
            }
        }
        serializedObject.ApplyModifiedProperties(); 
    }
    #endregion

    #endregion
}
