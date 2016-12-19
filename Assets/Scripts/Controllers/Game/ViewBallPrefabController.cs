using UnityEngine;
using System.Collections;
using BubbleChip;
using UnityEngine.UI;

public class ViewBallPrefabController : MonoBehaviour
{

    private Rigidbody2D rigidbody2d;
    private PlayerBallEmiter emiter;
    private RectTransform rectTransform;
    private Image image;

    public void Initialize(PlayerBallEmiter emiter)
    {
        this.emiter = emiter;
        rigidbody2d = GetComponent<Rigidbody2D>();
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    /// <summary>
    /// Wprowadza view ball w stan emisji
    /// </summary>
    /// <param name="force"></param>
    public void Emit(Vector2 force, BallConfiguration configuration)
    {
        image.material = configuration.material;
        gameObject.SetActive(true);
        rigidbody2d.constraints = RigidbodyConstraints2D.None;
        rigidbody2d.AddForce(force, ForceMode2D.Impulse);
    }

    protected void OnCollisionEnter2D(Collision2D coll)
    {
        BallPrefabController ball = coll.gameObject.GetComponent<BallPrefabController>();
        if (ball == null &&  
            !coll.gameObject.name.Equals(gameObject.name) && 
            !coll.gameObject.name.Equals(PlayerBallPrefabController.TOP_ZONE_COLLIDER))
            return;

        ReUse();
        emiter.ReUse(this);
    }

    public void ReUse()
    {
        rigidbody2d.constraints = RigidbodyConstraints2D.FreezePositionY
            | RigidbodyConstraints2D.FreezePositionX;
        gameObject.SetActive(false);
        rectTransform.anchoredPosition = Vector2.zero;
    }
}
