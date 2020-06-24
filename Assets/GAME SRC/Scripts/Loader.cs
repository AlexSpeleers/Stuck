using UnityEngine;
using UnityEngine.UI;

public class Loader : MonoBehaviour
{
    [SerializeField] private GameObject defaultPodium;
    [SerializeField] private Text bestScore;

    private void Start()
    {
        RefreshScore();
        CreatePodium();
    }
    
    private void CreatePodium() 
    {
        var item = Instantiate(defaultPodium, this.transform.position, Quaternion.identity);
        item.transform.parent = this.transform;
        var yPos = item.transform.localScale.y / 2 + 0.5f;
        item.transform.localPosition = new Vector3(0f, -yPos, 0f);
    }

    public void RefreshScore() 
    {
        bestScore.text = PlayerPrefs.HasKey("maxScoreKey") ? PlayerPrefs.GetInt("maxScoreKey").ToString() : "";
    }
}