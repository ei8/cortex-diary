//MIT License

//Copyright(c) .NET Foundation and Contributors

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
//
// https://github.com/dotnet-architecture/eShopOnContainers
//
// Modifications copyright(C) 2018 ei8/Elmer Bool

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Mobile.Core.Animations.Base
{
    public abstract class AnimationBase : BindableObject
    {
        private bool _isRunning = false;

        public static readonly BindableProperty TargetProperty =
        BindableProperty.Create("Target", typeof(VisualElement), typeof(AnimationBase), null,
        propertyChanged: (bindable, oldValue, newValue) =>
        ((AnimationBase)bindable).Target = (VisualElement)newValue);

        public VisualElement Target
        {
            get { return (VisualElement)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        public static readonly BindableProperty DurationProperty =
            BindableProperty.Create("Duration", typeof(string), typeof(AnimationBase), "1000",
                propertyChanged: (bindable, oldValue, newValue) =>
                ((AnimationBase)bindable).Duration = (string)newValue);

        public string Duration
        {
            get { return (string)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        public static readonly BindableProperty EasingProperty =
            BindableProperty.Create("Easing", typeof(EasingType), typeof(AnimationBase), EasingType.Linear,
            propertyChanged: (bindable, oldValue, newValue) =>
            ((AnimationBase)bindable).Easing = (EasingType)newValue);

        public EasingType Easing
        {
            get { return (EasingType)GetValue(EasingProperty); }
            set { SetValue(EasingProperty, value); }
        }

        public static readonly BindableProperty RepeatForeverProperty =
          BindableProperty.Create("RepeatForever", typeof(bool), typeof(AnimationBase), false,
              propertyChanged: (bindable, oldValue, newValue) =>
              ((AnimationBase)bindable).RepeatForever = (bool)newValue);

        public bool RepeatForever
        {
            get { return (bool)GetValue(RepeatForeverProperty); }
            set { SetValue(RepeatForeverProperty, value); }
        }

        public static readonly BindableProperty DelayProperty =
           BindableProperty.Create("Delay", typeof(int), typeof(AnimationBase), 0,
               propertyChanged: (bindable, oldValue, newValue) =>
               ((AnimationBase)bindable).Delay = (int)newValue);

        public int Delay
        {
            get { return (int)GetValue(DelayProperty); }
            set { SetValue(DelayProperty, value); }
        }

        protected abstract Task BeginAnimation();

        public async Task Begin()
        {
            try
            {

                if (!_isRunning)
                {
                    _isRunning = true;

                    await InternalBegin()
                        .ContinueWith(t => t.Exception, TaskContinuationOptions.OnlyOnFaulted)
                        .ConfigureAwait(false);
                }
            }
            catch (TaskCanceledException)
            {
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception in animation {ex}");
            }
        }

        protected abstract Task ResetAnimation();

        public async Task Reset()
        {
            await ResetAnimation();
        }

        private async Task InternalBegin()
        {
            if (Delay > 0)
            {
                await Task.Delay(Delay);
            }

            if (!RepeatForever)
            {
                await BeginAnimation();
            }
            else
            {
                do
                {
                    await BeginAnimation();
                    await ResetAnimation();
                } while (RepeatForever);
            }
        }
    }
}
