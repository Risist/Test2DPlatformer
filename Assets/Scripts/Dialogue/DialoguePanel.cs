using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialoguePanel : ImageFader
{
    [Header("General")]
    #region General
    [Tooltip("Object that activates interaction. If null will be set up with object tagged \"Player\"")]
    public Transform player;
    public bool IsPlayerCloseEnough()
    {
        Vector2 toPlayer = transform.position - player.position;
        return toPlayer.sqrMagnitude < triggerDist * triggerDist;
    }

    [Tooltip("Radius interaction starts")]
    public float triggerDist = 5.0f;

    [Tooltip("The text by which agent speaks")]
    public Text mainText;

    [Tooltip("prefab of buttons")]
    public GameObject buttonPrefab;
    public Transform buttonSpawn;

    #endregion General


    [Tooltip("Currently shown node; while setting up represents root node\n" +
             "if null then no interaction occurs")]
    [SerializeField] DialogueNode currentNode;

    public UnityEvent[] nodeEvents;

    MinimalTimer timer = new MinimalTimer();
    Button[] buttons;
    AudioSource audioSource;

    void SwitchDialogueNode(DialogueNode node)
    {
        currentNode = node;

        if (!node)
            // transition to empty node, just ignore this call
            // no need to further set up
            return;

        mainText.text = node.mainText;
        CorrectButtons(node);
        UpdateButtonEvents(node);

        
        // play audio
        if (audioSource && node.mainTextVoiceover)
            audioSource.PlayOneShot(node.mainTextVoiceover);

        // do event
        if (node.actionId != -1 && node.actionId < nodeEvents.Length)
            nodeEvents[node.actionId].Invoke();

        // restart dialogue showoff timer
        timer.Restart();
    }
    void CorrectButtons(DialogueNode node)
    {
        // check if we currently have enough buttons
        int nOptions = node.options.Length;
        int nButtons = buttons.Length;

        if (nOptions <= nButtons)
        {
            // enable required amount of buttons
            int i = 0;
            for (; i < nOptions; ++i)
                buttons[i].gameObject.SetActive(true);

            // disable rest
            for (; i < nButtons; ++i)
                buttons[i].gameObject.SetActive(false);
        }
        else
        {
            // we need to spawn several new buttons
            while (nOptions > nButtons)
            {
                Instantiate(buttonPrefab, buttonSpawn);
                nButtons++;
            };

            // actualize button list
            buttons = GetComponentsInChildren<Button>(true);
            UpdateTargets();
        }
    }
    void UpdateButtonEvents(DialogueNode node)
    {
        int nOptions = node.options.Length;
        for (int i = 0; i < nOptions; ++i)
        {
            // update click event
            buttons[i].onClick.RemoveAllListeners();
            var transitionNode = node.options[i].nextNode;
            buttons[i].onClick.AddListener( () => SwitchDialogueNode(transitionNode) );

            // update button text 
            var txt = buttons[i].GetComponentInChildren<Text>();
            if (txt)
                txt.text = node.options[i].optionText;
            else
                Debug.LogWarning("Button has no text!");

        }
    }



    /// cache initial rotation to freeze it
    Vector3 rotation;
    new void Start()
    {
        base.Start();

        buttons = GetComponentsInChildren<Button>(true);
        InitPlayer();
        rotation = transform.eulerAngles;

        if (currentNode)
            SwitchDialogueNode(currentNode);

        audioSource = GetComponent<AudioSource>();
    }
    void InitPlayer()
    {
        if (!player)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p)
                player = p.transform;
            else
                Debug.LogWarning("No object with \"Player\" tag found");
        }
    }

    private void Update()
    {
        UpdateVisibility();

        // keep rotation of the panel constant yet following caller
        transform.eulerAngles = rotation;

        if (currentNode && timer.IsReady(currentNode.displayTime))
        {
            SwitchDialogueNode(currentNode.idleTransition);
        }
    }
    void UpdateVisibility()
    {
        bool selected =
            // do not display any text if dialogue node is not set up
            currentNode &&
            // player has to be close enough
            player && IsPlayerCloseEnough();

        if (selected)
            Show();
        else
            Hide();
    }

    
    void OnDrawGizmosSelected()
    {
        #if UNITY_EDITOR
        UnityEditor.Handles.color = Color.yellow;
        UnityEditor.Handles.DrawWireDisc(transform.position, transform.forward, triggerDist);
        #endif
    }
}
