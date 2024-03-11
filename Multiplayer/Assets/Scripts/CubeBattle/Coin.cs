using UnityEngine;

public class Coin : MonoBehaviour {
    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.CompareTag("Player")){
            this.gameObject.SetActive(false);
        }
    }
}