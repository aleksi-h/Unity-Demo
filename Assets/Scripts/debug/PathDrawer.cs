using UnityEngine;
using System.Collections;

//piirtää navmesh-agentin polun
//mouse 1 asettaa uuden polun
//mouse 2 piirtää polun
public class PathDrawer : MonoBehaviour {

    public LineRenderer line;
    public NavMeshAgent agent;

    void Start() {
        line = GetComponent<LineRenderer>();
        agent = GetComponent<NavMeshAgent>();
    }

    private LayerMask groundLayerMask = 1 << 11;
    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1100, groundLayerMask)) {
                line.SetPosition(0, transform.position);
                agent.SetDestination(hit.point);
            }
        }
        else if (Input.GetMouseButtonDown(1)) {
            DrawPath(agent.path);
        }
    }

    private void DrawPath(NavMeshPath path) {
        if (path.corners.Length < 2)
            return;

        line.SetVertexCount(path.corners.Length);
        for (var i = 1; i < path.corners.Length; i++) {
            line.SetPosition(i, path.corners[i]);
        }
    }
}