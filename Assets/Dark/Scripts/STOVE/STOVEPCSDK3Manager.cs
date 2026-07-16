namespace Dark.Scripts.STOVE
{
    using System.Text;
    using System.Collections;
    using UnityEngine;

// Using statements for each PC SDK 3.0 module are required.
    using static Stove.PCSDK.Base;
/*
using static Stove.PCSDK.IAP;		// When integrating IAPSDK_NET
*/

    public sealed class STOVEPCSDK3Manager : MonoBehaviour
    {
        // Declare necessary variables at the top of the class.

        // Variable to store initialization status
        private bool _isInitialized;

        // Variable to store coroutine execution interval
        private float _runCallbackInternval = 1.0f;

        // Variable to store RunCallbackLoop coroutine
        private Coroutine _runCallbackCoroutine;

        // Static variable for using object as Singleton
        private static STOVEPCSDK3Manager _instance;
        private static object _lockObject = new object();

        public static STOVEPCSDK3Manager Instance
        {
            get
            {
                lock (_lockObject)
                {
                    if (_instance == null)
                    {
                        _instance = FindObjectOfType<STOVEPCSDK3Manager>();

                        if (_instance == null)
                        {
                            _instance = new GameObject().AddComponent<STOVEPCSDK3Manager>();
                            _instance.name = "STOVEPCSDK3Manager";
                        }
                    }
                }

                return _instance;
            }
        }

        #region

        // Process DontDestroyOnLoad

        // Call UnInitialize in OnDestroy
        private void OnDestroy()
        {
            if (_isInitialized)
            {
                UnInitialize();
            }
        }

        #endregion        

        #region Coroutine

        // Write coroutine to process RunCallback
        private IEnumerator RunCallbackCoroutine()
        {
            var wfs = new WaitForSeconds(_runCallbackInternval);

            while (true)
            {        		
                Base_RunCallback();

                yield return wfs;
            }
        }

        #endregion
        
        #region STOVEPCSDK3Manager public methods

        // Result structure output method
        public void PrintResult(Result r)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("# Result");
            sb.AppendLine($" - Result.sdkName : {r.sdkName}");
            sb.AppendLine($" - Result.methodCode : {r.methodCode}");
            sb.AppendLine($" - Result.resultCode : {r.resultCode}");
            sb.AppendLine($" - Result.exceptionMessage : {r.exceptionMessage}");

            Debug.Log(sb.ToString());
        }

        // CallbackResult structure output method
        public void PrintCallbackResult(CallbackResult cr)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("# CallbackResult");
            sb.AppendLine($" - CallbackResult.Result.sdkName : {cr.result.sdkName}");
            sb.AppendLine($" - CallbackResult.Result.methodCode : {cr.result.methodCode}");
            sb.AppendLine($" - CallbackResult.Result.resultCode : {cr.result.resultCode}");
            sb.AppendLine($" - CallbackResult.Result.exceptionMessage : {cr.result.exceptionMessage}");
            sb.AppendLine($" - CallbackResult.message : {cr.errorMessage}");
            sb.AppendLine($" - CallbackResult.externalError : {cr.externalError}");

            Debug.Log(sb.ToString());
        }

        // Write Initialize method for integrated module initialization
        public void Initialize(string shopKey)
        {
            StartRunCallbackLoop();

            StovePCInitializeParam initParam;
            initParam.environment = "LIVE";
            initParam.gameId = "GM-2A6F-6A54B6C1_IND";
            initParam.applicationKey = "d5f8912403522362f29cd17427b57da47cb810b5309df82fd37b4a2f269d3c74";

            Base_Initialize(initParam, (CallbackResult callbackResult) =>
            {
                // Print CallbackResult
                PrintCallbackResult(callbackResult);

                if (callbackResult.result.IsSuccessful())
                {
                    _isInitialized = true;
                }
                else
                {
                    Debug.Log("Fail to initialize Base SDK");
                }
            });
        }

        // Write UnInitialize method for integrated module cleanup
        public void UnInitialize()
        {
            Result result;

            this.StopRunCallbackLoop();

            result = Base_UnInitialize();
            PrintResult(result);

            _isInitialized = false;
        }

        // Write method to periodically call RunCallback
        public void StartRunCallbackLoop()
        {
            if (_runCallbackCoroutine == null)
            {
                Debug.Log("Start RunCallbackLoop");

                _runCallbackCoroutine = StartCoroutine(RunCallbackCoroutine());
            }
        }

        // Write method to stop Coroutine
        public void StopRunCallbackLoop()
        {
            if (_runCallbackCoroutine != null)
            {
                Debug.Log("Stop RunCallbackLoop");

                StopCoroutine(_runCallbackCoroutine);
                _runCallbackCoroutine = null;
            }
        }

        #endregion
    }
}