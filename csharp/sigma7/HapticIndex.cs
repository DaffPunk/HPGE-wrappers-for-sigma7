// This file is part of HPGE, an Haptic Plugin for Game Engines
// -----------------------------------------

// Software License Agreement (BSD License)
// Copyright (c) 2017-2019,
// Istituto Italiano di Tecnologia (IIT), All rights reserved.

// (iit.it, contact: gbaudbovy <at> gmail.com)

// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are
// met:

// 1. Redistributions of source code must retain the above copyright
// notice, this list of conditions and the following disclaimer.

// 2. Redistributions in binary form must reproduce the above copyright
// notice, this list of conditions and the following disclaimer in the
// documentation and/or other materials provided with the distribution.

// 3. Neither the name of HPGE, Istituto Italiano di Tecnologia nor the
// names of its contributors may be used to endorse or promote products
// derived from this software without specific prior written permission.

// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
// THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System.IO;
using UnityEngine;

//Use this class to represent the "Index Finger" (the mobile part of the gripper)
public class HapticIndex : MonoBehaviour
{

	[Header("Tool Properties")]
	/// <summary>
	/// If the object has an attached collider and it's a
	/// sphere collider, use it's radius for the tool
	/// </summary>
	public bool UseSphereRadius = true;
	/// <summary>
	/// If UseSphereRadius is false, use this value instead
	/// </summary>
	public float Radius = 0.5f;

	/// <summary>
	/// Filter information and debug messages shown in unity
	/// </summary>
	[Header("Debug Settings")]
	[Range(0, 3)]
	public int Verbosity = 0;


	private void UpdateToolPositionAndRotation()
	{
		double[] Thumb = new double[3];
		double[] Index = new double[3];
		HapticNativePlugin.get_gripper_position(Thumb, Index);
		
		transform.SetPositionAndRotation(
				UnityHaptics.DoubleToVect(Index),
				UnityHaptics.GetToolRotation());
	}


	private void Awake()
	{
		if (UseSphereRadius)
		{
			//// This can only render a sphere.
			//// Warn if attached to something else (maybe they just want to get the input position)
			if (GetComponent<MeshFilter>().name != "Sphere"
			&& Verbosity > 0)
			{
				Debug.LogWarning("This script is not attached to a sphere " +
					"but you are trying to use the sphere radius. You should" +
					"instead set the radius manually");
			}
			// The sphere size in unity is 0.5 units.
			// We use the Transform scale on the X axis to get the real size
			Radius = transform.lossyScale.x * 0.5f;
		}

		// Set initial position
		UpdateToolPositionAndRotation();
	}

	void Start()
	{
		// FIXME: Not working#
		//HapticNativePlugin.setHook(HapticNativePlugin.exampleHook);
		// Update position (if device has been moved between Awake and Start)
		UpdateToolPositionAndRotation();

		gameObject.GetComponent<Renderer>().material.color = Color.red;
	}

	// Update is called once per frame
	private void Update()
	{

	}

	void FixedUpdate()
	{
		UpdateToolPositionAndRotation();
	}
}
