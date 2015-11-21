var target : Transform;            // The position that that camera will be following.
var smoothing : float = 5f;        // The speed with which the camera will be following.

private var offset : Vector3;                     // The initial offset from the target.

function Start ()
{
    // Calculate the initial offset.
    offset = transform.position - target.position;
}


function FixedUpdate ()
{
    // Create a postion the camera is aiming for based on the offset from the target.
    var targetCamPos : Vector3 = target.position + offset;

    // Smoothly interpolate between the camera's current position and it's target position.
    transform.position = Vector3.Lerp (transform.position, targetCamPos, smoothing * Time.deltaTime);
}