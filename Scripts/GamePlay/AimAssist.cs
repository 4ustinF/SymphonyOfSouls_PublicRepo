using System.Linq;
using UnityEngine;

public class AimAssist : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _targetPoint = null;            // An actual point graple will be target to
    [SerializeField] private Collider _triggerCollider = null;
    [SerializeField] private GameObject _targetMesh = null;             // Object with a mesh filter and materials 
    [SerializeField] private GameObject _helpingParticles = null;       // An actual point graple will be target to

    public Transform TargetPoint { get => _targetPoint.transform; }

    [Header("Sleep")]
    [SerializeField] private float _sleepTime = 2.0f;                    // Sleep is such state when target doesnt respond to player's interactions
    private bool _isSleep = false;
    public Transform GraplePoint { get => _targetPoint.transform; }

    [SerializeField] private Material _highlightMaterial = null;
    private int materialsCount = 0;

    [SerializeField] private bool _isStatue = false;

    public void SetHighlightMatirial(Material newMaterial)
    {
        _highlightMaterial = newMaterial;
        RemoveHighlightMaterial();
        AddHighlightMaterial();
    }

    private void Awake()
    {
        materialsCount = _targetMesh.GetComponent<MeshRenderer>().materials.Length;
    }

    private void OnMouseEnter()
    {
        if (_isSleep)
        {
            return;
        }

        if(_isStatue == false)
        {
            AddHighlightMaterial();
        }
    }

    private void AddHighlightMaterial()
    {
        if (_targetMesh)
        {
            var mats = _targetMesh.GetComponent<MeshRenderer>().materials.ToList();
            mats.Add(_highlightMaterial);
            _targetMesh.GetComponent<MeshRenderer>().materials = mats.ToArray();
        }
    }

    private void OnMouseExit()
    {
        if(_isStatue == false)
        {
            RemoveHighlightMaterial();
        }
    }

    private void RemoveHighlightMaterial()
    {
        if (!_targetMesh)
        {
            return;
        }

        //Debug.Log("RemoveHighlightMaterial");

        var mats = _targetMesh.GetComponent<MeshRenderer>().materials.ToList();

        if(_isStatue == true || mats.Count > materialsCount)
        {
            mats.RemoveAt(mats.Count - 1);
        }

        _targetMesh.GetComponent<MeshRenderer>().materials = mats.ToArray();
    }

    public void Sleep()
    {
        _isSleep = true;

        RemoveHighlightMaterial();

        if(_helpingParticles!=null)
        {
            _helpingParticles.SetActive(false);
        }

        if (_triggerCollider != null)
        {
            _triggerCollider.enabled = false;
        }
    }
}
