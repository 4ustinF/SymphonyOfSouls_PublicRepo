using UnityEditor;
using UnityEngine;

public class CrystalsTool : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private GameObject[] _crystals = null;
    [SerializeField] private float _sphereRadius = 2.5f;
    [SerializeField] private Vector2 _randYPosRange = Vector2.zero;
    [SerializeField] private Vector3 _randRotAmt = Vector3.zero;
    [SerializeField] private Vector3 _randScaleAmt = Vector3.zero;

    public void FindWall()
    {
        foreach (var crystal in _crystals)
        {
            Collider[] hitColliders = Physics.OverlapSphere(crystal.transform.position, _sphereRadius);
            Mesh mesh = hitColliders[0].gameObject.GetComponent<MeshFilter>().sharedMesh;

            Vector3 closestPoint = hitColliders[0].ClosestPoint(crystal.transform.position);
            Vector3 direction = (crystal.transform.localPosition - closestPoint).normalized;
            Quaternion rotation = crystal.transform.localRotation;
            if (Physics.Raycast(crystal.transform.position, direction, out RaycastHit hit, Mathf.Infinity))
            {
                rotation = Quaternion.FromToRotation(crystal.transform.up, -hit.normal);
            }

            Undo.RecordObject(crystal.transform, "Transform Position " + crystal.name);
            crystal.transform.localPosition = closestPoint;
            crystal.transform.localRotation = rotation;
        }
    }

    public void RandomPosition()
    {
        foreach (var crystal in _crystals)
        {
            Vector3 temp = crystal.transform.localPosition;

            temp += crystal.transform.up * Random.Range(_randYPosRange.x, _randYPosRange.y);

            Undo.RecordObject(crystal.transform, "Random position " + crystal.name);
            crystal.transform.localPosition = temp;
        }
    }

    public void RandomRotation()

    {
        foreach (var crystal in _crystals)
        {
            Vector3 temp = crystal.transform.localEulerAngles;

            temp.x = Random.Range(temp.x - _randRotAmt.x, temp.x + _randRotAmt.x);
            temp.y = Random.Range(temp.y - _randRotAmt.y, temp.y + _randRotAmt.y);
            temp.z = Random.Range(temp.z - _randRotAmt.z, temp.z + _randRotAmt.z);

            Undo.RecordObject(crystal.transform, "Random Rotation " + crystal.name);
            crystal.transform.localEulerAngles = temp;
        }
    }

    public void RandomScale()
    {
        foreach (var crystal in _crystals)
        {
            Vector3 temp = crystal.transform.localScale;

            temp.x = Random.Range(temp.x - _randScaleAmt.x, temp.x + _randScaleAmt.x);
            temp.y = Random.Range(temp.y - _randScaleAmt.y, temp.y + _randScaleAmt.y);
            temp.z = Random.Range(temp.z - _randScaleAmt.z, temp.z + _randScaleAmt.z);

            Undo.RecordObject(crystal.transform, "Random Scale " + crystal.name);
            crystal.transform.localScale = temp;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        foreach (var crystal in _crystals)
        {
            //Gizmos.DrawSphere(crystal.transform.position, _sphereRadius);
        }
    }
#endif
}
