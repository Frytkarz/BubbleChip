using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BubbleChip
{
    public class PlayerBallPrefabController : BallPrefabController
    {
        public const string TOP_ZONE_COLLIDER = "TopZoneCollider";

        public AudioSource audioSourceEmit;

        private Rigidbody2D rigidbody2d;
        private Collider2D collider2d;
        private PlayerBallEmiter emiter;

        // Use this for initialization
        protected override void Awake()
        {
            base.Awake();
            rigidbody2d = GetComponent<Rigidbody2D>();
            collider2d = GetComponent<Collider2D>();
            
            rigidbody2d.freezeRotation = true;
        }

        public void Set(BallConfiguration ballConfiguration, PlayerBallEmiter emiter)
        {
            this.ballConfiguration = ballConfiguration;
            this.emiter = emiter;
            image.material = ballConfiguration.material;
        }

        /// <summary>
        /// Wprowadza ball w stan emisji
        /// </summary>
        /// <param name="force"></param>
        public void Emit(Vector2 force)
        {
            audioSourceEmit.Play();
            collider2d.isTrigger = false;
            rigidbody2d.AddForce(force, ForceMode2D.Impulse);
        }

        protected void OnCollisionEnter2D(Collision2D coll)
        {
            if(emiter == null)
                return;

            BallPrefabController ball = coll.gameObject.GetComponent<BallPrefabController>();
            if (ball == null && !coll.gameObject.name.Equals(TOP_ZONE_COLLIDER))
                return;


            rigidbody2d.constraints = RigidbodyConstraints2D.FreezePositionY 
                    | RigidbodyConstraints2D.FreezePositionX;
            emiter.OnPlayerBallStay(ball);
            emiter = null;
        }

        public override void ReUse()
        {
            collider2d.isTrigger = true;
            animator.SetTrigger(TRIGGER_RESET);
            rigidbody2d.constraints = RigidbodyConstraints2D.None;
            rectTransform.anchoredPosition = Vector2.zero;
        }
    }
}
