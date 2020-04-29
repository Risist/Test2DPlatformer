using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/*
 * Play sound (doubling)
 * Play animation
 *
 */
[CreateAssetMenu(fileName = "Node#", menuName = "Ris/Dialogue/Node")]
public class DialogueNode : ScriptableObject
{
    [Header("Main Text")]
    #region Main Text

    [Tooltip("sentence displayed by the caller")]
    public string mainText;

    [Tooltip("Audio played when dialogue goes to this node\n"+
             "null means no audio will be played")]
    public AudioClip mainTextVoiceover;

    [Tooltip("id of UnityAction placed in DialoguePanel list\n" +
             "-1 means no action at all\n" +
             "will be executed upon node enter")]
    public int actionId = -1;
    #endregion Main Text

    #region Auto Switch
    [Tooltip("After given time dialogue will switch to another state")]
    public float displayTime = float.PositiveInfinity;
    
    [Tooltip("State to which dialogue will transition if player wont choose any option for too long\n" + 
             " null means that the dialogue will end instead")]
    public DialogueNode idleTransition;
    #endregion Auto Switch

    [Header("Options")]
    #region Options

    [Tooltip("dialogue options displayed as dialogue buttons")]
    public DialogueOption[] options;


    [Serializable]
    public struct DialogueOption
    {
        [Tooltip("Text displayed on button with that option")]
        public string optionText;

        [Tooltip("Node to transition after pressing")]
        public DialogueNode nextNode;
    }
    #endregion Options
}
