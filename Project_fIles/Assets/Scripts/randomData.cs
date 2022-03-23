using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class randomData
{
    public static int gameSeed =0;

    public static float PerlinNoise3D(Vector3 loc,float scale,Vector3 offset)
    {
        Vector3 trueLoc = (loc - offset) * scale;

        float xy = Mathf.PerlinNoise(trueLoc.x, trueLoc.y);
        float xz = Mathf.PerlinNoise(trueLoc.x, trueLoc.z);
        float yz = Mathf.PerlinNoise(trueLoc.y, trueLoc.z);
        float yx = Mathf.PerlinNoise(trueLoc.y, trueLoc.x);
        float zx = Mathf.PerlinNoise(trueLoc.z, trueLoc.x);
        float zy = Mathf.PerlinNoise(trueLoc.z, trueLoc.y);

        return (xy + xz + yz + yx + zx + zy) / 6;
    }
}
