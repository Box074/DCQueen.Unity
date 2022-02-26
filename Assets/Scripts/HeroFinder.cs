
using UnityEngine;

class HeroFinder : MonoBehaviour
{
    public QueenController queen;
    public GameObject BGM0;
    public GameObject BGM1;
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.transform.root.name == "Knight")
        {
            queen.target = other.gameObject;
            gameObject.SetActive(false);
            BGM0.SetActive(false);
            BGM1.SetActive(true);
        }
    }
}
