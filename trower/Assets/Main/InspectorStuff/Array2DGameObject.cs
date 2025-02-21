using UnityEngine;

namespace Array2DEditor
{
    [System.Serializable]
    public class Array2DGameObject : Array2D<GameObject>
    {
        [SerializeField]
        CellRowGameObject[] cells = new CellRowGameObject[Consts.defaultGridSize];

        protected override CellRow<GameObject> GetCellRow(int idx)
        {
            return cells[idx];
        }
    }
}

