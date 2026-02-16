using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Complete
{
    public class GameManager : MonoBehaviour
    {
        public float totalRoundTime = 120f;         // how long a round should last
        public float m_EndOfRoundCountdownDelay = 1f;             // End of round countdown delay between numbers counting
        public CameraControl m_CameraControl;       // Reference to the CameraControl script for control during different phases.
        public Text m_MessageText;                  // Reference to the overlay Text to display winning text, etc.
        public GameObject m_TankPrefab;             // Reference to the prefab the players will control.
        public TankManager[] m_Tanks;               // A collection of managers for enabling and disabling different aspects of the tanks.
        public GameObject joystick1;
        public GameObject joystick2;
        public GameObject joystick3;
        public GameObject joystick4;

        private int m_RoundNumber;                  // Which round the game is currently on.
        private int roundEndCountdown = 4;             // "3, 2, 1, FINISH!" right before the round ends
        private WaitForSeconds m_EndWait;           // Used to have a delay whilst the round ends.
        private TankManager m_RoundWinner;          // Reference to the winner of the current round.  Used to make an announcement of who won.
        private TankManager m_GameWinner;           // Reference to the winner of the game.  Used to make an announcement of who won.
        private int numPlayers;
        private GameObject gameSettings;

        private void Start()
        {
            // Create the delays so they only have to be made once.
            m_EndWait = new WaitForSeconds (m_EndOfRoundCountdownDelay);
            gameSettings = GameObject.Find("GameSettings");
            if( gameSettings != null )
            {
                numPlayers = gameSettings.GetComponent<GameSettings>().numPlayers;
                m_Tanks = m_Tanks[0..numPlayers];
                if( numPlayers < 4 )
                {
                    joystick4.SetActive(false);
                }
                if (numPlayers < 3)
                {
                    joystick3.SetActive(false);
                }
            }
            SpawnAllTanks();
            SetCameraTargets();
            m_MessageText.text = "";

            // Once the tanks have been created and the camera is using them as targets, start the game!
            StartCoroutine(GameLoop());
        }


        private void SpawnAllTanks()
        {
            // For all the tanks...
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                // ... create them, set their player number and references needed for control.
                m_Tanks[i].m_Instance =
                    Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
                m_Tanks[i].m_PlayerNumber = i + 1;
                m_Tanks[i].Setup();
            }
        }


        private void SetCameraTargets()
        {
            // Create a collection of transforms the same size as the number of tanks.
            Transform[] targets = new Transform[m_Tanks.Length];

            // For each of these transforms...
            for (int i = 0; i < targets.Length; i++)
            {
                // ... set it to the appropriate tank transform.
                targets[i] = m_Tanks[i].m_Instance.transform;
            }

            // These are the targets the camera should follow.
            m_CameraControl.m_Targets = targets;
        }


        // This is called from start and will run each phase of the game one after another.
        private IEnumerator GameLoop()
        {
            // Once the 'RoundStarting' coroutine is finished, run the 'RoundPlaying' coroutine but don't return until it's finished.
            yield return StartCoroutine(RoundPlaying());

            // Once execution has returned here, run RoundEnd
            RoundEnd();
        }


        private IEnumerator RoundPlaying ()
        {
            // As soon as the round begins playing let the players control the tanks.
            EnableTankControl ();

            // Clear the text from the screen.
            m_MessageText.text = string.Empty;

            yield return new WaitForSeconds(totalRoundTime - m_EndOfRoundCountdownDelay*roundEndCountdown);
            while (--roundEndCountdown > 0)
            {
                m_MessageText.text = roundEndCountdown.ToString();
                yield return new WaitForSeconds(m_EndOfRoundCountdownDelay);
            }

            // Stop tanks from moving.
            DisableTankControl();
            m_MessageText.text = "FINISH!";
            // wait for the final delay
            yield return new WaitForSeconds(m_EndOfRoundCountdownDelay);
        }


        public void RoundEnd()
        {
            if (gameSettings == null)
            {
                // just return to title screen
                SceneManager.LoadScene("MainMenu");
                return;
            }

            // save off scores
            string message = EndMessage();
            GameSettings g = gameSettings.GetComponent<GameSettings>();
            g.scoreText = message;
            // return to appropriate screen
            Debug.Log("Round End, homescene:"+g.homeSceneName);
            SceneManager.LoadScene(g.homeSceneName);
        }


        // Returns a string message to display at the end of each round.
        private string EndMessage()
        {
            string message = "";

            // Go through all the tanks and add each of their scores to the message.
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                message += m_Tanks[i].m_ColoredPlayerText + " : " + m_Tanks[i].m_Wins + "\n";
            }

            return message;
        }


        private void EnableTankControl()
        {
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                m_Tanks[i].EnableControl();
            }
        }


        private void DisableTankControl()
        {
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                m_Tanks[i].DisableControl();
            }
        }
    }
}