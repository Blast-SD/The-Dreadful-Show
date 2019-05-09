﻿using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TDS_EventsSystem))]
public class TDS_EventSystemEditor : Editor 
{
    /* TDS_EventSystemEditor :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Custom editor ofr the TDS_EventSystem class, to have something prettier and easier to use.
	 *
	 *	#####################
	 *	####### TO DO #######
	 *	#####################
	 *
	 *	...
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[02 / 05 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    Creation of the TDS_EventSystemEditor class.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties
    /// <summary>SerializedProperty for <see cref="TDS_EventsSystem.isActivated"/> of type <see cref="bool"/>.</summary>
    private SerializedProperty isActivated = null;

    /// <summary>SerializedProperty for <see cref="TDS_EventsSystem.isWaitingOthers"/> of type <see cref="bool"/>.</summary>
    private SerializedProperty isWaitingOthers = null;

    /// <summary>SerializedProperty for <see cref="TDS_EventsSystem.events"/> of type <see cref="bool"/>.</summary>
    private SerializedProperty events = null;

    /// <summary>
    /// Folout booleans for events array.
    /// </summary>
    [SerializeField] private bool[] foldouts = new bool[] { };
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Draws the editor of the editing scripts.
    /// </summary>
    public void DrawEditor()
    {
        // Make a space at the beginning of the editor
        GUILayout.Space(10);
        Color _originalColor = GUI.backgroundColor;
        Color _originalGUIColor = GUI.color;

        GUI.backgroundColor = TDS_EditorUtility.BoxDarkColor;
        EditorGUILayout.BeginVertical("HelpBox");

        // Records any changements on the editing objects to allow undo
        Undo.RecordObjects(targets, "Event System settings");

        // Updates the SerializedProperties to get the latest values
        serializedObject.Update();

        TDS_EditorUtility.RadioToggle("Activated", "Is this event system activated or not", isActivated);

        // Button to add a new event
        GUI.backgroundColor = TDS_EditorUtility.BoxLightColor;
        GUI.color = Color.green;

        if (TDS_EditorUtility.Button("+", "Add a new event", EditorStyles.miniButton))
        {
            events.InsertArrayElementAtIndex(0);
            events.GetArrayElementAtIndex(0).FindPropertyRelative("Name").stringValue = "New Event";

            bool[] _newFoldouts = new bool[foldouts.Length + 1];
            Array.Copy(foldouts, 0, _newFoldouts, 1, foldouts.Length);
            foldouts = _newFoldouts;
        }

        GUI.color = Color.white;
        GUI.backgroundColor = TDS_EditorUtility.BoxDarkColor;

        for (int _i = 0; _i < events.arraySize; _i++)
        {
            GUILayout.Space(5);

            GUI.backgroundColor = TDS_EditorUtility.BoxLightColor;
            EditorGUILayout.BeginVertical("Box");

            SerializedProperty _event = events.GetArrayElementAtIndex(_i);
            SerializedProperty _eventName = _event.FindPropertyRelative("Name");

            EditorGUILayout.BeginHorizontal();

            // Button to show or not this event
            if (TDS_EditorUtility.Button(_eventName.stringValue, "Wrap / unwrap this event", TDS_EditorUtility.HeaderStyle)) foldouts[_i] = !foldouts[_i];

            GUILayout.FlexibleSpace();
            GUI.color = Color.red;

            // Button to delete this event
            if (TDS_EditorUtility.Button("X", "Delete this event", EditorStyles.miniButton))
            {
                events.DeleteArrayElementAtIndex(_i);

                bool[] _newFoldouts = new bool[foldouts.Length - 1];
                Array.Copy(foldouts, 0, _newFoldouts, 0, _i);
                Array.Copy(foldouts, _i + 1, _newFoldouts, _i, foldouts.Length - (_i + 1));
                foldouts = _newFoldouts;
                break;
            }

            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();

            // If unfolded, draws this event
            if (foldouts[_i])
            {
                SerializedProperty _eventType = _event.FindPropertyRelative("eventType");
                SerializedProperty _doRequireType = _event.FindPropertyRelative("doNeedSpecificPlayerType");

                TDS_EditorUtility.TextField("Name", "Name of this event", _eventName);

                GUILayout.Space(3);

                TDS_EditorUtility.PropertyField("Event Type", "Type of this event", _eventType);
                TDS_EditorUtility.Toggle("Require specific Player type", "Should this event require a specific player type to be triggered", _doRequireType);

                if (_doRequireType.boolValue)
                {
                    TDS_EditorUtility.PropertyField("Required type of Player", "Required type of player to trigger this event", _event.FindPropertyRelative("playerType"));
                }
                else
                {
                    TDS_EditorUtility.Toggle("Master Only", "Indicates of this event should only be called by the master", _event.FindPropertyRelative("isMasterOnly"));
                }

                if (_i > 0)
                {
                    GUILayout.Space(3);

                    TDS_EditorUtility.Toggle("Wait previous event complete", "Should this event only be triggered when the previous one is complete", _event.FindPropertyRelative("doWaitPreviousEvent"));
                }

                GUILayout.Space(5);

                switch ((CustomEventType)_eventType.enumValueIndex)
                {
                    case CustomEventType.Narrator:
                        TDS_EditorUtility.TextField("Text ID", "ID of the text to use for the Narrator", _event.FindPropertyRelative("textID"));

                        GUILayout.Space(2);

                        TDS_EditorUtility.Toggle("Online", "Should this event only interact in local or for other players too", _event.FindPropertyRelative("isOnline"));
                        break;

                    case CustomEventType.DisplayInfoBox:
                        TDS_EditorUtility.TextField("Text ID", "ID of the text to use for the Info Box", _event.FindPropertyRelative("textID"));
                        break;

                    case CustomEventType.DesactiveInfoBox:
                        break;

                    case CustomEventType.Instantiate:
                        TDS_EditorUtility.PropertyField("Prefab", "Prefab to instantiate", _event.FindPropertyRelative("prefab"));

                        TDS_EditorUtility.PropertyField("Instantiation transform reference", "Transform to use as reference for position & rotation for the transform of the instantiated object", _event.FindPropertyRelative("prefabTransform"));

                        GUILayout.Space(2);

                        TDS_EditorUtility.Toggle("Online", "Should this event only interact in local or for other players too", _event.FindPropertyRelative("isOnline"));
                        break;

                    case CustomEventType.Wait:
                        TDS_EditorUtility.FloatField("Time to wait", "Time to wait for this event (in seconds).", _event.FindPropertyRelative("waitTime"));
                        break;

                    case CustomEventType.WaitForAction:
                        TDS_EditorUtility.PropertyField("Action to wait", "Action to wait the player to perform", _event.FindPropertyRelative("actionType"));
                        break;

                    case CustomEventType.WaitOthers:
                        TDS_EditorUtility.RadioToggle("Waiting Other Players...", "Currently waiting for other players...", serializedObject.FindProperty("isWaitingOthers"));
                        break;

                    case CustomEventType.UnityEvent:
                        TDS_EditorUtility.PropertyField("Unity Event", "Associated Unity Event to this event", _event.FindPropertyRelative("unityEvent"));
                        break;

                    default:
                        // Mhmm...
                        break;
                }

                // Button to add a new event
                GUI.color = Color.green;

                if (TDS_EditorUtility.Button("+", "Add a new event", EditorStyles.miniButton))
                {
                    events.InsertArrayElementAtIndex(_i);

                    bool[] _newFoldouts = new bool[foldouts.Length + 1];
                    Array.Copy(foldouts, 0, _newFoldouts, 0, _i + 1);
                    Array.Copy(foldouts, _i + 1, _newFoldouts, _i + 2, foldouts.Length - (_i + 1));
                    foldouts = _newFoldouts;
                }

                GUI.color = Color.white;
            }

            EditorGUILayout.EndVertical();   
        }

        // Applies all modified properties on the SerializedObjects
        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.EndVertical();
        GUI.backgroundColor = _originalColor;
        GUI.color = _originalGUIColor;
    }
    #endregion

    #region Unity Methods
    // This function is called when the object is loaded
    private void OnEnable()
    {
        isActivated = serializedObject.FindProperty("isActivated");
        isWaitingOthers = serializedObject.FindProperty("isWaitingOthers");
        events = serializedObject.FindProperty("events");

        foldouts = new bool[events.arraySize];
    }

    // Implement this function to make a custom inspector
    public override void OnInspectorGUI()
    {
        DrawEditor();
    }
    #endregion

    #endregion
}