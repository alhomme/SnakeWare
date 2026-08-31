using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MG_BarManager : MonoBehaviour
{
    [SerializeField] private RSO_Source mSourceRSO;
    [SerializeField] private RSO_MG_Result mResultRSO;

    private IEnumerator mCoroutine;

    private void OnEnable()
    {
        mSourceRSO.Value = GameSource.MiniGame;

        mCoroutine = WaitCoroutine();
        StartCoroutine(mCoroutine);
    }

    private IEnumerator WaitCoroutine()
    {
        yield return new WaitForSeconds(2);
        mResultRSO.Value = MG_Result.Success;
        SceneManager.LoadScene(1);
    }
}
