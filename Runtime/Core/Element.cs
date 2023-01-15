using Crosswork.Core.Intents;
using UnityEngine;

namespace Crosswork.Core
{
    public class Element
    {
        public static readonly Vector2Int[] SingleCellPattern = new Vector2Int[1] { Vector2Int.zero };

        public int Id
        { get { return id; } }

        internal int id;

        protected CrossworkBoard board;
        protected Cell cell;

        private IElementModel model;

        public Element(IElementModel model)
        {
            this.model = model;
        }

        public virtual void OnCreated(CrossworkBoard board)
        {
            this.board = board;
        }

        public virtual void OnDestroyed()
        { }

        public virtual void OnCellChanged(Cell cell)
        {
            this.cell = cell;
        }

        public virtual ulong GetCollisionMask()
        {
            return 0;
        }

        public virtual Cell GetCell()
        {
            return cell;
        }

        public virtual IElementModel GetModel()
        {
            return model;
        }

        public virtual Vector2Int[] GetPattern()
        {
            return SingleCellPattern;
        }

        public virtual SlaveElement CreateSlave(Vector2Int offset)
        {
            return new SlaveElement(this, model);
        }

        public override string ToString()
        {
            return $"element {id} {GetType().Name} at <{cell}>";
        }
    }
}
