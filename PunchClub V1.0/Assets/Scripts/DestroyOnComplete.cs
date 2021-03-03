using UnityEngine;

public class DestroyOnComplete : MonoBehaviour {

  public void DidComplete() {
    Destroy(gameObject);
  }
}
