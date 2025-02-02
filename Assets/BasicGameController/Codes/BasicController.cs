using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

//4PILLOWS DEV BASIC CONTROLLER
//Code by Daniel Porras - 2020 - www.4pillowsinteractive.co

public class BasicController : MonoBehaviour {

    public bool Pause; //This variable turns on or off the controller, you can use it for cinematics or stuff you require the player to stop.
    [Header("Controller Mode")]
    [Tooltip("0 = ThirdPerson / 1 = 2D / 2 = FirstPerson / 3 = Point & Click")]
    [Range(0,3)]
    public int ControllerMode;
    [Space]
    public bool UseCharacter = true;//Active if your Controller will use a Mesh or skinmesh has character
    [Space]
    [Header("Controller Values")]
    public float MoveSpeed = 7; //Speed used by Rigidbody when is running or walking
    public float RotationSpeed = 7;//Speed used by Rigidbody to look at the Mark or target Point.
    public LayerMask Terrain; //Any object with a collider and one of the layers you selected here will recognize as terrain
    public Rigidbody CharacterBody;//This Component is Required - This is the Rigidbody used in ThirdPerson, 2D and FirstPerson Controllers
    public float PlusGravity = 0.6f;//This float will add extra value in gravity to increase speed when falling, especially useful with Jump.
    public GameObject GroundReference;//This gameobject is the point that draws a ray to the ground and detects the distance to the objects on the Terrain Layers

    [HideInInspector]
    public GameObject cam; //This is the Camera used by Controller
    [HideInInspector]
    public GameObject CameraContainer;//This container rotates with the mouse in Third Person and First Person and follows the rigidbody in all modes, it contains the camera references used for camera position, also is used to know the direction that will be used by the player
    [HideInInspector]
    public GameObject Ref3d;//This reference is inside the camera Container and is used in Third Person Controller Mode for camera position
    [HideInInspector]
    public GameObject Ref2d;//This references is inside the camera Container and is used in the 2D mode for camera position
    [HideInInspector]
    public GameObject RefTopView; //This reference is inside the camera Container and is used in the Point&Click mode for camera position
    [HideInInspector]
    public GameObject FPSView;//This reference could be the head bone of the Character or could be a gameobject child of the BasicController Object that simulates Head Position.
    [HideInInspector]
    public float CameraSpeed = 5; //This is the speed used by the camera to follow rigidbody
    [HideInInspector]
    public float CameraRotationSpeed = 5; //this is the speed used by the camera to look at rigidbody

    private float yaw = 0.0f; //this is used to calculate camera Rotation on Y
    private float pitch = 0.0f; //This is used to calculate camera Rotation on X
    [HideInInspector]
    public float CamFloorLimit = -20; //This is used to limit the camera rotation on X when the camera is closest to the floor 
    [HideInInspector]
    public float CamFloorMax = 50;//This is used as the max angle used by the camera rotation on X


    [HideInInspector]
    public GameObject Character; //This is the gameobject used as Character
    [HideInInspector]
    public bool UseAnimation; //Active this if your character uses animations.
    [HideInInspector]
    public bool UseMecanim = true;//Active this if your character animations are in generic or humanoid mode. Turn off if your use a Legacy animation system.                                         
    [HideInInspector]
    public Animator MecanimAnimator;//Animator used if useMecanim is active. 
                                    //This script call 2 variables from the Animator Controller: Move (float) and IsGround (bool)
                                    //Move is relative to the SpeedMagnitude variable and change animations between Idle and Run
                                    //IsGround is relative to the isground variable used to detect if floor is touching or not by player.
    [HideInInspector]
    public Animation LegacyAnimation;//Animation component if useMecanim is not active
    [HideInInspector]
    public string IdleAnimation = "Idle"; //In the inspector put the name of your Idle animation when you use a legacy animation system
    [HideInInspector]
    public string MoveAnimation = "Move";//In the inspector put the name of your run or walk animation when you use a legacy animation system
    [HideInInspector]
    public string FallAnimation = "Fall";//In the inspector put the name of your Fall animation when you use a legacy animation system

    [HideInInspector]
    public NavMeshAgent Agent;//This agent is used in the Point&Click Mode, it requires a navigation baking on scene.

    float v;//This variable captures the Vertical axis from Input Manager.
    float h;//This variable captures the Horizontal axis from Input Manager.
    float mouseX;//This variable captures the Mouse X axis from Input Manager.
    float mouseY;//This variable captures the Mouse Y axis from Input Manager.

    [HideInInspector]
    public bool UseJump;//Active if you will use jump action on your controller
    [HideInInspector]
    public float JumpForce = 16; //Force used by the RigidBody to be impulse in the Y axis
    float JumpWaitTime;//This time is used to wait 1 second after player leaves ground before IsGround is false, it helps with the player jump
    bool JumpDone;//This is used to verified if player touch the ground after jump and also is not pressing the Jump button

