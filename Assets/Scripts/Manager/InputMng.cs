#if UNITY_DEBUG
	#define DEBUG_MODE
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMng : MonoBehaviour
{
	//********************************************************************************
	//valiables
	//********************************************************************************
	//----------CONST----------
	//マウスボタンコード
	public enum MOUSECODE
	{
		LEFT,
		RIGHT,
		CENTER,
		END
	}
	//----------CLASS----------
	//----------PUBLIC STATIC----------
	//----------PUBLIC----------
	//----------PRIVATE----------
	//********************************************************************************
	//public static function
	//********************************************************************************
	//----------MAKE----------
	//----------RELEASE----------
	//----------IS----------
	//マウス系
	public static bool IsMousePush(MOUSECODE code)
	{
		return Input.GetMouseButton((int)code);
	}
	public static bool IsMouseDown(MOUSECODE code)
	{
		return Input.GetMouseButtonDown((int)code);
	}
	public static bool IsMouseUp(MOUSECODE code)
	{
		return Input.GetMouseButtonUp((int)code);
	}
	//キーボード系
	public static bool IsKeyPush(KeyCode code)
	{
		return Input.GetKey(code);
	}
	public static bool IsKeyDown( KeyCode code )
	{
		return Input.GetKeyDown( code );
	}
	public static bool IsKeyUp(KeyCode code)
	{
		return Input.GetKeyUp(code);
	}
	//----------SET----------
	//----------GET----------
	//----------OTHER----------
	//********************************************************************************
	//private static function
	//********************************************************************************
	//----------MAKE----------
	//----------RELEASE----------
	//----------IS----------
	//----------SET----------
	//----------GET----------
	//----------OTHER----------
	//********************************************************************************
	//public function
	//********************************************************************************
	//----------MAKE----------
	//----------RELEASE----------
	//----------IS----------
	//----------SET----------
	//----------GET----------
	//----------OTHER----------
	//********************************************************************************
	//private function
	//********************************************************************************
	//----------EVENT----------
	//----------MAKE----------
	//----------RELEASE----------
	//----------IS----------
	//----------SET----------
	//----------GET----------
	//----------OTHER----------
}
