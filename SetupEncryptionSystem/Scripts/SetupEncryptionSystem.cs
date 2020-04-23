using System;
using UdpKit.Security;
using UnityEngine;

namespace Bolt.Samples.Encryption
{
    /// <summary>
    /// Example class to fill the Encryption Keys
    ///
    /// Please take a look on the "SetupEncryption" prefab.
    /// If you want to use new keys, just press the "Generate keys" button on the Inspector
    /// </summary>
    public class SetupEncryptionSystem : MonoBehaviour
    {
        /// <summary>
        /// Initialization Vector
        /// </summary>
        [SerializeField] private string EncodedIV;
        
        /// <summary>
        /// Encryption Key
        /// </summary>
        [SerializeField] private string EncodedKey;
        
        /// <summary>
        /// Hash Secret
        /// </summary>
        [SerializeField] private string EncodedSecret;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            EncryptionManager.Instance.InitializeEncryption(EncodedIV, EncodedKey, EncodedSecret);
        }

        private void OnDestroy()
        {
            EncryptionManager.Instance.DeinitializeEncryption();
        }

        /// <summary>
        /// Generate new keys and store them
        /// </summary>
        public void GenerateKeys()
        {
            var IV = EncryptionManager.GenerateAesIV();
            var key = EncryptionManager.GenerateAesKey();
            var secret = EncryptionManager.GenerateHashSecret();

            EncodedIV = Convert.ToBase64String(IV);
            EncodedKey = Convert.ToBase64String(key);
            EncodedSecret = Convert.ToBase64String(secret);
        }
    }
}