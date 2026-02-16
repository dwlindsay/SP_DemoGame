using UnityEngine;
using UnityEngine.UI;

namespace Complete
{
    public class TankMovement : MonoBehaviour
    {
        public int m_PlayerNumber = 1;              // Used to identify which tank belongs to which player.  This is set by this tank's manager.
        public float m_Speed = 12f;                 // How fast the tank moves forward and back.
        public float m_TurnSpeed = 180f;            // How fast the tank turns in degrees per second.
        public AudioSource m_MovementAudio;         // Reference to the audio source used to play engine sounds. NB: different to the shooting audio source.
        public AudioClip m_EngineIdling;            // Audio to play when the tank isn't moving.
        public AudioClip m_EngineDriving;           // Audio to play when the tank is moving.
        public float m_PitchRange = 0.2f;           // The amount by which the pitch of the engine noises can vary.
        public Slider m_Slider;
        public bool hitFlashing = false;
        public float flashTime = .2f;
        public int flashCount = 2;

        private string m_MovementAxisName;          // The name of the input axis for moving forward and back.
        private string m_TurnAxisName;              // The name of the input axis for turning.
        private Rigidbody m_Rigidbody;              // Reference used to move the tank.
        private float m_MovementInputValue;         // The current value of the movement input.
        private float m_TurnInputValue;             // The current value of the turn input.
        private float m_OriginalPitch;              // The pitch of the audio source at the start of the scene.

        //private bl_Joystick joystick;
        private float flashingTimer;
        private int flashingCounter;

        private void Awake ()
        {
            m_Rigidbody = GetComponent<Rigidbody> ();
        }


        private void OnEnable ()
        {
            // When the tank is turned on, make sure it's not kinematic.
            m_Rigidbody.isKinematic = false;

            // Also reset the input values.
            m_MovementInputValue = 0f;
            m_TurnInputValue = 0f;
        }


        private void OnDisable ()
        {
            // When the tank is turned off, set it to kinematic so it stops moving.
            m_Rigidbody.isKinematic = true;
        }


        private void Start ()
        {
            // The axes names are based on player number.
            m_MovementAxisName = "Vertical" + m_PlayerNumber;
            m_TurnAxisName = "Horizontal" + m_PlayerNumber;

            //// the joystick is based on the player number.
            //joystick = GameObject.Find("Joystick" + m_PlayerNumber).GetComponent<bl_Joystick>();

            // Store the original pitch of the audio source.
            m_OriginalPitch = m_MovementAudio.pitch;
        }


        private void Update ()
        {
            // Store the value of both input axes.
            //m_MovementInputValue = Input.GetAxis (m_MovementAxisName);
            //m_TurnInputValue = Input.GetAxis (m_TurnAxisName);
            //m_MovementInputValue = joystick.Vertical;
            //m_TurnInputValue = joystick.Horizontal;
            //m_MovementInputValue = Vector2.SqrMagnitude(new Vector2(joystick.Horizontal, joystick.Vertical));
            m_MovementInputValue = Vector2.SqrMagnitude(new Vector2(0, 0));
            if (m_MovementInputValue > 14f)
            {
                m_MovementInputValue = 14f;
            }
            //m_TurnInputValue = Mathf.Atan2(-joystick.Vertical, -joystick.Horizontal);
            m_TurnInputValue = Mathf.Atan2(-0, -0);

            EngineAudio ();

            UpdateHitFlash();
        }

        private void EngineAudio ()
        {
            // If there is no input (the tank is stationary)...
            if (Mathf.Abs (m_MovementInputValue) < 0.1f && Mathf.Abs (m_TurnInputValue) < 0.1f)
            {
                // ... and if the audio source is currently playing the driving clip...
                if (m_MovementAudio.clip == m_EngineDriving)
                {
                    // ... change the clip to idling and play it.
                    m_MovementAudio.clip = m_EngineIdling;
                    m_MovementAudio.pitch = Random.Range (m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                    m_MovementAudio.Play ();
                }
            }
            else
            {
                // Otherwise if the tank is moving and if the idling clip is currently playing...
                if (m_MovementAudio.clip == m_EngineIdling)
                {
                    // ... change the clip to driving and play.
                    m_MovementAudio.clip = m_EngineDriving;
                    m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                    m_MovementAudio.Play();
                }
            }
        }


        private void FixedUpdate ()
        {
            // Adjust the rigidbodies position and orientation in FixedUpdate.
            Move ();
            Turn ();
        }

        private void Move ()
        {
            // Create a vector in the direction the tank is facing with a magnitude based on the input, speed and the time between frames.
            Vector3 movement = m_MovementInputValue * m_Speed * Time.deltaTime * transform.forward;

            // Apply this movement to the rigidbody's position.
            m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
        }

        private void Turn ()
        {
            //// Determine the number of degrees to be turned based on the input, speed and time between frames.
            //float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;

            if (m_MovementInputValue > 0.1)
            {
                float angle = m_TurnInputValue * Mathf.Rad2Deg;
                angle += 45f;  // isometric

                Quaternion turnRotation = Quaternion.Lerp(m_Rigidbody.rotation, Quaternion.Euler(new Vector3(0, -angle, 0)), Time.deltaTime * m_TurnSpeed);

                m_Rigidbody.rotation = turnRotation;
            }

            //// Make this into a rotation in the y axis.
            //Quaternion turnRotation = Quaternion.Euler (0f, turn, 0f);

            //// Apply this rotation to the rigidbody's rotation.
            //m_Rigidbody.MoveRotation (m_Rigidbody.rotation * turnRotation);
        }

        public void HitFlash()
        {
            hitFlashing = true;
            flashingCounter = 0;
        }

        private void UpdateHitFlash()
        {
            if( hitFlashing )
            {
                if (flashingCounter == 0)
                {
                    m_Slider.value = 100f;
                    flashingCounter = 1;
                    flashingTimer = Time.time + flashTime;
                }
                else if (flashingCounter < flashCount)
                {
                    if (Time.time > flashingTimer)
                    {
                        if (m_Slider.value == 0f)
                        {
                            m_Slider.value = 100f;
                        }
                        else
                        {
                            m_Slider.value = 0f;
                        }
                        flashingCounter++;
                        flashingTimer += flashTime;
                    }
                }
                else
                {
                    hitFlashing = false;
                    flashingCounter = 0;
                    m_Slider.value = 0f;
                }
            }
        }
    }
}