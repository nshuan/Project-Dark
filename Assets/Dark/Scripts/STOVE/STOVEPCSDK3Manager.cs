namespace Dark.Scripts.STOVE
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using UnityEngine;
    using static Stove.PCSDK.Base;
    using static Stove.PCSDK.GameSupport;
    using static Stove.PCSDK.Ownership;

    public sealed class STOVEPCSDK3Manager : MonoBehaviour
    {
        private const string GameObjectName = "STOVEPCSDK3Manager";
        private const string LauncherRequiredTitle = "STOVE Launcher Required";
        private const string LauncherRequiredMessage =
            "Ash Warden must be started from the STOVE launcher.\n\n" +
            "If STOVE opened automatically, please launch the game from there.";

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        private const uint MessageBoxOk = 0x00000000;
        private const uint MessageBoxIconWarning = 0x00000030;
        private const uint MessageBoxTaskModal = 0x00002000;
#endif

        private static readonly object LockObject = new object();
        private static STOVEPCSDK3Manager _instance;

        private STOVEPCSDK3Config _config;
        private bool _isInitializing;
        private bool _isInitialized;
        private bool _isOwnershipInitialized;
        private bool _isGameSupportInitialized;
        private bool _isQuitting;

        public static STOVEPCSDK3Manager Instance
        {
            get
            {
                lock (LockObject)
                {
                    if (_instance != null)
                        return _instance;

                    _instance = FindObjectOfType<STOVEPCSDK3Manager>();

                    if (_instance == null)
                    {
                        _instance = new GameObject(GameObjectName).AddComponent<STOVEPCSDK3Manager>();
                    }

                    EnsureRootObject(_instance);
                    DontDestroyOnLoad(_instance.gameObject);
                    return _instance;
                }
            }
        }

        public bool IsInitialized => _isInitialized;
        public bool IsInitializing => _isInitializing;
        public bool IsOwnershipInitialized => _isOwnershipInitialized;
        public bool IsGameSupportInitialized => _isGameSupportInitialized;
        public StovePCOwnership[] Ownerships { get; private set; } = Array.Empty<StovePCOwnership>();

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            EnsureRootObject(this);
            DontDestroyOnLoad(gameObject);
        }

        private void OnApplicationQuit()
        {
            _isQuitting = true;
            UnInitialize();
        }

        private void Update()
        {
            if (!_isInitializing && !_isInitialized)
                return;

            Base_RunCallback();
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                UnInitialize();
                _instance = null;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void AutoInitialize()
        {
            var config = STOVEPCSDK3Config.Load();

            if (config.autoInitialize)
            {
                Instance.Initialize(config);
            }
        }

        public void Initialize(STOVEPCSDK3Config config)
        {
            if (_isInitialized || _isInitializing)
                return;

            _config = config ?? STOVEPCSDK3Config.Default();

            if (!CanInitialize(_config))
                return;

            var initParam = CreateInitializeParam(_config);

            _isInitializing = true;

            if (!_config.enforceLauncher)
            {
                InitializeBase(initParam);
                return;
            }

            try
            {
                Base_RestartAppIfNecessaryAsync(
                    initParam,
                    (uint)Mathf.Max(0, _config.launcherCheckTimeoutMilliseconds),
                    (callbackResult, restartAppIfNecessary) =>
                    {
                        PrintCallbackResult(callbackResult);

                        if (!callbackResult.result.IsSuccessful())
                        {
                            HandleLauncherCheckFailed();
                            return;
                        }

                        if (restartAppIfNecessary)
                        {
                            HandleLauncherRestartRequired();
                            return;
                        }

                        InitializeBase(initParam);
                    });
            }
            catch (Exception exception)
            {
                FailInitialize($"STOVE launcher check threw an exception: {exception}");
            }
        }

        public void QueryOwnerships(Action<StovePCOwnership[]> onFinished = null)
        {
            if (!_isInitialized || !_isOwnershipInitialized)
            {
                Debug.LogWarning("STOVE Ownership query skipped because Ownership SDK is not initialized.");
                onFinished?.Invoke(Array.Empty<StovePCOwnership>());
                return;
            }

            try
            {
                Ownership_OwnershipList((callbackResult, ownerships) =>
                {
                    PrintCallbackResult(callbackResult);

                    Ownerships = callbackResult.result.IsSuccessful()
                        ? ownerships ?? Array.Empty<StovePCOwnership>()
                        : Array.Empty<StovePCOwnership>();

                    onFinished?.Invoke(Ownerships);
                });
            }
            catch (Exception exception)
            {
                Debug.LogError($"STOVE Ownership query threw an exception: {exception}");
                onFinished?.Invoke(Array.Empty<StovePCOwnership>());
            }
        }

        public bool TryGetUser(out StovePCUser user)
        {
            user = default;

            if (!_isInitialized)
                return false;

            var result = Base_GetUser(ref user);
            PrintResult(result);
            return result.IsSuccessful();
        }

        public bool TryGetAccessToken(out string accessToken)
        {
            accessToken = string.Empty;

            if (!_isInitialized)
                return false;

            var result = Base_GetAccessToken(ref accessToken, BASE_DEFAULT_STR_LEN_4096);
            PrintResult(result);
            return result.IsSuccessful();
        }

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);
#endif

        public void UnInitialize()
        {
            if (!_isInitialized && !_isInitializing)
                return;

            _isInitializing = false;

            if (_isGameSupportInitialized)
            {
                PrintResult(GameSupport_UnInitialize());
                _isGameSupportInitialized = false;
            }

            if (_isOwnershipInitialized)
            {
                PrintResult(Ownership_UnInitialize());
                _isOwnershipInitialized = false;
            }

            if (_isInitialized)
            {
                PrintResult(Base_UnInitialize());
                _isInitialized = false;
            }
        }

        public void PrintResult(Result result)
        {
            var sb = new StringBuilder();

            sb.AppendLine("# STOVE Result");
            sb.AppendLine($" - sdkName: {result.sdkName}");
            sb.AppendLine($" - methodCode: {result.methodCode}");
            sb.AppendLine($" - resultCode: {result.resultCode}");
            sb.AppendLine($" - exceptionMessage: {result.exceptionMessage}");

            Debug.Log(sb.ToString());
        }

        public void PrintCallbackResult(CallbackResult callbackResult)
        {
            var sb = new StringBuilder();

            sb.AppendLine("# STOVE CallbackResult");
            sb.AppendLine($" - sdkName: {callbackResult.result.sdkName}");
            sb.AppendLine($" - methodCode: {callbackResult.result.methodCode}");
            sb.AppendLine($" - resultCode: {callbackResult.result.resultCode}");
            sb.AppendLine($" - exceptionMessage: {callbackResult.result.exceptionMessage}");
            sb.AppendLine($" - errorMessage: {callbackResult.errorMessage}");
            sb.AppendLine($" - externalError: {callbackResult.externalError}");

            Debug.Log(sb.ToString());
        }

        private static StovePCInitializeParam CreateInitializeParam(STOVEPCSDK3Config config)
        {
            return new StovePCInitializeParam
            {
                environment = config.environment,
                gameId = config.gameId,
                applicationKey = config.applicationKey
            };
        }

        private static bool CanInitialize(STOVEPCSDK3Config config)
        {
#if UNITY_EDITOR
            Debug.Log("STOVE PC SDK initialization skipped in the Unity Editor. Test STOVE through a Windows player launched by STOVE.");
            return false;
#else
#if !UNITY_STANDALONE_WIN
            Debug.Log("STOVE PC SDK initialization skipped outside Windows standalone builds.");
            return false;
#else
            if (string.IsNullOrWhiteSpace(config.environment) ||
                string.IsNullOrWhiteSpace(config.gameId) ||
                string.IsNullOrWhiteSpace(config.applicationKey))
            {
                Debug.LogError("STOVE PC SDK initialization skipped because environment, gameId, or applicationKey is missing.");
                return false;
            }

            return true;
#endif
#endif
        }

        private static void EnsureRootObject(STOVEPCSDK3Manager manager)
        {
            if (manager.transform.parent == null)
                return;

            manager.transform.SetParent(null);
        }

        private void InitializeBase(StovePCInitializeParam initParam)
        {
            try
            {
                Base_Initialize(initParam, callbackResult =>
                {
                    PrintCallbackResult(callbackResult);

                    if (!callbackResult.result.IsSuccessful())
                    {
                        FailInitialize("STOVE Base SDK initialization failed.");
                        return;
                    }

                    _isInitialized = true;
                    _isInitializing = false;
                    InitializeModules();
                });
            }
            catch (Exception exception)
            {
                FailInitialize($"STOVE Base SDK initialization threw an exception: {exception}");
            }
        }

        private void InitializeModules()
        {
            if (_config.initializeOwnership)
            {
                var ownershipResult = Ownership_Initialize();
                PrintResult(ownershipResult);
                _isOwnershipInitialized = ownershipResult.IsSuccessful();

                if (_isOwnershipInitialized && _config.queryOwnershipOnInitialize)
                {
                    QueryOwnerships();
                }
            }

            if (_config.initializeGameSupport)
            {
                var gameSupportResult = GameSupport_Initialize();
                PrintResult(gameSupportResult);
                _isGameSupportInitialized = gameSupportResult.IsSuccessful();
            }
        }

        private void FailInitialize(string message)
        {
            Debug.LogError(message);
            _isInitializing = false;
            _isInitialized = false;
        }

        private void HandleLauncherRestartRequired()
        {
            Debug.LogWarning("STOVE launcher restart required. Direct-launched process will exit after notifying the user.");
            _isInitializing = false;
            ShowLauncherRequiredMessage();
            QuitApplication();
        }

        private void HandleLauncherCheckFailed()
        {
            Debug.LogError("STOVE launcher check failed. Process will exit after notifying the user.");
            _isInitializing = false;
            _isInitialized = false;
            ShowLauncherRequiredMessage();
            QuitApplication();
        }

        private static void ShowLauncherRequiredMessage()
        {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            MessageBox(IntPtr.Zero, LauncherRequiredMessage, LauncherRequiredTitle, MessageBoxOk | MessageBoxIconWarning | MessageBoxTaskModal);
#else
            Debug.LogWarning($"{LauncherRequiredTitle}: {LauncherRequiredMessage}");
#endif
        }

        private void QuitApplication()
        {
            if (_isQuitting)
                return;

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
