using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellStack : Stack<HexCell> {
    Vector3 position;

    public CellStack() {
        position = new Vector3(0, 0, 0);
    }

    public CellStack(Vector3 pos) {
        position = pos;
    }
}
