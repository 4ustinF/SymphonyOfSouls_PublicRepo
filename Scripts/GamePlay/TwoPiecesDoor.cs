using DG.Tweening;
using UnityEngine;

public class TwoPiecesDoor : MonoBehaviour
{
    [SerializeField] private Transform _leftPiece = null;
    [SerializeField] private Transform _rightPiece = null;
    [SerializeField] private float _leftAngle = 90.0f;
    [SerializeField] private float _rightAngle = -90.0f;
    [SerializeField] private float _fullRotationTime = 3.0f;
    [SerializeField] private Ease _rotationEase = Ease.Linear;

    public void OpenDoor()
    {
        RoatePiece(_leftPiece, _leftAngle);
        RoatePiece(_rightPiece, _rightAngle);
    }

    private void RoatePiece(Transform piece, float angle)
    {
        if (piece == null)
        {
            return;
        }

        var rotation = piece.transform.rotation.eulerAngles;
        rotation.y += angle;
        piece.transform.DORotate(rotation, _fullRotationTime).SetEase(_rotationEase);
    }
}
