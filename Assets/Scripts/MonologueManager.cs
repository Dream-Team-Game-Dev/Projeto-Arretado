﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class MonologueManager : MonoBehaviour
{
	public Text dialogueText;
	public Animator animator;

    public static MonologueManager instance = null;

    [SerializeField]
    private List<Interactable> interactables = new List<Interactable>();

	public Queue<string> Sentences{ get; set; }

    public bool Running { get; set; }
    public bool Typing { get; set; }
    public int InteractableId { get; set; }

    private WaitForSeconds delay = new WaitForSeconds(0.1f);
    private FirstPersonController firstPersonController;

    void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start () 
	{
		Sentences = new Queue<string> ();
        InteractableId = -1;
    }

    public void StartMonologue(string sentence)
    {
        Running = true;
        animator.SetBool("IsOpen", true);
        Sentences.Clear();
        Sentences.Enqueue(sentence);
        DisplayNextSentence();
    }

    public void StartMonologue(string[] sentences)
    {
        Running = true;
        animator.SetBool("IsOpen", true);
        Sentences.Clear();
        if (sentences.Length > 0)
        {
            foreach (string sentence in sentences)
            {
                Sentences.Enqueue(sentence);
            }
            DisplayNextSentence();
            return;
        }
        EndMonologue();
    }

    public void StartMonologue (int id)
	{
        Running = true;
        InteractableId = id;
		animator.SetBool("IsOpen", true);
		Sentences.Clear ();
        foreach (Interactable interactable in interactables)
        {
            if (interactable.iGameObject.GetInstanceID() == id)
            {
                foreach (string sentence in interactable.sentences)
                {
                    Sentences.Enqueue(sentence);
                }
                DisplayNextSentence();
                return;
            }
        }
        EndMonologue();
    }

	public void DisplayNextSentence()
	{
		if (Sentences.Count == 0) 
		{
			EndMonologue ();
			return;
		}

		string sentence = Sentences.Dequeue ();
		StopAllCoroutines ();
		StartCoroutine (TypeSentence (sentence));
	}

    public Sprite RedAction(int hitId)
    {
        foreach (Interactable interactable in interactables)
        {
            if (interactable.iGameObject.GetInstanceID() == hitId)
            {
                return interactable.redAction;
            }
        }
        return null;
    }

	IEnumerator TypeSentence (string sentence)
	{
        Typing = true;
		dialogueText.text = "";
		foreach (char letter in sentence.ToCharArray()) 
		{
			dialogueText.text += letter;
			yield return null;
		}
        yield return delay;
        Typing = false;
    }

	public void EndMonologue()
	{
		animator.SetBool("IsOpen", false);
        firstPersonController.Locked = false;
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        yield return delay;
        Running = false;
    }

    public void FirstPersonController(FirstPersonController firstPersonController)
    {
        this.firstPersonController = firstPersonController;
    }
}

[System.Serializable]
public class Interactable
{
    public GameObject iGameObject;
    [TextArea(1, 10)]
    public string[] sentences;
    public Sprite redAction;
}