    bool jump;//This variable captures the Jump button from Input Manager.
    bool isground;//This variable shows if player is touching the ground or not
    float groundDistance;//this calculates the distances between groundreference point and terrain
    [HideInInspector]
    public float minGroundDistance = 1;//this is the max distance to detecte terrain (isground = true)


    RaycastHit Hit;//This captures the info of ray used to detect distance between rigidbody and terrain

    Ray CamRay; //This creates a ray relative to mouse position 
    RaycastHit MouseHit; //this captures the collision point of the camRay (it depends of the Terrain layers)

    private Quaternion _lookRotationCamera;//This is used to calculate camera rotation
    private Vector3 _directionCamera;//This is used to calculate camera direction relative to Rigidbody

    Vector3 joystickMovement; //This capture the inputs h and v to generated speedMagnitute
    float speedMagnitude;
    float CharacterSpeed; //This is the speed used by the Character to takes the same position of Rigidbody
    float characterRotation;//This is the speed used by Character to takes the same rotation of rigidbody

    Vector3 Direccion; //This is used to calculate the character direction relative to RigidBody
    Vector3 Target; //This is the Rigidbody position
    Vector3 dir;
    Quaternion rotacion; //This is used to calculate the character rotation

    float DistanciaTarget = 5; //this is the max distance of routemark relative to rigidbody
    float CurrentTargetDistance; //this is the current distance betwwen rigidbody and routemark
    float minDistance = 0.5f; //this is the min distance betwwen rigidbody and routemark
    Vector3 RouteMark;//this is a vector that represents routemark
    Vector3 MoveLerp;//this calculates the move of rigidbody relative to routeMark

    float targetDistance; //this is the distance of navemesh agent with destination

    //this is used to increase agent rotation calculated if is rotating to left or right and adding extraRotationSpeed.
    Vector3 currentFacing;
    float currentAngularVelocity;
    Vector3 lastFacing;
    bool isRotating;
    float MaxRotationAngle = 45f;
    float rotationlowspeed = 0.4f;

    Vector3 oldEulerAngles;
    bool isRight;
    bool isleft;

    [HideInInspector]
    public float extraRotationSpeed = 6;

    //this mark shows the point create by the mouse on the point&click mode
    [HideInInspector]
    public GameObject NavMark;

    [HideInInspector]
    public bool UseFootStepsSound;//Active if you will use footSteps Sounds
    [HideInInspector]
    public AudioSource audioManager;//This plays the footsteps sounds
    [HideInInspector]
    public AudioClip FootStep; //this is one step sound
    [HideInInspector]
    public AudioClip FootStep2;//this is one step sound
    [HideInInspector]
    public float Volume = 1.5f; //this is the sound volume of steps
    [HideInInspector]
    public float maxMoveTime = 0.6f; //this is the duration of walk-run animation, you need to put this manually
    [HideInInspector]
    public float step1Time = 0.1f; //this is the time when the first foot touch the ground
    [HideInInspector]
    public float step2Time = 0.4f; //this is teh time when the second foot touch the ground
    float stepTime; //this is an internal timer that increase is value relative to deltaTime.
    bool isRFoot;//this is a bool is used to avoid multiple footsteps sounds

    [HideInInspector]
    public bool UseJumpSound; //Active if you will use a Jump Sound
    [HideInInspector]
    public AudioSource audioManagerJ; //This plaus the Jump sound
    [HideInInspector]
    public AudioClip JumpSound; //This is the jump sound
    [HideInInspector]
    public float JumpVolume = 1; //this is the sound volument of jump

    // Use this for initialization
    void Start () {
        DistanciaTarget = 5;
        minDistance = 1f;
        isRFoot = false;
    }
	
