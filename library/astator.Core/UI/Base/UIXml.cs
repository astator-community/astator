using Android.Views;
using System.Collections.Generic;
using System.Xml.Linq;

namespace astator.Core.UI.Base;

internal static class UiXml
{
    internal static View Parse(IManager manager, string xml)
    {
        var doc = XDocument.Parse(xml);
        ILayout root = manager.CreateFrameLayout(null);
        ParseElements(manager, doc.Elements(), ref root);
        return (View)root;
    }

    private static void ParseElements(IManager manager, IEnumerable<XElement> elements, ref ILayout root)
    {
        foreach (var element in elements)
        {
            var view = ParseElement(manager, element);
            root.AddView(view);
            if (element.HasElements)
            {
                var vg = (ILayout)view;
                ParseElements(manager, element.Elements(), ref vg);
            }
        }
    }

    private static View ParseElement(IManager manager, XElement element)
    {
        var args = new ViewArgs();
        foreach (var attr in element.Attributes())
        {
            args[attr.Name.ToString()] = attr.Value;
        }
        return manager.Create(element.Name.ToString(), args);
    }
}

