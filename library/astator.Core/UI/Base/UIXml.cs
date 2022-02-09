using Android.Views;
using astator.Core.Exceptions;
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

        var view = Create(manager, element.Name.ToString(), args);

        var id = args["id"];
        if (id is string temp)
        {
            manager[temp] = (IView)view;
        }

        return view;
    }

    private static View Create(IManager manager, string type, ViewArgs args)
    {
        View view = type switch
        {
            "frame" => manager.CreateFrameLayout(args),
            "linear" => manager.CreateLinearLayout(args),
            "scroll" => manager.CreateScrollView(args),
            "btn" => manager.CreateButton(args),
            "check" => manager.CreateCheckBox(args),
            "edit" => manager.CreateEditText(args),
            "text" => manager.CreateTextView(args),
            "switch" => manager.CreateSwitch(args),
            "web" => manager.CreateWebView(args),
            "img" => manager.CreateImageView(args),
            "pager" => manager.CreateViewPager(args),
            "spinner" => manager.CreateSpinner(args),
            "card" => manager.CreateCardView(args),
            "radioGroup" => manager.CreateRadioGroup(args),
            "radio" => manager.CreateRadioButton(args),
            _ => throw new AttributeNotExistException(type),
        };
        return view;
    }
}

