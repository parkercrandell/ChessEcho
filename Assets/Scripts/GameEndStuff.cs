using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEndStuff : MonoBehaviour
{
    [SerializeField] private GameObject youWin;
    [SerializeField] private GameObject youLose;
    [SerializeField] private GameObject resetText;
    [SerializeField] private ParticleSystem winEffect;

    public IEnumerator WinRoutine()
    {
        GameController.instance.audioController.PlayWin();
        yield return ScaleObjectRoutine(youWin.transform, Vector3.zero, Vector3.one, 0.3f, false, true);
        winEffect.Play();
    }

    public IEnumerator LoseRoutine()
    {
        yield return ScaleObjectRoutine(youLose.transform, Vector3.zero, Vector3.one, 0.6f, false, true);
    }

    public IEnumerator ShowResetText()
    {
        yield return ScaleObjectRoutine(resetText.transform, Vector3.zero, Vector3.one, 0.3f, false, true);
    }

    private IEnumerator ScaleObjectRoutine(Transform t, Vector3 posStart, Vector3 posEnd, float animationTime, bool deactivateAtEnd = false, bool activateAtStart = false)
    {
        float timer = 0;
        if (activateAtStart)
        {
            t.gameObject.SetActive(true);
        }

        while (timer <= animationTime)
        {
            yield return timer += Time.deltaTime;
            t.localScale = Vector3.Lerp(posStart, posEnd, timer / animationTime);
        }

        if (deactivateAtEnd)
        {
            t.gameObject.SetActive(false);
        }
    }
}
