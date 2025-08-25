using Mirror;
using UnityEngine;

public class ObjectSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private float spawnForwardOffset = 2f;
    [SerializeField] private float spawnHeightOffset = 1f;

    [Command]
    private void CmdSpawnCube(Vector3 pos)
    {
        GameObject cube = Instantiate(cubePrefab, pos, Quaternion.identity);
        NetworkServer.Spawn(cube);
    }

    public void TrySpawnCube()
    {
        if (isLocalPlayer)
        {
            Vector3 pos = transform.position
                        + transform.forward * spawnForwardOffset
                        + Vector3.up * spawnHeightOffset;

            CmdSpawnCube(pos);
        }
    }
}
