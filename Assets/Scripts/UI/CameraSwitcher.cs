using UnityEngine;

using System.Collections;

public class CameraSwitcher : MonoBehaviour
{
    [Header("Cele Kamery")]
    public Transform gameplayPosition; 
    public Transform gatherersPosition;

    [Header("Ustawienia")]
    public float transitionSpeed = 2.0f; // Jak szybko kamera ma się przesuwać

    private bool isAtGatherers = false; // Czy jesteśmy na dole?

    // Tę funkcję podepniesz pod przycisk
    public void ToggleCameraPosition()
    {
        isAtGatherers = !isAtGatherers; // Zmień stan na przeciwny

        if (isAtGatherers)
        {
            StopAllCoroutines(); // Zatrzymaj poprzedni ruch jeśli jakiś trwa
            StartCoroutine(MoveCamera(gatherersPosition));
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(MoveCamera(gameplayPosition));
        }
    }

    // Korutyna odpowiedzialna za płynny ruch
    IEnumerator MoveCamera(Transform target)
    {
        // Dopóki kamera nie jest "prawie" na miejscu
        while (Vector3.Distance(transform.position, target.position) > 0.01f)
        {
            // Płynne przesuwanie pozycji
            transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * transitionSpeed);
            
            // Opcjonalnie: Płynne obracanie (jeśli kamera ma też patrzeć w dół)
            transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, Time.deltaTime * transitionSpeed);

            yield return null; // Czekaj do następnej klatki
        }

        // Na koniec upewnij się, że pozycja jest idealna
        transform.position = target.position;
    }
}
