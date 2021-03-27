using UnityEngine;
using System.Collections;
using Photon.Pun;

namespace VmodMonkeMapLoader.Behaviours
{
    [System.Serializable]
    public class TagZone : GorillaMapTriggerBase
    {

#if PLUGIN

        static bool canBeTagged = true;

        public override void Trigger(Collider collider)
        {
            if (!canBeTagged)
                return;

            canBeTagged = false;
            TagLocalPlayer();
            StartCoroutine(TagCoroutine(1f));

            base.Trigger(collider);
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
                    PhotonNetwork.LocalPlayer
                });
            }
        }

#endif

    }
}
