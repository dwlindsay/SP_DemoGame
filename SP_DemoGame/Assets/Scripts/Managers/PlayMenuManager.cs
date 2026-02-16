using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayMenuManager : MonoBehaviour
{
    public TMP_InputField hostCodeInput;
    public TMP_Text warningLabel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPlayPressed()
    {
        if( int.TryParse(hostCodeInput.text, out int hostCode) )
        {
            if (hostCode >= 0 && hostCode < 256)
            {
                // TODO: do something with hostCode
                Debug.Log("hostCode:"+hostCode.ToString());
                warningLabel.gameObject.SetActive(false);
                SceneManager.LoadScene("SPGamepadScene");
                return;
            }
        }
        
        warningLabel.gameObject.SetActive(true);
    }
}
