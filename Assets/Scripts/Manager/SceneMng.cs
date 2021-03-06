﻿#if UNITY_DEBUG
	#define DEBUG_MODE
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SCENE
{
	TITLE,
	GAMEMAIN,
}

public class SceneMng : MonoBehaviour
{
	//********************************************************************************
	//valiables
	//********************************************************************************
	//----------CONST----------
	//SCENEのenumと同順
	public static readonly string[] SCENENAME =
	{
		"Title",
		"GameMain",
	};
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
	//----------SET----------
	//----------GET----------
	//----------OTHER----------
	//シーンチェンジ
	public static void Change( SCENE scene )
	{
		SceneManager.LoadScene( SCENENAME[(int)scene] );
	}
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
