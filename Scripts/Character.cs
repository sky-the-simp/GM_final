using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Character : MonoBehaviour
{
    public Dictionary<Vector3Int, Vector3Int> reachablePositions; // Stores reachable tiles within movement range
    public List<Vector3Int> attackablePositions;
    public List<Vector3Int> nearAttackablePositions;
    public Vector3Int currPos;
    public int group;
    public int maxMovementRange;
    public int currMovementRange;
    public float moveSpeed = 5.0f;
    public float knockBackSpeed = 10.0f;
    public int attackRange;
    public int attackPower;
    public int attackCount;
    public eCharacterStatus charStatus;
    public Vector3Int movePosition;
    public bool faceDirection;
    public GridSystem gs;
    public GridEventManager gem;

    private void Start()
    {
        reachablePositions = new Dictionary<Vector3Int, Vector3Int>();
        attackablePositions = new List<Vector3Int>();
        charStatus = eCharacterStatus.waiting;
        faceDirection = true;
    }
    private void Update(){
        if (charStatus == eCharacterStatus.moving){

        }
    }
    public IEnumerator MoveTowardsPosition(Vector3Int position){
        charStatus = eCharacterStatus.moving;
        Stack<Vector3> stack = new Stack<Vector3>();
        Stack<Vector3Int> stackCoord = new Stack<Vector3Int>();
        faceDirection = true;
        while (position != Vector3Int.zero){
            stackCoord.Push(position);
            stack.Push(gs.GridToWorldPosition(position));
            position = reachablePositions.ContainsKey(position) ? reachablePositions[position] : Vector3Int.zero;
        }

        stack.Pop(); // pops the position of current tile out
        stackCoord.Pop();
        while (stack.Count != 0){
            currMovementRange--;
            Vector3 startPosition = transform.position;
            Vector3 targetPosition = stack.Pop();
            float distance = Vector3.Distance(startPosition, targetPosition);
            float timeToMove = distance / moveSpeed; // Time needed to reach target based on speed
            float elapsedTime = 0;

            // Gradually move the character towards the target position
            while (elapsedTime < timeToMove)
            {
                transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / timeToMove);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure character snaps exactly to target position
            transform.position = targetPosition;
            Vector3Int tempPos = stackCoord.Pop();
            bool successfullyOnPos = gem.arrivePos(tempPos);
            if (!successfullyOnPos){
                KnockBackToPos(currPos);
                break;
            }
            else{
                currPos = tempPos;
                gs.GetTileAt(currPos).character = this;
            }
        }
        charStatus = eCharacterStatus.waiting;
    }

    public void KnockBackToPos(Vector3Int position){
        faceDirection = false;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = gs.GridToWorldPosition(position);
        float distance = Vector3.Distance(startPosition, targetPosition);
        float timeToMove = distance / knockBackSpeed; // Time needed to reach target based on speed
        float elapsedTime = 0;

        // Gradually move the character towards the target position
        while (elapsedTime < timeToMove)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / timeToMove);
            elapsedTime += Time.deltaTime;
        }
        transform.position = targetPosition;
    }
    // Call this function to update reachable tiles and paths
    public void UpdateReachablePositions()
    {
        reachablePositions.Clear();
        reachablePositions = gs.GetReachablePositions(currPos, currMovementRange);
    }
}
