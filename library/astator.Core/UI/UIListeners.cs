using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using Java.Lang;
using System;
using static Android.Views.View;
using static Android.Widget.AdapterView;
using static Android.Widget.CompoundButton;

namespace astator.Core.UI
{
    public class OnClickListener : Java.Lang.Object, IOnClickListener
    {
        private readonly Action<View> callBack;
        public OnClickListener(Action<View> callback)
        {
            this.callBack = callback;
        }
        public void OnClick(View v)
        {
            this.callBack.Invoke(v);
        }
    }
    public class OnLongClickListener : Java.Lang.Object, IOnLongClickListener
    {
        private readonly Func<View, bool> callBack;
        public OnLongClickListener(Func<View, bool> callback)
        {
            this.callBack = callback;
        }
        public bool OnLongClick(View v)
        {
            try
            {
                return this.callBack.Invoke(v);
            }
            catch
            {
                return true;
            }
        }
    }
    public class OnTouchListener : Java.Lang.Object, IOnTouchListener
    {
        private readonly Func<View, MotionEvent, bool> callBack;
        public OnTouchListener(Func<View, MotionEvent, bool> callBack)
        {
            this.callBack = callBack;
        }
        public bool OnTouch(View v, MotionEvent e)
        {
            try
            {
                return this.callBack.Invoke(v, e);
            }
            catch
            {
                return true;
            }
        }
    }
    public class OnCheckedChangeListener : Java.Lang.Object, IOnCheckedChangeListener
    {
        private readonly Action<CompoundButton, bool> callBack;
        public OnCheckedChangeListener(Action<CompoundButton, bool> callBack)
        {
            this.callBack = callBack;
        }
        public void OnCheckedChanged(CompoundButton v, bool isChecked)
        {
            try
            {
                this.callBack.Invoke(v, isChecked);
            }
            catch
            {
            }

        }
    }
    public class RadioGroupOnCheckedChangeListener : Java.Lang.Object, RadioGroup.IOnCheckedChangeListener
    {
        private readonly Action<RadioGroup, int> callBack;
        public RadioGroupOnCheckedChangeListener(Action<RadioGroup, int> callBack)
        {
            this.callBack = callBack;
        }

        public void OnCheckedChanged(RadioGroup group, int checkedId)
        {
            if (group is not null)
            {
                var position = group.IndexOfChild(group.FindViewById(checkedId));
                this.callBack.Invoke(group, position);
            }

        }
    }
    public class TextWatcher : Java.Lang.Object, ITextWatcher
    {
        private readonly Action<IEditable> callBack;
        public TextWatcher(Action<IEditable> callBack)
        {
            this.callBack = callBack;
        }
        public void AfterTextChanged(IEditable s)
        {
            this.callBack.Invoke(s);
        }

        public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
        {
            throw new NotImplementedException();
        }

        public void OnTextChanged(ICharSequence s, int start, int before, int count)
        {
            throw new NotImplementedException();
        }
    }
    public class OnScrollChangeListener : Java.Lang.Object, IOnScrollChangeListener
    {
        private readonly Action<View, int, int, int, int> callBack;
        public OnScrollChangeListener(Action<View, int, int, int, int> callBack)
        {
            this.callBack = callBack;
        }

        public void OnScrollChange(View v, int scrollX, int scrollY, int oldScrollX, int oldScrollY)
        {
            this.callBack.Invoke(v, scrollX, scrollY, oldScrollX, oldScrollY);
        }
    }
    public class OnAttachedListener
    {
        private readonly Action<View> callBack;
        public OnAttachedListener(Action<View> callBack)
        {
            this.callBack = callBack;
        }
        public void OnAttached(View v)
        {
            this.callBack.Invoke(v);
        }
    }
    public class OnItemSelectedListener : Java.Lang.Object, IOnItemSelectedListener
    {
        private readonly Action<AdapterView, View, int, long> callBack;
        public OnItemSelectedListener(Action<AdapterView, View, int, long> callBack)
        {
            this.callBack = callBack;
        }

        public void OnItemSelected(AdapterView parent, View view, int position, long id)
        {
            this.callBack.Invoke(parent, view, position, id);
        }

        public void OnNothingSelected(AdapterView parent)
        {
            throw new NotImplementedException();
        }
    }

    public class OnKeyListener : Java.Lang.Object, IOnKeyListener
    {
        private readonly Func<View, Keycode, KeyEvent, bool> callBack;
        public OnKeyListener(Func<View, Keycode, KeyEvent, bool> callBack)
        {
            this.callBack = callBack;
        }
        public bool OnKey(View v, [GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            try
            {
                return this.callBack.Invoke(v, keyCode, e);
            }
            catch
            {
                return true;
            }
        }
    }

    public class OnMenuItemClickListener : Java.Lang.Object, AndroidX.AppCompat.Widget.PopupMenu.IOnMenuItemClickListener
    {
        private readonly Func<IMenuItem, bool> callBack;
        public OnMenuItemClickListener(Func<IMenuItem, bool> callBack)
        {
            this.callBack = callBack;
        }
        public bool OnMenuItemClick(IMenuItem item)
        {
            return this.callBack.Invoke(item);
        }
    }
}
