using System.Collections.Generic;
using UnityEngine;

namespace UIX.Rendering
{
    /// <summary>
    /// Stores element metadata for style reapplication (pseudo-classes).
    /// </summary>
    public class UIXElementData : MonoBehaviour
    {
        public string ElementType;
        public string Id;
        public List<string> Classes = new List<string>();
        public int SiblingIndex;
        public int SiblingCount;
        public string ParentElementType;
        public List<string> ParentClasses = new List<string>();
        public string ParentId;
        public bool IsDirectChild;
    }
}
