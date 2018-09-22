using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellStack : MonoBehaviour {

    Stack<HexCell> stack = new Stack<HexCell>();
    public HexCoordinates coordinates;

    public int Count() {
        return stack.Count;
    }

    public HexCell Peek() {
        return stack.Peek();
    }

    public HexCell Pop() {
        return stack.Pop();
    }

    public void Push(HexCell cell) {
        cell.cellStack = this;
        cell.elevation = Count();

        stack.Push(cell);
    }

    public bool CanPop() {
        return Count() > 0 && !(Peek().GetType() == typeof(BedrockCell));
    }
}
