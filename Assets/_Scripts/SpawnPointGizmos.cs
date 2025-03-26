using UnityEngine;

namespace _Scripts
{
    public class SpawnPointGizmos : MonoBehaviour
    {
        public Color color = Color.blue;
        public float radius;
        public bool cubeGizmo;
        public bool sphereGizmo;
        
        private void OnDrawGizmos()
        {
            if (cubeGizmo)
            {
                Gizmos.color = color;
                Gizmos.DrawWireCube(transform.position, new Vector3(radius, radius, radius));
            }

            if (sphereGizmo)
            {
                Gizmos.color = color;
                Gizmos.DrawWireSphere(transform.position, radius);
            }
        }
    }
}
