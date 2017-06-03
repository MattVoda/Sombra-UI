/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using UnityEngine;
using UnityEditor;
using System.Collections;

//-----------------------------------------------------------------------------
// Copyright 2012-2013 RenderHeads Ltd.  All rights reserved.
//-----------------------------------------------------------------------------

[CustomEditor(typeof(AVProMovieCaptureFromCamera))]
public class AVProMovieCaptureFromCameraEditor : Editor
{
	private AVProMovieCaptureFromCamera _capture;
	
	public override void OnInspectorGUI()
	{
		_capture = (this.target) as AVProMovieCaptureFromCamera;
		
		DrawDefaultInspector();
				
		GUILayout.Space(8.0f);
		
		if (Application.isPlaying)
		{		
			if (!_capture.IsCapturing())
			{
		   		if (GUILayout.Button("Start Recording"))
				{
					_capture.SelectCodec(false);
					_capture.SelectAudioDevice(false);
					// We have to queue the start capture otherwise Screen.width and height aren't correct
					_capture.QueueStartCapture();
				}
			}
			else
			{				
				GUILayout.BeginHorizontal();
				if (_capture._frameTotal > (int)_capture._frameRate * 2)
				{
					Color originalColor = GUI.color;
					float fpsDelta = Mathf.Abs(_capture._fps - (int)_capture._frameRate);
					GUI.color = Color.red;
					if (fpsDelta < 10)
						GUI.color = Color.yellow;
					if (fpsDelta < 2)
						GUI.color = Color.green;
					GUILayout.Label("Recording at " + _capture._fps.ToString("F1") + " fps");
					
					GUI.color = originalColor;
				}
				else
				{
					GUILayout.Label("Recording at ... fps");	
				}
					
				if (!_capture.IsPaused())
				{
					if (GUILayout.Button("Pause Capture"))
					{
						_capture.PauseCapture();
					}
				}
				else
				{
					if (GUILayout.Button("Resume Capture"))
					{
						_capture.ResumeCapture();
					}					
				}				
		   		if (GUILayout.Button("Stop Recording"))
				{
					_capture.StopCapture();
				}				
				GUILayout.EndHorizontal();
				
				GUILayout.Space(8.0f);
				GUILayout.Label("Recording at: " + _capture.GetRecordingWidth() + "x" + _capture.GetRecordingHeight() + " @ " + ((int)_capture._frameRate).ToString() + "fps");
				GUILayout.Space(8.0f);
				GUILayout.Label("Using video codec: '" + _capture._codecName + "'");
				GUILayout.Label("Using audio device: '" + _capture._audioDeviceName + "'");
			}	
		}
	}
}
