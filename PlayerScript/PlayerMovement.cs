using System.Collections;
using UnityEngine;

public enum PlayerState
{
    walk, attack, interact, stagger, idle, attackIdle
}
public class PlayerMovement : MonoBehaviour
{
    public Inventory playerInventory;
    public SpriteRenderer receivedItemSprite;
    public PlayerState currentState;
    public float Normalspeed;
    private Rigidbody2D myRigidbody;
    private Vector3 change;
    private Animator animator;
    PlayerMovement playerMovement;
    public FloatValue currentHealth;
    public Notification playerHealthNotification;
    public VectorValue startingPosition;
    //hitboxes
    public GameObject downHitbox;
    public GameObject upHitbox;
    public GameObject leftHitbox;
    public GameObject rightHitbox;
    void Start()
    {
        currentState = PlayerState.walk;
        animator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
        transform.position = startingPosition.initialValue;

    }

    // Update is called once per frame
    void Update()
    {
        //is the player in an interaction
        if(currentState == PlayerState.interact)
        {
            return;
        }
        change = Vector3.zero;
        change.x = Input.GetAxisRaw("Horizontal") * Time.deltaTime * Normalspeed;
        change.y = Input.GetAxisRaw("Vertical") * Time.deltaTime * Normalspeed;

        if (Input.GetButtonDown("attack") && currentState != PlayerState.attack && currentState != PlayerState.stagger)
        {
            StartCoroutine(Wait());
        }
        else if (currentState == PlayerState.walk || currentState == PlayerState.idle)
        {
            playerMovement.UpdateAnimationAndMove();
        }
    }

    private IEnumerator Wait()
    {
        //right
        if (animator.GetFloat("moveX") > 0 && animator.GetFloat("moveY") == 0)
        {
            rightHitbox.SetActive(true);
            animator.SetBool("attacking", true);
            yield return new WaitForSeconds(.40f);
            animator.SetBool("attacking", false);
            rightHitbox.SetActive(false);
        }
        //up
        if (animator.GetFloat("moveX") == 0 && animator.GetFloat("moveY") > 0)
        {
            upHitbox.SetActive(true);
            animator.SetBool("attacking", true);
            yield return new WaitForSeconds(.40f);
            animator.SetBool("attacking", false);
            upHitbox.SetActive(false);
        }
        //left
        if (animator.GetFloat("moveX") < 0 && animator.GetFloat("moveY") == 0)
        {
            leftHitbox.SetActive(true);
            animator.SetBool("attacking", true);
            yield return new WaitForSeconds(.40f);
            animator.SetBool("attacking", false);
            leftHitbox.SetActive(false);
        }
        //down
        if (animator.GetFloat("moveX") == 0 && animator.GetFloat("moveY") < 0)
        {
            downHitbox.SetActive(true);
            animator.SetBool("attacking", true);
            yield return new WaitForSeconds(.40f);
            animator.SetBool("attacking", false);
            downHitbox.SetActive(false);
        }
        if(currentState != PlayerState.interact)
        {
            currentState = PlayerState.walk;
        }

    }
    public void RaiseItem()
    {
        if(playerInventory .currentItem !=null)
        {
            if (currentState != PlayerState.interact)
            {
                animator.SetBool("receive Item", true);
                currentState = PlayerState.interact;
                receivedItemSprite.sprite = playerInventory.currentItem.itemSprite;
            }
            else
            {
                animator.SetBool("receive Item", false);
                currentState = PlayerState.idle;
                receivedItemSprite.sprite = null;
                playerInventory.currentItem = null;
            }
        }
    }
    public void UpdateAnimationAndMove()
    {
        if (change != Vector3.zero)
        {
            transform.Translate(new Vector3(change.x, change.y));
            animator.SetFloat("moveX", change.x);
            animator.SetFloat("moveY", change.y);
            animator.SetBool("moving", true);
            
        }
        else
        {
            animator.SetBool("moving", false);
        }
        if(animator.GetBool("moving") == true)
        {
            animator.StopPlayback();
        }

        if (animator.GetBool("moving") == true && Input.GetKey(KeyCode.LeftShift))
        {
            transform.Translate(new Vector3(change.x, change.y));
            animator.SetFloat("moveX", change.x);
            animator.SetFloat("moveY", change.y);
            animator.SetBool("running", true);
        }
        else
        {
            animator.SetBool("running", false);
        }
        if (animator.GetBool("moving") == true)
        {
            animator.StopPlayback();
        }

    }
    void MoveCharacter()
    {
        myRigidbody.MovePosition(transform.position + change.normalized * Normalspeed * Time.deltaTime);
    }

    public void Knock(float knockTime, float damage)
    {
        currentHealth.RuntimeValue -= damage;
        playerHealthNotification.Raise();
        if (currentHealth.RuntimeValue > 0)
        {
            StartCoroutine(KnockCo(myRigidbody, knockTime));

        }
        else
        {
            this.gameObject.SetActive(false);
        }

    }

    private IEnumerator KnockCo(Rigidbody2D myRigidbody, float knockTime)
    {
        if (myRigidbody != null)
        {
            yield return new WaitForSeconds(knockTime);
            myRigidbody.velocity = Vector2.zero;
            currentState = PlayerState.idle;
            myRigidbody.velocity = Vector2.zero;
        }
    }
}
