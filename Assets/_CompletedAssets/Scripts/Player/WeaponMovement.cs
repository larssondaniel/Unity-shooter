using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;
using System.Collections.Generic;
using Valve.VR;

namespace CompleteProject
{
	public class WeaponMovement : MonoBehaviour
	{
		public float speed = 6f;            // The speed that the weapon will move at.
		
		List<int> controllerIndices = new List<int>();

		Vector3 movement;                   // The vector to store the direction of the weapon's movement.
		Animator anim;                      // Reference to the animator component.
		Rigidbody weaponRigidbody;          // Reference to the weapon's rigidbody.
		#if !MOBILE_INPUT
		int floorMask;                      // A layer mask so that a ray can be cast just at gameobjects on the floor layer.
		float camRayLength = 100f;          // The length of the ray from the camera into the scene.
		#endif

		public Transform point, pointer;

		void Awake ()
		{
			#if !MOBILE_INPUT
			// Create a layer mask for the floor layer.
			floorMask = LayerMask.GetMask ("Floor");
			#endif
			
			// Set up references.
			anim = GetComponent <Animator> ();
			weaponRigidbody = GetComponent <Rigidbody> ();
		}

		private void OnDeviceConnected(params object[] args)
		{
			var index = (int)args[0];

			Debug.Log("Index is:");
			Debug.Log (index);
			
			var vr = SteamVR.instance;
			if (vr.hmd.GetTrackedDeviceClass((uint)index) != TrackedDeviceClass.Controller)
				return;
			
			var connected = (bool)args[1];
			if (connected)
			{
				Debug.Log("-----------------------------------------------");
				Debug.Log(string.Format("Controller {0} connected.", index));
				controllerIndices.Add(index);
			}
			else
			{
				Debug.Log("-----------------------------------------------");
				Debug.Log(string.Format("Controller {0} disconnected.", index));
				controllerIndices.Remove(index);
			}
		}
		
		void OnEnable()
		{
			SteamVR_Utils.Event.Listen("device_connected", OnDeviceConnected);
		}
		
		void OnDisable()
		{
			SteamVR_Utils.Event.Remove("device_connected", OnDeviceConnected);
		}
		
		void FixedUpdate ()
		{
			if (true)
			{	
				var t = SteamVR_Controller.Input(0).transform;
				
				// Move the weapon around the scene.
				Move (t.pos.x, t.pos.y);
				
				// Turn the weapon.
				Turning (t);
				
				// Animate the weapon.
				Animating (t.pos.x, t.pos.y);
			}
		}
		
		
		void Move (float h, float v)
		{
			// Set the movement vector based on the axis input.
			movement.Set (h, 0f, v);
			
			// Normalise the movement vector and make it proportional to the speed per second.
			movement = movement.normalized * speed * Time.deltaTime;
			
			// Move the weapon to it's current position plus the movement.
			weaponRigidbody.MovePosition (movement);
		}
		
		
		void Turning (SteamVR_Utils.RigidTransform t)
		{
			// Create a ray from the mouse cursor on screen in the direction of the camera.
			// Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);
			Ray camRay = Camera.main.ScreenPointToRay (t.rot * Vector3.forward);

			// Create a RaycastHit variable to store information about what was hit by the ray.
			RaycastHit floorHit;
			
			// Perform the raycast and if it hits something on the floor layer...
			if(Physics.Raycast (camRay, out floorHit, camRayLength, floorMask))
			{
				// Create a vector from the weapon to the point on the floor the raycast from the mouse hit.
				Vector3 weaponToMouse = floorHit.point - transform.position;
				
				// Ensure the vector is entirely along the floor plane.
				weaponToMouse.y = 0f;
				
				// Create a quaternion (rotation) based on looking down the vector from the weapon to the mouse.
				Quaternion newRotatation = Quaternion.LookRotation (weaponToMouse);

				// Set the weapon's rotation to this new rotation.
				weaponRigidbody.MoveRotation (newRotatation);
			}
		}
		
		
		void Animating (float h, float v)
		{
			// Create a boolean that is true if either of the input axes is non-zero.
			bool walking = h != 0f || v != 0f;
			
			// Tell the animator whether or not the player is walking.
			anim.SetBool ("IsWalking", walking);
		}
	}
}