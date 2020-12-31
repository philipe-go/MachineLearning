using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

/// <summary> A Hummingbird Neural Network ML Agent </summary>
public class HummingbirdAgent : Agent
{
    [Tooltip("Force to apply when moving")]
    public float MoveForce = 2f;

    [Tooltip("Speed to pitch up/down")]
    public float PitchSpeed = 100f;

    [Tooltip("Speed to rotate around up axis")]
    public float YawSpeed = 100f;

    [Tooltip("Transform at the tip of the beak")]
    public Transform BeakTip;

    [Tooltip("Agent's camera")]
    public Camera AgentCamera;

    [Tooltip("Is it in training or gameplay mode")]
    public bool IsTrainingMode;

    //Agent's rigid body
    private Rigidbody _rigidbody;

    //Flower where the agent is in
    private FlowerArea _flowerArea;

    //Nearest flower
    private Flower _nearestFlower;

    //Allows for smoothers pitch changes
    private float _smoothPitchChange = 0f;

    //Allows for smoothers yaws changes
    private float _smoothYawChange = 0f;

    //Maximum angle that the bird can pitch up or down
    private const float MAX_PITCH_ANGLE = 80f;

    //Maximum distance from the beak tip to accept nectar collision
    private const float BEAK_TIP_RADIUS = 0.008f;

    //Whether the agent is frozen, not flying
    private bool _frozen = false;

    //Amount of nectar the agent had obtained
    public float NectarObtained { get; private set; }

    /// <summary> Initialize the agent </summary>
    public override void Initialize()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _flowerArea = GetComponentInParent<FlowerArea>();

