using System;
using UnityEngine;

[Serializable]
public struct SerializableVector2
{
    public float x;
    public float y;

    public SerializableVector2(Vector2 v)
    {
        x = v.x;
        y = v.y;
    }

    public static implicit operator Vector2(SerializableVector2 s) => new Vector2(s.x, s.y);
    public static implicit operator SerializableVector2(Vector2 v) => new SerializableVector2(v);
    public static implicit operator Vector3(SerializableVector2 s) => new Vector3(s.x, s.y, 0);
    public static implicit operator SerializableVector2(Vector3 v) => new SerializableVector2(v);
}