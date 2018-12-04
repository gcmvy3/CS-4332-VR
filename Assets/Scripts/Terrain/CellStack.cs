using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellStack : ScriptableObject {

    List<CellType> cells = new List<CellType>();
    public HexCoordinates coordinates;
    public Vector2Int indexWithinChunk;

    public bool initialized = false;

    public void Init(HexCoordinates coords, Vector2Int indexInChunk) {
        coordinates = coords;
        indexWithinChunk = indexInChunk;
        initialized = true;
    }

    public int Count() {
        if(initialized) {
            return cells.Count;
        }
        else {
            throw new System.Exception("ERROR - Trying to access CellStack before initialization");
        }
    }

    public CellType Peek() {
        if(initialized) {
            return cells[Count() - 1];
        }
        else {
            throw new System.Exception("ERROR - Trying to access CellStack before initialization");
        }
    }

    public CellType Pop() {
        if(initialized) {
            CellType top = Peek();
            cells.RemoveAt(Count() - 1);
            return top;
        }
        else {
            throw new System.Exception("ERROR - Trying to access CellStack before initialization");
        }
    }

    public void Push(CellType cell) {
        if(initialized) {
            cells.Add(cell);
        }
        else {
            throw new System.Exception("ERROR - Trying to access CellStack before initialization");
        }
    }

    public CellType PeekAt(int index) {
        if(initialized) {
            return cells[index];
        }
        else {
            throw new System.Exception("ERROR - Trying to access CellStack before initialization");
        }
    }

    public bool CanPop() {
        if(initialized) {
            return Count() > 0 && Peek() != CellType.Bedrock;
        }
        else {
            throw new System.Exception("ERROR - Trying to access CellStack before initialization");
        }
    }
}
