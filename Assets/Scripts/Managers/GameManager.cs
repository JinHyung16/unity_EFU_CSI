using HughGenerics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

public class GameManager : Singleton<GameManager>, IDisposable
{
    [Header("Player Prefab")]
    [SerializeField] private GameObject playerPrefab;
    private GameObject player;
    private PlayerManager playerManager;

    [Header("Game Option Canvas")]
    [SerializeField] private Canvas gameOptionCanvas;
    [SerializeField] private Button gameExitBtn;

    private bool isOptionKeyDown;

    public bool IsGamePause { get; private set; }
    public bool IsEndTheme { private get; set; }

    private void Start()
    {
        gameOptionCanvas.enabled = false;
        isOptionKeyDown = false;
        IsGamePause = false;

        gameExitBtn.onClick.AddListener(QuitGameAndSaveData);
    }

    /// <summary>
    /// Player가 esc버튼을 누르면 OptionCanvas를 연다
    /// </summary>
    /// <param name="isOption"> esc버튼 눌렀는지 안눌렀는지 </param>
    public void OptionCanvasOpen(bool isOption)
    {
        if (isOption)
        {
            if (!isOptionKeyDown)
            {
                Time.timeScale = 0;
                IsGamePause = true;

                gameOptionCanvas.enabled = true;
                isOptionKeyDown = true;
            }
            else
            {
                Time.timeScale = 1;
                IsGamePause = false;

                gameOptionCanvas.enabled = false;
                isOptionKeyDown = false;

                //playerManager.PlayerCameraAdjustment();
            }
        }
    }

    /// <summary>
    /// 게임을 진행하다 그만하려고 나가고자 Exit Button을 누르면 호출
    /// 자동으로 게임 진행상황을 저장해준다.
    /// </summary>
    private void QuitGameAndSaveData()
    {
        int saveIndex = 0;
        switch (SceneController.GetInstance.CurSceneName)
        {
            case "ThemeFrist":
                saveIndex = 1;
                break;
            case "ThemeSecond":
                saveIndex = 2;
                break;
            case "ThemeThird":
                saveIndex = 3;
                break;
        }
        DataManager.GetInstance.SaveData(saveIndex, SceneController.GetInstance.CurSceneName);

        gameOptionCanvas.enabled = false;
        Dispose();
        SceneController.GetInstance.LoadScene("Main");
    }

    /// <summary>
    /// ThemeFirst의 진입하면 Player를 생성시킨다.
    /// 최초 1회만 생성시킨다.
    /// </summary>
    public void SpawnPlayer()
    {
        if (player == null)
        {
            player = Instantiate(playerPrefab, this.transform);
            playerManager = player.GetComponent<PlayerManager>();
        }
        player.SetActive(true);
    }

    /// <summary>
    /// Theme를 이동할때마다 호출한다.
    /// </summary>
    public void DespawnPlayer()
    {
        if (player != null)
        {
            player.SetActive(false);
        }
    }

    /// <summary>
    /// 테마를 진행하다 게임을 끌 때 호출
    /// 테마를 다 클리어했을때 호출한다.
    /// </summary>
    public void Dispose()
    {
        Destroy(player);
        GC.SuppressFinalize(player);
    }

    #region Game 진행 플로우 관련 함수
    public void FailedGameAndRestart()
    {
        DespawnPlayer();
        SceneController.GetInstance.LoadScene(SceneController.GetInstance.CurSceneName);
    }

    /// <summary>
    /// Scene을 Clear하면 다음 씬으로 보낸다.
    /// 이때, 마지막 Scene이면 Main으로 간다.
    /// </summary>
    /// <param name="nextScene">이동할 다음 Scene 이름</param>
    public void CelarGame()
    {
        if (IsEndTheme)
        {
            Dispose();
        }
        else
        {
            DespawnPlayer();
        }
        SceneController.GetInstance.LoadScene("Main");
    }
    #endregion
}
