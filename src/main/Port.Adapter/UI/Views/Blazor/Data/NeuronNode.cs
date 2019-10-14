using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Data
{
    public class NeuronNode
    {
        public bool isExpanded = false;
        public NeuronNode[] children;
        public string tag;

        public NeuronNode(string tag, NeuronNode[] children)
        {
            this.tag = tag;
            this.children = children;
        }

        public void Toggle()
        {
            this.isExpanded = !this.isExpanded;
        }

        public string GetIcon()
        {
            if (isExpanded)
            {
                return "-";
            }

            return "+";
        }
    }
}
