using System;
using System.Collections.Generic;
using System.Text;

namespace FireEngine.GameEngine.UIFramework
{
    public class DependencyProperty
    {
        public static DependencyProperty Register(Type propertyType, Type ownerType, object defaultValue, PropertyChangedCallback propertyChangedCallback)
        {
            throw new System.NotImplementedException();
        }
    }

    public delegate void PropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e);

    public class DependencyPropertyChangedEventArgs
    {

    }
}
