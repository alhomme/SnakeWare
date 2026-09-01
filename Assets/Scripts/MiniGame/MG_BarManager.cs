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
        yield return new WaitForSeconds(1);
        mResultRSO.Value = MG_Result.Success;
        //mResultRSO.Value = MG_Result.Fail;
        SceneManager.LoadScene(1);
    }
}
