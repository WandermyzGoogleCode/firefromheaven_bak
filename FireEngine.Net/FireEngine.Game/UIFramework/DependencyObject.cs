using System;
using System.Collections.Generic;
using System.Text;

namespace FireEngine.GameEngine.UIFramework
{
    public abstract class DependencyObject
    {
        public object GetValue(DependencyProperty targetProperty)
        {
            //if(targetProperty.ownerType.IsSubclassOf(targetProperty.ownerType)

            throw new System.NotImplementedException();
        }

        public void SetValue(DependencyProperty targetProperty, object value)
        {
            throw new System.NotImplementedException();
        }

        public void BeginAnimation(DependencyProperty targetProperty, Media.Animation animation, object from, object to, TimeSpan duration)
        {
            throw new System.NotImplementedException();
        }
    }
}
