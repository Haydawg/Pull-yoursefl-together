using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npc : MonoBehaviour
{
    public Detection[] detections;
    public bool isMoving;
    Collider2D collider;
    public bool caught;
    public LimbType LimbType;
    public GameObject limb;
    [SerializeField]
    float floorLevel;

    [SerializeField]
    Animator anim;

    float move = 1;
    [SerializeField]
    float raycastSize = 5;
    [SerializeField]
    float avoidenceSize = 10;
    public bool stop;
    public bool droppedLimb = false;

    public AudioSource audioSource;
    public AudioClip[] clips;
    // Start is called before the first frame update
    void Start()
    {
        detections = FindObjectsOfType<Detection>();
        collider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        CheckWalkArea();
        StartCoroutine(CheckMoving());
        if (caught)
        {
            collider.enabled = false;
            GetComponent<SpriteRenderer>().sortingOrder = -4;
            stop = true;

            if (!droppedLimb)
            {
                DropLimb();
            }
        }

        if(transform.position.y < floorLevel)
        {
            Destroy(gameObject);
        }
        anim.SetFloat("move", move);
        anim.SetBool("isMoving", !stop);

        stop = false;
        foreach (Detection detection in detections)
        {
            if (Vector2.Distance(detection.transform.position, transform.position) < avoidenceSize)
            {
                stop = true;
            }
        }

    }




    public void Movement()
    {
        if (stop)
            return;
        else if (caught)
            return;
        transform.position = transform.position + new Vector3(move,0,0) * Time.deltaTime;
    }
    public IEnumerator CheckMoving()
    {
        Vector3 startPos = transform.position;
        yield return new WaitForSeconds(.1f);
        Vector3 finalPos = transform.position;
        isMoving = (finalPos != startPos);
    }

    public void DropLimb()
    {
        droppedLimb = true;
        GameObject droppedlimb = Instantiate(limb, transform.position, Quaternion.identity);
        audioSource.PlayOneShot(clips[1]);
        Limb limbType = droppedlimb.GetComponent<Limb>();

        //int randomValue = Random.Range(0, 4);
        //switch(randomValue)
        //{
        //    case 0:
        //        limbType.type = LimbType.Leg;
        //        break;
        //    case 1:
        //        limbType.type = LimbType.Arm;
        //        break;
        //    case 2:
        //        limbType.type = LimbType.Head;
        //        break;
        //    case 3:
        //        limbType.type = LimbType.Torso;
        //        break;
        //}
        limbType.type = LimbType;
        limbType.SetSprite();
    }

    public void CheckWalkArea()
    {
        
        RaycastHit2D[] hit = new RaycastHit2D[1];
        if(move > 0)
            collider.Raycast(-transform.up + transform.right, hit, raycastSize);
        else if (move < 0)
            collider.Raycast(-transform.up + -transform.right, hit, raycastSize);
        if (hit[0])
        {
            Debug.DrawLine(transform.position, hit[0].point, Color.red);

            if (hit[0].collider.tag != "Platform")
                move *= -1;
        } 
        else
            move *= -1;

    }
}
