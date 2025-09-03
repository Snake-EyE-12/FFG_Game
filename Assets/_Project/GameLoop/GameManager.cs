using Cobra.DesignPattern;
using NUnit.Framework;
using Unity.Netcode;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
	[HideInInspector] public bool inGame = false;

	
	private void Start()
	{
		GameStarter.OnGameStart += OnGameStart;
		GameStarter.OnGameEnd += OnGameEnd;
	}

	private void OnGameStart()
	{
		inGame = true;
	}

	private void OnGameEnd()
	{
		inGame = false; // not set to happen anywhere yet. OnGameEnd needs to be invoked.
		changer.ChangeScene();
	}

	[SerializeField] private NetworkSceneChanger changer;
}
