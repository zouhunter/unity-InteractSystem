using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace WorldActionSystem
{

    [System.Serializable]
    public class LinkData
    {
        public List<string> linkItems;
        public List<LinkGroup> defultLink;

        private List<string> _elementNames;
        public List<string> ElementNames
        {
            get
            {
                if (_elementNames == null)
                {
                    foreach (var item in linkItems)
                    {
                        if (!_elementNames.Contains(item))
                        {
                            _elementNames.Add(item);
                        }
                    }
                }
                return _elementNames;
            }

        }
    }

}