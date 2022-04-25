using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Photon.Realtime;

namespace CantStop
{
    public class PawnStand : MonoBehaviour
    {
        public Transform socket;
        [SerializeField]
        private TextMeshPro textMesh;

        [SerializeField]
        private float moveTime = 0.5f;

        [SerializeField]
        private Ease ease;

        [SerializeField]
        private float switchMoveHeight = 0.007f;

        [SerializeField]
        private Transform woodPart;

        private float initialSwitchHeight;
        private float pressedSwitchHeight;

        private void Awake()
        {
            initialSwitchHeight = woodPart.localPosition.y;
            pressedSwitchHeight = initialSwitchHeight - switchMoveHeight;
        }

        public void SetNameText(string name)
        {
            textMesh.text = name;
        }

        public void Press(PlayerColor color)
        {
            woodPart.DOLocalMoveY(initialSwitchHeight - switchMoveHeight, moveTime).SetEase(Ease.Linear).
                OnComplete(() => textMesh.color = PlayerManager.colors[color]);
        }

        public void Init(Player owner)
        {
            textMesh.text = owner.NickName;
            var ownerColor = (PlayerColor)owner.CustomProperties[PlayerManager.ColorKey];
            if (ownerColor == PlayerColor.None)
                return;
            woodPart.localPosition = new Vector3(woodPart.localPosition.x, pressedSwitchHeight, woodPart.localPosition.z);
            textMesh.color = PlayerManager.colors[ownerColor];
        }

        public void Release()
        {
            woodPart.DOLocalMoveY(initialSwitchHeight, moveTime).SetEase(Ease.Linear).
                OnComplete(() => textMesh.color = PlayerManager.colors[(PlayerColor)PlayerColor.None]);
        }

        public void MoveTo(Vector3 pos)
        {
            transform.DOMove(pos, moveTime).SetEase(ease);
        }
    }
}
