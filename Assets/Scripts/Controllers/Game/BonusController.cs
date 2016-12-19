using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BonusController : MonoBehaviour
{
    private const string TRIGGER_SHOW = "Show";

    private Text text;
    private Animator animator;
    private AudioSource audioSource;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        text = GetComponent<Text>();
    }

    public void Set(string content)
    {
        text.text = content;

        animator.SetTrigger(TRIGGER_SHOW);
        audioSource.PlayDelayed(0.1f);
    }
}