	// Update is called once per frame
	void Update () {

        if(ControllerMode > 3 || ControllerMode < 0)
        {
            ControllerMode = 0;
        }

        if (ControllerMode == 1)
        {
            CharacterBody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        }
        else
        {
            CharacterBody.constraints = RigidbodyConstraints.FreezeRotation;
        }

      
        if(ControllerMode != 3)
        {
            Agent.enabled = false;
            CharacterBody.isKinematic = false;
            NavMark.SetActive(false);
        }
        else
        {
            CharacterBody.isKinematic = true;
            Agent.enabled = true;
        }
  

        //ThirdPersonController
        if (ControllerMode == 0)
        {
            if (Pause == false)
            {
                h = Input.GetAxis("Horizontal");
                v = Input.GetAxis("Vertical");
                mouseX = Input.GetAxis("Mouse X");
                mouseY = Input.GetAxis("Mouse Y");
                jump = Input.GetButton("Jump");
            }
            else
            {
                h = 0;
                v = 0;
                mouseX = 0;
                mouseY = 0;
                jump = false;
                isground = true;
            }

            joystickMovement = new Vector3(h, v, 0);
            speedMagnitude = Mathf.Clamp(joystickMovement.magnitude, 0, 1);
            CharacterSpeed = MoveSpeed * speedMagnitude;
            characterRotation = RotationSpeed * speedMagnitude;

        
            
                Direccion = CameraContainer.transform.position + CameraContainer.transform.forward * DistanciaTarget * v + CameraContainer.transform.right * DistanciaTarget * h;
                RouteMark = new Vector3(Direccion.x, CameraContainer.transform.position.y, Direccion.z);

                CurrentTargetDistance = Vector3.Distance(CharacterBody.transform.position, RouteMark);

                Target = RouteMark - CharacterBody.transform.position;

                dir = new Vector3(Target.x, Target.y, Target.z);
                if (dir != Vector3.zero)
                {
                rotacion = Quaternion.Lerp(CharacterBody.transform.rotation, Quaternion.LookRotation(dir, Vector3.up), characterRotation * Time.deltaTime);
                }
          
                CharacterBody.transform.rotation = rotacion;
           
            if (speedMagnitude > 0.6f)
            {
                CharacterSpeed = MoveSpeed * speedMagnitude;
                characterRotation = RotationSpeed * speedMagnitude;
                if (UseCharacter)
                {
                    Character.transform.rotation = transform.rotation;
                }
            }       
            else
            {
                CharacterSpeed = 0;
                characterRotation = 0;
            }
            if (UseCharacter)
            {
                Character.transform.position = Vector3.Lerp(Character.transform.position, CharacterBody.transform.position, MoveSpeed * 2 * Time.deltaTime);


                if (UseAnimation)
                {
                    if (UseMecanim)
                    {
                        MecanimAnimator.SetFloat("Move", speedMagnitude);
                        MecanimAnimator.SetBool("IsGround", isground);
                    }
                    if (!UseMecanim)
                    {
                        if (isground)
                        {
                            if (speedMagnitude > 0.6f)
                            {
                                LegacyAnimation.CrossFade(MoveAnimation, 0.2f);
                            }
                            else
                            {
                                LegacyAnimation.CrossFade(IdleAnimation, 0.2f);
                            }
                        }
                        else
                        {
                            LegacyAnimation.CrossFade(FallAnimation, 0.2f);
                        }
                    }
                }
            }

            if (UseFootStepsSound)
            {
                if(speedMagnitude > 0.6f && isground)
                {
                    if (stepTime < maxMoveTime) {
                        stepTime += 1 * Time.deltaTime;
                            }

                    if(stepTime > step1Time && stepTime < step2Time)
                    {
                        if (isRFoot == false)
                        {
                           audioManager.PlayOneShot(FootStep, Volume);
              
                            isRFoot = true;
                        }
                    }

                    if (stepTime > step2Time && stepTime < maxMoveTime)
                    {
                        if (isRFoot == true)
                        {
                             audioManager.PlayOneShot(FootStep2, Volume);
                          
                            isRFoot = false;
                        }
                    }

                    if(stepTime > maxMoveTime)
                    {
                        stepTime = 0;
                    }
                }
                else
                {
                    stepTime = 0;
                    isRFoot = false;
                }
            }
        }




        //2D Controller
        if (ControllerMode == 1)
        {
        if (Pause == false) { 
            h = Input.GetAxis("Horizontal");
            jump = Input.GetButton("Jump");
            }
        else
        {
            h = 0;
            v = 0;
            mouseX = 0;
            mouseY = 0;
            jump = false;
            isground = true;
        }

            joystickMovement = new Vector3(h, 0, 0);
            speedMagnitude = Mathf.Clamp(joystickMovement.magnitude, 0, 1);
            CharacterSpeed = MoveSpeed * speedMagnitude;
            characterRotation = RotationSpeed * speedMagnitude;

        

                Direccion = CameraContainer.transform.position + CameraContainer.transform.right * DistanciaTarget * h;
                RouteMark = new Vector3(Direccion.x, CameraContainer.transform.position.y, Direccion.z);

                CurrentTargetDistance = Vector3.Distance(CharacterBody.transform.position, RouteMark);

                Target = RouteMark - CharacterBody.transform.position;

                dir = new Vector3(Target.x, Target.y, Target.z);
                if (dir != Vector3.zero)
                {
                rotacion =  Quaternion.LookRotation(dir, Vector3.up);
                }
          
                CharacterBody.transform.rotation = rotacion;
           
            if (speedMagnitude > 0.6f)
            {
                CharacterSpeed = MoveSpeed * speedMagnitude;
                characterRotation = RotationSpeed * speedMagnitude;
                if (UseCharacter)
                {
                    Character.transform.rotation = transform.rotation;
                }
            }       
            else
            {
                CharacterSpeed = 0;
                characterRotation = 0;
            }
            if (UseCharacter)
            {
                Character.transform.position = Vector3.Lerp(Character.transform.position, CharacterBody.transform.position, MoveSpeed * 2 * Time.deltaTime);
            }

            if (UseAnimation)
            {
                if (UseMecanim)
                {
                    MecanimAnimator.SetFloat("Move", speedMagnitude);
                    MecanimAnimator.SetBool("IsGround", isground);
                }
                if (!UseMecanim)
                {
                    if(speedMagnitude > 0.6f)
                    {
                        LegacyAnimation.CrossFade(MoveAnimation, 0.2f);
                    }
                    else
                    {
                        LegacyAnimation.CrossFade(IdleAnimation, 0.2f);
                    }
                }
            }

            if (UseFootStepsSound)
            {
                if (speedMagnitude > 0.6f && isground)
                {
                    if (stepTime < maxMoveTime)
                    {
                        stepTime += 1 * Time.deltaTime;
                    }

                    if (stepTime > step1Time && stepTime < step2Time)
                    {
                        if (isRFoot == false)
                        {
                            audioManager.PlayOneShot(FootStep, Volume);

                            isRFoot = true;
                        }
                    }

                    if (stepTime > step2Time && stepTime < maxMoveTime)
                    {
                        if (isRFoot == true)
                        {
                            audioManager.PlayOneShot(FootStep2, Volume);

                            isRFoot = false;
                        }
                    }

                    if (stepTime > maxMoveTime)
                    {
                        stepTime = 0;
                    }
                }
                else
                {
                    stepTime = 0;
                    isRFoot = false;
                }
            }
        }




        //First Person Controller
        if (ControllerMode == 2)
        {
            if(Pause == false) { 
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");
            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");
            jump = Input.GetButton("Jump");
            }
            else
            {
                h = 0;
                v = 0;
                mouseX = 0;
                mouseY = 0;
                jump = false;
                isground = true;
            }

            CameraContainer.transform.position = transform.position;
           
            joystickMovement = new Vector3(h, v, 0);
            speedMagnitude = Mathf.Clamp(joystickMovement.magnitude,0,1);
            CharacterSpeed = MoveSpeed * speedMagnitude;

            Direccion = CameraContainer.transform.position + CameraContainer.transform.forward * DistanciaTarget * v + CameraContainer.transform.right * DistanciaTarget * h;
            RouteMark = new Vector3(Direccion.x, CameraContainer.transform.position.y, Direccion.z);

            Target = RouteMark - CharacterBody.transform.position;
            dir = new Vector3(Target.x, Target.y, Target.z);
            if (dir != Vector3.zero)
            {
                rotacion = Quaternion.LookRotation(dir, Vector3.up);
            }
            CharacterBody.transform.rotation = rotacion;

            if (UseCharacter)
            {
                Character.transform.position = CharacterBody.transform.position;
                Character.transform.rotation = CameraContainer.transform.rotation;


                if (UseAnimation)
                {
                    if (UseMecanim)
                    {
                        MecanimAnimator.SetFloat("Move", speedMagnitude);
                        MecanimAnimator.SetBool("IsGround", isground);
                    }
                    if (!UseMecanim)
                    {
                        if (isground)
                        {
                            if (speedMagnitude > 0.6f)
                            {
                                LegacyAnimation.CrossFade(MoveAnimation, 0.2f);
                            }
                            else
                            {
                                LegacyAnimation.CrossFade(IdleAnimation, 0.2f);
                            }
                        }
                        else
                        {
                            LegacyAnimation.CrossFade(FallAnimation, 0.2f);
                        }
                    }

                }
            }

            if (UseFootStepsSound)
            {
                if (speedMagnitude > 0.6f && isground)
                {
                    if (stepTime < maxMoveTime)
                    {
                        stepTime += 1 * Time.deltaTime;
                    }

                    if (stepTime > step1Time && stepTime < step2Time)
                    {
                        if (isRFoot == false)
                        {
                            audioManager.PlayOneShot(FootStep, Volume);

                            isRFoot = true;
                        }
                    }

                    if (stepTime > step2Time && stepTime < maxMoveTime)
                    {
                        if (isRFoot == true)
                        {
                            audioManager.PlayOneShot(FootStep2, Volume);

                            isRFoot = false;
                        }
                    }

                    if (stepTime > maxMoveTime)
                    {
                        stepTime = 0;
                    }
                }
                else
                {
                    stepTime = 0;
                    isRFoot = false;
                }
            }
        }





        //Point & Click Controller
        if (ControllerMode == 3)
        {

            if(Pause == false) { 
            if (Input.GetKeyDown(KeyCode.Mouse0))//this work on Android too, for IOS you requires use touches
            {
                CamRay = cam.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                Physics.Raycast(CamRay, out MouseHit, 200, Terrain);
                Agent.destination = MouseHit.point;
                //Debug.DrawRay(CamRay.origin, MouseHit.point, Color.red); //Use Only for Debugging
                    
                NavMark.transform.position = MouseHit.point;
                NavMark.SetActive(true);

             }
              
            }
            else
            {
                h = 0;
                v = 0;
                mouseX = 0;
                mouseY = 0;
                jump = false;
                isground = true;
                Agent.destination = Agent.transform.position;
                NavMark.SetActive(false);
            }

            targetDistance = Vector3.Distance(Agent.transform.position, Agent.destination);

            if (isRotating == false)
            {
                oldEulerAngles = transform.rotation.eulerAngles;
            }
            if (isRotating)
            {
                if (oldEulerAngles.y > transform.rotation.eulerAngles.y)
                {
                    isRight = false;
                    isleft = true;
                }
                else
                {
                    isRight = true;
                    isleft = false;
                }
            }
            else
            {
                isRight = false;
                isleft = false;
            }

            currentFacing = transform.forward;
            currentAngularVelocity = Vector3.Angle(currentFacing, lastFacing) / Time.deltaTime; //degrees per second
            lastFacing = currentFacing;

            isRotating = currentAngularVelocity > MaxRotationAngle;

            if (isRotating)
            {
                if (isRight || isleft)
                {
                    extraRotation();
                }
            }




            if (targetDistance > 0.5f)
            {
                if (Agent.isOnOffMeshLink == false)
                {
                    Agent.speed = MoveSpeed;
                }
                else
                {
                    Agent.speed = MoveSpeed/2;
                }
                NavMark.SetActive(true);
            }
            else
            {
                Agent.speed = 0;
                NavMark.SetActive(false);
            }

            if (UseCharacter)
            {
                Character.transform.position = Agent.transform.position;
                Character.transform.rotation = Agent.transform.rotation;


                if (UseAnimation)
                {
                    if (UseMecanim)
                    {
                        MecanimAnimator.SetBool("IsGround", isground);
                        if (targetDistance > 1f)
                        {
                            MecanimAnimator.SetFloat("Move", 1);
                        }
                        else
                        {
                            MecanimAnimator.SetFloat("Move", 0);
                        }

                    }
                    if (!UseMecanim)
                    {
                        if (isground)
                        {
                            if (targetDistance > 1f)
                            {
                                LegacyAnimation.CrossFade(MoveAnimation, 0.2f);
                            }
                            else
                            {
                                LegacyAnimation.CrossFade(IdleAnimation, 0.2f);
                            }
                        }
                        else
                        {
                            LegacyAnimation.CrossFade(FallAnimation, 0.2f);
                        }
                    }
                }
            }


            if (UseFootStepsSound)
            {
                if (targetDistance > 1f && isground)
                {
                    if (stepTime < maxMoveTime)
                    {
                        stepTime += 1 * Time.deltaTime;
                    }

                    if (stepTime > step1Time && stepTime < step2Time)
                    {
                        if (isRFoot == false)
                        {
                            audioManager.PlayOneShot(FootStep, Volume);

                            isRFoot = true;
                        }
                    }

                    if (stepTime > step2Time && stepTime < maxMoveTime)
                    {
                        if (isRFoot == true)
                        {
                            audioManager.PlayOneShot(FootStep2, Volume);

                            isRFoot = false;
                        }
                    }

                    if (stepTime > maxMoveTime)
                    {
                        stepTime = 0;
                    }
                }
                else
                {
                    stepTime = 0;
                    isRFoot = false;
                }
            }

        }
    }

