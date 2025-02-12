using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICameraEventDispatch: IEventDispatch
{
    void CameraMove(Vector3 v3);
    
    void CameraRotate(Vector3 v3);

    void CameraZoom(Vector3 v3);

}
