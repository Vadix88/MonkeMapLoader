using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Collections;
using Photon.Pun;

namespace VmodMonkeMapLoader.Behaviours
{
    [System.Serializable]
    public class TagZone : MonoBehaviour
    {
        static bool canBeTagged = true;

        void OnTriggerEnter(Collider collider)
        {
            if (!canBeTagged)
                return;

            if (collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() == null)
                return;

            canBeTagged = false;
            StartCoroutine(TagCoroutine(1f));
        }

        private IEnumerator TagCoroutine(float time)
        {
            yield return new WaitForSeconds(time);

            canBeTagged = true;
        }

        public static void TagLocalPlayer()
        {
            if (PhotonNetwork.InRoom)
            {
                PhotonView.Get(GorillaTagManager.instance.GetComponent<GorillaGameManager>()).RPC("ReportTagRPC", RpcTarget.MasterClient, new object[]
                {
                    PhotonNetwork.LocalPlayer,
                    PhotonNetwork.LocalPlayer
                });
            }
        }
    }
}
