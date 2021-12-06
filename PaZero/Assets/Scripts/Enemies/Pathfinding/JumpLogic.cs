using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpLogic : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private GameObject trigger;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private GameObject pivot;
#pragma warning restore 0649
    private Rigidbody2D rb;
    public float detectionLength = 0.2f;
    private bool stopMoving = false;
    public Vector3 offset;
    Vector3 startingPoint;

    Vector3 resetPos;
    Vector3 blockPos;
    Vector3 checkPos;
    public GameObject prop;
    public GameObject point;
    public GameObject[] points;
    public float launchForce;

    public Vector2 direction;
    private Vector2 raycastDirection;
    public int numberOfPoints;

    public bool hit;

    Vector3[,] map;
    public Vector2 objective;

    // Start is called before the first frame update
    void Start()
    {
        resetPos = transform.position;

        jumpSpotsHardcoded();

        points = new GameObject[numberOfPoints];

        for(int i = 0; i < numberOfPoints; i++)
        {
            points[i] = Instantiate(point, transform.position, Quaternion.identity);
        }

        rb = GetComponent<Rigidbody2D>();
        startingPoint = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        for(int i = 0; i < points.Length; i++)
        {
            points[i].transform.position = PointPosition(i * 0.1f);
        }

        stopMoving = Physics2D.Raycast(transform.position + offset, Vector2.down, detectionLength, groundLayer);

        if (stopMoving && rb.velocity.y <= 0.1f)
        {
            rb.velocity = Vector2.zero;
        }

        raycastDirection = trigger.transform.position - pivot.transform.position;
        hit = Physics2D.Raycast(pivot.transform.position, raycastDirection.normalized, Mathf.Sqrt((raycastDirection.x * raycastDirection.x) + (raycastDirection.y * raycastDirection.y)), groundLayer);

    }

    private void jumpSpotsHardcoded()
    {
        map = new Vector3[4,7]; //height by distance 0 = -3

        // vector and power

        map[0, 0] = new Vector3(1, 2, 8);
        map[1, 0] = new Vector3(2, 2, 8);
        map[2, 0] = new Vector3(3, 2, 10);
        map[3, 0] = new Vector3(3, 2, 12);

        map[0, 1] = new Vector3(1, 2, 9);
        map[1, 1] = new Vector3(2, 2, 9);
        map[2, 1] = new Vector3(3, 2, 11);
        map[3, 1] = new Vector3(3, 2, 13);

        map[0, 2] = new Vector3(1, 2, 10);
        map[1, 2] = new Vector3(2, 2, 10);
        map[2, 2] = new Vector3(3, 2, 12);
        map[3, 2] = new Vector3(3, 2, 14);

        map[0, 3] = new Vector3(3, 3, 10);
        map[1, 3] = new Vector3(3, 3, 12);
        map[2, 3] = new Vector3(3, 3, 14);
        map[3, 3] = new Vector3(3, 3, 16);

        map[0, 4] = new Vector3(2, 4, 13);
        map[1, 4] = new Vector3(2, 4, 15);
        map[2, 4] = new Vector3(2, 4, 17);
        map[3, 4] = new Vector3(2, 4, 19);

        map[0, 5] = new Vector3(1, 5, 17);
        map[1, 5] = new Vector3(2, 5, 17);
        map[2, 5] = new Vector3(3, 5, 18);
        map[3, 5] = new Vector3(4, 5, 19);

        map[0, 6] = new Vector3(0.79f, 5, 20);
        map[1, 6] = new Vector3(1, 5, 20);
        map[2, 6] = new Vector3(2, 5, 20);
        map[3, 6] = new Vector3(2, 5, 21);

    }

    private Vector2 PointPosition(float t)
    {
        Vector2 currentPosition = (Vector2) transform.position + (direction.normalized * launchForce * t) + 0.5f * Physics2D.gravity * prop.GetComponent<Rigidbody2D>().gravityScale * t * t;
        return currentPosition;
    }
    

    public void ApplyTest()
    {
        SeekPlatform();
    }

    public void ResetPosition()
    {
        rb.velocity = Vector2.zero;
        transform.position = resetPos;
    }

    public void SeekPlatform()
    {
        objective = Vector2.zero;
        StartCoroutine(Seek(1));
    }

    // 1 is up, -1 is down
    IEnumerator Seek(int dir)
    {
        Collider2D _col = trigger.GetComponent<CircleCollider2D>();
        int count = 0;
        bool end = false;
        

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j <= 4; j++)
            {
                trigger.transform.position = (Vector2)pivot.transform.position + new Vector2(j, i * dir);

                yield return new WaitForSeconds(.01f);
                if (hit)
                {
                    for(int k = i; k <= i+3; k++)
                    {
                        trigger.transform.position = (Vector2)pivot.transform.position + new Vector2(j, k);

                        if (!hit)
                        {
                            count++;
                        }
                        else
                        {
                            count = 0;
                        }
                        if (count == 2)
                        {
                            Debug.Log(j + " " + i * dir);
                            // check air for the jump
                            for (int y = j; y >= 0; y--)
                            {
                                for(int p = 0; p < 2; p++)
                                {
                                    hit = false;
                                    trigger.transform.position = (Vector2)pivot.transform.position + new Vector2(y, k - (p * dir));
                                    yield return new WaitForSeconds(.01f);
                                    if (hit)
                                    {
                                        // does not jump
                                        trigger.transform.position = pivot.transform.position;
                                        yield break;
                                    }
                                }
                            }

                            objective = new Vector2(j, i * dir);

                            direction = new Vector2(map[(int)objective.x - 1, (int)objective.y + 3].x, map[(int)objective.x - 1, (int)objective.y + 3].y);
                            launchForce = map[(int)objective.x - 1, (int)objective.y + 3].z;

                            rb.velocity = direction.normalized * launchForce;

                            Debug.Log(j + " " + i);
                            end = true;

                            trigger.transform.position = pivot.transform.position;
                            yield break;
                        }

                        yield return new WaitForSeconds(.01f);
                    }
                    if (end)
                    {
                        break;
                    }
                    count = 0;
                }

                yield return new WaitForSeconds(.01f);
            }
        }
        
        trigger.transform.position = pivot.transform.position;

        // could not find anything. look down
        if (dir != -1)
        {
            StartCoroutine(Seek(-1));
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(pivot.transform.position, trigger.transform.position);
    }
}
