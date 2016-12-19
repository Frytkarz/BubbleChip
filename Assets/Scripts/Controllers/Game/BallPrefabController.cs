using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngineInternal;

namespace BubbleChip
{
    public class BallPrefabController : MonoBehaviour
    {
        private const string TRIGGER_DESTROY = "Destroy";
        protected const string TRIGGER_RESET = "Reset";

        public BallConfiguration ballConfiguration { get; protected set; }
        public Vector2Int position { get; protected set; }
        public RectTransform rectTransform { get; protected set; }
        public Animator animator { get; protected set; }
        public AudioSource audioSource { get; private set; }

        protected Image image;
        protected Text text;

        private BallsEmiter ballsEmiter;

        // Use this for initialization
        protected virtual void Awake ()
        {
            audioSource = GetComponent<AudioSource>();
            animator = GetComponent<Animator>();
            image = GetComponent<Image>();
            rectTransform = GetComponent<RectTransform>();
            text = GetComponentInChildren<Text>();

            rectTransform.Rotate(new Vector3(0, 0, 1), Random.Range(0, 360));
        }

        public void Set(Vector2Int position, Vector2 anchoredPosition)
        {
            this.position = position;
            rectTransform.anchoredPosition = anchoredPosition;
        }

        public void Set(Vector2Int position, Vector2 anchoredPosition, BallConfiguration ballConfiguration)
        {
            Set(position, anchoredPosition);
            this.ballConfiguration = ballConfiguration;
            image.material = ballConfiguration.material;
        }

        public void DestroyBall(BallsEmiter ballsEmiter, int points)
        {
            this.ballsEmiter = ballsEmiter;
            text.text = points.ToString();
            animator.SetTrigger(TRIGGER_DESTROY);
            audioSource.Play();
        }

        private void OnDestroyAnimationFinished()
        {
            ballsEmiter.ReUse(this);
        }

        public virtual void ReUse()
        {
            animator.SetTrigger(TRIGGER_RESET);
            rectTransform.anchoredPosition = Vector2.zero;
        }
    }
}