    void FixedUpdate()
    {
        if (Pause == false)
        {

            //Third Person Controller Physics
            if (ControllerMode == 0)
        {
            if (h != 0 && v != 0 || h == 0 && v != 0 || h != 0 && v == 0)
            {

                CharacterBody.MovePosition(CharacterBody.transform.position + CharacterBody.transform.forward * CharacterSpeed * Time.deltaTime);
            }

            CharacterBody.velocity = new Vector3(Mathf.Clamp(CharacterBody.velocity.x, -MoveSpeed, MoveSpeed), Mathf.Clamp(CharacterBody.velocity.y - PlusGravity, -20, 20), Mathf.Clamp(CharacterBody.velocity.z, -MoveSpeed, MoveSpeed));

            Physics.Raycast(GroundReference.transform.position, -Vector3.up, out Hit, 200, Terrain);
            groundDistance = Hit.distance;

            if (groundDistance < minGroundDistance)
            {
                isground = true;
            }
            else
            {
                isground = false;
            }

            if (UseJump)
            {
                    if (jump && JumpWaitTime < 1f)
                    {
                        if (JumpDone == false)
                        {
                            CharacterBody.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
                            if (UseJumpSound)
                            {
                                audioManagerJ.PlayOneShot(JumpSound, JumpVolume);
                            }
                            JumpDone = true;
                        }
                    }
                    if (isground == false)
                    {
                        if (JumpWaitTime < 1)
                        {
                            JumpWaitTime += 1 * Time.deltaTime;
                        }
                    }
                    if (isground)
                    {
                        if (jump == false)
                        {
                            JumpDone = false;
                        }
                        JumpWaitTime = 0;
                    }
                }

        }


            //2D Controller Physics
            if (ControllerMode == 1)
            {

                if (h != 0)
                {
                    CharacterBody.MovePosition(CharacterBody.transform.position + CharacterBody.transform.forward * CharacterSpeed * Time.deltaTime);

                }
                if (h == 0)
                {
                    CharacterBody.transform.eulerAngles = new Vector3(0, CharacterBody.transform.eulerAngles.y, 0);
                }
                CharacterBody.velocity = new Vector3(Mathf.Clamp(CharacterBody.velocity.x, -MoveSpeed, MoveSpeed), Mathf.Clamp(CharacterBody.velocity.y - PlusGravity, -20, 20), Mathf.Clamp(CharacterBody.velocity.z, -MoveSpeed, MoveSpeed));

                Physics.Raycast(GroundReference.transform.position, -Vector3.up, out Hit, 200, Terrain);
                groundDistance = Hit.distance;

                if (groundDistance < minGroundDistance)
                {
                    isground = true;
                }
                else
                {
                    isground = false;
                }

                if (UseJump)
                {
                    if (jump && JumpWaitTime < 1f)
                    {
                        if (JumpDone == false)
                        {
                            CharacterBody.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
                            if (UseJumpSound)
                            {
                                audioManagerJ.PlayOneShot(JumpSound, JumpVolume);
                            }
                            JumpDone = true;
                        }
                    }
                    if (isground == false)
                    {
                        if (JumpWaitTime < 1)
                        {
                            JumpWaitTime += 1 * Time.deltaTime;
                        }
                    }
                    if (isground)
                    {
                        if (jump == false)
                        {
                            JumpDone = false;
                        }
                        JumpWaitTime = 0;
                    }
                }
            }
        }



        //First Person Controller Physics
        if (ControllerMode == 2)
        {
            if (h != 0 && v != 0 || h == 0 && v != 0 || h != 0 && v == 0)
            {

                CharacterBody.MovePosition(CharacterBody.transform.position + CharacterBody.transform.forward * CharacterSpeed * Time.deltaTime);
            }

            CharacterBody.velocity = new Vector3(Mathf.Clamp(CharacterBody.velocity.x, -MoveSpeed, MoveSpeed), Mathf.Clamp(CharacterBody.velocity.y - PlusGravity, -20, 20), Mathf.Clamp(CharacterBody.velocity.z, -MoveSpeed, MoveSpeed));

            Physics.Raycast(GroundReference.transform.position, -Vector3.up, out Hit, 200, Terrain);
            groundDistance = Hit.distance;

            if (groundDistance < minGroundDistance)
            {
                isground = true;
            }
            else
            {
                isground = false;
            }

            if (UseJump)
            {
                if (jump && JumpWaitTime < 1f)
                {
                    if (JumpDone == false)
                    {
                        CharacterBody.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
                        if (UseJumpSound)
                        {
                            audioManagerJ.PlayOneShot(JumpSound, JumpVolume);
                        }
                        JumpDone = true;
                    }
                }
                if (isground == false)
                {
                    if (JumpWaitTime < 1)
                    {
                        JumpWaitTime += 1 * Time.deltaTime;
                    }
                }
                if (isground)
                {
                    if (jump == false)
                    {
                        JumpDone = false;
                    }
                    JumpWaitTime = 0;
                }
            }
        }

        //Point & Click Controller Physics
        if (ControllerMode == 3)
        {
            Physics.Raycast(GroundReference.transform.position, -Vector3.up, out Hit, 200, Terrain);
            groundDistance = Hit.distance;

            if (groundDistance < minGroundDistance)
            {
                isground = true;
            }
            else
            {
                isground = false;
            }
        }

     
    }

