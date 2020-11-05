using System.Collections;
using UnityEngine;

public class InitChildrenAtStart : MonoBehaviour {
    protected virtual IEnumerator Start() {
        for (int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).gameObject.SetActive(true);
        }

        yield return null;
        for (int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}
