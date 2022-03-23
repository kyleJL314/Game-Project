using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Itask 
{
    void start();

    int priority();

    bool ready();

}