    private void LateUpdate()
    {

        //Third Person Controller Camera
        if (ControllerMode == 0)
        {

            CameraContainer.transform.position = transform.position;
            cam.transform.position = Vector3.Lerp(cam.transform.position, Ref3d.transform.position, CameraSpeed * Time.deltaTime);

            _directionCamera = new Vector3(transform.position.x, transform.position.y+1, transform.position.z);
            _lookRotationCamera = Quaternion.LookRotation(_directionCamera - cam.transform.position, new Vector3(0, 1, 0));
            cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, _lookRotationCamera, 10 * Time.deltaTime);

            yaw += CameraRotationSpeed * mouseX;
            pitch -= CameraRotationSpeed * mouseY;
            pitch = Mathf.Clamp(pitch, CamFloorLimit, CamFloorMax);

            CameraContainer.transform.eulerAngles = new Vector3(pitch, yaw, CameraContainer.transform.eulerAngles.z);
        }

        //2D Controller Camera
        if (ControllerMode == 1)
        {
            CameraContainer.transform.position = transform.position;
            CameraContainer.transform.eulerAngles = new Vector3(0,0,0);
            cam.transform.position = Vector3.Lerp(cam.transform.position, Ref2d.transform.position, CameraSpeed * Time.deltaTime);
            cam.transform.rotation = Ref2d.transform.rotation;
   
        }

