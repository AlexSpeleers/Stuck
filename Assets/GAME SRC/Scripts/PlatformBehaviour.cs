using UnityEngine;

public class PlatformBehaviour : MonoBehaviour
{
    [SerializeField] private float tileSpeed = 20f;    
    private GameState gameState;    
    private Vector3 nextPos;       
    private bool isMovingX;
    private bool isDead = false;

    public void SetConfig(GameState state, Vector3 nextPos, bool isMovingX)
    {
        this.gameState = state;        
        this.isMovingX = isMovingX;
        this.transform.localPosition = (isMovingX) ? new Vector3(-nextPos.x, nextPos.y, nextPos.z) : new Vector3(nextPos.x, nextPos.y, -nextPos.z);
        this.nextPos = nextPos;
    }

    private void Update()
    {
        if (gameState.State == State.InGame && !isDead) 
        {    
            LoopMove();
        }
    }
    public void LoopMove()
    {
        if (nextPos == this.transform.localPosition)
            nextPos = (isMovingX) ? new Vector3(-this.nextPos.x, nextPos.y, this.nextPos.z) : new Vector3(this.nextPos.x, nextPos.y, -this.nextPos.z);
        this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, nextPos, tileSpeed * Time.deltaTime);
    }

    public Vector3 Place() 
    {        
        this.GetComponent<PlatformBehaviour>().enabled = false;
        return this.transform.localPosition;        
    }
}