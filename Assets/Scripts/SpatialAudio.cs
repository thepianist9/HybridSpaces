using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using agora_gaming_rtc;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace HybridSpaces
{


    public class SpatialAudio : MonoBehaviour
    {

        [SerializeField] private float radius;

        private PhotonView PV;
        private static Dictionary<Player, SpatialAudio> SpatialAudios = new Dictionary<Player, SpatialAudio>();
        private IAudioEffectManager agoraAudioEffects;

        private void Awake()
        {
            PV = GetComponent<PhotonView>();
            agoraAudioEffects = VoiceChatManager.Instance.GetEngine().GetAudioEffectManager();
            SpatialAudios[PV.Owner] = this;
        }

        private void OnDestroy()
        {
            SpatialAudios.Remove(PV.Owner);
        }

        private void Update()
        {
            if (!PV.IsMine)
                return;
            foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
            {
                if (player.IsLocal)
                {
                    continue;
                }

                if (player.CustomProperties.TryGetValue("agoraID", out object agoraID))

                {
                    SpatialAudio other;
                    other = SpatialAudios[player];


                    float gain = GetGain(other.transform.position);
                    float pan = GetPan(other.transform.position);
                    agoraAudioEffects.SetRemoteVoicePosition(uint.Parse((string) agoraID), pan, gain);
                }

            }

        }

        float GetGain(Vector3 otherPosition)
        {
            float distance = Vector3.Distance(transform.position, otherPosition);
            float gain = Mathf.Max(1 - (distance / radius), 0) * 100f;
            return gain;
        }

        float GetPan(Vector3 otherPosition)
        {
            Vector3 direction = otherPosition - transform.position;
            direction.Normalize();
            float dotProduct = Vector3.Dot(transform.right, direction);
            return dotProduct;
        }
    }
}