        //First Person Controller Camera
        if (ControllerMode == 2)
        {
       
            cam.transform.position = FPSView.transform.position;

            yaw += CameraRotationSpeed * mouseX;
            pitch -= CameraRotationSpeed * mouseY;
            pitch = Mathf.Clamp(pitch, CamFloorLimit, CamFloorMax);

            cam.transform.eulerAngles = new Vector3(pitch, yaw, cam.transform.eulerAngles.z);
            CameraContainer.transform.eulerAngles = new Vector3(CameraContainer.transform.eulerAngles.x, yaw, CameraContainer.transform.eulerAngles.z);
        }

         //Point & Click Controller Camera
        if(ControllerMode == 3)
        {
            CameraContainer.transform.position = transform.position;
            CameraContainer.transform.eulerAngles = new Vector3(0, 0, 0);

            cam.transform.position = Vector3.Lerp(cam.transform.position, RefTopView.transform.position, CameraSpeed * Time.deltaTime);
            _directionCamera = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
            _lookRotationCamera = Quaternion.LookRotation(_directionCamera - cam.transform.position, new Vector3(0, 1, 0));
            cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, _lookRotationCamera, 10 * Time.deltaTime);
        }
    }

    
    //This adds an extra rotation on navmesh agent
    void extraRotation()
    {
        Vector3 lookrotation = Agent.steeringTarget - transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookrotation), extraRotationSpeed * Time.deltaTime);

    }


    //This is used for custom editor GUI
