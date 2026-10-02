using UnityEngine;
using System.Collections;

public class Dice : MonoBehaviour
{
    private Rigidbody rb;
    private bool isRolling = false;
    public int diceResult = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
   public IEnumerator RollDice()
   {
        isRolling  = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        float randomForce = Random.Range(3f, 6f);
        rb.AddForce(Vector3.up * randomForce, ForceMode.Impulse);

        rb.AddTorque(new Vector3(
            Random.Range(-1f,1f),
            Random.Range(-1f,1f),
            Random.Range(-1f,1f)
        ) * 6f, ForceMode.Impulse);

         while (!rb.IsSleeping())
        {
            yield return new WaitForSeconds(0.1f);
        }
    
        yield return new WaitForSeconds(0.2f);
        
        CalculateResult();
        
    }

    void CalculateResult() 
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, Vector3.up, out hit, 2f);
        
        diceResult = int.Parse(hit.collider.name.Substring(4));
        
        isRolling = false;
    }

    public bool IsRolling()
    {
    return isRolling;
    }
}