        //if not in training mode no next step
        if (!IsTrainingMode)
        {
            MaxStep = 0;
        }
    }

    /// <summary> Reset the agent when the episode begins </summary>
    public override void OnEpisodeBegin()
    {
        if (IsTrainingMode)
        {
            //Only reset the flowers in training when there is one agent per area
            _flowerArea.ResetFlowers();
        }

        //Reset nectar obtained
        NectarObtained = 0;

        //Zero all velocities
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        //Default to spawning in front of flower
        bool inFrontOfFlower = true;
        if (IsTrainingMode)
        {
            //Spawn in front of flower 50% of time
            inFrontOfFlower = UnityEngine.Random.value > .5f;
        }

        //Move the agent to a new random position
        MoveToSafeRandomPosition(inFrontOfFlower);

        //Recalculate the nearest flower
        UpdateNearestFlower();
    }

    /// <summary> Called when an action is received from the player INPUT or Neural Network AI 
    /// VectorAction[i] represents:
    /// Index 0: move vector x (+1 = right, -1 = left)
    /// Index 1: move vector y (+1 = up, -1 = down)
    /// Index 2: move vector z (+1 = fwd, -1 = backwards)
    /// Index 3: pitch angle (+1 = pitch up, -1 = pitch down)
    /// Index 4: yaw angle (+1 = turn right, -1 = turn left)
    ///</summary>
    /// <param name='vectorAction'> Actions to take </param>
    public override void OnActionReceived(float[] vectorAction)
    {
        //Don't take actions if frozen
        if (_frozen) return;

        //Not frozen then calculate movement vector
        Vector3 move = new Vector3(vectorAction[0], vectorAction[1], vectorAction[2]);

        //Add force in the direction of the move vector
        _rigidbody.AddForce(move * MoveForce);

        //ROTATE AGENT
        //Get current rotation
        Vector3 rotationVector = transform.rotation.eulerAngles;
        //Calculate Pitch and Yaw
        float pitchChange = vectorAction[3];
        float yawChange = vectorAction[4];
        //Calculate smooth rotation changes
        _smoothPitchChange = Mathf.MoveTowards(_smoothPitchChange, pitchChange, 2f * Time.fixedDeltaTime);
        _smoothYawChange = Mathf.MoveTowards(_smoothYawChange, yawChange, 2f * Time.fixedDeltaTime);
        //Calculate new pitch and Yaw based on new values
        float pitch = rotationVector.x + _smoothPitchChange * Time.fixedDeltaTime * PitchSpeed;
        if (pitch > 180f) pitch -= 360f;
        pitch = Mathf.Clamp(pitch, -MAX_PITCH_ANGLE, MAX_PITCH_ANGLE);

        float yaw = rotationVector.y + _smoothYawChange * Time.fixedDeltaTime * YawSpeed;

        //Apply new rotation
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

//======>>> IMPORTANT
    /// <summary> Collect vector observations from the environment </summary>
    /// <param name='sensor'> </param>
    public override void CollectObservations(VectorSensor sensor)
    {
        //If nearestFlower is null observe an empty array and return early
        if (_nearestFlower == null)
        {
            sensor.AddObservation(new float[10]);
            return;
        }

        //Observe agent locals rotation (4 observations)
        sensor.AddObservation(transform.localRotation.normalized);

        //Get a vector from the beak tip to the neares flower
        Vector3 toFlower = _nearestFlower.FlowerCenterPosition - BeakTip.position;

        //Observe a normalized vector pointing to the nearest flower (3 observations)
        sensor.AddObservation(toFlower.normalized);

        //Observe a dotproduct that indicates whether the beaktip is in front of a flower
        //(+1 means the beak is directly in front, -1 is directly behind)
        sensor.AddObservation(Vector3.Dot(toFlower.normalized, -_nearestFlower.FlowerUpVector.normalized));

        //Observe a dotproduct that indicates whether the beaktip is in pointint to a flower
        //(+1 means the beak is directly pointing, -1 is directly reversed)
        sensor.AddObservation(Vector3.Dot(BeakTip.forward.normalized, -_nearestFlower.FlowerUpVector.normalized));

        //Observe the relative distance from the beak tip to the flower
        sensor.AddObservation(toFlower.magnitude / FlowerArea.AREA_DIAMETER);
    }

    /// <summary> When behaviour type is set to heuristic only in the Agent paramenters 
    /// <see cref="OnActionReceived(float[])"/> if using neural network
    /// </summary>
    /// <param name='actionsOut'> output actions array </param>
    public override void Heuristic(float[] actionsOut)
    {
        //Place holders for the movements
        Vector3 forward = Vector3.zero;
        Vector3 left = Vector3.zero;
        Vector3 up = Vector3.zero;
        float pitch = 0f;
        float yaw = 0f;

        //Convert keyboard inputs to movements
        //As it is keyboard values will be either 1, 0 or -1
        /*
        if (Input.GetKey(KeyCode.W)) forward = transform.forward;
        else if (Input.GetKey(KeyCode.S)) forward = -transform.forward;

        if(Input.GetKey(KeyCode.A)) left = -transform.right;
        else if (Input.GetKey(KeyCode.D)) left = transform.right;

        if (Input.GetKey(KeyCode.E)) up = transform.up;
        else if (Input.GetKey(KeyCode.C)) up = -transform.up;

        if(Input.GetKey(KeyCode.UpArrow)) pitch = 1f;
        else if (Input.GetKey(KeyCode.DownArrow)) pitch = -1;

        if(Input.GetKey(KeyCode.LeftArrow)) yaw = -1f;
        else if (Input.GetKey(KeyCode.RightArrow)) yaw = 1;

        //Combine the movement vectors and normalize
        Vector3 combined = (forward + left + up).normalized;
        */
        ///*
        // Forward/backward
        forward = Input.GetAxis("Vertical") * transform.forward;
        left = Input.GetAxis("Horizontal") * transform.right;

        // Up/down
        if (Input.GetKey(KeyCode.E)) up = transform.up;
        else if (Input.GetKey(KeyCode.Q)) up = -transform.up;

        // Pitch and way
        pitch = 1.2f * -Input.GetAxis("Mouse Y");
        yaw = 1.2f * Input.GetAxis("Mouse X");

        // Combine the movement vectors and normalize
        Vector3 combined = forward + left + up;
        combined = combined.sqrMagnitude > 1.0f ? combined.normalized : combined;
        //*/

        //Add 3 movements values, pitch and yaw
        actionsOut[0] = combined.x;
        actionsOut[1] = combined.y;
        actionsOut[2] = combined.z;
        actionsOut[3] = pitch;
        actionsOut[4] = yaw;
    }

    /// <summary> Prevent agent from moving and taking actions </summary>
    public void FreezeAgent()
    {
        Debug.Assert(IsTrainingMode == false, "Freeze/Unfreeze not supported in training");

        _frozen = true;
        _rigidbody.Sleep();
    }

    /// <summary> Resume agent to moving and taking actions </summary>
    public void UnFreezeAgent()
    {
        Debug.Assert(IsTrainingMode == false, "Freeze/Unfreeze not supported in training");

        _frozen = false;
        _rigidbody.WakeUp();
    }

    /// <summary> Move the agent to a safe random position </summary>
    /// <param name='inFrontOfFlower'> 50% of time to chose a spot in front of flower </param>
    private void MoveToSafeRandomPosition(bool inFrontOfFlower)
    {
        bool safePositionFound = false;
        int attemptsRemaining = 100; //prevent to keep trying forever to place in safe position
        Vector3 potentialPosition = Vector3.zero;
        Quaternion potentialRotation = new Quaternion();

        //Loop to place the hummingbird until the number of attempts
        while (!safePositionFound && attemptsRemaining > 0)
        {
            attemptsRemaining--;
            if (inFrontOfFlower)
            {
                //Pick a random flower
                Flower randomFlower = _flowerArea.Flowers[UnityEngine.Random.Range(0, _flowerArea.Flowers.Count)];

                //Position 10 to 20cm in front of a flower
                float distanceFromFlower = UnityEngine.Random.Range(.1f, .2f);
                potentialPosition = randomFlower.transform.position + randomFlower.FlowerUpVector * distanceFromFlower;

                //Point beak at flower (bird's head is center of transform)
                Vector3 toFlower = randomFlower.FlowerCenterPosition - potentialPosition;
                potentialRotation = Quaternion.LookRotation(toFlower, Vector3.up);
            }
            else
            {
                //position at a random height from the ground
                float height = UnityEngine.Random.Range(1.2f, 2.5f);
                //pick a randim position from the center of the area
                float radius = UnityEngine.Random.Range(2f, 7f);
                //pick a random direction
                Quaternion direction = Quaternion.Euler(0f, UnityEngine.Random.Range(-180f, 180f), 0f);

                //combine height, radius and direction to pick a potential postion
                potentialPosition = _flowerArea.transform.position + Vector3.up * height + direction * Vector3.forward * radius;

                //Choose and set random starting pitch and yaw
                float pitch = UnityEngine.Random.Range(-60f, 60f);
                float yaw = UnityEngine.Random.Range(-180f, 180f);

                potentialRotation = Quaternion.Euler(pitch, yaw, 0f);
            }

            //check to see if the agent will collide with anything
            Collider[] colliders = Physics.OverlapSphere(potentialPosition, 0.05f);

            //safe position found if no colliders overlap found
            safePositionFound = colliders.Length == 0;

            Debug.Assert(safePositionFound, "Could not find a safe position to spawn");

            //Set the position and rotation
            transform.position = potentialPosition;
            transform.rotation = potentialRotation;
        }
    }

    /// <summary> Update the neares flower to the agent </summary>
    private void UpdateNearestFlower()
    {
        foreach (Flower flower in _flowerArea.Flowers)
        {
            if (_nearestFlower == null && flower.HasNectar)
            {
                //No current nearest flower and the nearest has nectar
                _nearestFlower = flower;
            }
            else if (flower.HasNectar)
            {
                // Calculate distance to this flower and distance to the current nearest flower
                float distanceToFlower = Vector3.Distance(flower.transform.position, BeakTip.position);
                float distanceToCurrentNearestFlower = Vector3.Distance(_nearestFlower.transform.position, BeakTip.position);

                //if current nearest flower is empty or the flower is closer than the current flower
                if (!_nearestFlower.HasNectar || distanceToFlower < distanceToCurrentNearestFlower)
                {
                    _nearestFlower = flower;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        TriggerEnterOrStay(other);
    }

    private void OnTriggerStay(Collider other) {
        TriggerEnterOrStay(other);
    }

    /// <summary> Handles when the agent collider enters or stays in a trigger collider </summary>
    /// <param name='other'> The trigger collider (Flower Nectar Area)</param>
    private void TriggerEnterOrStay(Collider other)
    {
        //Check if the trigger is the nectar area
        if(other.CompareTag("nectar"))
        {
            Vector3 closestPointToBeakTip = other.ClosestPoint(BeakTip.position);

            //Check if the closest collision point is close to the beak tip (Only the beak tip)
            if (Vector3.Distance(BeakTip.position, closestPointToBeakTip) < BEAK_TIP_RADIUS)
            {
                //Look up the flower for the nectar collider
                Flower flower = _flowerArea.GetFlowerFromNectar(other);

                //Attempt to take .01 nectar from timestep (fixedDeltaTime = 0.2sec)
                float nectarReceived = flower.Feed(0.01f);

                //Keep track in the UI
                NectarObtained += nectarReceived;

                if(IsTrainingMode)
                {
                    //Calculate reward for getting nectar
                    float bonus = .02f * Mathf.Clamp01(Vector3.Dot(transform.forward.normalized, -_nearestFlower.FlowerUpVector.normalized));
//======>>> IMPORTANT    
                    //Give reward to the Agent to stimulate Neural Network reinforcement learning.
                    AddReward(0.01f + bonus);
                }

                //If flower is empty
                if(!flower.HasNectar)
                {
                    UpdateNearestFlower();
                }
            }
        }
    }

    private void OnCollisionEnter(Collision other) {
        if(IsTrainingMode && other.collider.CompareTag("boundary"))
        {
            //Collided with area boundary so give negative reward
            AddReward(-.5f);
        }
    }

    private void Update() {
        //Draw a line from the beaktip to the nearest flower
        if (_nearestFlower)
        {
            Debug.DrawLine(BeakTip.position, _nearestFlower.FlowerCenterPosition, Color.green);
        }
    }

    private void FixedUpdate() {
        if (_nearestFlower && !_nearestFlower.HasNectar)
        {
            UpdateNearestFlower();
        }
    }
}
