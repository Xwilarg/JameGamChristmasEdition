using System.Collections.Generic;
using UnityEngine;

namespace JameGam
{
    public class NextNode : MonoBehaviour
    {
        public List<NextNode> NextNodes;

        private void Awake()
        {
            foreach (var n in NextNodes)
            {
                if (!n.NextNodes.Contains(this)) n.NextNodes.Add(this);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            foreach (var n in NextNodes)
            {
                Gizmos.DrawLine(n.transform.position, transform.position);
            }
        }
    }
}
