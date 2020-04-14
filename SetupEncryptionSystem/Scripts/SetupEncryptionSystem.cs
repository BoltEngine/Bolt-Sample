using System;
using UdpKit.Security;
using UnityEngine;

namespace Bolt.Samples.Encryption
{
    public class SetupEncryptionSystem : MonoBehaviour
    {
        [SerializeField] private string EncodedIV;

        [SerializeField] private string EncodedKey;

        [SerializeField] private string EncodedSecret;

        private void Start()
        {
            EncryptionManager.Instance.InitializeEncryption(EncodedIV, EncodedKey, EncodedSecret);
        }

        private void OnDestroy()
        {
            EncryptionManager.Instance.DeinitializeEncryption();
        }

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