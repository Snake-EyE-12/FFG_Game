using Unity.Netcode.Components;
using UnityEngine;

public class ClientTransformNetwork : NetworkTransform
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }

}