#if UNITY_EDITOR
    [CustomEditor(typeof(BasicController))]
    public class RandomScript_Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField(" ", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("4PillowsDev Basic Controller", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(" ", EditorStyles.boldLabel);



            DrawDefaultInspector(); // for other non-HideInInspector fields

            BasicController script = (BasicController)target;

            if (script.ControllerMode == 3)
            {
                EditorGUILayout.LabelField(" ", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("THIS MODE REQUIRES A NAVIGATION BAKING", EditorStyles.boldLabel);
                EditorGUILayout.LabelField(" ", EditorStyles.boldLabel);
            }

                if (script.ControllerMode == 0)
            {
                EditorGUILayout.LabelField(" ", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("ThirdPerson Controller", EditorStyles.boldLabel);
                EditorGUILayout.LabelField(" ", EditorStyles.boldLabel);
            }

            if (script.ControllerMode == 1)
            {
                EditorGUILayout.LabelField(" ", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("2D Controller", EditorStyles.boldLabel);
                EditorGUILayout.LabelField(" ", EditorStyles.boldLabel);
            }

            if (script.ControllerMode == 2)
            {
                EditorGUILayout.LabelField(" ", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("FirstPerson Controller", EditorStyles.boldLabel);
                EditorGUILayout.LabelField(" ", EditorStyles.boldLabel);
            }

            if (script.ControllerMode == 3)
            {
                EditorGUILayout.LabelField(" ", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Point&Click Controller", EditorStyles.boldLabel);
                EditorGUILayout.LabelField(" ", EditorStyles.boldLabel);
            }

            if (script.ControllerMode == 3)
            {
                EditorGUILayout.LabelField(" ", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("NavMeshAgent", EditorStyles.boldLabel);
                script.Agent = EditorGUILayout.ObjectField("Agent", script.Agent, typeof(NavMeshAgent), true) as NavMeshAgent;
                script.extraRotationSpeed = EditorGUILayout.FloatField("Extra Rotation", script.extraRotationSpeed);
                script.NavMark = EditorGUILayout.ObjectField("Navigation Mark", script.NavMark, typeof(GameObject), true) as GameObject;

            }

            if (script.ControllerMode != 2)
            {
                EditorGUILayout.LabelField(" ", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Camera", EditorStyles.boldLabel);
                script.cam = EditorGUILayout.ObjectField("Camera", script.cam, typeof(GameObject), true) as GameObject;
                script.CameraContainer = EditorGUILayout.ObjectField("Camera Container", script.CameraContainer, typeof(GameObject), true) as GameObject;
                script.CameraSpeed = EditorGUILayout.FloatField("Cam Move Speed", script.CameraSpeed);
                script.CameraRotationSpeed = EditorGUILayout.FloatField("Cam Rotation Speed", script.CameraRotationSpeed);
                if (script.ControllerMode == 0)
                {
                    script.Ref3d = EditorGUILayout.ObjectField("Cam Position 3D Reference", script.Ref3d, typeof(GameObject), true) as GameObject;
                    script.CamFloorLimit = EditorGUILayout.FloatField("CamFloorLimit", script.CamFloorLimit);
                    script.CamFloorMax = EditorGUILayout.FloatField("CamFloorMax", script.CamFloorMax);
                }
                if (script.ControllerMode == 1)
                {
                    script.Ref2d = EditorGUILayout.ObjectField("Cam Position 2D Reference", script.Ref2d, typeof(GameObject), true) as GameObject;
                }
                
                if (script.ControllerMode == 3)
                {
                    script.RefTopView = EditorGUILayout.ObjectField("Top view Camera Reference", script.RefTopView, typeof(GameObject), true) as GameObject;

                 }
            }
            if (script.ControllerMode == 2)
            {
                script.FPSView = EditorGUILayout.ObjectField("First Person Camera Reference", script.FPSView, typeof(GameObject), true) as GameObject;
                script.CamFloorLimit = EditorGUILayout.FloatField("CamFloorLimit", script.CamFloorLimit);
                script.CamFloorMax = EditorGUILayout.FloatField("CamFloorMax", script.CamFloorMax);
            }

            if (script.UseCharacter) // if bool is true, show other fields
            {
                EditorGUILayout.LabelField(" ", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Character", EditorStyles.boldLabel);
                script.Character = EditorGUILayout.ObjectField("Character", script.Character, typeof(GameObject), true) as GameObject;
                script.UseAnimation = EditorGUILayout.Toggle("Use Animation?", script.UseAnimation);
                if (script.UseAnimation)
                {
                    script.UseMecanim = EditorGUILayout.Toggle("Use Mecanim?", script.UseMecanim);
                    if (script.UseMecanim)
                    {
                        script.MecanimAnimator = EditorGUILayout.ObjectField("Animator Component", script.MecanimAnimator, typeof(Animator), true) as Animator;

                    }
                    else
                    {
                        script.LegacyAnimation = EditorGUILayout.ObjectField("Animation Component", script.LegacyAnimation, typeof(Animation), true) as Animation;
                        script.IdleAnimation = EditorGUILayout.TextField("Idle Animation", script.IdleAnimation);
                        script.MoveAnimation = EditorGUILayout.TextField("Move Animation", script.MoveAnimation);
                        script.FallAnimation = EditorGUILayout.TextField("Fall Animation", script.FallAnimation);
                    }
                   
                }
                script.UseFootStepsSound = EditorGUILayout.Toggle("Use FootSteps Sounds?", script.UseFootStepsSound);
                if (script.UseFootStepsSound)
                {
                    script.audioManager = EditorGUILayout.ObjectField("FootStep AudioSource", script.audioManager, typeof(AudioSource), true) as AudioSource;
                    script.FootStep = EditorGUILayout.ObjectField("FootStep sound 1", script.FootStep, typeof(AudioClip), true) as AudioClip;
                    script.FootStep2 = EditorGUILayout.ObjectField("FootStep sound 2", script.FootStep2, typeof(AudioClip), true) as AudioClip;

                    script.Volume= EditorGUILayout.FloatField("FootStep Volume", script.Volume);
                    script.maxMoveTime = EditorGUILayout.FloatField("Max Move Time (sec)", script.maxMoveTime);
                    script.step1Time = EditorGUILayout.FloatField("Step 1 Time (sec)", script.step1Time);
                    script.step2Time = EditorGUILayout.FloatField("Step 2 Time (sec)", script.step2Time);
                }
            }
            if (script.ControllerMode != 3)
            {
                EditorGUILayout.LabelField(" ", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Extra", EditorStyles.boldLabel);
                EditorGUILayout.LabelField(" ", EditorStyles.boldLabel);
                script.UseJump = EditorGUILayout.Toggle("Use Jump?", script.UseJump);
                if(script.UseJump == true)
                {
                    script.JumpForce = EditorGUILayout.FloatField("Jump Force", script.JumpForce);
                    script.minGroundDistance = EditorGUILayout.FloatField("minGroundDistance", script.minGroundDistance);
                    script.UseJumpSound = EditorGUILayout.Toggle("Use jump sound?", script.UseJumpSound);
                    if (script.UseJumpSound)
                    {
                        script.audioManagerJ = EditorGUILayout.ObjectField("Jump AudioSource", script.audioManagerJ, typeof(AudioSource), true) as AudioSource;
                        script.JumpSound = EditorGUILayout.ObjectField("Jump Sound", script.JumpSound, typeof(AudioClip), true) as AudioClip;
                        script.JumpVolume = EditorGUILayout.FloatField("Jump Volume", script.JumpVolume);
                    }

                }
            }
        }
    }
#endif
}
