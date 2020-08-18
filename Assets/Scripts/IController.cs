using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IController
{
    void Throttle(float percent);
    void Steer(Vector2 axis);
    void Roll(Vector2 axis);
    void Shoot();
    void Stop(float lerpVal);

}
