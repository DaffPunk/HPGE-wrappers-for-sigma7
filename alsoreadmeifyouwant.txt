Installation

	To install the Unity assets you can either:
		1. Copy the HapticPlugin folder and its contents from the Assets folder of a Unity project with an existing installation into the Assets folder of your new Unity project
		2. Install the assets directly from the github repository (github.com/DaffPunk/HPGE-wrappers-for-sigma7.git).
Use
      For instructions on basic use and set-up, you can refer to the HPGE-wrappers github page. (https://github.com/HapticPlugin/HPGE-wrappers).
	The only notable difference from the instructions on the HPGE-wrappers github page is that the HapticTool.cs represents the “Thumb” of the haptic tool instead of the default cursor (this means that opening or closing the gripper will change the position of the game object). The position of the midpoint of the haptic cursor can be tracked with the included TestToolCursor.cs file.
	Note that all functions in the library return an integer. This integer will always be returned as a zero if the Haptic Device is initialized and detected by the system, and will be some integer otherwise. All functions that return a value require you to pass in a parameter where the return values will be saved.

Grasping Mode

	<Note> You may completely ignore this section if you plan to only have the haptic tool interact with static/immobile objects within the virtual space </Note>
	Relevant Functions:
		enable_grasping_mode(bool isFixed, double xOffset, double yOffset, double zOffset);
		disable_grasping_mode();
	Unfortunately, the haptic feedback capabilities of chai3d do not work great when both the haptic tool and the haptic shape it is touching are both in motion, such as when picking up and moving a haptic object.* Thus, if we still want to simulate haptic feedback for picking up haptic objects, we need to get clever. This is the purpose of Grasping Mode.
	I will walk through step-by-step how Grasping Mode is currently used to simulate picking up objects to try to help explain it.
		1. Grasping Mode is not yet enabled. Both haptic points (fingers) interact with objects in the environment as one would expect.
		2. Both haptic points of the sigma7 gripper come within range of a haptic object (indicating that the object is about to be grabbed). A script detects this and enables Grasping Mode.**
		3. When Grasping Mode is enabled, the haptic points’ places are taken by proxy points (points that only track the position of where the haptic points “should be”. They serve as visual indicators and have no haptic feedback capabilities of their own). 
		4. The actual haptic points are shifted to some arbitrary space in the unity world (whose position is by default (0, 0, 0) within the chai3d world-space and can be changed with the xOffset, yOffset, zOffset variables). These are the haptic points that are used to actually calculate haptic feedback. If isFixed is set to true (which it should be, unless you really know what you’re doing), the haptic points are fixed around a central rotational point (whose position can be gotten via UnityHaptics.GetToolPosition()).
		5. At this center rotational point is spawned an exact copy of the haptic object we are attempting to pick up. Its position is fixed in place, but its rotation is set to continually copy the rotation of the object we are picking up.
		6. Meanwhile, the object we are grabbing’s transformation is set to be the child of (i.e. follow) a game object representing the mid-point of our proxy points to mimic picking up the object. We also freeze the position and rotation of the object we are grabbing here, so that it determined entirely by how we hold it and not by physics calculations. 
		7. At this point we have successfully mimicked the sensation of picking up the object.
		8. When the gripper is opened enough that both haptic points are no longer in contact with the object (i.e. the user is releasing the object), we disable Grasping Mode and unfreeze the position and rotation of the object we were grasping. The proxy points are replaced by the actual haptic points, and simulation returns to normal.
   Simple! In essence, we are creating a static version of a mobile object to simulate all haptic interactions with. This method has some limitations, namely that it assumes that we are grabbing a generally uniform object smaller than the space between our haptic points whose center point is equal to (or close enough to) the center point between our haptic points.
   If one were inclined to improve the functionality of this feature, I have a theory it could be done by having the copy of the object we are grabbing (the “pseudo-object”) move perpendicularly to the haptic points (i.e. along the plane whose normal vector is the vector connecting the two haptic points) such as to keep the pseudo-object’s position relative to the haptic points equivalent to the real object’s position relative to the proxy points. That is, however, purely hypothetical.

ProxyIndex.cs and ProxyThumb.cs
	These scripts track the position of the proxy haptic points representing the “Index Finger” and “Thumb” of the device’s gripper. Not particularly relevant unless you are utilizing Grasping Mode.

HapticIndex.cs
	This script represents the “Index Finger” of the haptic device’s gripper. This script is used alongside HapticTool.cs (which both represents the “Thumb” and is responsible for connecting with the physical device) to simulate both haptic points. 

ToolCursor.cs
	This script represents the midpoint of the haptic device’s gripper (the default “cursor” of the haptic object).

SigmaGrabScript.cs
	This script was attached to objects we wished to pick up to implement that functionality. It is not recommended to use this script in your own project as it is rather hacky and inefficient; it is included here to be used as a reference for how grasping functionality can be implemented.

* It may be possible to get this sort of interaction to work natively with enough fine-tuning and metaphorical duct tape. I was never able to do so.
** In reality, at this point we also sleep the thread for a millisecond after enable Grasping Mode to avoid a race condition. If you think that sounds like an irresponsible and risky way of resolving that issue, you are right.
