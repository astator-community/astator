using Android.Widget;
using Com.Amrdeveloper.Codeview;
using Java.Util.Regex;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

namespace astator.Views;

internal class CodeEditorView : View
{
    public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(CodeEditorView), string.Empty);
    public string Text
    {
        get => GetValue(TextProperty) as string;
        set => SetValue(TextProperty, value);
    }

    public static readonly BindableProperty TextSizeProperty = BindableProperty.Create(nameof(TextSize), typeof(int), typeof(CodeEditorView), 18);
    public int TextSize
    {
        get => (int)GetValue(TextSizeProperty);
        set => SetValue(TextSizeProperty, value);
    }

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(CodeEditorView), Color.Parse("#333333"));
    public Color TextColor
    {
        get => GetValue(TextColorProperty) as Color;
        set => SetValue(TextColorProperty, value);
    }

    public static readonly BindableProperty BackgroudColorProperty = BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(CodeEditorView), Color.Parse("#f0f3f6"));
    public new Color BackgroundColor
    {
        get => GetValue(TextColorProperty) as Color;
        set => SetValue(TextColorProperty, value);
    }

    public static readonly BindableProperty LineNumberEnabledProperty = BindableProperty.Create(nameof(LineNumberEnabled), typeof(bool), typeof(CodeEditorView), true);
    public bool LineNumberEnabled
    {
        get => (bool)GetValue(LineNumberEnabledProperty);
        set => SetValue(LineNumberEnabledProperty, value);
    }

    public static readonly BindableProperty LineNumberTextColorProperty = BindableProperty.Create(nameof(LineNumberTextColor), typeof(Color), typeof(CodeEditorView), Color.Parse("#666666"));
    public Color LineNumberTextColor
    {
        get => GetValue(LineNumberTextColorProperty) as Color;
        set => SetValue(LineNumberTextColorProperty, value);
    }

    public static readonly BindableProperty TabLengthProperty = BindableProperty.Create(nameof(TabLength), typeof(int), typeof(CodeEditorView), 4);
    public int TabLength
    {
        get => (int)GetValue(TabLengthProperty);
        set => SetValue(TabLengthProperty, value);
    }


    public string GetText()
    {
        if (this.Handler.NativeView is CodeView view)
        {
            return view.Text;
        }

        return string.Empty;
    }

    public CodeEditorView() : base()
    {

    }
}

internal class CodeEditorViewHandler : ViewHandler<CodeEditorView, CodeView>
{
    private readonly string[] keyWords = new string[]
     {
        "abstract",
        "as",
        "base",
        "bool",
        "break",
        "byte",
        "case",
        "catch",
        "char",
        "checked",
        "class",
        "const",
        "continue",
        "decimal",
        "default",
        "delegate",
        "do",
        "double",
        "else",
        "enum",
        "event",
        "explicit",
        "extern",
        "false",
        "finally",
        "fixed",
        "float",
        "for",
        "foreach",
        "goto",
        "if",
        "implicit",
        "in",
        "int",
        "interface",
        "internal",
        "is",
        "lock",
        "long",
        "namespace",
        "new",
        "null",
        "object",
        "operator",
        "out",
        "override",
        "params",
        "private",
        "protected",
        "public",
        "readonly",
        "ref",
        "return",
        "sbyte",
        "sealed",
        "short",
        "sizeof",
        "stackalloc",
        "static",
        "string",
        "struct",
        "switch",
        "this",
        "throw",
        "true",
        "try",
        "typeof",
        "uint",
        "ulong",
        "unchecked",
        "unsafe",
        "ushort",
        "using",
        "virtual",
        "void",
        "volatile",
        "while",
        "add",
        "and",
        "alias",
        "ascending",
        "async",
        "await",
        "by",
        "descending",
        "dynamic",
        "equals",
        "from",
        "get",
        "global",
        "group",
        "init",
        "into",
        "join",
        "let",
        "managed",
        "nameof",
        "nint",
        "not",
        "notnull",
        "nuint",
        "on",
        "or",
        "orderby",
        "partial",
        "partial",
        "record",
        "remove",
        "select",
        "set",
        "unmanaged",
        "unmanaged",
        "value",
        "var",
        "when",
        "where",
        "where",
        "with",
        "yield",
     };


    public static PropertyMapper<CodeEditorView, CodeEditorViewHandler> Mapper = new(ViewMapper)
    {
        [nameof(CodeEditorView.Text)] = MapText,
        [nameof(CodeEditorView.TextColor)] = MaptextColor,
        [nameof(CodeEditorView.TextSize)] = MapTextSize,
        [nameof(CodeEditorView.BackgroundColor)] = MapBackgroundColor,
        [nameof(CodeEditorView.LineNumberEnabled)] = MapLineNumberEnabled,
        [nameof(CodeEditorView.LineNumberTextColor)] = MapLineNumberTextColor,
        [nameof(CodeEditorView.TabLength)] = MapTabLength,
    };


    public CodeEditorViewHandler() : base(Mapper)
    {

    }

    public CodeEditorViewHandler(PropertyMapper mapper) : base(mapper)
    {

    }

    protected override CodeView CreateNativeView()
    {
        var view = new CodeView(this.Context);
        view.SetIndentationStarts(new List<Java.Lang.Character> { new Java.Lang.Character('{') });
        view.SetIndentationEnds(new List<Java.Lang.Character> { new Java.Lang.Character('}') });
        view.SetEnableAutoIndentation(true);

        view.SetHorizontallyScrolling(false);

        var keyWords = new List<string>();
        var color = Colors.Blue.ToInt();

        foreach (var keyWord in this.keyWords)
        {
            keyWords.Add(keyWord);
            view.AddSyntaxPattern(Pattern.Compile($"(?=\\b){keyWord}(?=\\b)"), color);
        }

        var codeAdapter = new ArrayAdapter(this.Context, Resource.Layout.suggestion_list_item, Resource.Id.suggestItemTextView, keyWords);
        view.Adapter = codeAdapter;
        return view;
    }




    private static void MapText(CodeEditorViewHandler handler, CodeEditorView view)
    {
        handler.NativeView.Text = view.Text;
    }

    private static void MaptextColor(CodeEditorViewHandler handler, CodeEditorView view)
    {
        handler.NativeView.SetTextColor(view.TextColor.ToNative());
    }

    private static void MapTextSize(CodeEditorViewHandler handler, CodeEditorView view)
    {
        handler.NativeView.TextSize = view.TextSize;
        handler.NativeView.SetLineNumberTextSize((float)(handler.NativeView.TextSize * 0.75));
    }

    private static void MapBackgroundColor(CodeEditorViewHandler handler, CodeEditorView view)
    {
        handler.NativeView.SetBackgroundColor(view.BackgroundColor.ToNative());
    }

    private static void MapLineNumberEnabled(CodeEditorViewHandler handler, CodeEditorView view)
    {
        handler.NativeView.SetEnableLineNumber(view.LineNumberEnabled);
    }

    private static void MapLineNumberTextColor(CodeEditorViewHandler handler, CodeEditorView view)
    {
        handler.NativeView.SetLineNumberTextColor(view.LineNumberTextColor.ToNative());
    }

    private static void MapTabLength(CodeEditorViewHandler handler, CodeEditorView view)
    {
        handler.NativeView.SetTabLength(view.TabLength);
    }
}
