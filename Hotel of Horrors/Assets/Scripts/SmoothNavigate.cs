using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothNavigate : MonoBehaviour
{
    [SerializeField] Transform transformToMove;
    [SerializeField] Vector2 driftAmount = new Vector2(20, 20);

    Vector3 defaultPosition;

    // Start is called before the first frame update
    void Start()
    {
        if (transformToMove == null) transformToMove = transform;

        defaultPosition = transformToMove.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = Input.mousePosition;

        Vector2 centeredMousePos = mousePos - new Vector2(Screen.width / 2, Screen.height / 2);
        Vector2 normalizedMousePos = new Vector2(centeredMousePos.x / (Screen.width / 2), centeredMousePos.y / (Screen.height / 2));

        transformToMove.position = (Vector2)defaultPosition - new Vector2(driftAmount.x * normalizedMousePos.x, driftAmount.y * normalizedMousePos.y);
    }
}
